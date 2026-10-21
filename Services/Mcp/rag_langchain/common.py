from langchain_openai import OpenAIEmbeddings
from langchain_qdrant import FastEmbedSparse
from config import settings

COLLECTION = "knowledge_langchain"  # separate from the by-hand collection "knowledge"
COLLECTION_HYBRID = "knowledge_langchain_hybrid"


def make_sparse() -> FastEmbedSparse:
    # BM25 keyword model, runs on your machine; the model files download on first use
    return FastEmbedSparse(model_name="Qdrant/bm25")

def make_embeddings() -> OpenAIEmbeddings:
    return OpenAIEmbeddings(
        model=settings.embedding_model,
        base_url=settings.llm_base_url,   # Gemini's OpenAI-compatible endpoint
        api_key=settings.llm_api_key,
        # by default it sends token ids (an OpenAI-only format); Gemini needs raw text
        check_embedding_ctx_length=False,
        chunk_size=50,                    # texts per request, same as our by-hand batch
    )