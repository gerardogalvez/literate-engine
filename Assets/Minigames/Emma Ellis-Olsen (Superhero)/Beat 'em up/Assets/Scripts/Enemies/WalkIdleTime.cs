using UnityEngine;

namespace MoodyBlues.BeatEmUp
{
    public class WalkIdleTime : StateMachineBehaviour
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.GetComponent<BaseEnemyBehaviour>().LastWalkIdleTimeAnimationStart = Time.time;
        }
    }
}
