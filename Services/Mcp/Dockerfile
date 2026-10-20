# ---- build stage: install locked dependencies ----
FROM python:3.12-slim AS build
ENV UV_PYTHON_DOWNLOADS=never
RUN pip install --no-cache-dir uv
WORKDIR /app
# deps first, so this layer is cached until pyproject.toml / uv.lock change
COPY pyproject.toml uv.lock ./
RUN uv sync --frozen --no-dev --no-install-project

# ---- runtime stage ----
FROM python:3.12-slim
WORKDIR /app
COPY --from=build /app/.venv /app/.venv
COPY *.py ./
COPY rag ./rag
ENV PATH="/app/.venv/bin:$PATH" PYTHONUNBUFFERED=1
RUN useradd --system --uid 1000 app
USER app
EXPOSE 8200
CMD ["python", "server.py"]