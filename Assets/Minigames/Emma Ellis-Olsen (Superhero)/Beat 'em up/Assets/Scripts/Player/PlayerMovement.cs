using UnityEngine;
using System.Collections;

namespace MoodyBlues.BeatEmUp
{
    [RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private float movementSpeed;

        [SerializeField]
        private float jumpHeight;

        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private Rigidbody2D rb;
        private Transform feetPosition;
        private SavableBehaviour savable;
        private float timeSaveStart;
        private float downBoundary;
        private float upBoundary;
        private bool isWalking;
        private bool isFacingRight;
        private bool isRescuing;

        [SerializeField]
        private float reviveRange;

        #endregion

        #region Properties

        public float MovementSpeed => this.movementSpeed;

        public Vector2 FeetPosition => this.feetPosition.position;

        public bool IsGrounded { get; set; }

        public bool IsJumping { get; set; }

        #endregion

        #region Methods

        /*private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(this.FeetPosition, this.reviveRange);
        }*/

        private void HandleMovement()
        {
            float x = Input.GetAxisRaw(Constants.Axis.Horizontal);
            float y = Input.GetAxisRaw(Constants.Axis.Vertical);
            Vector2 direction = new Vector2(x, y).normalized;

            if (direction == Vector2.zero && !IsJumping)
            {
                this.isWalking = false;
                this.IsGrounded = true;
            }

            if (direction.y != 0)
            {
                this.isWalking = true;
            }

            if (direction.x > 0)
            {
                if (this.IsGrounded)
                {
                    this.isWalking = true;
                }

                if (!this.isFacingRight)
                {
                    this.Flip();
                }
            }

            if (direction.x < 0)
            {
                if (this.IsGrounded)
                {
                    this.isWalking = true;
                }

                if (this.isFacingRight)
                {
                    this.Flip();
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                RaycastHit2D hit = Physics2D.CircleCast(this.FeetPosition, this.reviveRange, Vector2.up, this.reviveRange, LayerMask.GetMask(new string[] { "Civilian", "Playable" }));
                if (hit.collider != null)
                {
                    var s = hit.collider.gameObject.GetComponent<SavableBehaviour>();
                    if (!s.IsBeingSaved)
                    {
                        this.savable = hit.collider.gameObject.GetComponent<SavableBehaviour>();
                        this.savable.IsBeingSaved = true;
                        this.savable.SavedBy = this.gameObject;
                        this.timeSaveStart = Time.time;
                    }
                }
            }

            if (Input.GetKey(KeyCode.R))
            {
                if (this.savable != null)
                {
                    if (Time.time - this.timeSaveStart >= this.savable.TimeToSave)
                    {
                        this.savable.OnSave();
                        this.savable = null;
                    }
                }
            }

            if (Input.GetKeyUp(KeyCode.R))
            {
                if (this.savable != null)
                {
                    this.savable.IsBeingSaved = false;
                    this.savable.SavedBy = null;
                }

                this.savable = null;
            }

            this.isRescuing = this.savable != null;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.IsGrounded = false;
                this.IsJumping = true;
            }

            this.animator.SetBool("isWalking", this.isWalking);
            this.animator.SetBool("isGrounded", this.IsGrounded);
            this.animator.SetBool("isRescuing", this.isRescuing);
            this.animator.SetInteger("x", (int)direction.x);
            this.animator.SetInteger("y", (int)direction.y);

            if (!this.isRescuing)
            {
                this.rb.velocity = direction * this.movementSpeed;
                this.transform.position = new Vector3(
                    x: Mathf.Clamp(this.transform.position.x, GameManager.instance.LeftBoundary, GameManager.instance.RightBoundary),
                    y: Mathf.Clamp(this.transform.position.y, this.downBoundary, this.upBoundary),
                    z: this.transform.position.z
                );
            }
        }

        private void Flip()
        {
            Vector3 scale = this.transform.localScale;
            scale.x *= -1.0f;
            this.transform.localScale = scale;
            this.isFacingRight = !this.isFacingRight;
        }

        private void Awake()
        {
            this.animator = this.gameObject.GetComponent<Animator>();
            this.rb = this.gameObject.GetComponent<Rigidbody2D>();
            this.spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
            this.feetPosition = this.transform.Find("FeetPosition");
            this.isFacingRight = true;
            this.isRescuing = false;
            this.IsGrounded = true;
            this.feetPosition.localPosition = this.feetPosition.localPosition + new Vector3(0f, -this.spriteRenderer.bounds.size.y / 2.0f, 0f);
            Debug.Assert(this.feetPosition != null, "Player object needs a child object named FeetPosition.");
        }

        // Use this for initialization
        void Start()
        {
            this.downBoundary = GameManager.instance.DownBoundary;
            this.downBoundary += (this.transform.position.y - this.feetPosition.position.y) * 1.15f;
            this.upBoundary = GameManager.instance.UpBoundary;
            this.upBoundary += (this.transform.position.y - this.feetPosition.position.y) * 0.85f;
        }

        private void Update()
        {
            this.HandleMovement();
        }

        public void StopMovement()
        {
            this.rb.velocity = Vector2.zero;
        }

        #endregion
    }
}
