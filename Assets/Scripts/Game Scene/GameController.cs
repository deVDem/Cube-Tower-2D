using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Application = UnityEngine.Application;

namespace Game_Scene
{
    public class GameController : MonoBehaviour
    {
        public GameObject allCubes;
        public Transform placingCube;
        public GameObject prefabOfCube;
        public Transform playerCamera;
        public ExplodeCubes explodeCubes;
        public int bestScore, score;
        public Text scoreText;
        public Animator uiAnimator;
        public AudioSource placeSource;
        public FPSCounter FPSText;


        // Google Play Services

        private string[] achivementsIds =
        {
            "CgkIqMWb58IFEAIQAA", "CgkIqMWb58IFEAIQAQ", "CgkIqMWb58IFEAIQAg", "CgkIqMWb58IFEAIQAw",
            "CgkIqMWb58IFEAIQBA", "CgkIqMWb58IFEAIQBQ", "CgkIqMWb58IFEAIQDA", "CgkIqMWb58IFEAIQDQ"
        };
        // Open the "on time" mode, hard, creative, player discovered, interested in this game, pro detected, am i crazy, i am the creator

        private string[] leaderBoardsIds =
            {"CgkIqMWb58IFEAIQBg", "CgkIqMWb58IFEAIQBw", "CgkIqMWb58IFEAIQCA", "CgkIqMWb58IFEAIQCQ"};
        // normal mode, on time, hard, loss rate

        public Button GPGConnect;
        public Button GPGAchievements;
        public Button GPGLeaderBoards;
        
        // AdMob
        public GameObject addScoreAdButton;
        private bool _addscore;


        // GameModes UI

        public Text DescriptionDefault;
        public Text DescriptionOnTime;
        public Text DescriptionHard;
        public Text DescriptionCreative;
        public Button ButtonDefault;
        public Button ButtonOnTime;
        public Button ButtonHard;
        public Button ButtonCreative;

        public void changeGameMode(int mode)
        {
            string gamemode = "default";
            switch (mode)
            {
                case 0:
                    gamemode = "default";
                    break;
                case 1:
                    gamemode = "ontime";
                    break;
                case 2:
                    gamemode = "hard";
                    break;
                case 3:
                    gamemode = "creative";
                    break;
            }

            PlayerPrefs.SetString("gamemode", gamemode);
            PlayerPrefs.Save();
            ButtonDefault.interactable = false;
            ButtonOnTime.interactable = false;
            ButtonHard.interactable = false;
            ButtonCreative.interactable = false;
            _isLose = true;
            RestartGame();
        }

