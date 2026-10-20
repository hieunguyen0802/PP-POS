import logging
import time

import openai
from openai import AsyncOpenAI
from langsmith.wrappers import wrap_openai
from config import settings

log = logging.getLogger(__name__)

_client = wrap_openai(
    AsyncOpenAI(
        base_url=settings.llm_base_url,
        api_key=settings.llm_api_key,
        max_retries=0,
        timeout=45,
    )
)

SKIPPABLE = (
    openai.RateLimitError,
    openai.InternalServerError,
    openai.NotFoundError,
    openai.APIConnectionError,  # includes APITimeoutError
)

COOLDOWN_SECONDS = 300  # skip a model that just failed for 5 minutes
_cooldown: dict[str, float] = {}  # model -> time.monotonic() when it may be tried again


async def chat_completion(messages: list[dict], tools: list[dict] | None = None):
    kwargs = {"tools": tools} if tools else {}
    models = [settings.llm_model, *settings.llm_fallback_models]
    now = time.monotonic()
    ready = [m for m in models if _cooldown.get(m, 0) <= now]

    last_error: Exception | None = None
    for model in ready or models:  # if every model is cooling down, try them all anyway
        try:
            resp = await _client.chat.completions.create(
                model=model, messages=messages, **kwargs
            )
            return resp.choices[0].message
        except SKIPPABLE as e:
            log.warning("model %s failed (%s), skipping it for %ss",
                        model, e.__class__.__name__, COOLDOWN_SECONDS)
            _cooldown[model] = time.monotonic() + COOLDOWN_SECONDS
            last_error = e
    raise last_error