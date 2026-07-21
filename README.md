# Fox Weapon Sounds Reforged

Full port of **Fox's Weapon Sound Mod** (3.0.0) to **SPT-AKI 4.0.13**.

| | |
|--|--|
| **Version** | **0.1.0** |
| GitHub | https://github.com/Rayllienstery/Fox-Weapon-Sounds-Reforged |
| Staging | `c:\Games\SPT\dev\Fox-Weapon-Sounds-Reforged` |
| Live | `SPT/user/mods/Fox-Weapon-Sounds-Reforged/` |
| Upstream | FoxSoundMod 3.0.0 (`_upstream/`, gitignored) |

---

## How it works (same as original Fox)

1. `bundles.json` + `bundles/` register Fox `*sounds` packs, `*fox` audio banks, and **weapon container** overrides (same keys as vanilla).
2. C# PostDB loader remaps `item.Properties.Prefab.Path` so the client loads those containers.
3. Containers depend on Fox banks → new gunfire/foley.

**Risk on 4.0.13:** container bundles are from an older EFT client. Some guns may get invisible parts / bad anims. TT / SKS / VAL / VSS Prefab remaps are **skipped** (known 3.8 breakage). If other guns break, remove them from remaps or delete their container entries and re-deploy.

---

## Identity

| Field | Value |
|-------|-------|
| ModGuid | `com.raylee.fox-weapon-sounds-reforged` |
| Assembly | `Fox-Weapon-Sounds-Reforged.dll` |
| IsBundleMod | `true` |
| Author | Raylee (credits: Fox, Krinkova) |
| SPT | `~4.0.0` (tested target **4.0.13**) |

## Deploy

```powershell
# Requires extracted Fox 3.0.0 under _upstream\FoxSoundMod 3.0.0
powershell -ExecutionPolicy Bypass -File scripts\deploy.ps1
```

Then **restart SPT Server** and the game client.

## Coverage (0.1.0)

- **75** Prefab remaps applied (from Fox JS; 6 skipped).
- **42** fox banks, **13** sound packs, **69** weapon containers in upstream manifest.
- Details: `mapping/fox_upstream_inventory.json`, `mapping/prefab_remaps_filtered.json`.

## Credits

- **Fox** — original Weapon Sound Mod
- **Krinkova** — later SPT updates
- **Raylee** — 4.0.13 C# / bundle packaging fork
