namespace MoodyBlues.Constants
{
    public static class Axis
    {
        public static string Horizontal => "Horizontal";
        public static string Vertical => "Vertical";
    }

    namespace Fire
    {
        public static class Tags
        {
            public static string StartingPoint => "StartingPoint";
        }
    }

    namespace BeatEmUp
    {
        public static class Tags
        {
            public static string Player => "Player";
            public static string Emma => "Emma";
            public static string Enemy => "Enemy";
            public static string Civilian => "Civilian";
            public static string Collectible => "Collectible";
        }

        public static class Scoring
        {
            public static string VictimRescued => "Civilians rescued ";
            public static string VictimKilled => "Civilians killed";
            public static string MalandroKilled => "Malandros killed";
            public static string MalandroPistolaKilled => "Malandros con pistola killed";
            public static string BrayanKilled => "Brayans killed";
            public static string EmmaDied => "Lost Emma";
        }
    }
}