        private void UpdateGameModeList()
        {
            string[] gamemodes = {"default", "ontime", "hard", "creative"};
            bool[] available = new bool[gamemodes.Length];
            string[] buttontexts = new string[4];
            string[] descTexts = new string[4];
            for (int i = 0; i < gamemodes.Length; i++)
            {
                int bestScoreGameMode;
                if (i != 0)
                {
                    bestScoreGameMode = PlayerPrefs.GetInt(gamemodes[i - 1] + ".bestScore");

                    buttontexts[i] = LocaleManager.GetLocalizedText("Game.UI.GameModes.Unavailable");
                    descTexts[i] =
                        LocaleManager.GetLocalizedText("Game.UI.GameModes." + gamemodes[i] + ".Description") + "\n" +
                        String.Format(LocaleManager.GetLocalizedText("Game.UI.GameModes.Needs"),
                            PlayerPrefs.GetInt(gamemodes[i] + ".bestScore"), 50);
                }
                else
                {
                    bestScoreGameMode = PlayerPrefs.GetInt(gamemodes[i] + ".bestScore");
                    descTexts[i] =
                        LocaleManager.GetLocalizedText("Game.UI.GameModes." + gamemodes[i] + ".Description") + "\n" +
                        String.Format(LocaleManager.GetLocalizedText("Game.UI.GameModes.Best"),
                            bestScoreGameMode);
                }

                if (i == 0 || bestScoreGameMode >= 50)
                {
                    available[i] = true;
                    buttontexts[i] = LocaleManager.GetLocalizedText("Game.UI.GameModes.Change");
                }

                if (i == 3)
                {
                    available[i] = false;
                    buttontexts[i] = LocaleManager.GetLocalizedText("Game.UI.GameModes.TemporaryUnavailable");
                }

                if (String.Equals(_gameModeController.GameMode, gamemodes[i],
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    available[i] = false;
                    buttontexts[i] = LocaleManager.GetLocalizedText("Game.UI.GameModes.Current");
                }
            }

            ButtonDefault.interactable = available[0];
            ButtonDefault.GetComponentInChildren<Text>().text = buttontexts[0];
            ButtonOnTime.interactable = available[1];
            ButtonOnTime.GetComponentInChildren<Text>().text = buttontexts[1];
            ButtonHard.interactable = available[2];
            ButtonHard.GetComponentInChildren<Text>().text = buttontexts[2];
            ButtonCreative.interactable = available[3];
            ButtonCreative.GetComponentInChildren<Text>().text = buttontexts[3];

            DescriptionDefault.text = descTexts[0];
            DescriptionOnTime.text = descTexts[1];
            DescriptionHard.text = descTexts[2];
            DescriptionCreative.text = descTexts[3];
        }

        //Settings UI
        public Toggle musicToggle;
        public Dropdown graphicsDropdown;
        public Toggle fpsToggle;
        private bool started;

        private float speedCubeChange = 0.5f;
        private CubePos _nowCube = new CubePos(0, 0);
        private Rigidbody2D _allCubesRb;
        private bool _isLose;
        private bool _alreadyTouched;
        private bool _alreadyTouchedPC;
        private float _minX, _maxX;
        private Camera _playerCameraComponent;
        private bool _gameStarted;
        private MusicLogic _musicLogic;
        private bool _exitGame;
        private GameModeController _gameModeController;
        private GameObject _zeroCube;

        private static GameController _instance;

        private List<Vector2> _allCubesPositions = new List<Vector2>
        {
            new Vector2(0, 0)
        };

        private Coroutine _changeCubeCoroutine;

        void Start()
        {
            _instance = this;
            _gameModeController = transform.GetChild(0).GetComponent<GameModeController>();
            _musicLogic = FindObjectOfType<MusicLogic>();
            _allCubesRb = allCubes.GetComponent<Rigidbody2D>();
            _allCubesRb.simulated = false;
            _playerCameraComponent = playerCamera.GetComponent<Camera>();
            bestScore = PlayerPrefs.GetInt(_gameModeController.GameMode + ".bestScore", 0);
            musicToggle.isOn = PlayerPrefs.GetInt("musicSetting", 1) == 1;
            graphicsDropdown.options.Add(new Dropdown.OptionData(LocaleManager.GetLocalizedText("Game.UI.fastest")));
            graphicsDropdown.options.Add(new Dropdown.OptionData(LocaleManager.GetLocalizedText("Game.UI.medium")));
            graphicsDropdown.options.Add(new Dropdown.OptionData(LocaleManager.GetLocalizedText("Game.UI.high")));
            FPSText.TextEnabled = PlayerPrefs.GetInt("fpsShow", 0) == 1;
            fpsToggle.isOn = PlayerPrefs.GetInt("fpsShow", 0) == 1;

            UpdateGameModeList();
            _zeroCube = allCubes.transform.GetChild(0).gameObject;
            _changeCubeCoroutine = StartCoroutine(CheckingAvailablePos());

            graphicsDropdown.value = PlayerPrefs.GetInt("graphicsSetting", 1);
            graphicsDropdown.value = 1;
            graphicsDropdown.value = PlayerPrefs.GetInt("graphicsSetting", 1);
            started = true;
            switch (graphicsDropdown.value)
            {
                case 0:
                    QualitySettings.SetQualityLevel(0);
                    break;
                case 1:
                    QualitySettings.SetQualityLevel(2);
                    break;
                case 2:
                    QualitySettings.SetQualityLevel(5);
                    break;
            }

            if (GooglePlayUtils.Instance == null)
            {
                GameObject gameObject = new GameObject(name = "GPG Manager");
                gameObject.AddComponent<GooglePlayUtils>();
                DontDestroyOnLoad(gameObject);
            }

            Invoke(nameof(CheckGms), 1);
        }

        private void CheckGms()
        {
            if (GooglePlayUtils.Instance.Connected)
            {
                GPGConnect.gameObject.SetActive(false);
                GPGAchievements.gameObject.SetActive(true);
                GPGLeaderBoards.gameObject.SetActive(true);
            }
            else
            {
                GPGConnect.gameObject.SetActive(true);
                GPGAchievements.gameObject.SetActive(false);
                GPGLeaderBoards.gameObject.SetActive(false);
            }
        }

        public void RestartGame()
        {
            StopAllCoroutines();
            SceneTransition.SwitchToScene("Game");
        }

        // Update is called once per frame
        void Update()
        {
            if (_addscore)
            {
                GetReward();
            }
                if (_changeCubeCoroutine == null)
                _changeCubeCoroutine = StartCoroutine(CheckingAvailablePos());
            if (!_isLose)
            {
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    if (!_alreadyTouched)
                    {
                        _alreadyTouched = true;
                        if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;
                        PlaceCube();
                    }
                }
                else
                {
                    _alreadyTouched = false;
                }

                if (Math.Abs(Input.GetAxis("Jump") - 1) < 0.1f)
                {
                    if (!_alreadyTouchedPC)
                    {
                        PlaceCube();
                        _alreadyTouchedPC = true;
                    }
                }
                else
                {
                    _alreadyTouchedPC = false;
                }


                scoreText.text = "<color=#ff0000>" + LocaleManager.GetLocalizedText("Game.UI.best") + "</color>: " +
                                 bestScore +
                                 "\n<color=#ff5555>" + LocaleManager.GetLocalizedText("Game.UI.score") + "</color>: " +
                                 score;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneTransition.sendSceneClosing();
                _exitGame = true;
            }
        }

