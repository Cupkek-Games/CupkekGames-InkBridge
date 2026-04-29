# CupkekGames Ink — AI Agent Instructions

## Package Overview

**CupkekGames Ink** (`com.cupkekgames.inkbridge`) integrates the Ink narrative engine with Luna UI.

## Critical: Do not hand-edit Unity serialized assets or `.meta` files

Apply scene/prefab/SO changes in Unity Editor; preserve `.meta` GUIDs.

## Package Structure

```
com.cupkekgames.inkbridge/
  package.json
  README.md
  AGENTS.md
  CupkekGamesInk/                 ← CupkekGames.Luna.Ink.asmdef
    Runtime/                        (InkStoryControllerBase, narrative event bus)
```

## Dependencies

- `com.cupkekgames.keyvaluedatabases` (story character database)
- `com.cupkekgames.luna`
- **External: `com.inkle.ink-unity-integration`** (third-party; user installs separately per inkle instructions)

## Coding Conventions

- **Namespace**: `CupkekGames.Luna.Ink` (kept for back-compat; package name is `com.cupkekgames.inkbridge` but namespace stays `Luna.Ink`)
- **Asmdefs**: GUID references; uses `versionDefines` for ink-unity-integration if needed
- **Strict typing**
