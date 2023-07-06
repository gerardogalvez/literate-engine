using UnityEngine;
using MoodyBlues.Constants.BeatEmUp;

namespace MoodyBlues.BeatEmUp
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        private float damage;
        private Vector2 shootPosition;

        private AudioSource audioSource;

        [SerializeField]
        private AudioClip clip;

        private void Start()
        {
            this.audioSource = this.gameObject.GetComponent<AudioSource>();
            this.audioSource.clip = this.clip;
            Destroy(this.gameObject, 10.0f);
        }

        public void Initialize(float damage, Vector2 shootPosition)
        {
            this.damage = damage;
            this.shootPosition = shootPosition;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(Tags.Player))
            {
                var player = collision.gameObject.GetComponent<PlayerBehaviour>();
                Vector2 playerFeetPosition = collision.gameObject.transform.Find("FeetPosition").position;
                if (EnemyHitbox.ShouldTriggerDamage(this.shootPosition, playerFeetPosition))
                {
                    // this.audioSource.Play();
                    player.gameObject.GetComponent<PlayerBehaviour>().DecreaseHealth(this.damage);
                    Destroy(this.gameObject);
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
    }
}
