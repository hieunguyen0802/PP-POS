from functools import lru_cache

from langchain_qdrant import QdrantVectorStore, RetrievalMode
from qdrant_client import QdrantClient, models

from config import settings
from rag_langchain.common import COLLECTION, COLLECTION_HYBRID, make_embeddings, make_sparse


@lru_cache
def _store(hybrid: bool) -> QdrantVectorStore:
    client = QdrantClient(url=settings.qdrant_url)
    if hybrid:
        return QdrantVectorStore(
            client=client,
            collection_name=COLLECTION_HYBRID,
            embedding=make_embeddings(),
            sparse_embedding=make_sparse(),
            retrieval_mode=RetrievalMode.HYBRID,
        )
    return QdrantVectorStore(
        client=client, collection_name=COLLECTION, embedding=make_embeddings()
    )


async def search(query: str, limit: int | None = None, hybrid: bool = False) -> list[dict]:
    audience_filter = models.Filter(
        must=[
            models.FieldCondition(
                key="metadata.audience",
                match=models.MatchAny(any=settings.knowledge_audiences),
            )
        ]
    )
    results = await _store(hybrid).asimilarity_search_with_score(
        query, k=limit or settings.knowledge_top_k, filter=audience_filter
    )
    return [
        {
            "title": doc.metadata["title"],
            "section": doc.metadata["section"],
            "source": doc.metadata["source"],
            "score": round(score, 3),
            "text": doc.page_content,
        }
        for doc, score in results
    ]