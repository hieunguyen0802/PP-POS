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


@app.post("/chat", response_model=ChatResponse)
async def chat(req: ChatRequest) -> ChatResponse:
    conversation_id = req.conversation_id or uuid.uuid4().hex
    history = get_history(conversation_id)
    try:
        reply, new_history = await run_agent(history, req.message)
    except openai.RateLimitError:
        raise HTTPException(status_code=429, detail="LLM rate limit reached, try again shortly")
    except openai.APIError as e:
        raise HTTPException(status_code=502, detail=f"LLM provider error: {e.__class__.__name__}")
    save_history(conversation_id, new_history)
    return ChatResponse(reply=reply, conversation_id=conversation_id)