# Porting Fox sounds to SPT 4.0.13

## Why rebuild?

Fox 3.8 ships Unity asset bundles for an older client. On 4.0.13, AudioClip
pathIDs and bank MonoBehaviours differ. Dropping old bundles causes:

- invisible weapon parts
- silent guns
- broken animations (historical MP-153 / Glock issues)

## Pipeline (full mod)

1. **Ingest** Fox 3.0.1 into `_upstream/` and extract.
2. **Index** Fox `audio/banks` + `audio/weapons` vs
   `mapping/vanilla_audio_index.json`.
3. **Intersect** — port only names present on both sides; respect
   `mapping/skip_list.json`.
4. **Per bank:**
   - Export AudioClips from Fox weapons bundle (AssetStudio).
   - Open vanilla 4.0 weapons bundle (UABEA / AssetsTools.NET).
   - Replace clip data keeping asset names when possible.
   - If pathIDs diverge, edit vanilla bank MonoBehaviours to point at new IDs
     (Fox's own post-16584 workflow).
5. Write results to `dist/audio/{banks,weapons}/`.
6. `scripts/install.ps1` → StreamingAssets; playtest; `restore.ps1` on failure.

## Tools

- AssetStudio (GUI) — inventory / WAV export
- UABEA — bundle edit
- AssetsTools.NET — scripted replace (optional later)

## Do not touch

- `weapons/<gun>/weapon_*_container.bundle`
- texture / mesh client_assets
- anything outside `audio/banks` and `audio/weapons` unless inventory proves
  a required dependency
