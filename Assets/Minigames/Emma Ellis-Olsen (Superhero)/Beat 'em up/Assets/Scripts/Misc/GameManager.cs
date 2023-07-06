using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MoodyBlues.Constants.BeatEmUp;
namespace MoodyBlues.BeatEmUp
{
    [RequireComponent(typeof(EntityManager))]
    public class GameManager : MonoBehaviour
    {
        #region Fields

        private Transform leftBoundary;
        private Transform rightBoundary;
        private Transform upBoundary;
        private Transform downBoundary;

        [SerializeField]
        private GameObject healthObjectPrefab;

        [SerializeField]
        private GameObject playerPrefab, emmaPrefab;

        [SerializeField]
        private FloatingScore scorePrefab;

        [SerializeField]
        private ScreenParameters[] LevelParameters;

        private ScreenParameters currentParams;

        [SerializeField]
        private GameObject LivesUILayoutGroup;

        [SerializeField]
        private GameObject ScoringUILayoutGroup;

        [SerializeField]
        private GameObject ScoreElementPrefab;

        [SerializeField]
        private GameObject playerFacePrefab, emmaFacePrefab, heartPrefab;

        [SerializeField]
        private GameObject healthBarPrefab, continueArrowPrefab;

        [SerializeField]
        private Sprite brokenHeart;

        private GameObject worldSpaceCanvas;

        private PlayerBehaviour playerReference, emmaReference;

        private Image[] playerLivesImages, emmaLivesImages;

        private int civiliansToRescue;

        private List<string> ScoreKeysForNextScreen = new List<string>
        {
            Scoring.MalandroKilled,
            Scoring.MalandroPistolaKilled,
            Scoring.BrayanKilled,
            Scoring.VictimKilled,
            Scoring.VictimRescued,
        };

        private int MaxTotalScore;

        private int currentScreen;

        private Dictionary<string, int> scoreCountDictionary = new Dictionary<string, int>
        {
            { Scoring.VictimKilled, 0 },
            { Scoring.VictimRescued, 0 },
            { Scoring.MalandroKilled, 0 },
            { Scoring.BrayanKilled, 0 },
            { Scoring.MalandroPistolaKilled, 0 },
            { Scoring.EmmaDied, 0 },
        };

        private readonly Dictionary<string, int> remainingEnemiesAndCivilians = new Dictionary<string, int>
        {
            { Scoring.VictimKilled + Scoring.VictimRescued, 0 },
            { Scoring.MalandroKilled, 0 },
            { Scoring.BrayanKilled, 0 },
            { Scoring.MalandroPistolaKilled, 0 },
        };

        public static readonly Dictionary<string, int> scoreDictionary = new Dictionary<string, int>
        {
            { Scoring.VictimKilled, -5000 },
            { Scoring.VictimRescued, 5000 },
            { Scoring.MalandroKilled, new Malandro().Points },
            { Scoring.BrayanKilled, new ElBrayan().Points},
            { Scoring.MalandroPistolaKilled, new MalandroConPistola().Points },
            { Scoring.EmmaDied, -100000 },
        };

        public const float VERTICAL_POSITION_DAMAGE_TRESHOLD = 0.2f;

        public static GameManager instance;
        public static HealthObjectFactory healthObjectFactory;

        #endregion

        #region Properties

        public float LeftBoundary => this.leftBoundary.position.x;
        public float RightBoundary => this.rightBoundary.position.x;
        public float UpBoundary => this.upBoundary.position.y;
        public float DownBoundary => this.downBoundary.position.y;

        public FloatingScore ScorePrefab => this.scorePrefab;

        #endregion

        #region Methods

        public static void ResetAnimatorParameters(Animator animator)
        {
            foreach (AnimatorControllerParameter p in animator.parameters)
            {
                switch (p.type)
                {
                    case AnimatorControllerParameterType.Bool:
                        animator.SetBool(p.nameHash, p.defaultBool);
                        break;
                    case AnimatorControllerParameterType.Int:
                        animator.SetInteger(p.nameHash, p.defaultInt);
                        break;
                    case AnimatorControllerParameterType.Float:
                        animator.SetFloat(p.nameHash, p.defaultFloat);
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        animator.ResetTrigger(p.nameHash);
                        break;
                }
            }
        }

