using UnityEngine;

namespace MoodyBlues.BeatEmUp
{
    public class EnemyPunchBehaviour : StateMachineBehaviour
    {
        #region Fields
        [SerializeField]
        private int punchIndex; // Assigned in inspector

        #endregion

        #region Methods

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.transform.GetChild(punchIndex).gameObject.SetActive(true);
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.transform.GetChild(punchIndex).gameObject.SetActive(false);
        }

        #endregion
    }
}

