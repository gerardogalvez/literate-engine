using UnityEngine;

namespace MoodyBlues.BeatEmUp
{
    public class HealthObjectFactory
    {
        #region Fields

        private readonly GameObject healthObjectPrefab;

        #endregion

        #region Methods

        public HealthObjectFactory(GameObject prefab)
        {
            this.healthObjectPrefab = prefab;
        }

        public void CreateHealthObject(float playerHealthPercentageToRecover, Vector2 pos)
        {
            GameObject p = Object.Instantiate(healthObjectPrefab, pos, Quaternion.identity);
            p.GetComponent<HealthObject>().Initialize(playerHealthPercentageToRecover);
        }

        #endregion
    }
}
