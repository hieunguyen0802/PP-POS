import time

import httpx

from config import settings


class TokenProvider:
    """Fetches a service token via client-credentials and caches it."""

    def __init__(self) -> None:
        self._token: str | None = None
        self._expires_at: float = 0.0

    async def get_token(self, client: httpx.AsyncClient) -> str:
        # refresh 30s early, same as the POS KeycloakServiceTokenProvider
        if self._token and time.monotonic() < self._expires_at - 30:
            return self._token

        resp = await client.post(
            settings.keycloak_token_url,
            data={
                "grant_type": "client_credentials",
                "client_id": settings.keycloak_client_id,
                "client_secret": settings.keycloak_client_secret,
            },
        )
        resp.raise_for_status()
        body = resp.json()
        self._token = body["access_token"]
        self._expires_at = time.monotonic() + body["expires_in"]
        return self._token


token_provider = TokenProvider()