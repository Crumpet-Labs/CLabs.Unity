using Buttr.Injection;
using CLabs.Belfry;
using UnityEngine;

namespace CLabs.Belfry.Samples {
    /// <summary>
    /// Rings the pass bell. Press Space (or use the inspector context menu) to announce a batch.
    /// The bell does not know — or care — who is listening.
    /// </summary>
    public sealed partial class PassBell : MonoBehaviour {
        [Inject] private IBellTower i_Tower;

        [SerializeField] private string m_Table = "Table 3";
        [SerializeField] private int m_Count = 2;

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) Ring();
        }

        [ContextMenu("Ring the pass bell")]
        public void Ring() =>
            i_Tower.Rope(PassBellKeys.Service).RingBell(new CrumpetReady(m_Table, m_Count));
    }
}
