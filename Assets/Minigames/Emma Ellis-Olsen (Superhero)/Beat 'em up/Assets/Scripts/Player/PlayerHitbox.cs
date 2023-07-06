using UnityEngine;
using MoodyBlues.Constants.BeatEmUp;

namespace MoodyBlues.BeatEmUp
{
    public class PlayerHitbox : MonoBehaviour
    {
        #region Fields

        private int damage;
        private PlayerMovement playerMovementReference;
        private AudioSource audioSource;

        [SerializeField]
        private AudioClip clip; // Assigned in inspector

        #endregion

        #region Methods

        // Use this for initialization
        private void Start()
        {
            this.damage = this.gameObject.GetComponentInParent<PlayerFighting>().PunchDamage;
            this.audioSource = this.gameObject.GetComponentInParent<AudioSource>();
            this.audioSource.clip = this.clip;
        }

        private bool ShouldTriggerDamage(Vector2 playerFeetPosition, Vector2 enemyFeetPosition)
        {
            return Mathf.Abs(playerFeetPosition.y - enemyFeetPosition.y) < GameManager.VERTICAL_POSITION_DAMAGE_TRESHOLD;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(Tags.Enemy))
            {
                var enemy = collision.gameObject.GetComponent<BaseEnemyBehaviour>();
                Vector2 playerFeetPosition = this.gameObject.transform.parent.Find("FeetPosition").position;
                if (this.ShouldTriggerDamage(playerFeetPosition, enemy.FeetPosition))
                {
                    this.audioSource.Play();
                    enemy.DecreaseHealth(this.damage);
                }
            }
        }

        #endregion
    }
}
