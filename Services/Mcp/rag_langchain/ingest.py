"""LangChain version of ingest: same documents, same chunk text, different plumbing."""
from pathlib import Path

from langchain_core.documents import Document
from langchain_text_splitters import MarkdownHeaderTextSplitter, RecursiveCharacterTextSplitter
from langchain_qdrant import QdrantVectorStore
from config import settings
from rag.chunking import MAX_CHARS, parse_front_matter  # LangChain has no front-matter parser
from rag_langchain.common import COLLECTION, make_embeddings
import sys

from langchain_qdrant import QdrantVectorStore, RetrievalMode
from rag_langchain.common import COLLECTION, COLLECTION_HYBRID, make_embeddings, make_sparse


KNOWLEDGE_DIR = Path(__file__).resolve().parent.parent / "knowledge"


def load_documents() -> list[Document]:
    # stage 1: cut at "## " headings, remembering the heading as metadata "section"
    by_header = MarkdownHeaderTextSplitter(
        headers_to_split_on=[("##", "section")], strip_headers=True
    )
    # stage 2: cut anything still too long (ours are short, so this rarely does anything)
    by_size = RecursiveCharacterTextSplitter(chunk_size=MAX_CHARS, chunk_overlap=0)

    docs: list[Document] = []
    for path in sorted(KNOWLEDGE_DIR.glob("*.md")):
        meta, body = parse_front_matter(path.read_text(encoding="utf-8"))
        title = meta.get("title", path.stem)
        audience = meta.get("audience", "staff")  # unknown = restricted (fail closed)

        for part in by_header.split_text(body):
            section = part.metadata.get("section")
            if not section:  # the "# Title" line before the first "##"
                continue
            text = part.page_content.replace("  \n", "\n")  # splitter joins lines with two spaces
            for piece in by_size.split_text(text):
                docs.append(
                    Document(
                        page_content=f"{title} - {section}\n{piece}",
                        metadata={
                            "title": title,
                            "section": section,
                            "audience": audience,
                            "source": path.name,
                        },
                    )
                )
    return docs


if __name__ == "__main__":
    docs = load_documents()
    print(f"{len(docs)} chunks")
    if "--hybrid" in sys.argv:
        QdrantVectorStore.from_documents(
            docs,
            embedding=make_embeddings(),
            sparse_embedding=make_sparse(),
            retrieval_mode=RetrievalMode.HYBRID,
            url=settings.qdrant_url,
            collection_name=COLLECTION_HYBRID,
            force_recreate=True,
        )
        print(f"stored in '{COLLECTION_HYBRID}'")
    else:
        QdrantVectorStore.from_documents(
            docs,
            embedding=make_embeddings(),
            url=settings.qdrant_url,
            collection_name=COLLECTION,
            force_recreate=True,
        )
        print(f"stored in '{COLLECTION}'")