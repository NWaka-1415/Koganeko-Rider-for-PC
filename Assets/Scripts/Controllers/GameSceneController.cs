using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using Values;

namespace Controllers
{
    public class GameSceneController : MonoBehaviour
    {
        private static GameSceneController _instance = null;

        public static GameSceneController instance => _instance;

        private readonly List<List<GameObject>> _stagePrefabsInChapters = new List<List<GameObject>>();

        private GameObject _gameClearPanel;

        private GameObject _gameOverPanel;
        private GameObject _continue;

        [SerializeField] private GameObject _production;
        [SerializeField] private AudioClip _startAudioClip;

        private OverallController _overallController;
        private AudioSource _audioSource;

        private bool _isGameStart;

        private bool _gameClear;
        private bool _gameOver;

        private float _gameClearDelta = 0f;
        private float _gameOverDelta = 0f;

        private float _clearTime = 0f;

        private int _chapter;
        private int _stage;

        private int _getExp;

        private void Awake()
        {
            if (_instance == null) _instance = this;
            else if (_instance != this) Destroy(gameObject);
        }

        // Use this for initialization
        void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _overallController = GameObject.Find("OverallManager").GetComponent<OverallController>();
            _gameClearPanel = GameObject.Find("GameClearPanel");
            _gameOverPanel = GameObject.Find("GameOverPanel");
            _continue = GameObject.Find("AreYouContinue");
            Init();
            SetGameStage();
            SetGame();
        }

        // Update is called once per frame
        void Update()
        {
            if (_isGameStart)
            {
                _clearTime += Time.deltaTime;
            }

            GameOverLoad();
            GameClearLoad();
        }

        void Init()
        {
            _isGameStart = false;
            _gameClear = false;
            _gameOver = false;
            _chapter = _overallController.UserSelectChapter;
            _stage = _overallController.UserSelectStage;
            Debug.Log("GameSceneManager Init End");
        }

        void SetGameStage()
        {
            //Instantiate(_stagePrefabs[0]);
            Instantiate(_overallController.StagePrefabsInChapters[_chapter - 1][_stage - 1]);
        }

        void SetGame()
        {
            _continue.SetActive(false);
            _gameOverPanel.SetActive(false);
            _gameClearPanel.SetActive(false);
        }

        void GameOverLoad()
        {
            if (_gameOver && !_gameClear)
            {
                _gameOverPanel.SetActive(true);
                if (_gameOverDelta >= 2.5f)
                {
                    _continue.SetActive(true);
                }
                else
                {
                    _gameOverDelta += Time.deltaTime;
                }
            }
        }

        void GameClearLoad()
        {
            if (_gameClear && !_gameOver)
            {
                _gameClearPanel.SetActive(true);
                if (_gameClearDelta >= 2f)
                {
                    //Resultに遷移
                    if (_chapter == _overallController.MaxChapter && _stage == _overallController.MaxStage)
                    {
                        if (_overallController.MaxStage + 1 > _overallController.StagePerChapter)
                        {
                            _overallController.MaxChapter += 1;
                            _overallController.MaxStage = 1;
                        }
                        else
                        {
                            _overallController.MaxStage += 1;
                        }
                    }

                    _overallController.AddExperiencePoint(_getExp);
                    _overallController.ClearTime = this._clearTime;
                    _overallController.SaveGameData();
                    SceneManager.LoadScene(Scenes.Result);
                }
                else
                {
                    _gameClearDelta += Time.deltaTime;
                }
            }
        }

        public void PlayStartAudio()
        {
            _audioSource.clip = _startAudioClip;
            _audioSource.Play();
        }

        public void AddWillGetExp(int exp)
        {
            _getExp += exp;
        }

        public bool GameClear
        {
            get { return _gameClear; }
            set { _gameClear = value; }
        }

        public bool GameOver
        {
            get { return _gameOver; }
            set { _gameOver = value; }
        }

        public bool IsGameStart
        {
            get { return _isGameStart; }
        }

        public void GameStarted()
        {
            _isGameStart = true;
            _production.SetActive(false);
        }
    }
}