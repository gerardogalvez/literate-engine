using UnityEngine;
using MoodyBlues.Constants.BeatEmUp;
namespace MoodyBlues.BeatEmUp
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class CivilianBehaviour : SavableBehaviour
    {
        #region Fields

        [SerializeField]
        private float escapeSpeed; // Assigned in inspector

        [SerializeField]
        private float timeToDestroy; // Assigned in inspector

        [SerializeField]
        private int score = 100;

        private Transform feetPosition;

        #endregion

        #region Methods

        public void Awake()
        {
            this.feetPosition = this.transform.Find("FeetPosition");
            this.feetPosition.localPosition = this.feetPosition.localPosition + new Vector3(0f, -this.gameObject.GetComponent<SpriteRenderer>().bounds.size.y / 2.0f, 0f);
        }

        public override void OnSave()
        {
            this.gameObject.GetComponent<BoxCollider2D>().enabled = false;

            float direction = Random.Range(-1.0f, 1.0f);
            direction = (direction >= 0.0f) ? 1.0f : -1.0f;
            this.gameObject.GetComponent<SpriteRenderer>().flipX = direction < 0; // This assumes the sprite is facing to the right

            this.gameObject.GetComponent<Animator>().SetTrigger("Escape");
            this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(direction * this.escapeSpeed, 0.0f);

            FloatingScore s = Instantiate(GameManager.instance.ScorePrefab, this.transform.position, Quaternion.identity);
            s.SetText(this.score.ToString());
            s.enabled = true;

            GameManager.instance.IncreaseScoreCount(Scoring.VictimRescued);
            Destroy(this.gameObject, this.timeToDestroy);
        }

        public void Die()
        {
            this.gameObject.GetComponent<Animator>().SetTrigger("Die");
            this.gameObject.GetComponent<BoxCollider2D>().enabled = false;

            if (this.SavedBy != null)
            {
                if (this.SavedBy.GetComponent<EmmaMovement>() != null)
                {
                    this.SavedBy.GetComponent<EmmaMovement>().StopRescue();
                }
            }

            GameManager.instance.IncreaseScoreCount(Scoring.VictimKilled);
            Destroy(this.gameObject, 1.0f);
        }

        #endregion
    }
}
