using UnityEngine;
using System.Collections;

namespace MoodyBlues.BeatEmUp
{
    [RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerFighting : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private float timeBetweenPunchesCombo;

        [SerializeField]
        private float comboDelay;

        private float timeLastPunch;

        private Animator animator;

        #endregion

        #region Properties

        public float TimeComboEnded;

        public int PunchCounter { get; set; }

        public int PunchDamage;

        #endregion

        #region Methods

        private void Awake()
        {
            this.animator = this.gameObject.GetComponent<Animator>();
            this.PunchCounter = -1;
            this.timeLastPunch = -1.0f;

            if (this.gameObject.GetComponent<EmmaMovement>() != null)
            {
                this.enabled = false;
            }
        }

        private void Update()
        {
            this.HandleFighting();
        }

        private void HandleFighting()
        {
            if (Mathf.Abs(Time.timeSinceLevelLoad - this.timeLastPunch) > this.timeBetweenPunchesCombo)
            {
                this.PunchCounter = -1;
                this.animator.SetInteger("punchCounter", this.PunchCounter);
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (this.TimeComboEnded > 0.0f && Time.time - this.TimeComboEnded < this.comboDelay)
                {
                    return;
                }

                if (this.PunchCounter == -1 ||  Mathf.Abs(Time.timeSinceLevelLoad - this.timeLastPunch) <= this.timeBetweenPunchesCombo)
                {
                    this.PunchCounter = (this.PunchCounter + 1) % 3;
                }

                this.timeLastPunch = Time.timeSinceLevelLoad;
                this.animator.SetInteger("punchCounter", this.PunchCounter);
            }
        }

        #endregion
    }
}
