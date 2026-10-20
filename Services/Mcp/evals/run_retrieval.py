import asyncio
import json
import sys
from pathlib import Path

from rag.retrieve import search

CASES = Path(__file__).with_name("retrieval_cases.json")
K = 4


def key(p: dict) -> str:
    return f"{p['title']} / {p['section']}"


async def main() -> int:
    cases = json.loads(CASES.read_text(encoding="utf-8"))
    answerable = hits = leaks = 0
    reciprocal_ranks: list[float] = []
    top1_answerable: list[float] = []
    top1_unanswerable: list[float] = []
    failed = False

    for c in cases:
        results = await search(c["question"], limit=K)
        keys = [key(p) for p in results]
        top1 = results[0]["score"] if results else 0.0
        status, detail = "info", ""

        if "expect" in c:
            answerable += 1
            top1_answerable.append(top1)
            ranks = [keys.index(e) + 1 for e in c["expect"] if e in keys]
            if ranks:
                hits += 1
                reciprocal_ranks.append(1 / min(ranks))
                status, detail = "PASS", f"rank {min(ranks)}"
            else:
                reciprocal_ranks.append(0.0)
                status, detail = "FAIL", f"not in top {K}; got {keys[0] if keys else 'nothing'}"
                failed = True
        elif "forbid" in c:
            bad = [f for f in c["forbid"] if f in keys]
            if bad:
                leaks += 1
                status, detail = "LEAK", ", ".join(bad)
                failed = True
            else:
                status, detail = "PASS", "forbidden doc not returned"
        else:
            top1_unanswerable.append(top1)
            detail = f"unanswerable; top hit {keys[0] if keys else 'nothing'}"

        print(f"{status:5} {top1:.3f}  {c['question'][:58]:58}  {detail}")
        await asyncio.sleep(0.3)  # be gentle with the free embedding quota

    print()
    print(f"hit@{K}: {hits}/{answerable}  ({hits / answerable:.0%})")
    print(f"MRR:    {sum(reciprocal_ranks) / answerable:.2f}   (1.0 = right chunk always first)")
    print(f"leaks:  {leaks}")
    if top1_answerable:
        print(f"avg top score, answerable questions:   {sum(top1_answerable) / len(top1_answerable):.3f}")
    if top1_unanswerable:
        print(f"avg top score, unanswerable questions: {sum(top1_unanswerable) / len(top1_unanswerable):.3f}")
    return 1 if failed else 0


if __name__ == "__main__":
    sys.exit(asyncio.run(main()))