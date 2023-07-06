using UnityEngine;
using System.Collections;
using MoodyBlues.Constants.BeatEmUp;
using System.Linq;

namespace MoodyBlues.BeatEmUp
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class EmmaMovement : MonoBehaviour
    {
        #region Fields

        private EnemyState currentState;
        private Transform feetPosition;
        private SpriteRenderer spriteRenderer;
        private Animator animator;
        private Vector3 target;
        private float downBoundary;
        private float upBoundary;
        private bool isFacingRight;
        private Rigidbody2D rb;
        private float timeLastPunch;
        private Transform targetReference;
        public bool isRescuing;

        private readonly float waitTimeToAttack = 1.5f;
        private readonly float timeBetweenPunches = 0.5f;
        private readonly float timeToSwitchStates = 5.0f;

        [SerializeField]
        [Range(0.01f, 10.0f)]
        private float searchRange;

        [SerializeField]
        [Range(0.01f, 10.0f)]
        private float stoppingDistance;

        [SerializeField]
        private float movementSpeed;

        #endregion

        #region Methods

        private void Awake()
        {
            this.spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
            this.rb = this.gameObject.GetComponent<Rigidbody2D>();
            this.animator = this.gameObject.GetComponent<Animator>();
            this.isFacingRight = true;
            this.feetPosition = this.transform.Find("FeetPosition");
            this.currentState = EnemyState.Wandering;
            Debug.Assert(this.feetPosition != null, "Emma object needs a child object named FeetPosition.");
        }

        private void Start()
        {
            InvokeRepeating("SetTarget", 0.0f, this.timeToSwitchStates);
            this.feetPosition.localPosition = this.feetPosition.localPosition + new Vector3(0f, -this.spriteRenderer.bounds.size.y / 2.0f, 0f);
            this.downBoundary = GameManager.instance.DownBoundary;
            this.downBoundary += (this.transform.position.y - this.feetPosition.position.y) * 1.15f;
            this.upBoundary = GameManager.instance.UpBoundary;
            this.upBoundary += (this.transform.position.y - this.feetPosition.position.y) * 0.85f;
        }

        private void Update()
        {
            this.Walk();
        }

        private void SetTarget()
        {
            if (this.currentState != EnemyState.Wandering)
            {
                return;
            }

            float leftRange, rightRange;
            if (this.transform.position.x - searchRange < GameManager.instance.LeftBoundary)
            {
                leftRange = - (this.transform.position.x - GameManager.instance.LeftBoundary);
            }
            else
            {
                leftRange = -searchRange;
            }

            if (this.transform.position.x + searchRange > GameManager.instance.RightBoundary)
            {
                rightRange = GameManager.instance.RightBoundary - this.transform.position.x;
            }
            else
            {
                rightRange = searchRange;
            }

            this.target = new Vector2(
                x: this.transform.position.x + Random.Range(leftRange, rightRange),
                y: Random.Range(this.downBoundary, this.upBoundary)
            );
        }

        private void Flip()
        {
            Vector3 scale = this.transform.localScale;
            scale.x *= -1.0f;
            this.transform.localScale = scale;
            this.isFacingRight = !this.isFacingRight;
        }

        private IEnumerator Punch()
        {
            this.animator.SetTrigger("Idle");
            yield return new WaitForSeconds(this.waitTimeToAttack);
            if (Time.time - this.timeLastPunch >= this.timeBetweenPunches)
            {
                this.animator.SetTrigger("Punch");
                this.timeLastPunch = Time.time;
            }
            else
            {
                this.animator.SetTrigger("Idle");
            }
        }

        private IEnumerator Rescue(SavableBehaviour savable)
        {
            if (!this.isRescuing && !savable.IsBeingSaved)
            {
                this.isRescuing = true;
                savable.IsBeingSaved = true;
                savable.SavedBy = this.gameObject;
                this.animator.SetBool("isRescuing", this.isRescuing);
                float rescueTime = savable.TimeToSave;
                yield return new WaitForSeconds(rescueTime);

                if (!this.gameObject.GetComponent<PlayerBehaviour>().IsDownOrDead && savable != null)
                {
                    savable.OnSave();
                    this.isRescuing = false;
                    savable.IsBeingSaved = false;
                    this.animator.SetBool("isRescuing", this.isRescuing);
                    this.currentState = EnemyState.Wandering;
                }
            }
        }

        private void Walk()
        {
            switch (this.currentState)
            {
                case EnemyState.FollowingPlayer:
                    this.FollowPlayer();
                    break;
                case EnemyState.Wandering:
                    this.Wander();
                    break;
            }

            this.Move();
        }

        public void StopRescue()
        {
            this.isRescuing = false;
            this.animator.SetBool("isRescuing", this.isRescuing);
            var savable = this.targetReference.gameObject.GetComponent<SavableBehaviour>();
            savable.IsBeingSaved = false;
            savable.SavedBy = null;
        }

        public void ResetStateMachine()
        {
            this.currentState = EnemyState.Wandering;
            this.targetReference = null;
        }

        private void Move()
        {
            if (this.targetReference == null)
            {
                this.currentState = EnemyState.Wandering;
            }

            Vector2 dir = this.target - this.transform.position;
            if ((dir.x > 0 && !this.isFacingRight) || dir.x < 0 && this.isFacingRight)
            {
                this.Flip();
            }

            if (dir.magnitude < this.stoppingDistance)
            {
                dir = Vector2.zero;
                if (this.currentState == EnemyState.FollowingPlayer)
                {
                    if (this.targetReference.CompareTag(Tags.Enemy))
                    {
                        StartCoroutine("Punch");
                    }
                    else if (this.targetReference.gameObject.GetComponent<SavableBehaviour>() != null)
                    {
                        var savable = this.targetReference.gameObject.GetComponent<SavableBehaviour>();
                        if (savable.SavedBy != null && savable.SavedBy != this.gameObject)
                        {
                            this.targetReference = null;
                        }
                        else
                        {
                            StartCoroutine(Rescue(savable));
                        }
                    }
                }
                else
                {
                    this.animator.SetTrigger("Idle");
                }
            }
            else
            {
                this.animator.SetTrigger("Walk");
            }

            dir.Normalize();
            this.rb.velocity = this.movementSpeed * dir;
        }

        protected virtual void FollowPlayer()
        {
            if (this.targetReference == null) // Target got destroyed (enemy died or civilian got rescued by player)
            {
                this.currentState = EnemyState.Wandering;
                return;
            }

            this.target = this.targetReference.position;
            if (Vector2.Distance(this.transform.position, this.target) > this.searchRange * 1.2f)
            {
                this.target = this.transform.position;
                this.currentState = EnemyState.Wandering;
            }
        }

        protected virtual void Wander()
        {
            Collider2D[] enemyResults = Physics2D.OverlapCircleAll(this.transform.position, this.searchRange, LayerMask.GetMask(new string[] { "Enemy" }));
            Collider2D[] savables = Physics2D.OverlapCircleAll(this.transform.position, this.searchRange * 2.0f, LayerMask.GetMask(new string[] { "Civilian", "Default" }));
            Collider2D[] civilianResults = savables.Where(r => r.gameObject.CompareTag(Tags.Civilian)).ToArray();
            Collider2D[] playerResults = savables.Where(r => r.gameObject.CompareTag(Tags.Player) && r.gameObject.GetComponent<PlayerBehaviour>().IsDownOrDead).ToArray();

            if (playerResults.Length > 0)
            {
                this.currentState = EnemyState.FollowingPlayer;
                this.targetReference = playerResults[0].gameObject.transform;
                this.target = this.targetReference.position;
            }
            else if (civilianResults.Length > 0)
            {
                int rand = Random.Range(0, civilianResults.Length);
                this.currentState = EnemyState.FollowingPlayer;
                this.targetReference = civilianResults[rand].gameObject.transform;
                this.target = this.targetReference.position;
            }
            else
            {
                if (enemyResults.Length > 0)
                {
                    int rand = Random.Range(0, enemyResults.Length);
                    if (enemyResults[rand].CompareTag(Tags.Enemy)) // TODO: This check should be redundant, remove
                    {
                        this.currentState = EnemyState.FollowingPlayer;
                        this.targetReference = enemyResults[rand].gameObject.transform;
                        this.target = this.targetReference.position;
                    }
                }
            }
        }

        #endregion
    }
}