        // Use this for initialization
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            if (healthObjectFactory == null)
            {
                healthObjectFactory = new HealthObjectFactory(this.healthObjectPrefab);
            }

            this.LevelParameters = BeatEmUpManager.DayToLoadParameters;
            this.worldSpaceCanvas = GameObject.Find("WorldSpaceCanvas");
            this.leftBoundary = GameObject.Find("LeftBoundary").transform;
            this.rightBoundary = GameObject.Find("RightBoundary").transform;
            this.upBoundary = GameObject.Find("UpBoundary").transform;
            this.downBoundary = GameObject.Find("DownBoundary").transform;
            this.MaxTotalScore = 0;
            this.currentScreen = 0;
            this.civiliansToRescue = 0;
        }

        private void Start()
        {
            float initialX = this.LeftBoundary + (this.RightBoundary - this.LeftBoundary) * 0.15f;
            var player = Instantiate(this.playerPrefab, new Vector2(initialX, Random.Range(this.DownBoundary, this.UpBoundary)), Quaternion.identity);
            var emma = Instantiate(this.emmaPrefab, new Vector2(initialX, Random.Range(this.DownBoundary, this.UpBoundary)), Quaternion.identity);

            player.name = Tags.Player;
            emma.name = Tags.Emma;

            this.playerReference = player.GetComponent<PlayerBehaviour>();
            this.emmaReference = emma.GetComponent<PlayerBehaviour>();

            this.ShowLives(this.playerReference.Lives, this.emmaReference.Lives);
            this.ShowHealthBars(new GameObject[]{ player, emma});

            foreach (var levelParemeters in this.LevelParameters)
            {
                this.MaxTotalScore += levelParemeters.GetTotalScreenScore();
                this.civiliansToRescue += levelParemeters.Civilians;
            }

            this.MaxTotalScore += this.playerReference.Lives * (int)this.playerReference.HealthPoints * 1000;
            this.MaxTotalScore += this.emmaReference.Lives * (int)this.playerReference.HealthPoints * 1000;

            this.LoadScreen();
        }

        private void ShowHealthBars(GameObject[] players)
        {
            foreach (var player in players)
            {
                var healthBar = Instantiate(this.healthBarPrefab);
                healthBar.transform.SetParent(this.worldSpaceCanvas.transform, false);
                healthBar.GetComponent<FollowPlayer>().owner = player;
                healthBar.GetComponent<FollowPlayer>().verticalOffset = player.GetComponent<SpriteRenderer>().bounds.size.y * 0.6f;
                healthBar.GetComponent<FollowPlayer>().enabled = true;
                player.GetComponent<PlayerBehaviour>().HealthBar = healthBar;
            }
        }

        private void ShowLives(int playerLives, int emmaLives)
        {
            this.playerLivesImages = new Image[playerLives];
            this.emmaLivesImages = new Image[emmaLives];

            var playerPortrait = Instantiate(this.playerFacePrefab);
            playerPortrait.transform.SetParent(this.LivesUILayoutGroup.transform, false);

            for (int i = 0; i < playerLives; i++)
            {
                var heart = Instantiate(this.heartPrefab);
                heart.transform.SetParent(this.LivesUILayoutGroup.transform, false);
                this.playerLivesImages[i] = heart.GetComponent<Image>();
            }

            var emmaPortrait = Instantiate(this.emmaFacePrefab);
            emmaPortrait.transform.SetParent(this.LivesUILayoutGroup.transform, false);

            for (int i = 0; i < emmaLives; i++)
            {
                var heart = Instantiate(this.heartPrefab);
                heart.transform.SetParent(this.LivesUILayoutGroup.transform, false);
                this.emmaLivesImages[i] = heart.GetComponent<Image>();
            }
        }

