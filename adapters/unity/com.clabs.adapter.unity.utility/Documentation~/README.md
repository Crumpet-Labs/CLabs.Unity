# CLabs Adapter: Utility (Unity)

Unity bindings for `com.clabs.utility`, the foundational adapter. Every other adapter references this one. Supplies colour conversions, `OwnerId` resolution, player-loop injection, property wrappers, editor drawers, and a set of Unity-flavoured helpers.

## What this provides

**Attributes + drawers:**
- `RequireInterfaceAttribute`: `[RequireInterface(typeof(IFoo))]` on `[SerializeField] ScriptableObject` fields (drawer in the Editor asmdef).
- `ReadOnlyAttribute` + `ReadOnlyDrawer`: show a serialised field as disabled.
- `EditorButton` + `EditorButtonUIE`: decorate a serialised field to render a button that fires an `Action`.

**Conversions + helpers:**
- `ColorExtensions`: `ToUnityColor()` / `ToCLabsColor()` (both `Color` and `Color32`).
- `MonoBehaviourExtensions.GetOwnerId()` / `GameObject.GetOwnerId()`: resolve `OwnerId` from Unity scene objects.
- `CameraExtensions`, `RectTransformExt`, `TransformUtils`, `GameObjectUtils`, `CoroutineUtils`.

**Infrastructure:**
- `PlayerLoopInjector`: `InjectUpdate<T>(Action)` hooks a custom update into the Unity `PlayerLoop`.
- `Property<T>` (+ `BoolProperty`, `Vector2Property`): observable value wrapper with a dirty flag.
- `SerializableDictionary<TKey, TValue>`: `ISerializationCallbackReceiver`-backed dictionary usable in the Inspector.

**Editor (in `CLabs.Utility.Unity.Editor`):**
- `RequireInterfaceDrawer`: drawer that constrains an SO field to implementers of an interface.
- `ScriptableObjectList<T>`: utility for building `ListView`s of SOs in editor windows.
- `AddressableUtilities.GetAssetAddress(...)`: resolve a Unity object's Addressables address (requires `com.unity.addressables` in the consumer project; version-gated, not bundled).
- `SerializableDictionaryDrawer`, `UIToolkitUtils`.

## Setup

This adapter is consumed transitively by every other adapter. If you're writing your own adapter or editor tool, reference `CLabs.Utility.Unity` (Runtime) and `CLabs.Utility.Unity.Editor` (Editor).

## Dependencies

- `com.clabs.utility`: core engine-agnostic types (`OwnerId`, `Color`, `Registry<,>`)
- `com.crumpetlabs.buttr.unity`
- `com.unity.addressables` (optional, consumer-provided): enables `AddressableUtilities.GetAssetAddress`

## See also

- [../Code-Index.md](../Code-Index.md)
- The `com.clabs.utility` package's `Documentation~/README.md`
