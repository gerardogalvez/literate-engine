using UnityEngine;

namespace MoodyBlues.Fire
{
    public class PlayerMovement : MonoBehaviour
    {

        private GameObject player;

        private bool movingRight;
        private bool movingLeft;

        private void HandleMovement()
        {
            // Try to move to the right
            if (Input.GetAxisRaw(MoodyBlues.Constants.Axis.Horizontal) > 0)
            {
                float playerHorizontalPosition = this.player.transform.position.x;
                if (playerHorizontalPosition < GameManager.instance.MaxPlayerHorizontalMovement)
                {
                    if (!this.movingRight)
                    {
                        this.movingRight = true;
                        this.movingLeft = false;
                        this.player.transform.Translate(Vector3.right);
                    }
                }
                else
                {
                    this.movingRight = false;
                    this.movingLeft = false;
                }
            }
            // Try to move to the left
            else if (Input.GetAxisRaw(MoodyBlues.Constants.Axis.Horizontal) < 0)
            {
                float playerHorizontalPosition = this.player.transform.position.x;
                if (playerHorizontalPosition > (-1.0 * GameManager.instance.MaxPlayerHorizontalMovement))
                {
                    if (!this.movingLeft)
                    {
                        this.movingRight = false;
                        this.movingLeft = true;
                        this.player.transform.Translate(Vector3.left);
                    }
                }
                else
                {
                    this.movingRight = false;
                    this.movingLeft = false;
                }
            }
            // Not moving
            else
            {
                this.movingRight = false;
                this.movingLeft = false;
            }
        }

        // Use this for initialization
        void Start()
        {
            this.player = this.gameObject;
        }

        // Update is called once per frame
        void Update()
        {
            this.HandleMovement();
        }
    }
}
