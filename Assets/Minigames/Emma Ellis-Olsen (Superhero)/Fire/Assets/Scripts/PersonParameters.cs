using UnityEngine;

namespace MoodyBlues.Fire
{
    public class PersonParameters
    {
        public readonly GameObject owner;
        public readonly Transform endingPoint;
        public readonly float tickSpeed;
        public Transform startingPoint;

        public PersonParameters(GameObject owner, Transform startingPoint, Transform endingPoint, float tickSpeed)
        {
            this.owner = owner;
            this.startingPoint = startingPoint;
            this.endingPoint = endingPoint;
            this.tickSpeed = tickSpeed;
        }
    }
}
