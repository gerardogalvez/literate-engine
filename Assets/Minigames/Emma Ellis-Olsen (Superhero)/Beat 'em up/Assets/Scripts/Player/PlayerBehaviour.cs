using UnityEngine;
using UnityEngine.UI;
using MoodyBlues.Constants.BeatEmUp;

namespace MoodyBlues.BeatEmUp
{
    public class PlayerBehaviour : MonoBehaviour
    {

        #region Fields

        [SerializeField]
        private int lives;

        #endregion

        #region Properties

        public int Lives {
            get { 
                return this.lives; 
            } 

            set { 
                this.lives = value;
                if (this.lives >= 0)
                {
                    GameManager.instance.RemovePlayerLife(this.gameObject.name == "Player", value);
                }
            }
        }

        public float HealthPoints => 10.0f;

        public float CurrentHealth { get; private set; }

        [HideInInspector]
        public GameObject HealthBar;

        [HideInInspector]
        public bool IsDownOrDead;

        #endregion

        #region Methods

        // Use this for initialization
        private void Awake()
        {
            this.CurrentHealth = this.HealthPoints;
            this.IsDownOrDead = false;
        }

        private void UpdateHealthBar()
        {
            this.HealthBar.transform.Find("Health").GetComponent<Image>().fillAmount = this.CurrentHealth / this.HealthPoints;
        }

        private void Die()
        {
            this.GetComponent<Animator>().SetTrigger("Die");
            if (this.GetComponent<PlayerMovement>() != null)
            {
                GameManager.instance.GameOver();
            }
            else
            {
                this.HealthBar.GetComponent<FollowPlayer>().enabled = false;
                this.HealthBar.transform.Find("Help").gameObject.SetActive(false);
                this.HealthBar.transform.Find("HealthBorder").gameObject.SetActive(false);
                this.HealthBar.transform.Find("Health").gameObject.SetActive(false);
                GameManager.instance.IncreaseScoreCount(Scoring.EmmaDied);
                Destroy(this.gameObject, 2.0f);
            }
        }

        public void ToggleUIElement()
        {
            if (this.IsDownOrDead && this.Lives > 0)
            {
                this.HealthBar.transform.Find("Help").gameObject.SetActive(true);
                this.HealthBar.transform.Find("HealthBorder").gameObject.SetActive(false);
                this.HealthBar.transform.Find("Health").gameObject.SetActive(false);
                return;
            }

            if (!this.IsDownOrDead)
            {
                this.HealthBar.transform.Find("Help").gameObject.SetActive(false);
                this.HealthBar.transform.Find("HealthBorder").gameObject.SetActive(true);
                this.HealthBar.transform.Find("Health").gameObject.SetActive(true);
                return;
            }

            // TODO: Add revive progress bar (circle?)
        }

        public void IncreaseHealth(float health)
        {
            this.CurrentHealth += health;
            if (this.CurrentHealth > this.HealthPoints)
            {
                this.CurrentHealth = this.HealthPoints;
            }

            this.UpdateHealthBar();
        }

        public void DecreaseHealth(float damage)
        {
            if (!IsDownOrDead)
            {
                this.CurrentHealth -= damage;
                if (this.CurrentHealth <= 0.0f)
                {
                    this.IsDownOrDead = true;
                    this.CurrentHealth = 0.0f;

                    this.Lives--;
                    if (this.Lives <= 0)
                    {
                        this.Die();
                        return;
                    }
                    else
                    {
                        this.GetComponent<Animator>().SetTrigger("Down");
                        if (this.GetComponent<PlayerMovement>() != null)
                        {
                            var emma = GameObject.Find("Emma");
                            if (emma != null)
                            {
                                if (emma.GetComponent<PlayerBehaviour>().IsDownOrDead)
                                {
                                    GameManager.instance.GameOver();
                                }
                                else
                                {
                                    emma.GetComponent<EmmaMovement>().ResetStateMachine();
                                }   
                            }
                            else
                            {
                                GameManager.instance.GameOver();
                            }
                        }
                    }

                    if (this.GetComponent<PlayerMovement>() != null)
                    {
                        this.GetComponent<PlayerMovement>().StopMovement();
                        this.GetComponent<PlayerMovement>().enabled = false;
                    }
                    else
                    {
                        this.GetComponent<EmmaMovement>().enabled = false;
                        if (this.GetComponent<EmmaMovement>().isRescuing)
                        {
                            this.GetComponent<EmmaMovement>().StopRescue();
                        }
                    }
                }

                this.ToggleUIElement();
                this.UpdateHealthBar();
            }
        }

        #endregion
    }
}

