import asyncio

import openai
from openai import AsyncOpenAI

from config import settings

BATCH_SIZE = 50


def _client() -> AsyncOpenAI:
    if not settings.llm_api_key:
        raise RuntimeError("MCP_LLM_API_KEY is not set (needed to create embeddings)")
    return AsyncOpenAI(base_url=settings.llm_base_url, api_key=settings.llm_api_key)


async def embed_texts(texts: list[str]) -> list[list[float]]:
    """One vector per text, in the same order."""
    client = _client()
    vectors: list[list[float]] = []
    for i in range(0, len(texts), BATCH_SIZE):
        batch = texts[i : i + BATCH_SIZE]
        for attempt in range(5):
            try:
                resp = await client.embeddings.create(
                    model=settings.embedding_model, input=batch
                )
                break
            except openai.RateLimitError:
                wait = 15 * (attempt + 1)
                print(f"rate limited, waiting {wait}s...")
                await asyncio.sleep(wait)
        else:
            raise RuntimeError("embedding failed: still rate limited after 5 attempts")
        vectors.extend(item.embedding for item in resp.data)
    return vectors