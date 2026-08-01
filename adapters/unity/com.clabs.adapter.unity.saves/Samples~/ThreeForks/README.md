# Three Forks — Fork sample

Three save slots — three "forks" — for three playthroughs. Create, load, list, and delete them, each
with its own payload. Fork does the dangerous parts (integrity checks, write-then-swap, migration) so
you just hand it serializable data.

## Run it
1. Import this sample.
2. Create **Assets → Create → CLabs → Fork → Application Loader** and (recommended) a
   **Fork Configuration SO** so saves land under `Application.persistentDataPath`. Wire the loader
   into your app's loader set.
3. Add `SaveBench` to a GameObject. In the inspector, set a Slot Id + Chef Name.
4. Right-click `SaveBench` → **Save**, then **Load**, **List slots**, **Delete slot**. Watch the Console.
5. Open **Window → Crumpet Labs → Fork Viewer** to see the slots on disk.

## What to look at
- `IForkService.SaveAsync(slotId, data)` / `LoadAsync<T>(slotId)` — async, returns a `Ticket` you `await`.
- The **result types**: `SaveResult.Success/Reason`, and `SaveLoadResult<T>.Status` (Ok / Migrated /
  FromBackup / failure) — Fork tells you *how* a load succeeded, not just whether.
- `GetAvailableSlots()` → `SaveSlotInfo[]` with `SlotId`, `LastSaveTime`, `SchemaVersion`, and more.

## Stretch: schema migration
Add a field to `ChefSave`, bump the schema version, and register an `ISaveMigrationStep` via
`RegisterMigrationStep`. Load an old slot and watch `Status == Migrated`.