        private void SpawnMalandro()
        {
            if (this.currentParams.Malandros > 0)
            {
                this.gameObject.GetComponent<EntityManager>().SpawnMalandro();
                this.currentParams.Malandros--;
            }
            else
            {
                CancelInvoke("SpawnMalandro");
            }
        }

        private void SpawnEntitiesWithColorChange()
        {
            if (this.currentParams.MalandrosWithColorChange != null)
            {
                foreach (var colorChange in this.currentParams.MalandrosWithColorChange)
                {
                    StartCoroutine(this.SpawnMalandroColorChange(colorChange));
                }
            }

            if (this.currentParams.MalandrosConPistolaWithColorChange != null)
            {
                foreach (var colorChange in this.currentParams.MalandrosConPistolaWithColorChange)
                {
                    StartCoroutine(this.SpawnMalandroConPistolaColorChange(colorChange));
                }
            }

            if (this.currentParams.BrayansWithColorChange != null)
            {
                foreach (var colorChange in this.currentParams.BrayansWithColorChange)
                {
                    StartCoroutine(this.SpawnBrayanColorChange(colorChange));
                }
            }
        }

        private IEnumerator SpawnMalandroColorChange(ColorChange colorChange)
        {
            while (colorChange.ToSpawn > 0)
            {
                this.gameObject.GetComponent<EntityManager>().SpawnMalandro(colorChange);
                colorChange.ToSpawn--;
                yield return new WaitForSeconds(2.0f);
            };
        }

        private IEnumerator SpawnMalandroConPistolaColorChange(ColorChange colorChange)
        {
            while (colorChange.ToSpawn > 0)
            {
                this.gameObject.GetComponent<EntityManager>().SpawnMalandroConPistola(colorChange);
                colorChange.ToSpawn--;
                yield return new WaitForSeconds(4.0f);
            };
        }

        private IEnumerator SpawnBrayanColorChange(ColorChange colorChange)
        {
            while (colorChange.ToSpawn > 0)
            {
                this.gameObject.GetComponent<EntityManager>().SpawnBrayan(colorChange);
                colorChange.ToSpawn--;
                yield return new WaitForSeconds(5.0f);
            };
        }

        private void SpawnMalandroPistola()
        {
            if (this.currentParams.MalandrosConPistola > 0)
            {
                this.gameObject.GetComponent<EntityManager>().SpawnMalandroConPistola();
                this.currentParams.MalandrosConPistola--;
            }
            else
            {
                CancelInvoke("SpawnMalandroPistola");
            }
        }

        private void SpawnBrayan()
        {
            if (this.currentParams.Brayans > 0)
            {
                this.gameObject.GetComponent<EntityManager>().SpawnBrayan();
                this.currentParams.Brayans--;
            }
            else
            {
                CancelInvoke("SpawnBrayan");
            }
        }

        private void SpawnCivilians()
        {
            var entityManager = this.gameObject.GetComponent<EntityManager>();
            for (; this.currentParams.Civilians > 0; this.currentParams.Civilians--)
            {
                entityManager.SpawnCivilian();
            }
        }

        private int GetHealthScore()
        {
            int healthScore = 0;
            var player = this.playerReference;
            var emma = this.emmaReference;

            int playerHealthScore = ((int)player.CurrentHealth + (player.Lives - 1) * (int)player.HealthPoints) * 1000;
            int emmaHealthScore = ((int)emma.CurrentHealth + (emma.Lives - 1) * (int)emma.HealthPoints) * 1000;

            healthScore += playerHealthScore;
            var playerHealthScoreElement = this.CreateScoreElement($"Player remaining health", $"{playerHealthScore}");

            healthScore += emmaHealthScore;
            var emmaHealthScoreElement = this.CreateScoreElement($"Emma remaining health", $"{emmaHealthScore}");

            return healthScore;
        }

        private GameObject CreateScoreElement(string keyText, string valueText)
        {
            var scoreElement = Instantiate(this.ScoreElementPrefab);
            scoreElement.transform.SetParent(this.ScoringUILayoutGroup.transform, false);
            scoreElement.transform.Find("KeyCount").GetComponent<Text>().text = keyText;
            scoreElement.transform.Find("Value").GetComponent<Text>().text = valueText;

            return scoreElement;
        }

