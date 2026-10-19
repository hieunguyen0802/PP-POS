import json


def to_openai_tool(tool) -> dict:
    """MCP tool -> OpenAI 'function' tool definition."""
    return {
        "type": "function",
        "function": {
            "name": tool.name,
            "description": tool.description or "",
            "parameters": tool.input_schema,
        },
    }


def result_to_text(result) -> str:
    """MCP tool result -> plain text for the model."""
    parts = [c.text for c in result.content if getattr(c, "type", None) == "text"]
    text = "\n".join(parts)
    if not text and result.structured_content is not None:
        text = json.dumps(result.structured_content)
    return f"Error: {text}" if result.is_error else text