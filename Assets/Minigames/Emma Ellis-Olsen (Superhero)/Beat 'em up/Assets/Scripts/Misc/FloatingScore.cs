using UnityEngine;

namespace MoodyBlues.BeatEmUp
{
    [RequireComponent(typeof(TextMesh))]
    public class FloatingScore : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private float timeToDestroy;

        [SerializeField]
        private float speed;

        #endregion

        #region Methods

        // Use this for initialization
        private void Start()
        {
            Destroy(this.gameObject, timeToDestroy);
        }

        // Update is called once per frame
        private void Update()
        {
            this.transform.Translate(Vector3.up * speed * Time.deltaTime);
        }

        public void SetText(string t)
        {
            this.gameObject.GetComponent<TextMesh>().text = t;
        }

        #endregion
    }
}
