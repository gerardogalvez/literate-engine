using UnityEngine;
using MoodyBlues.Constants.BeatEmUp;

namespace MoodyBlues.BeatEmUp
{
    public class EnemyHitbox : MonoBehaviour
    {
        #region Fields

        private float damage;
        private BaseEnemyBehaviour owner;
        private AudioSource audioSource;

        [SerializeField]
        private AudioClip clip;

        #endregion

        #region Methods

        // Use this for initialization
        private void Start()
        {
            this.owner = this.gameObject.GetComponentInParent<BaseEnemyBehaviour>();
            this.damage = this.owner.Damage;
            this.audioSource = this.gameObject.GetComponentInParent<AudioSource>();
            this.audioSource.clip = this.clip;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(Tags.Player))
            {
                var player = collision.gameObject.GetComponent<PlayerBehaviour>();
                Vector2 playerFeetPosition = collision.gameObject.transform.Find("FeetPosition").position;
                if (EnemyHitbox.ShouldTriggerDamage(this.owner.FeetPosition, playerFeetPosition))
                {
                    this.audioSource.Play();
                    player.gameObject.GetComponent<PlayerBehaviour>().DecreaseHealth(this.damage);
                }
            }
            else if (collision.CompareTag(Tags.Civilian))
            {
                var civilian = collision.gameObject.GetComponent<CivilianBehaviour>();
                if (!civilian.IsBeingSaved)
                {
                    this.audioSource.Play();
                    civilian.Die();
                }
            }
        }

        public static bool ShouldTriggerDamage(Vector2 playerFeetPosition, Vector2 enemyFeetPosition)
        {
            return Mathf.Abs(playerFeetPosition.y - enemyFeetPosition.y) < GameManager.VERTICAL_POSITION_DAMAGE_TRESHOLD;
        }

        #endregion
    }
}
