from pydantic_settings import BaseSettings, SettingsConfigDict


class Settings(BaseSettings):
    model_config = SettingsConfigDict(env_file=".env", env_prefix="MCP_")

    inventory_base_url: str = "http://localhost:5100"
    keycloak_token_url: str = (
        "http://localhost:8080/realms/pos-staff/protocol/openid-connect/token"
    )
    keycloak_client_id: str = "pos-mcp"
    keycloak_client_secret: str = "8OyFViize1RFgYfTltV9yPiGhSq8IWi5SHEPfRW1LNAXQJhOAffQGTXhN123QpiAIwK2lv8T7jfnF40fpICZCv"


settings = Settings()