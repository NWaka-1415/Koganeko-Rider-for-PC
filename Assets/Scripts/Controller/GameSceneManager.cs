using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Values;

public class GameSceneManager : MonoBehaviour
{
    private readonly List<List<GameObject>> _stagePrefabsInChapters = new List<List<GameObject>>();

    private GameObject _gameClearPanel;

    private GameObject _gameOverPanel;
    private GameObject _continue;

    [SerializeField] private GameObject _production;
    [SerializeField] private AudioClip _startAudioClip;

    private OverallManager _overallManager;
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


    // Use this for initialization
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _overallManager = GameObject.Find("OverallManager").GetComponent<OverallManager>();
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
        _chapter = _overallManager.UserSelectChapter;
        _stage = _overallManager.UserSelectStage;
        Debug.Log("GameSceneManager Init End");
    }

    void SetGameStage()
    {
        //Instantiate(_stagePrefabs[0]);
        Instantiate(_overallManager.StagePrefabsInChapters[_chapter - 1][_stage - 1]);
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
                if (_chapter == _overallManager.MaxChapter && _stage == _overallManager.MaxStage)
                {
                    if (_overallManager.MaxStage + 1 > _overallManager.StagePerChapter)
                    {
                        _overallManager.MaxChapter += 1;
                        _overallManager.MaxStage = 1;
                    }
                    else
                    {
                        _overallManager.MaxStage += 1;
                    }
                }

                _overallManager.AddExperiencePoint(_getExp);
                _overallManager.ClearTime = this._clearTime;
                _overallManager.SaveGameData();
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