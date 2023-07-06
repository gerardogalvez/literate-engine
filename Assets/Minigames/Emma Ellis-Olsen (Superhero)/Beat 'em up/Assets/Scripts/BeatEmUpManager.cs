using UnityEngine;
using MoodyBlues.BeatEmUp;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class BeatEmUpManager : MonoBehaviour
{
    private List<ScreenParameters[]> days;

    public static ScreenParameters[] DayToLoadParameters;

    public static int DayToLoad;

    private void Awake()
    {
        this.days = new List<ScreenParameters[]>();

        #region Day 1
        this.days.Add(new ScreenParameters[]
        {
                new ScreenParameters
                {
                    Background = Resources.Load<Sprite>("Sprites/Backgrounds/1-1"),
                    Malandros = 1,
                    MalandrosConPistola = 1,
                    Civilians = 1,
                },
                new ScreenParameters
                {
                    Background = Resources.Load<Sprite>("Sprites/Backgrounds/1-2"),
                    Malandros = 1,
                    MalandrosConPistola = 1,
                    Civilians = 0,
                },
                new ScreenParameters
                {
                    Background = Resources.Load<Sprite>("Sprites/Backgrounds/1-3"),
                    Malandros = 1,
                    MalandrosConPistola = 0,
                    Brayans = 0,
                    Civilians = 0,
                },
        });
        #endregion

        #region Day 2
        this.days.Add(new ScreenParameters[]
        {
            new ScreenParameters
            {
                Background = Resources.Load<Sprite>("Sprites/Backgrounds/2-1"),
                Malandros = 7,
                MalandrosConPistola = 3,
                Civilians = 1,
            },
            new ScreenParameters
            {
                Background = Resources.Load<Sprite>("Sprites/Backgrounds/2-2"),
                Malandros = 12,
                MalandrosConPistola = 5,
                Civilians = 2,
            },
            new ScreenParameters
            {
                Background = Resources.Load<Sprite>("Sprites/Backgrounds/2-3"),
                Malandros = 5,
                MalandrosConPistola = 2,
                Brayans = 2,
                Civilians = 3,
            },
        });
        #endregion

        #region Day 3
        this.days.Add(new ScreenParameters[]
        {
            new ScreenParameters
            {
                Background = Resources.Load<Sprite>("Sprites/Backgrounds/3-1"),
                MalandrosWithColorChange = new List<ColorChange>
                {
                    new ColorChange
                    {
                        ToSpawn = 6,
                        BaseHealthMultiplier = 1.0f,
                        BaseDamageMultiplier = 1.0f,
                        BaseMovementSpeedMultiplier = 1.0f
                    }
                },
                MalandrosConPistolaWithColorChange = new List<ColorChange>
                {
                    new ColorChange
                    {
                        ToSpawn = 3,
                        BaseHealthMultiplier = 1.0f,
                        BaseDamageMultiplier = 1.0f,
                        BaseMovementSpeedMultiplier = 1.0f
                    }
                },
                Civilians = 2,
            },
            new ScreenParameters
            {
                Background = Resources.Load<Sprite>("Sprites/Backgrounds/3-2"),
                MalandrosWithColorChange = new List<ColorChange>
                {
                    new ColorChange
                    {
                        ToSpawn = 5,
                        BaseHealthMultiplier = 1.0f,
                        BaseDamageMultiplier = 1.0f,
                        BaseMovementSpeedMultiplier = 1.0f
                    }
                },
                MalandrosConPistolaWithColorChange = new List<ColorChange>
                {
                    new ColorChange
                    {
                        ToSpawn = 5,
                        BaseHealthMultiplier = 1.0f,
                        BaseDamageMultiplier = 1.0f,
                        BaseMovementSpeedMultiplier = 1.0f
                    }
                },
                Brayans = 1,
                Civilians = 3,
            },
            new ScreenParameters
            {
                Background = Resources.Load<Sprite>("Sprites/Backgrounds/3-3"),
                MalandrosWithColorChange = new List<ColorChange>
                {
                    new ColorChange
                    {
                        ToSpawn = 3,
                        BaseHealthMultiplier = 1.0f,
                        BaseDamageMultiplier = 1.0f,
                        BaseMovementSpeedMultiplier = 1.0f
                    }
                },
                MalandrosConPistolaWithColorChange = new List<ColorChange>
                {
                    new ColorChange
                    {
                        ToSpawn = 3,
                        BaseHealthMultiplier = 1.0f,
                        BaseDamageMultiplier = 1.0f,
                        BaseMovementSpeedMultiplier = 1.0f
                    }
                },
                BrayansWithColorChange = new List<ColorChange>
                {
                    new ColorChange
                    {
                        ToSpawn = 1,
                        BaseHealthMultiplier = 1.0f,
                        BaseDamageMultiplier = 1.0f,
                        BaseMovementSpeedMultiplier = 1.0f
                    }
                },
                Civilians = 3,
            },
        });
        #endregion

        #region Day 4
        this.days.Add(new ScreenParameters[]
        {
            new ScreenParameters
            {
                Background = Resources.Load<Sprite>("Sprites/Backgrounds/4-1"),
                MalandrosWithColorChange = new List<ColorChange>
                {
                    new ColorChange
                    {
                        ToSpawn = 6,
                        BaseHealthMultiplier = 1.0f,
                        BaseDamageMultiplier = 1.0f,
                        BaseMovementSpeedMultiplier = 1.0f
                    }
                },
                MalandrosConPistolaWithColorChange = new List<ColorChange>
                {
                    new ColorChange
                    {
                        ToSpawn = 3,
                        BaseHealthMultiplier = 1.0f,
                        BaseDamageMultiplier = 1.0f,
                        BaseMovementSpeedMultiplier = 1.0f
                    }
                },
                Civilians = 2,
            },
            new ScreenParameters
            {
                Background = Resources.Load<Sprite>("Sprites/Backgrounds/4-2"),
                MalandrosWithColorChange = new List<ColorChange>
                {
                    new ColorChange
                    {
                        ToSpawn = 5,
                        BaseHealthMultiplier = 1.0f,
                        BaseDamageMultiplier = 1.0f,
                        BaseMovementSpeedMultiplier = 1.0f
                    }
                },
                MalandrosConPistolaWithColorChange = new List<ColorChange>
                {
                    new ColorChange
                    {
                        ToSpawn = 5,
                        BaseHealthMultiplier = 1.0f,
                        BaseDamageMultiplier = 1.0f,
                        BaseMovementSpeedMultiplier = 1.0f
                    }
                },
                Brayans = 2,
                Civilians = 3,
            },
            new ScreenParameters
            {
                Background = Resources.Load<Sprite>("Sprites/Backgrounds/4-3"),
                Malandros = 10,
                MalandrosWithColorChange = new List<ColorChange>
                {
                    new ColorChange
                    {
                        ToSpawn = 5,
                        BaseHealthMultiplier = 1.0f,
                        BaseDamageMultiplier = 1.0f,
                        BaseMovementSpeedMultiplier = 1.0f
                    }
                },
                MalandrosConPistolaWithColorChange = new List<ColorChange>
                {
                    new ColorChange
                    {
                        ToSpawn = 3,
                        BaseHealthMultiplier = 1.0f,
                        BaseDamageMultiplier = 1.0f,
                        BaseMovementSpeedMultiplier = 1.0f
                    }
                },
                BrayansWithColorChange = new List<ColorChange>
                {
                    new ColorChange
                    {
                        ToSpawn = 1,
                        BaseHealthMultiplier = 1.0f,
                        BaseDamageMultiplier = 1.0f,
                        BaseMovementSpeedMultiplier = 1.0f
                    }
                },
                Civilians = 5,
            },
        });
        #endregion

        #region Day 5
        this.days.Add(new ScreenParameters[]
        {
            new ScreenParameters
            {
                Background = Resources.Load<Sprite>("Sprites/Backgrounds/5-1"),
                MalandrosWithColorChange = new List<ColorChange>
                {
                    new ColorChange
                    {
                        ToSpawn = 8,
                        BaseHealthMultiplier = 1.0f,
                        BaseDamageMultiplier = 1.0f,
                        BaseMovementSpeedMultiplier = 1.0f
                    }
                },
                MalandrosConPistolaWithColorChange = new List<ColorChange>
                {
                    new ColorChange
                    {
                        ToSpawn = 4,
                        BaseHealthMultiplier = 1.0f,
                        BaseDamageMultiplier = 1.0f,
                        BaseMovementSpeedMultiplier = 1.0f
                    }
                },
                Civilians = 3,
            },
            new ScreenParameters
            {
                Background = Resources.Load<Sprite>("Sprites/Backgrounds/5-2"),
                MalandrosConPistolaWithColorChange = new List<ColorChange>
                {
                    new ColorChange
                    {
                        ToSpawn = 6,
                        BaseHealthMultiplier = 1.0f,
                        BaseDamageMultiplier = 1.0f,
                        BaseMovementSpeedMultiplier = 1.0f
                    }
                },
                Brayans = 3,
                Civilians = 5,
            },
            new ScreenParameters
            {
                Background = Resources.Load<Sprite>("Sprites/Backgrounds/5-3"),
                MalandrosWithColorChange = new List<ColorChange>
                {
                    new ColorChange
                    {
                        ToSpawn = 12,
                        BaseHealthMultiplier = 1.0f,
                        BaseDamageMultiplier = 1.0f,
                        BaseMovementSpeedMultiplier = 1.0f
                    }
                },
                MalandrosConPistolaWithColorChange = new List<ColorChange>
                {
                    new ColorChange
                    {
                        ToSpawn = 6,
                        BaseHealthMultiplier = 1.0f,
                        BaseDamageMultiplier = 1.0f,
                        BaseMovementSpeedMultiplier = 1.0f
                    }
                },
                BrayansWithColorChange = new List<ColorChange>
                {
                    // Final Boss
                    new ColorChange
                    {
                        ToSpawn = 1,
                        BaseHealthMultiplier = 1.0f,
                        BaseDamageMultiplier = 1.0f,
                        BaseMovementSpeedMultiplier = 1.0f
                    }
                },
                Civilians = 10,
            },
        });
        #endregion
    }

    public void LoadDay(int index)
    {
        Debug.Assert(index >= 0 && index < this.days.Count);
        BeatEmUpManager.DayToLoadParameters = this.days[index];
        BeatEmUpManager.DayToLoad = index;
        SceneManager.LoadScene("Day");
    }
}
