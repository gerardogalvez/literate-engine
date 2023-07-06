using UnityEngine;

namespace MoodyBlues.BeatEmUp
{
    public abstract class SavableBehaviour : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private float timeToSave; // Assigned in inspector

        public float TimeToSave => this.timeToSave;

        public bool IsBeingSaved;

        public GameObject SavedBy;

        #endregion

        #region Methods

        public abstract void OnSave();

        #endregion
    }
}
