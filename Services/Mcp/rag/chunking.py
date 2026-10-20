from dataclasses import dataclass
from pathlib import Path

MAX_CHARS = 1200


@dataclass
class Chunk:
    source: str      # file name
    title: str
    audience: str
    section: str
    text: str        # what gets embedded and later shown to the LLM
    index: int       # position within the file


def parse_front_matter(raw: str) -> tuple[dict[str, str], str]:
    """Split the '---' header (title, audience) from the body."""
    if not raw.startswith("---"):
        return {}, raw
    parts = raw.split("---", 2)
    if len(parts) < 3:
        return {}, raw
    meta = {}
    for line in parts[1].strip().splitlines():
        if ":" in line:
            key, value = line.split(":", 1)
            meta[key.strip()] = value.strip()
    return meta, parts[2].lstrip()


def split_sections(body: str) -> list[tuple[str, str]]:
    """Cut the body at each '## ' heading -> (section name, section text)."""
    sections: list[tuple[str, str]] = []
    name, lines = "Introduction", []
    for line in body.splitlines():
        if line.startswith("## "):
            sections.append((name, "\n".join(lines).strip()))
            name, lines = line[3:].strip(), []
        elif line.startswith("# "):
            continue  # the document title; already in the front matter
        else:
            lines.append(line)
    sections.append((name, "\n".join(lines).strip()))
    return [(n, t) for n, t in sections if t]


def split_long(text: str, max_chars: int = MAX_CHARS) -> list[str]:
    """If a section is very long, split it between paragraphs."""
    if len(text) <= max_chars:
        return [text]
    pieces, current = [], ""
    for para in text.split("\n\n"):
        if current and len(current) + len(para) + 2 > max_chars:
            pieces.append(current)
            current = para
        else:
            current = f"{current}\n\n{para}" if current else para
    pieces.append(current)
    return pieces


def chunk_document(path: Path) -> list[Chunk]:
    meta, body = parse_front_matter(path.read_text(encoding="utf-8"))
    title = meta.get("title", path.stem)
    audience = meta.get("audience", "staff")  # unknown = restricted (fail closed)

    chunks: list[Chunk] = []
    for section, text in split_sections(body):
        for piece in split_long(text):
            chunks.append(
                Chunk(
                    source=path.name,
                    title=title,
                    audience=audience,
                    section=section,
                    # context in front: "Faulty items" alone is ambiguous
                    text=f"{title} - {section}\n{piece}",
                    index=len(chunks),
                )
            )
    return chunks