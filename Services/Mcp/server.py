import httpx
from mcp.server.mcpserver import MCPServer

from auth import token_provider
from config import settings

mcp = MCPServer("pos-mcp")


@mcp.tool()
async def get_stock_level(product_id: str) -> dict:
    """Get the current stock level for one product.

    Args:
        product_id: The product's GUID, e.g. ccc39fe6-05c5-4cce-8301-00f1a62a0903.

    Returns quantity on hand, reorder level, and whether stock is below reorder level.
    """
    async with httpx.AsyncClient(timeout=10) as client:
        token = await token_provider.get_token(client)
        resp = await client.get(
            f"{settings.inventory_base_url}/api/Inventory/GetStockLevel/{product_id}",
            headers={"Authorization": f"Bearer {token}"},
        )
    if resp.status_code == 404:
        return {"error": f"No inventory item for product {product_id}"}
    resp.raise_for_status()
    return resp.json()


if __name__ == "__main__":
    mcp.run()