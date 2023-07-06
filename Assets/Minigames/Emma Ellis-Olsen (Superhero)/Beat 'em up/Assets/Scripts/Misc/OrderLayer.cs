using UnityEngine;

namespace MoodyBlues.BeatEmUp
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer))]
    public class OrderLayer : MonoBehaviour
    {
        #region Fields

        private SpriteRenderer spriteRenderer;

        #endregion

        #region Methods

        // Use this for initialization
        private void Start()
        {
            this.spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        private void Update()
        {
            this.spriteRenderer.sortingOrder = -1 * Mathf.RoundToInt(this.transform.position.y);
        }

        #endregion
    }
}

