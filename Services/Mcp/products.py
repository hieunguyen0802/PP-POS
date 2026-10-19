import time

from inventory_client import pos_get

_TTL_SECONDS = 300
_cache: dict[str, tuple[float, str]] = {}


async def get_product_name(product_id: str) -> str | None:
    """Product name for an ID, or None if it can't be resolved.
    A failure here must never break the tool that asked."""
    hit = _cache.get(product_id)
    if hit and time.monotonic() - hit[0] < _TTL_SECONDS:
        return hit[1]
    try:
        data = await pos_get(f"/api/Product/GetProductById/{product_id}")
    except Exception:
        return None
    if isinstance(data, dict) and data.get("productName"):
        _cache[product_id] = (time.monotonic(), data["productName"])
        return data["productName"]
    return None