        private System.Tuple<int, bool> GetScore()
        {
            int score = 0;
            foreach (var item in scoreCountDictionary)
            {
                Debug.Assert(scoreDictionary.ContainsKey(item.Key), item.Key + " is not present in GameManager.scoreDictionary");

                int itemScore = scoreCountDictionary[item.Key] * scoreDictionary[item.Key];

                if (item.Key != Scoring.EmmaDied)
                {
                    var scoreElement = this.CreateScoreElement($"{item.Key} x {scoreCountDictionary[item.Key]}", $"{itemScore}");
                }
                else
                {
                    if (scoreCountDictionary[item.Key] > 0)
                    {
                        var scoreElement = this.CreateScoreElement($"{item.Key}", $"{itemScore}");
                    }
                }


                score += itemScore;
            }

            int healthScore = this.GetHealthScore();
            score += healthScore;

            return System.Tuple.Create(score, healthScore == 0);
        }

        private void CancelSpawns()
        {
            CancelInvoke("SpawnMalandro");
            CancelInvoke("SpawnMalandroPistola");
            CancelInvoke("SpawnBrayan");
        }

        private void DisableEntities()
        {
            var enemies = GameObject.FindGameObjectsWithTag(Tags.Enemy);
            var player = GameObject.Find(Tags.Player);
            var emma = GameObject.Find(Tags.Emma);

            foreach (var enemy in enemies)
            {
                enemy.GetComponent<BaseEnemyBehaviour>().enabled = false;
                enemy.GetComponent<Animator>().Play("Idle");
            }

            player.GetComponent<PlayerMovement>().enabled = false;
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            player.GetComponent<PlayerFighting>().enabled = false;

            if (!player.GetComponent<PlayerBehaviour>().IsDownOrDead)
            {
                GameManager.ResetAnimatorParameters(player.GetComponent<Animator>());
                player.GetComponent<Animator>().Play("Idle");
            }

            if (emma != null)
            {
                emma.GetComponent<EmmaMovement>().enabled = false;
                emma.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                emma.GetComponent<PlayerFighting>().enabled = false;

                if (!emma.GetComponent<PlayerBehaviour>().IsDownOrDead)
                {
                    GameManager.ResetAnimatorParameters(emma.GetComponent<Animator>());
                    emma.GetComponent<Animator>().Play("Idle");
                }
            }
        }

        private void SpawnEntities()
        {
            InvokeRepeating("SpawnMalandro", 1.0f, 2.0f);
            InvokeRepeating("SpawnMalandroPistola", 3.0f, 4.0f);
            InvokeRepeating("SpawnBrayan", 4.0f, 5.0f);
            this.SpawnEntitiesWithColorChange();
            this.SpawnCivilians();
        }

        private void SetRemainingEntitiesDictionary()
        {
            int malandros = this.currentParams.Malandros + this.currentParams.MalandrosWithColorChange.Sum((ColorChange arg) => arg.ToSpawn);
            int malandrosConPistola = this.currentParams.MalandrosConPistola + this.currentParams.MalandrosConPistolaWithColorChange.Sum((ColorChange arg) => arg.ToSpawn);
            int brayans = this.currentParams.Brayans + this.currentParams.BrayansWithColorChange.Sum((ColorChange arg) => arg.ToSpawn);
            int civilians = this.currentParams.Civilians;

            this.remainingEnemiesAndCivilians[Scoring.MalandroKilled] = malandros;
            this.remainingEnemiesAndCivilians[Scoring.MalandroPistolaKilled] = malandrosConPistola;
            this.remainingEnemiesAndCivilians[Scoring.BrayanKilled] = brayans;
            this.remainingEnemiesAndCivilians[Scoring.VictimKilled + Scoring.VictimRescued] = civilians;
        }

