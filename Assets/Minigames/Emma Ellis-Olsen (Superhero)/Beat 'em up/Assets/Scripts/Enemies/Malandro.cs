using System;
using System.Collections.Generic;
using MoodyBlues.Constants.BeatEmUp;

namespace MoodyBlues.BeatEmUp
{
    public class Malandro : BaseEnemyBehaviour
    {
        #region Properties

        protected override float TimeToSwitchStates => 5.0f;

        protected override float MovementSpeed =>
            this.baseMovementMultiplier * this.mainPlayerReference.GetComponent<PlayerMovement>().MovementSpeed * 0.75f;

        protected override List<Tuple<float, float>> HealthDropProbabilitiesPercentage => new List<Tuple<float, float>> {
            new Tuple<float, float>(0.334f, 0.1f),
        };

        protected override int HealthPoints => (int)(this.baseHealthMultiplier * 3);

        protected override float WaitTimeToAttack => 1.5f;

        protected override float TimeBetweenPunches => 1.0f;

        protected override bool CanBeThrown => true;

        public override float Damage => this.baseDamageMultiplier * 1.0f;

        public override int Points => 100;

        #endregion

        #region Methods

        protected override void OnDie()
        {
            GameManager.instance.IncreaseScoreCount(Scoring.MalandroKilled);
        }

        #endregion
    }
}
