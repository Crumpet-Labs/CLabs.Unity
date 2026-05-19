# CLabs Adapter: Editor (Unity)

Shared Unity editor framework used across every other adapter's editor tooling. Stable foundation — used by editor windows in every CLabs adapter. Not a wrapper for a core package; this is foundational UI infrastructure.

## What this provides

### Window bases

| Type | Purpose |
|---|---|
| `CLabsEditorWindow` | Abstract base for panel-style editor windows. Owns header, theme load, status-bar plumbing. |
| `CLabsGraphWindow` | Abstract base for graph-based editor windows. Owns header, toolbar, graph view, property panel, status bar. |

### Components

| Type | Purpose |
|---|---|
| `CLabsHeader` | Reusable window-header (`CRUMPET LABS · Window Title`). Created automatically by the base classes. |
| `CLabsPanel` | Titled panel with a `ScrollView` body. Compose into `clabs-panels` containers for side-by-side layouts. |
| `CLabsToolbar` | Toolbar with search field + fluent `AddButton(label, onClick)`. |
| `CLabsStatusBar` | Footer status bar with `.Text` label + fluent `AddDot(enabled)` for status indicators. |
| `CLabsRow` | Horizontal list-row with name + click callback + selected/disabled state. Compose badges via fluent `Add()`. |
| `CLabsBadge` | Static factory for coloured pills (`BadgeKind.Accent/Success/Warning/Error/Neutral`) plus a dynamic-color overload with auto-contrast text. |
| `CLabsEmptyState` | Empty-state label with `SetVisible(bool)` and `BindTo(...siblings)` — flips the empty-state and bound panels in opposite directions in one call. |
| `CLabsProgressBar` | Fill bar with `SetProgress(normalized)` or `SetProgress(current, max)`, optional label, optional fill colour. |
| `CLabsDetailRow` | Property-panel row with a fixed-width label and arbitrary value content. Compose `PropertyField`, badges, etc. into `ValueContainer`. |

All components are styled by `CrumpetLabs.uss` (in `Editor/Styles/`).

## Theme tokens

Adapters writing their own USS may reference these tokens:

| Family | Tokens |
|---|---|
| Spacing | `--clabs-spacing-xs/sm/md/lg/xl` (2/4/8/12/16px) |
| Typography | `--clabs-font-xs/sm/md/lg` (9/10/12/14px) |
| Surfaces | `--clabs-surface-0/1/2`, `--clabs-border` |
| Brand | `--clabs-accent`, `--clabs-accent-hover`, `--clabs-accent-text`, `--clabs-secondary`, `--clabs-secondary-hover`, `--clabs-brand-orange/gold/pink/rose/magenta/sky/cyan/blue` |
| Semantic | `--clabs-success`, `--clabs-warning`, `--clabs-error`, `--clabs-info`, `--clabs-neutral` |

The brand palette is the extension surface — adapters can pull a brand colour into their own selectors without hard-coding hex. Some tokens (e.g. `--clabs-accent-hover`, `--clabs-info`, `--clabs-secondary-hover`) are declared but not yet referenced inside the framework — they're available for adapter-side use today and the framework will wire them in as more components arrive.

## Composition patterns

### Panel-style window

Subclass `CLabsEditorWindow`. The base wires the header, theme, and editor-tick. You override `WindowTitle` + `OnCreateContent(root)` and compose `CLabsPanel` / `CLabsToolbar` / `CLabsStatusBar` / `CLabsEmptyState` into the root. Optional `OnEditorUpdate()` runs every frame. See `DoughViewerWindow` in `com.clabs.adapter.unity.dough` for a canonical full example exercising every component.

### Graph-style window

Subclass `CLabsGraphWindow`. The base builds the split layout (header / toolbar / graph view / property panel / status bar). You override `OnCreateGraphView()` to provide the `GraphView` instance, optionally `OnCreateToolbar(toolbar)` to populate the toolbar, and write into `PropertyPanel.Content` from your selection-change callbacks. `CLabsDetailRow` is the property-panel building block. See `SprigGraphWindow` in `com.clabs.adapter.unity.sprig` for a canonical full example.

### Row composition

```csharp
panel.Content.Add(
    CLabsRow.Create("Iron Sword", onClick: () => Select(item))
        .Add(CLabsBadge.Create("Lv 3", BadgeKind.Accent))
        .Add(CLabsBadge.Create("Equipped", BadgeKind.Success))
);
```

`CLabsRow.Add` is fluent — it returns `this` and shadows `VisualElement.Add` so badges and value labels chain cleanly. `SetSelected(bool)` and `SetDisabled(bool)` toggle the visual state.

## Setup

Add a dependency on `com.clabs.adapter.unity.editor` in your adapter's `package.json`, then in your editor asmdef reference `CLabs.Editor.Unity.Editor`. The base classes load `CrumpetLabs.uss` automatically; if your window needs additional styling, override `PackageStyleSheetPath` to return your sheet's relative path.

## Dependencies

No asmdef references — this is the foundation layer that other editor asmdefs build on top of.

## See also

- [Example.md](Example.md) — recipe cookbook with copy-paste snippets for every component
- [../Code-Index.md](../Code-Index.md)
