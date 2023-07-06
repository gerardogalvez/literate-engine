using UnityEngine;

namespace MoodyBlues.BeatEmUp
{
    public class EntityManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject Malandro;

        [SerializeField]
        private GameObject MalandroConPistola;

        [SerializeField]
        private GameObject Brayan;

        [SerializeField]
        private GameObject Civilian;

        private Vector2 GetEnemySpawnPosition()
        {
            Vector2 spawnPos;
            float leftBound = GameManager.instance.LeftBoundary;
            float rightBound = GameManager.instance.RightBoundary;
            float upperBound = GameManager.instance.UpBoundary;
            float lowerBound = GameManager.instance.DownBoundary;
            if (Random.value <= 0.5f)
            {
                spawnPos = new Vector2(leftBound - (rightBound - leftBound) * Random.Range(0.2f, 0.3f), Random.Range(lowerBound + (upperBound - lowerBound) * 0.4f, upperBound));
            }
            else
            {
                spawnPos = new Vector2(rightBound + (rightBound - leftBound) * Random.Range(0.2f, 0.3f), Random.Range(lowerBound + (upperBound - lowerBound) * 0.4f, upperBound));
            }

            return spawnPos;
        }

        private Vector2 GetCivilianSpawnPosition()
        {
            float leftBound = GameManager.instance.LeftBoundary;
            float rightBound = GameManager.instance.RightBoundary;
            float upperBound = GameManager.instance.UpBoundary;
            float lowerBound = GameManager.instance.DownBoundary;
            float horizontalDiff = rightBound - leftBound;
            float verticalDiff = upperBound - lowerBound;
            return new Vector2(Random.Range(leftBound + horizontalDiff * 0.15f, rightBound - horizontalDiff * 0.15f), Random.Range(lowerBound + verticalDiff * 0.3f, upperBound - verticalDiff * 0.15f));
        }

        public void SpawnMalandro()
        {
            Instantiate(this.Malandro, this.GetEnemySpawnPosition(), this.Malandro.transform.rotation);
        }

        public void SpawnMalandro(ColorChange colorChange)
        {
            GameObject malandro = Instantiate(this.Malandro, this.GetEnemySpawnPosition(), this.Malandro.transform.rotation);

            malandro.GetComponent<BaseEnemyBehaviour>().SetBaseMultipliers(
                colorChange.BaseHealthMultiplier,
                colorChange.BaseMovementSpeedMultiplier,
                colorChange.BaseDamageMultiplier);
        }

        public void SpawnMalandroConPistola()
        {
            Instantiate(this.MalandroConPistola, this.GetEnemySpawnPosition(), this.MalandroConPistola.transform.rotation);
        }

        public void SpawnMalandroConPistola(ColorChange colorChange)
        {
            GameObject malandroConPistola = 
                Instantiate(this.MalandroConPistola, this.GetEnemySpawnPosition(), this.MalandroConPistola.transform.rotation);

            malandroConPistola.GetComponent<BaseEnemyBehaviour>().SetBaseMultipliers(
                colorChange.BaseHealthMultiplier,
                colorChange.BaseMovementSpeedMultiplier,
                colorChange.BaseDamageMultiplier);
        }

        public void SpawnBrayan()
        {
            Instantiate(this.Brayan, this.GetEnemySpawnPosition(), this.Brayan.transform.rotation);
        }

        public void SpawnBrayan(ColorChange colorChange)
        {
            GameObject brayan = Instantiate(this.Brayan, this.GetEnemySpawnPosition(), this.Brayan.transform.rotation);

            brayan.GetComponent<BaseEnemyBehaviour>().SetBaseMultipliers(
                colorChange.BaseHealthMultiplier,
                colorChange.BaseMovementSpeedMultiplier,
                colorChange.BaseDamageMultiplier);
        }

        public void SpawnCivilian()
        {
            Instantiate(this.Civilian, this.GetCivilianSpawnPosition(), this.Civilian.transform.rotation);
        }
    }
}
