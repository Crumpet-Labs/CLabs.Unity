# Editor Framework by Example

A recipe cookbook for building editor windows on the CLabs editor framework. Each recipe is independent; copy whichever you need.

## The mental model

The framework gives you **two window bases** (`CLabsEditorWindow` for panel-style, `CLabsGraphWindow` for graph-style) and **eight reusable components**. Bases own the layout chrome: header, theme load, status bar and editor tick. Components are composed into the content area.

```
   CLabsEditorWindow                 CLabsGraphWindow
   ┌──────────────────────────┐      ┌─────────────────────────────────┐
   │ CLabsHeader (auto)       │      │ CLabsHeader (auto)              │
   ├──────────────────────────┤      ├─────────────────────────────────┤
   │ OnCreateContent(root)    │      │ OnCreateToolbar(toolbar)        │
   │  ├ CLabsToolbar          │      ├─────────────────────────────────┤
   │  ├ clabs-panels          │      │ Graph view  │  PropertyPanel    │
   │  │   ├ CLabsPanel        │      │             │  ├ CLabsDetailRow │
   │  │   └ CLabsPanel        │      │             │  └ ...            │
   │  └ CLabsEmptyState       │      ├─────────────┴───────────────────┤
   ├──────────────────────────┤      │ CLabsStatusBar (auto)           │
   │ CLabsStatusBar           │      └─────────────────────────────────┘
   └──────────────────────────┘
```

---

## Recipes

### 1. Minimal panel-style window

```csharp
using CLabs.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class MinimalWindow : CLabsEditorWindow {
    protected override string WindowTitle => "Minimal";

    [MenuItem("Window/Crumpet Labs/Minimal")]
    public static void ShowWindow() => GetWindow<MinimalWindow>();

    private CLabsStatusBar m_StatusBar;

    protected override void OnCreateContent(VisualElement root) {
        var panel = CLabsPanel.Create("Items");
        panel.Content.Add(new Label("Hello"));

        m_StatusBar = CLabsStatusBar.Create();

        root.Add(panel);
        root.Add(m_StatusBar);
    }
}
```

Header is wired automatically. `OnEditorUpdate()` is available as an override but not required.

### 2. Window with a toolbar

```csharp
protected override void OnCreateContent(VisualElement root) {
    var toolbar = CLabsToolbar.Create(onSearch: text => Filter(text))
        .AddButton("Refresh", Refresh)
        .AddButton("Export", Export);

    var panel = CLabsPanel.Create("Items");

    root.Add(toolbar);
    root.Add(panel);
}
```

`SearchField` is exposed as a property if you need to set its placeholder or subscribe to other events.

### 3. Row composition

```csharp
panel.Content.Add(
    CLabsRow.Create("Iron Sword", onClick: () => Select(itemId))
        .Add(CLabsBadge.Create("Lv 3", BadgeKind.Accent))
        .Add(CLabsBadge.Create("Equipped", BadgeKind.Success))
);
```

`Add()` is fluent and returns the row, so badges and inline values chain. The click handler captures whatever local you close over.

### 4. Badges: fixed and dynamic colour

```csharp
// Fixed semantic kinds: type-safe, themed by CSS.
row.Add(CLabsBadge.Create("Active", BadgeKind.Success));
row.Add(CLabsBadge.Create("Warning", BadgeKind.Warning));

// Data-driven colour, with auto-contrast text.
row.Add(CLabsBadge.Create(season.DisplayName, season.Color.ToUnityColor()));

// Long-lived badge mutated each frame (e.g. world-time phase):
m_PhaseBadge = CLabsBadge.Create("--", BadgeKind.Accent);
// ... later, in OnEditorUpdate:
m_PhaseBadge.text = phase.DisplayName;
m_PhaseBadge.style.backgroundColor = phase.Color.ToUnityColor();
m_PhaseBadge.style.color = CLabsBadge.ContrastColor(phase.Color.ToUnityColor());
```

### 5. Empty state with `BindTo`