        public bool ShouldShowContinueArrow()
        {
            var emma = GameObject.Find(Tags.Emma);
            var player = GameObject.Find(Tags.Player);
            if (emma == null)
            {
                return this.remainingEnemiesAndCivilians.Sum((arg) => arg.Value) == 0 && !player.GetComponent<PlayerBehaviour>().IsDownOrDead && !this.IsLastScreen();
            }
            else
            {
                return this.remainingEnemiesAndCivilians.Sum((arg) => arg.Value) == 0 && !emma.GetComponent<PlayerBehaviour>().IsDownOrDead && !player.GetComponent<PlayerBehaviour>().IsDownOrDead && !this.IsLastScreen();
            }
        }

        public void ShowContinueArrow()
        {
            var arrow = GameObject.Find("Continue Arrow");
            if (arrow != null)
            {
                arrow.GetComponent<Image>().enabled = true;
            }
            else
            {
                arrow = Instantiate(this.continueArrowPrefab);
                arrow.name = "Continue Arrow";
                arrow.transform.SetParent(this.worldSpaceCanvas.transform);
                arrow.transform.localPosition = new Vector2(
                    x: this.LeftBoundary + (this.RightBoundary - this.LeftBoundary) * 0.9f,
                    y: this.DownBoundary + (this.UpBoundary - this.DownBoundary) * 1.5f
                );
            }
        }

        public void HideContinueArrow()
        {
            var arrow = GameObject.Find("Continue Arrow");
            if (arrow != null)
            {
                arrow.GetComponent<Image>().enabled = false;
            }
        }

        public void RemovePlayerLife(bool isPlayer, int lifeIndex)
        {
            if (isPlayer)
            {
                this.playerLivesImages[lifeIndex].sprite = this.brokenHeart;
            }
            else
            {
                this.emmaLivesImages[lifeIndex].sprite = this.brokenHeart;
            }
        }

        public void IncreaseScoreCount(string key)
        {
            this.scoreCountDictionary[key]++;

            if (this.ScoreKeysForNextScreen.Contains(key))
            {
                if (key != Scoring.VictimKilled && key != Scoring.VictimRescued)
                {
                    this.remainingEnemiesAndCivilians[key]--;
                }
                else
                {
                    this.remainingEnemiesAndCivilians[Scoring.VictimKilled + Scoring.VictimRescued]--;
                }

                if (this.ShouldShowContinueArrow())
                {
                    this.ShowContinueArrow();
                }

            }
        }

        public void CreateScoreCountKey(string key)
        {
            if (!scoreCountDictionary.ContainsKey(key))
            {
                scoreCountDictionary.Add(key, 0);
            }
        }

        public void LoadScreen()
        {
            float initialX = this.LeftBoundary + (this.RightBoundary - this.LeftBoundary) * 0.15f;

            this.currentParams = this.LevelParameters[this.currentScreen];

            GameObject.Find(Tags.Player).transform.position = new Vector2(initialX, this.DownBoundary + (this.UpBoundary - this.DownBoundary) * 0.45f);
            GameObject.Find(Tags.Emma).transform.position = new Vector2(initialX, this.DownBoundary + (this.UpBoundary - this.DownBoundary) * 0.9f);

            GameObject.Find("Background").GetComponent<SpriteRenderer>().sprite = this.currentParams.Background;

            this.SetRemainingEntitiesDictionary();
            this.SpawnEntities();

            this.HideContinueArrow();
            this.currentScreen++;
        }

        private LevelResults.Rank GetRank(int scoreObtained, int maxScore, bool gameOver, int civiliansRescued, int totalCiviliansToRescue)
        {
            if (gameOver)
            {
                return LevelResults.Rank.F;
            }

            if (civiliansRescued == totalCiviliansToRescue && scoreObtained >= maxScore * 0.9f)
            {
                return LevelResults.Rank.S;
            }

            if (civiliansRescued >= totalCiviliansToRescue * 0.75f && scoreObtained >= maxScore * 0.75f)
            {
                return LevelResults.Rank.A;
            }

            if (civiliansRescued >= totalCiviliansToRescue * 0.50f && scoreObtained >= maxScore * 0.65f)
            {
                return LevelResults.Rank.B;
            }

            if (scoreObtained >= maxScore * 0.50f)
            {
                return LevelResults.Rank.C;
            }

            if (scoreObtained >= maxScore * 0.25f)
            {
                return LevelResults.Rank.D;
            }

            return LevelResults.Rank.E;
        }

