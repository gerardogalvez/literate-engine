using System;
using System.Collections;
using System.Collections.Generic;
using MoodyBlues.Constants.BeatEmUp;
using UnityEngine;

namespace MoodyBlues.BeatEmUp
{
    public class MalandroConPistola : BaseEnemyBehaviour
    {
        #region Properties

        protected override float TimeToSwitchStates => 5.0f;

        protected override float MovementSpeed =>
            this.baseMovementMultiplier * this.mainPlayerReference.GetComponent<PlayerMovement>().MovementSpeed * 0.5f;

        protected override List<Tuple<float, float>> HealthDropProbabilitiesPercentage => new List<Tuple<float, float>> {
            new Tuple<float, float>(0.3334f, 0.1f),
            new Tuple<float, float>(0.5f, 0.2f),
        };

        protected override int HealthPoints => (int)(this.baseHealthMultiplier * 3);

        protected override float WaitTimeToAttack => 1.5f;

        protected override float TimeBetweenPunches => 1.5f;

        protected override bool CanBeThrown => true;

        public override float Damage => this.baseDamageMultiplier * 1.0f;

        public override int Points => 150;

        #endregion

        #region Fields

        [SerializeField]
        private Projectile projectilePrefab;

        #endregion

        #region Methods

        protected override IEnumerator Punch()
        {
            this.animator.SetTrigger("Idle");
            yield return new WaitForSeconds(this.WaitTimeToAttack);
            if (Time.time - this.timeLastPunch >= this.TimeBetweenPunches)
            {
                this.animator.SetTrigger("Shoot");
                this.timeLastPunch = Time.time;

                var attackHitbox = this.transform.Find("AttackHitbox");
                var projectileInstance = Instantiate(this.projectilePrefab, attackHitbox.position, attackHitbox.rotation);
                projectileInstance.Initialize(this.Damage, this.FeetPosition);
                projectileInstance.GetComponent<Rigidbody2D>().velocity = attackHitbox.right * 5.0f;
            }
            else
            {
                this.animator.SetTrigger("Idle");
            }
        }

        protected override void OnDie()
        {
            GameManager.instance.IncreaseScoreCount(Scoring.MalandroPistolaKilled);
        }

        #endregion
    }
}
