using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoodyBlues.Constants.BeatEmUp;
using System.Linq;

namespace MoodyBlues.BeatEmUp
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class BaseEnemyBehaviour : MonoBehaviour
    {
        #region Fields

        protected Transform playerReference;
        protected GameObject mainPlayerReference;
        protected Animator animator;

        protected float baseHealthMultiplier = 1.0f;
        protected float baseMovementMultiplier = 1.0f;
        protected float baseDamageMultiplier = 1.0f;
        protected float timeLastPunch;

        private EnemyState currentState;
        private int currentHealth;
        private float downBoundary;
        private float upBoundary;
        private bool isFacingRight;
        private bool reachedStartingPosition;
        private Rigidbody2D rb;
        private Transform feetPosition;
        private SpriteRenderer spriteRenderer;
        private Vector3 target;
        private Vector2 startingTargetPosition;

        [SerializeField]
        [Range(0.01f, 10.0f)]
        private float searchRange; // Assigned in inspector

        [SerializeField]
        [Range(0.01f, 10.0f)]
        private float stoppingDistance; // Assigned in inspector

        #endregion

        #region Properties

        protected abstract float TimeToSwitchStates { get; }

        protected abstract float MovementSpeed { get; }

        // Ex.
        // [
        //  <0.5, 0.1>, // 50% chance of dropping a health pack that recovers 10% of player's health
        //  <0.5, 0.2>, // 50% chance of dropping a health pack that recovers 20% of player's health
        // ]
        protected abstract List<System.Tuple<float, float>> HealthDropProbabilitiesPercentage { get; }

        protected abstract int HealthPoints { get; }

        protected abstract bool CanBeThrown { get; }

        protected abstract float WaitTimeToAttack { get; }

        protected abstract float TimeBetweenPunches { get; }

        public abstract float Damage { get; }

        public abstract int Points { get; }

        public Vector2 FeetPosition => this.feetPosition.position;

        public float LastWalkIdleTimeAnimationStart { get; set; }

        #endregion

        #region Methods

        private void Awake()
        {
            this.spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
            this.rb = this.gameObject.GetComponent<Rigidbody2D>();
            this.animator = this.gameObject.GetComponent<Animator>();
            this.isFacingRight = false;
            this.feetPosition = this.transform.Find("FeetPosition");
            this.currentState = EnemyState.Wandering;
            Debug.Assert(this.feetPosition != null, "Enemy object needs a child object named FeetPosition.");
        }

        private void Start()
        {
            this.timeLastPunch = 0.0f;
            this.mainPlayerReference = GameObject.Find("Player");
            this.playerReference = GameObject.Find("Player").GetComponent<Transform>();
            this.currentHealth = this.HealthPoints;
            this.feetPosition.localPosition = this.feetPosition.localPosition + new Vector3(0f, -this.spriteRenderer.bounds.size.y / 2.0f, 0f);
            this.downBoundary = GameManager.instance.DownBoundary;
            this.downBoundary += (this.transform.position.y - this.feetPosition.position.y) * 1.15f;
            this.upBoundary = GameManager.instance.UpBoundary;
            this.upBoundary += (this.transform.position.y - this.feetPosition.position.y) * 0.85f;

            float leftBound = GameManager.instance.LeftBoundary;
            float rightBound = GameManager.instance.RightBoundary;
            if (this.transform.position.x < GameManager.instance.LeftBoundary)
            {
                this.startingTargetPosition = new Vector2(leftBound + (rightBound - leftBound) * 0.1f, this.transform.position.y);
                this.Flip();
            }
            else
            {
                this.startingTargetPosition = new Vector2(leftBound + (rightBound - leftBound) * 0.9f, this.transform.position.y);
            }
        }

        private void Update()
        {
            if (!this.reachedStartingPosition)
            {
                float step = this.MovementSpeed * Time.deltaTime;
                this.transform.position = Vector2.MoveTowards(transform.position, this.startingTargetPosition, step);

                if (Vector2.Distance(this.transform.position, this.startingTargetPosition) < 0.001f)
                {
                    this.reachedStartingPosition = true;
                    InvokeRepeating("SetTarget", 0.0f, this.TimeToSwitchStates);
                }
            }
            else
            {
                this.Walk();
            }
        }

        private void SetTarget()
        {
            if (this.currentState != EnemyState.Wandering)
            {
                return;
            }

            this.target = new Vector2(
                x: this.transform.position.x + Random.Range(-searchRange, searchRange),
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

        private void DropHealthObject()
        {
            if (this.HealthDropProbabilitiesPercentage.Count == 0)
            {
                return;
            }

            float lowProb = 0.0f;
            float highProb = 0.0f;
            float prob = Random.Range(0.0f, 1.0f);

            for (int i = 0; i < this.HealthDropProbabilitiesPercentage.Count; ++i)
            {
                highProb = this.HealthDropProbabilitiesPercentage[i].Item1 + highProb;
                if (prob >= lowProb && prob <= highProb)
                {
                    float percentHealth = this.HealthDropProbabilitiesPercentage[i].Item2;
                    GameManager.healthObjectFactory.CreateHealthObject(percentHealth, this.FeetPosition);
                    return;
                }
                else
                {
                    lowProb = highProb;
                }
            }
        }

        private IEnumerator Die()
        {
            this.OnDie();
            this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            this.enabled = false;
            this.rb.velocity = Vector2.zero;
            this.animator.SetTrigger("Die");
            FloatingScore s = Instantiate(GameManager.instance.ScorePrefab, this.transform.position, Quaternion.identity);
            s.SetText(this.Points.ToString());
            s.enabled = true;
            yield return new WaitForSeconds(1.0f);
            this.DropHealthObject();
            Destroy(this.gameObject);
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

        private void Move()
        {
            if (this.playerReference == null)
            {
                this.currentState = EnemyState.Wandering;
            }
            else if (this.playerReference.gameObject.CompareTag(Tags.Player))
            {
                if (this.playerReference.gameObject.GetComponent<PlayerBehaviour>().IsDownOrDead)
                {
                    this.currentState = EnemyState.Wandering;
                }
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
                    Vector2 playerFeetPosition = this.playerReference.gameObject.transform.Find("FeetPosition").position;
                    if (Mathf.Abs(this.FeetPosition.y - playerFeetPosition.y) <= GameManager.VERTICAL_POSITION_DAMAGE_TRESHOLD)
                    {
                        StartCoroutine("Punch");
                    }
                    else
                    {
                        dir = new Vector2(this.transform.position.x, dir.y);
                        this.animator.SetTrigger("Walk");
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

            this.animator.SetFloat("WalkIdleTime", Time.time - this.LastWalkIdleTimeAnimationStart);

            dir.Normalize();
            this.rb.velocity = this.MovementSpeed * dir;
        }

        private void CheckHealth()
        {
            if (this.currentHealth <= 0)
            {
                this.enabled = false;
                StartCoroutine(this.Die());
            }
        }

        protected virtual IEnumerator Punch()
        {
            this.animator.SetTrigger("Idle");
            yield return new WaitForSeconds(this.WaitTimeToAttack);
            if (Time.time - this.timeLastPunch >= this.TimeBetweenPunches)
            {
                this.animator.SetTrigger("Punch");
                this.timeLastPunch = Time.time;
            }
            else
            {
                this.animator.SetTrigger("Idle");
            }
        }

        protected abstract void OnDie();

        /*protected void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, searchRange);
            Gizmos.DrawWireSphere(target, 0.2f);
        }*/

        protected virtual void FollowPlayer()
        {
            if (this.playerReference == null)
            {
                return;
            }

            this.target = this.playerReference.position;
            if (Vector2.Distance(this.transform.position, this.target) > this.searchRange * 1.2f)
            {
                this.target = this.transform.position;
                this.currentState = EnemyState.Wandering;
            }
        }

        protected virtual void Wander()
        {
            Collider2D[] results = Physics2D.OverlapCircleAll(this.transform.position, this.searchRange);
            results = results.Where(r => 
                (r.gameObject.CompareTag(Tags.Player) && !r.gameObject.GetComponent<PlayerBehaviour>().IsDownOrDead) ||
                (r.gameObject.CompareTag(Tags.Civilian) && r.gameObject.GetComponent<CivilianBehaviour>() != null && !r.gameObject.GetComponent<CivilianBehaviour>().IsBeingSaved))
                .ToArray();

            if (results.Length > 0)
            {
                int rand = Random.Range(0, results.Length);
                if (results[rand].CompareTag(Tags.Player) || results[rand].CompareTag(Tags.Civilian))
                {
                    this.currentState = EnemyState.FollowingPlayer;
                    this.playerReference = results[rand].gameObject.transform;
                }
            }
        }

        public void IncreaseHealth(int health)
        {
            this.currentHealth += health;
            this.CheckHealth();
        }

        public void DecreaseHealth(int damage)
        {
            this.currentHealth -= damage;
            this.CheckHealth();
        }

        public void SetBaseMultipliers(float baseHealthMultiplier, float baseMovementMultiplier, float baseDamageMultiplier)
        {
            this.baseHealthMultiplier = baseHealthMultiplier;
            this.baseMovementMultiplier = baseMovementMultiplier;
            this.baseDamageMultiplier = baseDamageMultiplier;
        }

        #endregion
    }
}