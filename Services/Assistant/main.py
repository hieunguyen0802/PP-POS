import logging
import uuid

import openai
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel

from agent import run_agent
from store import get_history, save_history

logging.basicConfig(level=logging.INFO)

app = FastAPI(title="POS Assistant")


class ChatRequest(BaseModel):
    message: str
    conversation_id: str | None = None


class ChatResponse(BaseModel):
    reply: str
    conversation_id: str

def _find(exc: BaseException, kind: type) -> BaseException | None:
    """Find an exception of `kind`, even when wrapped in an ExceptionGroup."""
    if isinstance(exc, kind):
        return exc
    for sub in getattr(exc, "exceptions", []):
        found = _find(sub, kind)
        if found:
            return found
    return None

@app.post("/chat", response_model=ChatResponse)
async def chat(req: ChatRequest) -> ChatResponse:
    conversation_id = req.conversation_id or uuid.uuid4().hex
    history = get_history(conversation_id)
    try:
        reply, new_history = await run_agent(
            history,
            req.message,
            langsmith_extra={"metadata": {"conversation_id": conversation_id}},
        )
    except Exception as e:
        if _find(e, openai.RateLimitError):
            raise HTTPException(status_code=429, detail="All LLM models are rate limited, try again later")
        if _find(e, openai.APIError):
            raise HTTPException(status_code=502, detail="LLM provider error")
        raise
    save_history(conversation_id, new_history)
    return ChatResponse(reply=reply, conversation_id=conversation_id)