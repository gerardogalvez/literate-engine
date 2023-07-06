namespace MoodyBlues.BeatEmUp
{
    [System.Serializable]
    public class LevelResults
    {
        public enum Rank
        {
            F,
            E,
            D,
            C,
            B,
            A,
            S,
        }

        public int MaxScore;
        public int ScoreObtained;
        public Rank RankObtained;
        public int Affinity;
        public int Coupons;
    }
}
