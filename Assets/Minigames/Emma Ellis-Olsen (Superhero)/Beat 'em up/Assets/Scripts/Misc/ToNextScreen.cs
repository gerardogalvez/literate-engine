using UnityEngine;
using MoodyBlues.Constants.BeatEmUp;

namespace MoodyBlues.BeatEmUp
{
    public class ToNextScreen : MonoBehaviour
    {
        #region Methods

        private void CleanUp()
        {
            var collectibles = GameObject.FindGameObjectsWithTag(Tags.Collectible);
            foreach (var collectible in collectibles)
            {
                Destroy(collectible);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Check for name because Emma is algo tagged as Player
            if (collision.gameObject.name == Tags.Player)
            {
                if (GameManager.instance.FinishedSpawningEntities() && GameManager.instance.CanContinue())
                {
                    if (GameManager.instance.IsLastScreen())
                    {
                        GameManager.instance.GameOver();
                    }
                    else
                    {
                        this.CleanUp();
                        GameManager.instance.LoadScreen();
                    }
                }
            }
        }

        #endregion
    }
}
