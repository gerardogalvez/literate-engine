using UnityEngine;
using MoodyBlues.Constants.BeatEmUp;

namespace MoodyBlues.BeatEmUp
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class HealthObject : MonoBehaviour
    {
        #region Fields

        private float playerHealthPercentageToRecover;
        private SpriteRenderer spriteRenderer;

        #endregion

        #region Methods

        private void Awake()
        {
            this.spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            this.spriteRenderer.sortingOrder = -1 * Mathf.RoundToInt(this.transform.position.y);
        }

        public void Initialize(float playerHealthPercentageToRecover)
        {
            this.playerHealthPercentageToRecover = playerHealthPercentageToRecover;
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(Tags.Player))
            {
                PlayerBehaviour p = collision.gameObject.GetComponent<PlayerBehaviour>();
                float playerMaxHealth = p.HealthPoints;
                p.IncreaseHealth(playerMaxHealth * this.playerHealthPercentageToRecover);
                Destroy(this.gameObject);
            }
        }

        #endregion
    }
}
