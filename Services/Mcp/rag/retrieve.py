import asyncio
import sys

from qdrant_client import AsyncQdrantClient, models

from config import settings
from rag.embeddings import embed_texts

_client = AsyncQdrantClient(url=settings.qdrant_url)


async def search(query: str, limit: int | None = None) -> list[dict]:
    """Return the chunks closest in meaning to the question."""
    [vector] = await embed_texts([query])
    result = await _client.query_points(
        collection_name=settings.qdrant_collection,
        query=vector,
        limit=limit or settings.knowledge_top_k,
        # only chunks this caller is allowed to see
        query_filter=models.Filter(
            must=[
                models.FieldCondition(
                    key="audience",
                    match=models.MatchAny(any=settings.knowledge_audiences),
                )
            ]
        ),
        with_payload=True,
    )
    return [
        {
            "title": p.payload["title"],
            "section": p.payload["section"],
            "source": p.payload["source"],
            "score": round(p.score, 3),
            "text": p.payload["text"],
        }
        for p in result.points
    ]


if __name__ == "__main__":
    # quick manual test:  python -m rag.retrieve "your question" "another question"
    async def _demo(queries: list[str]) -> None:
        for q in queries:
            print(f"\n? {q}")
            for r in await search(q):
                print(f"  {r['score']:.3f}  {r['title']} / {r['section']}")

    asyncio.run(_demo(sys.argv[1:]))