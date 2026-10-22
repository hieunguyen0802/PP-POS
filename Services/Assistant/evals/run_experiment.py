"""Run the answer-eval cases as a LangSmith experiment.

    python -m uv run --env-file ../../.env python -m evals.run_experiment <experiment-name>
    add --refresh to rebuild the dataset from the JSON files first
"""
import os
import asyncio
import json
import sys
import uuid
from pathlib import Path

from langsmith import Client, evaluate

from evals.run_answers import ask, check

DATASET = "pos-assistant-answers"
FILES = ["answer_cases.json", "answer_cases_hard.json"]
HERE = Path(__file__).parent


def _load_cases() -> list[dict]:
    cases = []
    for f in FILES:
        for c in json.loads((HERE / f).read_text(encoding="utf-8")):
            c["_set"] = f.removesuffix(".json")
            cases.append(c)
    return cases


def _sync_dataset(client: Client, refresh: bool) -> None:
    exists = client.has_dataset(dataset_name=DATASET)
    if exists and refresh:
        client.delete_dataset(dataset_name=DATASET)
        exists = False
    if exists:
        return
    ds = client.create_dataset(DATASET, description="POS assistant answer-eval cases (easy + hard)")
    cases = _load_cases()
    client.create_examples(
        dataset_id=ds.id,
        inputs=[{"turns": c.get("turns") or [c["question"]]} for c in cases],
        # what a good answer must / must not contain: used by the evaluator
        outputs=[
            {k: c[k] for k in ("contains", "expect_unknown", "expect_clarify", "forbid") if k in c}
            for c in cases
        ],
        metadata=[{"name": c.get("name") or c["question"][:60], "set": c["_set"]} for c in cases],
    )
    print(f"created dataset '{DATASET}' with {len(cases)} examples")


def target(inputs: dict) -> dict:
    """What is being tested: send the conversation to the live /chat."""
    async def run() -> str:
        conversation_id = "eval-" + uuid.uuid4().hex[:8]
        reply = ""
        for message in inputs["turns"]:
            reply, conversation_id = await ask(message, conversation_id)
            await asyncio.sleep(4)  # stay under the free-tier per-minute limit
        return reply

    return {"reply": asyncio.run(run())}


def answer_checks(outputs: dict, reference_outputs: dict) -> dict:
    """Same string checks as run_answers.py, as a LangSmith evaluator."""
    reply = outputs["reply"]
    if reply.startswith("[HTTP") or reply.startswith("[no reply"):
        return {"key": "checks_pass", "score": None, "comment": "infrastructure error, not a real result"}
    problems = check(reference_outputs, reply)
    return {"key": "checks_pass", "score": 0 if problems else 1, "comment": "; ".join(problems)}


if __name__ == "__main__":
    args = [a for a in sys.argv[1:] if not a.startswith("--")]
    client = Client(api_key=os.environ["LANGSMITH_EVAL_API_KEY"])
    _sync_dataset(client, refresh="--refresh" in sys.argv)
    evaluate(
        target,
        data=DATASET,
        evaluators=[answer_checks],
        experiment_prefix=args[0] if args else "baseline",
        max_concurrency=1,  # one chat at a time: the free-tier quota is tiny
        client=client,
    )