using UnityEngine;
namespace MoodyBlues.BeatEmUp
{
    public class PlayerSavableBehaviour : SavableBehaviour
    {
        #region Methods

        public override void OnSave()
        {
            var playerBehaviour = this.gameObject.GetComponent<PlayerBehaviour>();
            var animator = this.gameObject.GetComponent<Animator>();

            this.SavedBy = null;
            this.IsBeingSaved = false;
            if (this.gameObject.GetComponent<PlayerMovement>() != null)
            {
                var playerMovement = this.gameObject.GetComponent<PlayerMovement>();
                playerMovement.enabled = true;
                playerBehaviour.IsDownOrDead = false;
                playerBehaviour.IncreaseHealth(playerBehaviour.HealthPoints / 2.0f);
                animator.SetTrigger("onRescue");
            }
            else
            {
                var emmaMovement = this.gameObject.GetComponent<EmmaMovement>();
                emmaMovement.ResetStateMachine();
                emmaMovement.enabled = true;
                playerBehaviour.IsDownOrDead = false;
                playerBehaviour.IncreaseHealth(playerBehaviour.HealthPoints / 2.0f);
                animator.SetTrigger("onRescue");
            }
            
            playerBehaviour.ToggleUIElement();
            if (GameManager.instance.ShouldShowContinueArrow())
            {
                GameManager.instance.ShowContinueArrow();
            }
        }

        #endregion
    }
}
