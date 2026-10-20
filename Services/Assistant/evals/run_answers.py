import asyncio
import json
import sys
import urllib.error
import urllib.request
import uuid
from pathlib import Path

URL = "http://localhost:8100/chat"
CASES = Path(__file__).with_name(sys.argv[1] if len(sys.argv) > 1 else "answer_cases.json")
UNKNOWN_PHRASES = [
    "don't have", "do not have", "no information", "not covered",
    "couldn't find", "could not find", "cannot find", "can't find", "isn't covered",
]


def _post(message: str, conversation_id: str | None) -> tuple[int, str]:
    payload = {"message": message}
    if conversation_id:
        payload["conversation_id"] = conversation_id
    req = urllib.request.Request(
        URL,
        data=json.dumps(payload).encode("utf-8"),
        headers={"Content-Type": "application/json"},
    )
    try:
        with urllib.request.urlopen(req, timeout=240) as r:
            return r.status, r.read().decode("utf-8")
    except urllib.error.HTTPError as e:
        return e.code, e.read().decode("utf-8")
    except (TimeoutError, urllib.error.URLError) as e:
        return 0, f"{e.__class__.__name__}: {e}"


async def ask(message: str, conversation_id: str | None) -> tuple[str, str | None]:
    for attempt in range(3):
        status, body = await asyncio.to_thread(_post, message, conversation_id)
        if status in (429, 502):  # free-tier limit or upstream hiccup: wait, retry
            await asyncio.sleep(20 * (attempt + 1))
            continue
        if status != 200:
            return f"[HTTP {status}] {body[:200]}", conversation_id
        data = json.loads(body)
        return data["reply"], data.get("conversation_id")
    return "[no reply: still rate limited after retries]", conversation_id


def check(case: dict, reply: str) -> list[str]:
    low = reply.lower()
    problems = []
    for group in case.get("contains", []):
        if not any(g.lower() in low for g in group):
            problems.append(f"missing one of {group}")
    if case.get("expect_unknown") and not any(p in low for p in UNKNOWN_PHRASES):
        problems.append("should say it does not know")
    if case.get("expect_clarify") and "?" not in reply:
        problems.append("should ask a clarifying question")
    for bad in case.get("forbid", []):
        if bad.lower() in low:
            problems.append(f"contains forbidden text {bad!r}")
    return problems


async def main() -> int:
    cases = json.loads(CASES.read_text(encoding="utf-8"))
    only = [a.lower() for a in sys.argv[2:]]
    if only:
        cases = [c for c in cases if any(o in (c.get("name") or c.get("question", "")).lower() for o in only)]
    passed = 0
    for c in cases:
        messages = c.get("turns") or [c["question"]]
        conversation_id = "eval-" + uuid.uuid4().hex[:8]
        reply = ""
        for m in messages:
            reply, conversation_id = await ask(m, conversation_id)
            await asyncio.sleep(4)  # stay under the free-tier per-minute limit
        errored = reply.startswith("[HTTP") or reply.startswith("[no reply")
        problems = ["infrastructure error, not a real result"] if errored else check(c, reply)
        passed += not problems
        label = c.get("name") or messages[-1]
        print(f"{'PASS' if not problems else 'FAIL'}  {label[:70]}")
        if problems:
            print(f"      {'; '.join(problems)}")
            print(f"      reply: {reply[:300]!r}")
    print(f"\npassed {passed}/{len(cases)}")
    return 0 if passed == len(cases) else 1


if __name__ == "__main__":
    sys.exit(asyncio.run(main()))