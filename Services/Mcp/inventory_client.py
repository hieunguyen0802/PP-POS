import uuid
from typing import Any

import httpx

from auth import token_provider
from config import settings


def parse_guid(value: str) -> str:
    """Validate an ID before it goes into a URL. Tool arguments come from an
    LLM, so treat them as untrusted input."""
    try:
        return str(uuid.UUID(value))
    except ValueError:
        raise ValueError(f"'{value}' is not a valid GUID") from None


async def _get(base_url: str, path: str, params: dict | None = None) -> Any:
    async with httpx.AsyncClient(timeout=10) as client:
        token = await token_provider.get_token(client)
        resp = await client.get(
            f"{base_url}{path}",
            params=params,
            headers={"Authorization": f"Bearer {token}"},
        )

    if resp.status_code == 404:
        return {"error": "Not found"}
    if resp.status_code == 403:
        return {"error": "The MCP service account is not allowed to read this"}
    resp.raise_for_status()
    return resp.json()


async def inventory_get(path: str) -> Any:
    return await _get(settings.inventory_base_url, path)


async def pos_get(path: str, params: dict | None = None) -> Any:
    return await _get(settings.pos_base_url, path, params)