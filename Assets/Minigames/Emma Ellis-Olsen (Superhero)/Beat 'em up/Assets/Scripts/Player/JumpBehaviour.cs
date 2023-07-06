using UnityEngine;

namespace MoodyBlues.BeatEmUp
{
    public class JumpBehaviour : StateMachineBehaviour
    {
        #region Fields

        [SerializeField]
        private float jumpHeight;

        #endregion

        #region Methods

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
            Debug.Log($"{stateInfo.length}");
            //animator.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0.0f, this.jumpHeight), ForceMode2D.Impulse);
            animator.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0.0f, 9.81f / (stateInfo.length / 2.0f)), ForceMode2D.Impulse);
            Debug.Log($"Jump height: {this.jumpHeight}");
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("isWalking", false);
            animator.gameObject.GetComponent<PlayerMovement>().IsGrounded = true;
            animator.gameObject.GetComponent<PlayerMovement>().IsJumping = false;
            animator.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            animator.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0.0f;
        }

        #endregion
    }
}
