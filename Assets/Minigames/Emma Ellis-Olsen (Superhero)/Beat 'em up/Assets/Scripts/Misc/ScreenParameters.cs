using UnityEngine;
using MoodyBlues.Constants.BeatEmUp;
using System.Collections.Generic;

namespace MoodyBlues.BeatEmUp
{
    [System.Serializable]
    public class ScreenParameters
    {
        public Sprite Background;

        public int Malandros;
        public List<ColorChange> MalandrosWithColorChange;

        public int MalandrosConPistola;
        public List<ColorChange> MalandrosConPistolaWithColorChange;

        public int Brayans;
        public List<ColorChange> BrayansWithColorChange;

        public int Civilians;

        public ScreenParameters()
        {
            this.Background = null;

            this.Malandros = 0;
            this.MalandrosWithColorChange = new List<ColorChange>();

            this.MalandrosConPistola = 0;
            this.MalandrosConPistolaWithColorChange = new List<ColorChange>();

            this.Brayans = 0;
            this.BrayansWithColorChange = new List<ColorChange>();

            this.Civilians = 0;
        }

        public int GetTotalScreenScore()
        {
            int score = 0;
            score += this.Malandros * GameManager.scoreDictionary[Scoring.MalandroKilled];
            score += this.MalandrosConPistola * GameManager.scoreDictionary[Scoring.MalandroPistolaKilled];
            score += this.Brayans * GameManager.scoreDictionary[Scoring.BrayanKilled];
            score += this.Civilians * GameManager.scoreDictionary[Scoring.VictimRescued];

            foreach (var colorChange in this.MalandrosWithColorChange)
            {
                score += colorChange.ToSpawn * GameManager.scoreDictionary[Scoring.MalandroKilled];
            }

            foreach (var colorChange in this.MalandrosConPistolaWithColorChange)
            {
                score += colorChange.ToSpawn * GameManager.scoreDictionary[Scoring.MalandroPistolaKilled];
            }

            foreach (var colorChange in this.BrayansWithColorChange)
            {
                score += colorChange.ToSpawn * GameManager.scoreDictionary[Scoring.BrayanKilled];
            }

            return score;
        }
    }

    [System.Serializable]
    public class ColorChange
    {
        public int ToSpawn;
        public float BaseHealthMultiplier;
        public float BaseMovementSpeedMultiplier;
        public float BaseDamageMultiplier;
    }
}
