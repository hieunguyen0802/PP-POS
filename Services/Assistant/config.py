from pydantic_settings import BaseSettings, SettingsConfigDict


class Settings(BaseSettings):
    model_config = SettingsConfigDict(env_file=".env")

    # Gemini's OpenAI-compatible endpoint. To switch provider, change these
    # three values (base url, model, key) and nothing else.
    llm_base_url: str = "https://generativelanguage.googleapis.com/v1beta/openai/"
    llm_model: str = "gemini-3.5-flash"
    llm_fallback_models: list[str] = [
        "gemini-3.8-flash",
        "gemini-3.5-flash-lite",
        "gemini-3.1-flash-lite"
    ]
    llm_api_key: str  # required, read from .env
    
    mcp_url: str = "http://127.0.0.1:8200/mcp"
    max_tool_rounds: int = 5   # safety limit for the agent loop


settings = Settings()