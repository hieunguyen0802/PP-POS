import argparse
import asyncio
import uuid
from pathlib import Path

from qdrant_client import QdrantClient, models

from config import settings
from rag.chunking import chunk_document
from rag.embeddings import embed_texts

KNOWLEDGE_DIR = Path(__file__).resolve().parent.parent / "knowledge"


async def main(recreate: bool) -> None:
    files = sorted(KNOWLEDGE_DIR.glob("*.md"))
    chunks = [c for f in files for c in chunk_document(f)]
    print(f"{len(files)} files -> {len(chunks)} chunks")
    if not chunks:
        return

    # embed first: if this fails, we have not touched the database
    vectors = await embed_texts([c.text for c in chunks])

    client = QdrantClient(url=settings.qdrant_url)
    name = settings.qdrant_collection

    if recreate and client.collection_exists(name):
        client.delete_collection(name)
    if not client.collection_exists(name):
        client.create_collection(
            collection_name=name,
            vectors_config=models.VectorParams(
                size=len(vectors[0]), distance=models.Distance.COSINE
            ),
        )

    # drop old chunks of these files, so an edited document leaves nothing stale
    for f in files:
        client.delete(
            collection_name=name,
            points_selector=models.FilterSelector(
                filter=models.Filter(
                    must=[models.FieldCondition(key="source", match=models.MatchValue(value=f.name))]
                )
            ),
        )

    points = [
        models.PointStruct(
            # same file + position -> same id, so re-running never duplicates
            id=str(uuid.uuid5(uuid.NAMESPACE_URL, f"pos-knowledge/{c.source}#{c.index}")),
            vector=vec,
            payload={
                "text": c.text,
                "title": c.title,
                "section": c.section,
                "audience": c.audience,
                "source": c.source,
            },
        )
        for c, vec in zip(chunks, vectors)
    ]
    client.upsert(collection_name=name, points=points)
    print(f"stored {len(points)} chunks in '{name}' (vector size {len(vectors[0])})")


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--recreate", action="store_true", help="delete and rebuild the collection")
    args = parser.parse_args()
    asyncio.run(main(args.recreate))