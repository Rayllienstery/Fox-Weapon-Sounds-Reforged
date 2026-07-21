# Fox Weapon Sounds Reforged

Full port of **Fox's Weapon Sound Mod** to **SPT-AKI 4.0.13**.

Replaces vanilla weapon fire / foley audio by rebuilding into modern client
`audio/banks` + `audio/weapons` bundles. **Not** a drop-in of SPT 3.8 Fox
bundles (those break meshes / pathIDs on 4.0).

| | |
|--|--|
| **Version** | **0.1.0** (WIP — awaiting upstream ingest) |
| GitHub | https://github.com/Rayllienstery/Fox-Weapon-Sounds-Reforged |
| Changelog | [CHANGELOG.md](CHANGELOG.md) |
| Staging | `c:\Games\SPT\dev\Fox-Weapon-Sounds-Reforged` |
| Live overlay | `EscapeFromTarkov_Data/StreamingAssets/Windows/assets/content/audio/` |
| Upstream | Fox (NCSA) + Krinkova 3.8 update — see [NOTICE.md](NOTICE.md) |

---

## Identity

| Field | Value |
|-------|-------|
| Author | Raylee |
| SPT | **4.0.13** |
| Scope | **Full Fox pack** (all overlapping banks/weapons), not M4-only |
| License | NCSA (this repo) + upstream NOTICE |

## Versioning (`x.y.z`)

| Segment | When to bump |
|---------|----------------|
| **x** | Only when you explicitly ask for a major bump |
| **y** | New weapon families / significant port coverage |
| **z** | Bugfixes / clip remaps |

---

## Quick start

1. Place Fox **3.0.1** archive in `_upstream/` (gitignored).
2. Run inventory + rebuild (see [docs/PORTING.md](docs/PORTING.md)).
3. Install overlay:

```powershell
powershell -ExecutionPolicy Bypass -File scripts\install.ps1
```

4. Launch client, test guns in Hideout.
5. Rollback:

```powershell
powershell -ExecutionPolicy Bypass -File scripts\restore.ps1
```

**Do not** copy 3.8 Fox bundles straight into StreamingAssets.

---

## Coverage (target = full Fox set)

Upstream Fox historically covers (non-exhaustive): M4 / HK416 / ADAR / TX-15 /
MCX / AKs / VPO / RPK / VAL-VSS / MDR / FAL / SA-58 / Glocks / P226 / M9 /
MPX / MP7 / UMP / Vityaz / Saiga-9 / MP5 / pistols / shotguns / SVD / M1A /
M700 foley / SCAR / G28 / USP / Vector / many more.

On 4.0.13 we port every Fox bank that still has a matching
`audio/banks/<name>.bundle` and/or `audio/weapons/<name>.bundle`. Weapons that
only exist in newer clients stay vanilla. Known 3.8 breakages (e.g. TT/SKS
mesh issues, VAL/VSS/SR3M) go on a skip-list until remapped safely — see
`mapping/skip_list.json`.

Vanilla index: [`mapping/vanilla_audio_index.json`](mapping/vanilla_audio_index.json).

---

## Layout

```
_upstream/          # Fox 3.0.1 archive (local only)
vanilla-backup/     # copies of untouched 4.0.13 bundles
mapping/            # inventories + clip maps + skip-list
scripts/            # backup / install / restore
dist/audio/         # rebuilt banks + weapons for install
docs/PORTING.md
```

---

## Credits

- **Fox** — original Weapon Sound Mod
- **Krinkova** — SPT 3.8 updated package
- **BooMoon** — historical animation/asset fixes (per 3.8 thread)
- **Raylee** — 4.0.13 rebuild / this fork
