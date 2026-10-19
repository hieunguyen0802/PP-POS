from openai import AsyncOpenAI

from config import settings

_client = AsyncOpenAI(base_url=settings.llm_base_url, api_key=settings.llm_api_key)


async def chat_completion(messages: list[dict], tools: list[dict] | None = None):
    kwargs = {"tools": tools} if tools else {}
    resp = await _client.chat.completions.create(
        model=settings.llm_model,
        messages=messages,
        **kwargs,
    )
    return resp.choices[0].message