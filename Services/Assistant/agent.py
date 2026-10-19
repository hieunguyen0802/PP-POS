import json
import logging

from mcp import Client

from config import settings
from llm import chat_completion
from mcp_tools import result_to_text, to_openai_tool

log = logging.getLogger("assistant")

SYSTEM_PROMPT = (
    "You are the assistant for a retail point-of-sale system. "
    "Use the available tools to look up data; never guess numbers or IDs. "
    "Users refer to products by name. Always call search_products first to find the product ID, "
    "then use that ID with other tools. "
    "If search_products returns more than one product, do NOT choose one: list the matches by name "
    "(and code) and ask the user which they mean. If it returns none, say no product matched. "
    "In answers, refer to products by name, not by ID. "
    "If no tool can answer the question, say so. Keep answers short."
)


async def run_agent(history: list[dict], user_message: str) -> tuple[str, list[dict]]:
    messages: list[dict] = (
        [{"role": "system", "content": SYSTEM_PROMPT}]
        + history
        + [{"role": "user", "content": user_message}]
    )

    async with Client(settings.mcp_url) as mcp:
        tools = [to_openai_tool(t) for t in (await mcp.list_tools()).tools]

        for _ in range(settings.max_tool_rounds):
            msg = await chat_completion(messages, tools)

            if not msg.tool_calls:  # final answer
                messages.append({"role": "assistant", "content": msg.content or ""})
                return msg.content or "", messages[1:]  # drop the system prompt

            messages.append(msg.model_dump(exclude_none=True))

            for call in msg.tool_calls:
                args = json.loads(call.function.arguments or "{}")
                log.info("tool call: %s %s", call.function.name, args)
                result = await mcp.call_tool(call.function.name, args)
                messages.append(
                    {
                        "role": "tool",
                        "tool_call_id": call.id,
                        "content": result_to_text(result),
                    }
                )

    fallback = "I couldn't finish that within the allowed number of tool steps."
    messages.append({"role": "assistant", "content": fallback})
    return fallback, messages[1:]