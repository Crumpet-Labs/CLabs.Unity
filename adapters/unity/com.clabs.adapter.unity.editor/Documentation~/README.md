# CLabs Adapter: Editor (Unity)

> **Status: work-in-progress.** Shipped publicly as a foundation dependency of other adapters' editor windows, but the surface is not finalised. Some components are scaffolded but incomplete, some styling tokens aren't fully wired, and parts of the public API may move before the framework stabilises. Treat this as the underpinning that makes the `*ViewerWindow` / `*ManagerWindow` editor tools in CLabs adapters work — not as a stable framework to build your own large editor surfaces against.

Shared Unity editor framework used across every other adapter's editor tooling. Not a wrapper for a core package — this is foundational UI infrastructure.

## What this provides

| Type | Purpose |
|---|---|
| `CLabsEditorWindow` | Abstract base for panel-style editor windows (header, theme, status bar) |
| `CLabsGraphWindow` | Abstract base for graph-based editor windows (header, theme, property panel, status bar) |
| `CLabsHeader` | Reusable window-header component |
| `CLabsPanel` | Collapsible titled panel with a `ScrollView` body |
| `CLabsToolbar` | Toolbar with search field + `AddButton` fluent API |
| `CLabsStatusBar` | Status bar with label + boolean status dots |

All components are styled by `CrumpetLabs.uss` (not shown in the index; lives in the Editor folder). The theme uses design tokens (`--clabs-spacing-*`, `--clabs-font-*`, `--clabs-surface-*`).

## Setup

Inherit `CLabsEditorWindow` or `CLabsGraphWindow` in your window class, override `CreateGUI()`, and compose `CLabsPanel`/`CLabsToolbar`/`CLabsStatusBar` into the root `VisualElement`.

## Dependencies

No asmdef references — this is the foundation layer that other editor asmdefs build on top of.

## See also

- [../Code-Index.md](../Code-Index.md)
