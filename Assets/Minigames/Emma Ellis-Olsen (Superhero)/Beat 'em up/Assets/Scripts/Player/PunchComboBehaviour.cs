using UnityEngine;

namespace MoodyBlues.BeatEmUp
{
    public class PunchComboBehaviour : StateMachineBehaviour
    {
        #region Fields

        [SerializeField]
        private int punchIndex;

        #endregion

        #region Methods

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.transform.GetChild(punchIndex).gameObject.SetActive(true);
            animator.GetComponentInParent<Rigidbody2D>().velocity = Vector2.zero;
            animator.GetComponentInParent<PlayerMovement>().enabled = false;
            animator.SetBool("isWalking", false);
            animator.SetBool("isGrounded", true);
            animator.SetInteger("x", 0);
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.transform.GetChild(punchIndex).gameObject.SetActive(false);

            if (this.punchIndex == 2)
            {
                animator.gameObject.GetComponent<PlayerFighting>().PunchCounter = -1;
                animator.gameObject.GetComponent<PlayerFighting>().TimeComboEnded = Time.time;
            }

            animator.GetComponentInParent<PlayerMovement>().enabled = true;
        }

        #endregion
    }
}

