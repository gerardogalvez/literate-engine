using UnityEngine;

namespace MoodyBlues.BeatEmUp
{
    public class FollowPlayer : MonoBehaviour
    {

        [HideInInspector]
        public GameObject owner;

        public float verticalOffset;

        private void LateUpdate()
        {
            this.transform.position = this.owner.transform.position + new Vector3(0, verticalOffset, 0);
        }
    }
}

