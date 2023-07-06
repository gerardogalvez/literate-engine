using System;
using System.Collections.Generic;
using MoodyBlues.Constants.BeatEmUp;

namespace MoodyBlues.BeatEmUp
{
    public class ElBrayan : BaseEnemyBehaviour
    {
        #region Properties

        protected override float TimeToSwitchStates => 5.0f;

        protected override float MovementSpeed =>
            this.baseMovementMultiplier * this.mainPlayerReference.GetComponent<PlayerMovement>().MovementSpeed * 1.0f;

        protected override List<Tuple<float, float>> HealthDropProbabilitiesPercentage => new List<Tuple<float, float>>();

        protected override int HealthPoints =>
            (int)(this.baseHealthMultiplier * 10);

        protected override float WaitTimeToAttack => 1.5f;

        protected override float TimeBetweenPunches => 0.5f;

        protected override bool CanBeThrown => false;

        public override float Damage => this.baseDamageMultiplier * 1.0f;

        public override int Points => 1000;

        #endregion

        #region Methods

        protected override void OnDie()
        {
            GameManager.instance.IncreaseScoreCount(Scoring.BrayanKilled);
        }

        #endregion
    }
}
