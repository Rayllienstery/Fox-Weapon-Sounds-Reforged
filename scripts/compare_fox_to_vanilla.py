"""Compare Fox upstream audio tree to vanilla 4.0.13 index (full mod)."""
from __future__ import annotations

import json
import os
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[1]
VANILLA_INDEX = REPO / "mapping" / "vanilla_audio_index.json"
SKIP = REPO / "mapping" / "skip_list.json"
OUT = REPO / "mapping" / "port_intersection.json"


def list_bundles(root: Path) -> set[str]:
    if not root.is_dir():
        return set()
    names: set[str] = set()
    for p in root.rglob("*.bundle"):
        names.add(p.stem.lower())
    return names


def main() -> int:
    upstream = REPO / "_upstream"
    if len(sys.argv) > 1:
        upstream = Path(sys.argv[1])

    # Accept either extracted tree or nested **/audio/{banks,weapons}
    banks_dir = weapons_dir = None
    for cand in [upstream, *upstream.rglob("audio")]:
        b, w = cand / "banks", cand / "weapons"
        if b.is_dir() and w.is_dir():
            banks_dir, weapons_dir = b, w
            break

    if banks_dir is None:
        print("Fox audio tree not found under", upstream)
        print("Extract Fox 3.0.1 into _upstream/ so that .../audio/banks and .../audio/weapons exist.")
        return 1

    vanilla = json.loads(VANILLA_INDEX.read_text(encoding="utf-8"))
    skip = json.loads(SKIP.read_text(encoding="utf-8"))
    skip_banks = {x.lower() for x in skip.get("skip_banks", [])}
    skip_weapons = {x.lower() for x in skip.get("skip_weapons", [])}

    fox_banks = list_bundles(banks_dir)
    fox_weapons = list_bundles(weapons_dir)
    van_banks = {k.lower() for k in vanilla["banks"] if not vanilla["banks"][k].get("dir")}
    van_weapons = {k.lower() for k in vanilla["weapons"] if not vanilla["weapons"][k].get("dir")}

    port_banks = sorted((fox_banks & van_banks) - skip_banks)
    port_weapons = sorted((fox_weapons & van_weapons) - skip_weapons)
    fox_only_banks = sorted(fox_banks - van_banks)
    fox_only_weapons = sorted(fox_weapons - van_weapons)

    result = {
        "fox_audio_root": str(banks_dir.parent),
        "port_banks": port_banks,
        "port_weapons": port_weapons,
        "skipped_banks": sorted(skip_banks),
        "skipped_weapons": sorted(skip_weapons),
        "fox_only_banks_no_vanilla": fox_only_banks,
        "fox_only_weapons_no_vanilla": fox_only_weapons,
        "counts": {
            "port_banks": len(port_banks),
            "port_weapons": len(port_weapons),
            "fox_banks": len(fox_banks),
            "fox_weapons": len(fox_weapons),
        },
    }
    OUT.write_text(json.dumps(result, indent=2), encoding="utf-8")
    print(json.dumps(result["counts"], indent=2))
    print("wrote", OUT)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
