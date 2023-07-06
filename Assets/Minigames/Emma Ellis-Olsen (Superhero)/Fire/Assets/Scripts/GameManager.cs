using UnityEngine;
using System.Collections;

namespace MoodyBlues.Fire
{
    public class GameManager : MonoBehaviour
    {

        private readonly RangeInt bounceHeightRange = new RangeInt(5, 12);

        public int MaxPlayerHorizontalMovement => 5;

        public int TicksPerBounce => 10;

        public RangeInt BounceHeightRange => this.bounceHeightRange;

        public static GameManager instance;

        [Range(4.0f, 8.0f)]
        public float timeBetweenPerson;

        [Range(0.3f, 1.0f)]
        public float timeBetweenTicks;

        private PersonFactory factory;
        private PersonParameters playerParameters;
        private PersonParameters emmaParameters;
        private GameObject[] startingPoints;

        public GameObject personPrefab;
        public GameObject player; // Player
        public GameObject player2; // Emma
        public Transform endingPoint;

        void Awake()
        {
            Debug.Assert(this.personPrefab != null);
            Debug.Assert(this.player != null);
            // Debug.Assert(this.player2 != null);

            this.factory = new PersonFactory(this.personPrefab);
            this.startingPoints = GameObject.FindGameObjectsWithTag(MoodyBlues.Constants.Fire.Tags.StartingPoint);

            if (GameManager.instance == null)
            {
                GameManager.instance = this;
            }
        }

        private IEnumerator SpawnPerson(PersonParameters args)
        {
            while (true)
            {
                args.startingPoint = this.startingPoints[Random.Range(0, this.startingPoints.Length)].transform;
                this.factory.CreatePerson(args);
                yield return new WaitForSeconds(this.timeBetweenPerson);
            }

        }

        private void Start()
        {
            this.playerParameters = new PersonParameters(this.player, null, this.endingPoint, this.timeBetweenTicks);
            StartCoroutine(this.SpawnPerson(this.playerParameters));
        }
    }
}