```csharp
private CLabsPanel m_ListPanel;
private CLabsPanel m_DetailPanel;
private CLabsEmptyState m_EmptyState;

protected override void OnCreateContent(VisualElement root) {
    var panels = new VisualElement();
    panels.AddToClassList("clabs-panels");

    m_ListPanel = CLabsPanel.Create("Items");
    m_DetailPanel = CLabsPanel.Create("Details");
    panels.Add(m_ListPanel);
    panels.Add(m_DetailPanel);

    m_EmptyState = CLabsEmptyState
        .Create("No items; enter Play mode")
        .BindTo(m_ListPanel, m_DetailPanel);

    root.Add(panels);
    root.Add(m_EmptyState);
}

protected override void OnEditorUpdate() {
    var items = FindItems();
    m_EmptyState.SetVisible(items.Length == 0);  // flips bound panels in opposite direction
}
```

`BindTo` is optional. For more complex visibility logic, such as multi-mode editor and play switching, call `SetVisible(bool)` on the empty-state and manage sibling panels directly.

### 6. Progress bar

```csharp
// Normalised input: the most common form.
var bar = CLabsProgressBar.Create(showLabel: false, height: 6);
bar.SetProgress(snapshot.Progress);                  // 0..1
card.Add(bar);

// Current/max input, for raw counters.
var cooldownBar = CLabsProgressBar.Create();
cooldownBar.SetProgress(snapshot.Cooldown, snapshot.MaxCooldown);
cooldownBar.SetFillColor(snapshot.IsReady ? Color.green : Color.yellow);

// With built-in label (anchored right).
var xpBar = CLabsProgressBar.Create(showLabel: true, height: 8);
xpBar.SetProgress(xp.Current, xp.Required)
     .SetLabel($"{xp.Current} / {xp.Required} XP");
```

### 7. Detail row in a property panel

```csharp
// String value: the shortcut form.
panel.Content.Add(CLabsDetailRow.Create("Status").SetValue("Unlocked"));

// Badge value.
panel.Content.Add(CLabsDetailRow.Create("Tier").SetValue(
    CLabsBadge.Create(tier.DisplayName, tier.Color.ToUnityColor())));

// Composed value: bind a PropertyField into the value container.
var field = new IntegerField { value = state.Rank };
field.RegisterValueChangedCallback(evt => Apply(evt.newValue));
panel.Content.Add(CLabsDetailRow.Create("Req. Rank").SetValue(field));

// Multi-element value via AddValue.
panel.Content.Add(CLabsDetailRow.Create("Ranks")
    .AddValue(CLabsBadge.Create($"{state.Effective}/{state.Max}", BadgeKind.Accent))
    .AddValue(new Label($"({state.Purchased} purchased)")));
```

Default label width is 90px; pass a second arg to `Create(label, labelWidth)` for narrower (e.g. arrow prefixes at 16px) or wider rows.

### 8. Graph-style window

```csharp
using UnityEditor.Experimental.GraphView;
using CLabs.Editor;
using UnityEngine.UIElements;

public sealed class MyGraphWindow : CLabsGraphWindow {
    protected override string WindowTitle => "My Graph";

    private MyGraphView m_GraphView;

    protected override GraphView OnCreateGraphView() {
        m_GraphView = new MyGraphView();
        m_GraphView.OnSelectionUpdated += UpdateProperties;
        return m_GraphView;
    }

    protected override void OnCreateToolbar(VisualElement toolbar) {
        toolbar.Add(new Button(Refresh) { text = "Refresh" });
    }

    private void UpdateProperties() {
        var content = PropertyPanel.Content;
        content.Clear();

        if (m_GraphView.Selected is MyNode node) {
            content.Add(CLabsDetailRow.Create("Name").SetValue(node.DisplayName));
            content.Add(CLabsDetailRow.Create("Type").SetValue(node.Kind.ToString()));
            StatusBar.Text.text = $"Selected: {node.DisplayName}";
        }
    }
}
```

`Graph` (the `GraphView`), `PropertyPanel` (a `CLabsPanel`), and `StatusBar` are exposed as protected properties on the base. Override `OnGraphDestroy()` to clean up event subscriptions.

---

## Naming conventions

The framework uses BEM-style class names: `clabs-<component>__<part>--<modifier>`. Examples:

- `clabs-row` / `clabs-row__name` / `clabs-row--selected`
- `clabs-badge` / `clabs-badge--accent`
- `clabs-detail-row` / `clabs-detail-row__label` / `clabs-detail-row__value`

When you write adapter-specific USS, use **your own prefix** rather than extending `clabs-*`, for example `fork-toolbar__owner-section` or `sprig-properties__buttons`. The `clabs-*` namespace belongs to this framework and may be reorganised between versions.
