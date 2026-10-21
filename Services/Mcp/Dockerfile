# ---- build stage: install locked dependencies ----
FROM python:3.12-slim AS build
ENV UV_PYTHON_DOWNLOADS=never
RUN pip install --no-cache-dir uv
WORKDIR /app
# deps first, so this layer is cached until pyproject.toml / uv.lock change
COPY pyproject.toml uv.lock ./
RUN uv sync --frozen --no-dev --no-install-project

# download the BM25 model into the image now, so containers never fetch it at start
ENV FASTEMBED_CACHE_PATH=/app/fastembed_cache
RUN /app/.venv/bin/python -c "from fastembed import SparseTextEmbedding; SparseTextEmbedding('Qdrant/bm25')"

# ---- runtime stage ----
# ---- runtime stage ----
FROM python:3.12-slim
WORKDIR /app
RUN useradd --system --uid 1000 app
COPY --from=build /app/.venv /app/.venv
COPY --from=build --chown=app /app/fastembed_cache /app/fastembed_cache
ENV FASTEMBED_CACHE_PATH=/app/fastembed_cache
COPY *.py ./
COPY rag ./rag
COPY rag_langchain ./rag_langchain
ENV PATH="/app/.venv/bin:$PATH" PYTHONUNBUFFERED=1
USER app
EXPOSE 8200
CMD ["python", "server.py"]