        private System.Tuple<int, int> GetAffinityAndCoupons(LevelResults.Rank rank)
        {
            switch (rank)
            {
                case LevelResults.Rank.S:
                    return System.Tuple.Create(10, 40);
                case LevelResults.Rank.A:
                    return System.Tuple.Create(5, 20);
                case LevelResults.Rank.B:
                    return System.Tuple.Create(3, 10);
                case LevelResults.Rank.C:
                    return System.Tuple.Create(1, 5);
                case LevelResults.Rank.D:
                    return System.Tuple.Create(0, 0);
                case LevelResults.Rank.E:
                    return System.Tuple.Create(-1, 0);
                default:
                    return System.Tuple.Create(-3, 0);
            }
        }

        public void GameOver()
        {
            GameObject.Find("LivesPanel").SetActive(false);
            GameObject.Find("ScoringPanel").GetComponent<Image>().enabled = true;
            this.CancelSpawns();
            this.DisableEntities();

            System.Tuple<int, bool> results = this.GetScore();
            int totalScore = results.Item1;
            bool gameOver = results.Item2;

            if (totalScore < 0)
            {
                totalScore = 0;
            }

            var scoreElement = this.CreateScoreElement($"Total", $"{totalScore}");
            scoreElement.transform.Find("KeyCount").GetComponent<Text>().color = Color.red;
            scoreElement.transform.Find("Value").GetComponent<Text>().color = Color.red;

            LevelResults.Rank rank = this.GetRank(totalScore, this.MaxTotalScore, gameOver, this.scoreCountDictionary[Scoring.VictimRescued], this.civiliansToRescue);
            System.Tuple<int, int> AffinityCoupons = this.GetAffinityAndCoupons(rank);
            LevelResults levelResults = new LevelResults {
                MaxScore = this.MaxTotalScore,
                ScoreObtained = totalScore,
                RankObtained = rank,
                Affinity = AffinityCoupons.Item1,
                Coupons = AffinityCoupons.Item2,
            };

            SaveManager.SaveBeatEmUpResults(BeatEmUpManager.DayToLoad, levelResults);
        }

        public bool IsLastScreen()
        {
            return this.currentScreen == (this.LevelParameters.Length);
        }

        public bool CanContinue()
        {
            var emma = GameObject.Find(Tags.Emma);
            var enemies = GameObject.FindGameObjectsWithTag(Tags.Enemy);
            var civilians = GameObject.FindGameObjectsWithTag(Tags.Civilian);

            // Emma is alive
            if (emma != null)
            {
                var emmaBehaviour = emma.GetComponent<PlayerBehaviour>();
                // Can continue if Emma is NOT down, if if all enemies
                // have been killed and if all civilians have been killed/rescued
                return !emmaBehaviour.IsDownOrDead && enemies.Length == 0 && civilians.Length == 0;
            }
            else // Emma died
            {
                // Can continue if all enemies have been killed and if all
                // civilians have been killed/rescued
                return enemies.Length == 0 && civilians.Length == 0;
            }
        }

        public bool FinishedSpawningEntities()
        {
            var enemyColorChanges = new List<List<ColorChange>>
            {
                this.currentParams.MalandrosWithColorChange,
                this.currentParams.MalandrosConPistolaWithColorChange,
                this.currentParams.BrayansWithColorChange,
            };

            foreach (var enemyType in enemyColorChanges)
            {
                foreach (var colorChange in enemyType)
                {
                    if (colorChange.ToSpawn > 0)
                    {
                        return false;
                    }
                }
            }

            return
                this.currentParams.Malandros == 0
                && this.currentParams.MalandrosConPistola == 0
                && this.currentParams.Brayans == 0
                && this.currentParams.Civilians == 0;
        }

        public void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                SceneManager.LoadScene("Manager");
            }
        }

        #endregion
    }
}