        public static void quit()
        {
            if (_instance._exitGame)
            {
                Debug.Log("Application Closed");
                Application.Quit();
            }
        }

        private void PlaceCube()
        {
            if (!_gameStarted)
            {
                uiAnimator.SetTrigger("showScore");
                _gameModeController.onGameStarted();
                _gameStarted = true;
            }

            _alreadyTouched = true;
            _allCubesRb.isKinematic = true;
            var position = placingCube.position;
            Vector2 newPos = new Vector2(position.x,
                position.y);
            GameObject newCube = Instantiate(prefabOfCube, newPos, Quaternion.identity);
            newCube.name = "Cube";
            newCube.transform.SetParent(allCubes.transform);
            _allCubesPositions.Add(newPos);
            _nowCube.SetVector(newPos);
            _allCubesRb.isKinematic = false;
            if (score < _nowCube.GetVector().y) score = Mathf.RoundToInt(_nowCube.GetVector().y);
            if (bestScore < score)
            {
                bestScore = score;
                PlayerPrefs.SetInt(_gameModeController.GameMode + ".bestScore", score);
                PlayerPrefs.Save();
            }

            placeSource.Play();
            CheckCube();
            if (!_isLose) CheckPhysics();
            _gameModeController.CubePlaced();
        }

        private void CheckPhysics()
        {
            _allCubesRb.simulated = true;
            explodeCubes.checking = true;
            GameObject tmpObject = Instantiate(allCubes, allCubes.transform.position, Quaternion.identity);
            Rigidbody2D tmpRg = tmpObject.GetComponent<Rigidbody2D>();
            allCubes.SetActive(false);
            Physics2D.autoSimulation = false;

            try
            {
                int i = 0;
                for (; i < 250; i++)
                {
                    Physics2D.Simulate(0.02f);
                    if (explodeCubes.collided) break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Physics2D.autoSimulation = true;

            allCubes.SetActive(true);
            _allCubesRb.simulated = false;
            explodeCubes.checking = false;
            if (explodeCubes.collided || Mathf.Abs(tmpRg.velocity.magnitude) > 2f ||
                Mathf.Abs(tmpRg.angularVelocity) > 0.5f / score)
            {
                UpdateAchievementsLeaders();
                _isLose = true;
                StopCoroutine(_changeCubeCoroutine);
                Destroy(placingCube.gameObject);
                _gameModeController.onGameEnded();
                StartCoroutine(wait());
                _allCubesRb.simulated = true;
            }

            explodeCubes.collided = false;
            Destroy(tmpObject);
        }


        IEnumerator wait()
        {
            yield return new WaitForSeconds(10f);
            if (allCubes != null && _allCubesRb.velocity.magnitude < 0.1f &&
                _allCubesRb.angularVelocity < 0.2f) GameOver();
            else
            {
                StartCoroutine(wait());
            }
        }

        private void FixedUpdate()
        {
            CalculatePosCamera();
        }

        public static float GetSizeCamera()
        {
            return _instance._playerCameraComponent.orthographicSize;
        }

        private void CalculatePosCamera()
        {
            float lastPosition = playerCamera.position.y;
            float lastSize = _playerCameraComponent.orthographicSize;
            float newSize = 2f * Mathf.Max(Mathf.Abs(_minX), Mathf.Abs(_maxX)) + 5f;
            if (!_isLose)
                playerCamera.position =
                    new Vector3(0, lastPosition + (_nowCube.Y - lastPosition) * Time.fixedDeltaTime, -10);
            else
            {
                playerCamera.position = new Vector3(0, lastPosition + (0f - lastPosition) * Time.fixedDeltaTime, -10);
            }

            _playerCameraComponent.orthographicSize = lastSize + (newSize - lastSize) * Time.fixedDeltaTime;
        }


        IEnumerator CheckingAvailablePos()
        {
            while (!_isLose)
            {
                CheckCube();
                yield return new WaitForSeconds(speedCubeChange);
            }
        }

        private void CheckCube()
        {
            List<Vector2> positions = new List<Vector2>();
            if (IsPositionEmpty(new Vector2(_nowCube.X - 1f, _nowCube.Y)) &&
                Math.Abs(_nowCube.X - 1f - placingCube.position.x) > 0.1f)
                positions.Add(new Vector2(_nowCube.X - 1f, _nowCube.Y));
            if (IsPositionEmpty(new Vector2(_nowCube.X + 1f, _nowCube.Y)) &&
                Math.Abs(_nowCube.X + 1f - placingCube.position.x) > 0.1f)
                positions.Add(new Vector2(_nowCube.X + 1f, _nowCube.Y));
            if (IsPositionEmpty(new Vector2(_nowCube.X, _nowCube.Y + 1f)) &&
                Math.Abs(_nowCube.Y + 1f - placingCube.position.y) > 0.1f)
                positions.Add(new Vector2(_nowCube.X, _nowCube.Y + 1f));
            if (IsPositionEmpty(new Vector2(_nowCube.X, _nowCube.Y - 1f)) &&
                Math.Abs(_nowCube.Y - 1f - placingCube.position.y) > 0.1f)
                positions.Add(new Vector2(_nowCube.X, _nowCube.Y - 1f));
            int random = Random.Range(0, positions.Count);
            if (positions.Count == 0 && IsPositionEmpty(placingCube.position)) positions.Add(placingCube.position);
            else if (positions.Count == 0 && !IsPositionEmpty(placingCube.position))
            {
                GameOver();
            }

            if (!_isLose)
            {
                Vector2 vector2 = _zeroCube.transform.position;
                placingCube.position = vector2 + positions[random];
            }
        }

        private bool IsPositionEmpty(Vector2 targetPos)
        {
            if (targetPos.y <= -1)
                return false;
            foreach (Vector2 pos in _allCubesPositions)
            {
                if (pos.x < _minX) _minX = pos.x;
                if (pos.x > _maxX) _maxX = pos.x;
                if (Math.Abs(pos.x - targetPos.x) < 0.1f && Math.Abs(pos.y - targetPos.y) < 0.1f) return false;
            }

            return true;
        }

        public void settingChange(string setting)
        {
            if (started)
            {
                switch (setting)
                {
                    case "music":
                        PlayerPrefs.SetInt("musicSetting", musicToggle.isOn ? 1 : 0);
                        PlayerPrefs.Save();
                        _musicLogic.Change(musicToggle.isOn);
                        break;
                    case "graphics":
                        switch (graphicsDropdown.value)
                        {
                            case 0:
                                QualitySettings.SetQualityLevel(0);
                                break;
                            case 1:
                                QualitySettings.SetQualityLevel(2);
                                break;
                            case 2:
                                QualitySettings.SetQualityLevel(5);
                                break;
                        }

                        PlayerPrefs.SetInt("graphicsSetting", graphicsDropdown.value);
                        PlayerPrefs.Save();
                        break;
                    case "fps":
                        PlayerPrefs.SetInt("fpsShow", fpsToggle.isOn ? 1 : 0);
                        PlayerPrefs.Save();
                        FPSText.TextEnabled = PlayerPrefs.GetInt("fpsShow", 0) == 1;
                        break;
                    default:
                        Debug.LogError("Unknown \"" + setting + "\" setting.");
                        break;
                }
            }
        }


        public void GameOver()
        {
            UpdateAchievementsLeaders();
            addScoreAdButton.SetActive(AdMobManager.Instance.rewardedAd.IsLoaded());
            _isLose = true;
            StopCoroutine(_changeCubeCoroutine);
            if (placingCube != null) Destroy(placingCube.gameObject);
            _gameModeController.onGameEnded();
            explodeCubes.Explode(allCubes.transform, -20f);
        }

        private void UpdateAchievementsLeaders()
        {
            switch (_gameModeController.GameMode)
            {
                case "default":
                    GooglePlayUtils.GetAchievement(achivementsIds[0], bestScore / 50f * 100f);
                    GooglePlayUtils.GetAchievement(achivementsIds[3], bestScore / 100f * 100f);
                    GooglePlayUtils.UploadScore(leaderBoardsIds[0], bestScore);
                    break;
                case "ontime":
                    GooglePlayUtils.GetAchievement(achivementsIds[1], bestScore / 50f * 100f);
                    GooglePlayUtils.GetAchievement(achivementsIds[4], bestScore / 100f * 100f);
                    GooglePlayUtils.UploadScore(leaderBoardsIds[1], bestScore);
                    break;
                case "hard":
                    GooglePlayUtils.GetAchievement(achivementsIds[2], bestScore / 50f * 100f);
                    GooglePlayUtils.GetAchievement(achivementsIds[5], bestScore / 100f * 100f);
                    GooglePlayUtils.GetAchievement(achivementsIds[6], bestScore / 300f * 100f);
                    GooglePlayUtils.UploadScore(leaderBoardsIds[2], bestScore);
                    break;
                case "creative":
                    break;
            }
        }

        public void reLoginGPG()
        {
            GooglePlayUtils.ReLogin();
            CheckGms();
        }

        public void ShowAchievements()
        {
            GooglePlayUtils.ShowAchievements();
        }

        public void ShowLeaderBoard()
        {
            GooglePlayUtils.ShowLeaderBoard();
        }


        public bool IsLose
        {
            get => _isLose;
        }

        public void ClearRecords()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            RestartGame();
        }

        public void showRewindAd()
        {
            AdMobManager.showAd();
            addScoreAdButton.SetActive(false);
        }

        public static void getReward()
        {
            _instance._addscore = true;
        }

        private void GetReward()
        {
            _instance._addscore = false;
            score += 25;
            if (bestScore < score)
            {
                bestScore = score;
                PlayerPrefs.SetInt(_gameModeController.GameMode + ".bestScore", score);
                PlayerPrefs.Save();
            }
            scoreText.text = "<color=#ff0000>" + LocaleManager.GetLocalizedText("Game.UI.best") +
                                       "</color>: " +
                                       bestScore +
                                       "\n<color=#ff5555>" + LocaleManager.GetLocalizedText("Game.UI.score") +
                                       "</color>: " +
                                       score;
            UpdateAchievementsLeaders();
        }
    }


    struct CubePos
    {
        public int X, Y;

        public CubePos(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Vector2 GetVector()
        {
            return new Vector2(X, Y);
        }

        public void SetVector(Vector2 pos)
        {
            X = Convert.ToInt32(pos.x);
            Y = Convert.ToInt32(pos.y);
        }
    }
}