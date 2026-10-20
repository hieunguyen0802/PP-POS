from pydantic_settings import BaseSettings, SettingsConfigDict


class Settings(BaseSettings):
    model_config = SettingsConfigDict(env_file=".env", env_prefix="MCP_")

    inventory_base_url: str = "http://localhost:5100"
    keycloak_token_url: str = (
        "http://localhost:8080/realms/pos-staff/protocol/openid-connect/token"
    )
    keycloak_client_id: str = "pos-mcp"
    keycloak_client_secret: str 

    transport: str = "stdio"
    host: str = "127.0.0.1"
    port: int = 8200
    
    pos_base_url: str = "http://localhost:5200"
    
    # --- RAG ---
    llm_base_url: str = "https://generativelanguage.googleapis.com/v1beta/openai/"
    llm_api_key: str = ""
    embedding_model: str = "gemini-embedding-001"
    qdrant_url: str = "http://localhost:6333"
    qdrant_collection: str = "knowledge"
    
    knowledge_top_k: int = 4
    knowledge_audiences: list[str] = ["public"]

settings = Settings()