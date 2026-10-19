MAX_USER_TURNS = 10  # keep only the last N user questions (and everything after them)

_conversations: dict[str, list[dict]] = {}


def get_history(conversation_id: str) -> list[dict]:
    return _conversations.setdefault(conversation_id, [])


def save_history(conversation_id: str, messages: list[dict]) -> None:
    """Store messages, trimmed at a user-message boundary so tool calls
    are never separated from their results."""
    user_idx = [i for i, m in enumerate(messages) if m["role"] == "user"]
    if len(user_idx) > MAX_USER_TURNS:
        messages = messages[user_idx[-MAX_USER_TURNS]:]
    _conversations[conversation_id] = messages