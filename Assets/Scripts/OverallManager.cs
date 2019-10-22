﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverallManager : MonoBehaviour
{
    private int _gameStage = 0;
    private bool _isTouchMoving = false;

    //Player
    private int _maxPlayerHp;
    private int _attackPlayerPower;
    private int _defencePlayerPower;
    [Range(0.0f, 1.0f)] private float _shieldPower;
    private int _comboPlayer = 2;
    private int _playerLevel = 1;

    private int _experiencePoint = 0;
    private int _gettingExp;

    //Stage
    [SerializeField] private GameObject[] _stagePrefabs = new GameObject[1];

    private readonly List<List<GameObject>> _stagePrefabsInChapters = new List<List<GameObject>>();

    private int _maxChapter = 1; //ユーザーが進めたチャプター数
    private int _maxStage = 1; //ユーザーが進めたチャプターMaxの中でユーザーが進めたステージ数

    [SerializeField] private int _cleatedChapter = 1; //実装済みチャプター数

    [SerializeField] private int _stagePerChapter = 10; //1チャプターあたりのステージ数

    private float _clearTime;

    //User Select Stage
    private int _userSelectChapter;
    private int _userSelectStage;

    //操作方法の変更
    private JumpPatterns _jumpPattern;

    //DebugModeか否か
    private bool _isDebugMode;

    // Use this for initialization
    void Start()
    {
        SetGameData();
        SetStagesData();
    }

    private void Awake()
    {
        _clearTime = 0f;
        SetGameData();
        DontDestroyOnLoad(gameObject);
    }

    void SetGameData()
    {
        _maxChapter = PlayerPrefs.GetInt("MaxChapter", 1);
        _maxStage = PlayerPrefs.GetInt("MaxStage", 1);
        _playerLevel = PlayerPrefs.GetInt("PlayerLevel", 1);

        _maxPlayerHp = PlayerPrefs.GetInt("PlayerHP", 1000);
        _attackPlayerPower = PlayerPrefs.GetInt("PlayerAttack", 100);
        _defencePlayerPower = PlayerPrefs.GetInt("PlayerDefence", 50);
        _shieldPower = PlayerPrefs.GetFloat("PlayerShield", 0.40f); //40%
        _comboPlayer = PlayerPrefs.GetInt("PlayerCombo", 2);
        _experiencePoint = PlayerPrefs.GetInt("PlayerExperiencePoint", 0);

        int jumpPatternCache = PlayerPrefs.GetInt("JumpPattern", 0);
        switch (jumpPatternCache)
        {
            case 0:
                _jumpPattern = JumpPatterns.Flick;
                break;
            case 1:
                _jumpPattern = JumpPatterns.Button;
                break;
        }

        int debugMode = PlayerPrefs.GetInt("IsDebugMode", 0);
        switch (debugMode)
        {
            case 0:
                _isDebugMode = false;
                break;
            case 1:
                _isDebugMode = true;
                break;
        }
    }

    void SetStagesData()
    {
        int size = Mathf.FloorToInt(_stagePrefabs.Length / _stagePerChapter) + 1;

        for (int i = 0; i < size; i++)
        {
            _stagePrefabsInChapters.Add(new List<GameObject>());
        }

        int c = 0, s = 0;
        foreach (GameObject stagePrefab in _stagePrefabs)
        {
            if (s > this._stagePerChapter)
            {
                s = 0;
                c++;
            }

            this._stagePrefabsInChapters[c].Add(stagePrefab);
            s++;
        }
    }

    public void SaveGameData()
    {
        PlayerPrefs.SetInt("MaxChapter", _maxChapter);
        PlayerPrefs.SetInt("MaxStage", _maxStage);
        PlayerPrefs.SetInt("PlayerLevel", _playerLevel);

        PlayerPrefs.SetInt("PlayerHP", _maxPlayerHp);
        PlayerPrefs.SetInt("PlayerAttack", _attackPlayerPower);
        PlayerPrefs.SetInt("PlayerDefence", _defencePlayerPower);
        PlayerPrefs.SetFloat("PlayerShield", _shieldPower);
        PlayerPrefs.SetInt("PlayerCombo", _comboPlayer);
        PlayerPrefs.SetInt("PlayerExperiencePoint", _experiencePoint);

        switch (_jumpPattern)
        {
            case JumpPatterns.Flick:
                PlayerPrefs.SetInt("JumpPattern", 0);
                break;
            case JumpPatterns.Button:
                PlayerPrefs.SetInt("JumpPattern", 1);
                break;
        }
        PlayerPrefs.SetInt("IsDebugMode", _isDebugMode ? 1 : 0);

        PlayerPrefs.Save();
    }

    public void AddExperiencePoint(int exp)
    {
        Debug.Log("<color=red>得た経験値：" + exp + "足す前の経験値：" + _experiencePoint + "</color>");
        _gettingExp = exp;
        _experiencePoint += exp;
        Debug.Log("<color=blue>足した後：" + _experiencePoint + "</color>");
    }

    public void SetGrowing(int hp, int attack, int defence, int skill, int newExp, int newLv)
    {
        if (hp >= 0) _maxPlayerHp += hp;
        if (attack >= 0) _attackPlayerPower += attack;
        if (defence >= 0) _defencePlayerPower += defence;
        _experiencePoint = newExp;
        _playerLevel = newLv;
        this.SaveGameData();
    }

    public int ExperiencePoint
    {
        get { return _experiencePoint; }
    }

    public int GettingExp
    {
        get { return _gettingExp; }
    }

    public void ResetGameData()
    {
        PlayerPrefs.DeleteAll();
    }

    public int PlayerLevel
    {
        get { return _playerLevel; }
    }

    public int MaxPlayerHp
    {
        get { return _maxPlayerHp; }
    }

    public int AttackPlayerPower
    {
        get { return _attackPlayerPower; }
    }

    public int DefencePlayerPower
    {
        get { return _defencePlayerPower; }
    }

    public float ShieldPower
    {
        get { return _shieldPower; }
    }

    public int ComboPlayer
    {
        get { return _comboPlayer; }
    }

    public int UserSelectChapter
    {
        get { return _userSelectChapter; }
        set { _userSelectChapter = value; }
    }

    public int UserSelectStage
    {
        get { return _userSelectStage; }
        set { _userSelectStage = value; }
    }

    public int StagePerChapter
    {
        get { return _stagePerChapter; }
    }

    public int MaxChapter
    {
        get { return _maxChapter; }
        set { _maxChapter = value; }
    }

    public int MaxStage
    {
        get { return _maxStage; }
        set { _maxStage = value; }
    }

    public int CleatedChapter
    {
        get { return _cleatedChapter; }
        set { _cleatedChapter = value; }
    }

    public List<List<GameObject>> StagePrefabsInChapters
    {
        get { return _stagePrefabsInChapters; }
    }

    public float ClearTime
    {
        get { return _clearTime; }
        set { _clearTime = value >= 0 ? value : 0f; }
    }

    public void ChangeJumpPattern()
    {
        if (_jumpPattern == JumpPatterns.Flick)
        {
            _jumpPattern = JumpPatterns.Button;
        }
        else if (_jumpPattern == JumpPatterns.Button)
        {
            _jumpPattern = JumpPatterns.Flick;
        }
        else
        {
            _jumpPattern = JumpPatterns.Flick;
        }

        SaveGameData();
    }

    public void ChangeDebugMode()
    {
        _isDebugMode = !_isDebugMode;
        SaveGameData();
    }

    public JumpPatterns JumpPattern
    {
        get { return _jumpPattern; }
    }

    public bool IsDebugMode
    {
        get { return _isDebugMode; }
    }

    public enum JumpPatterns
    {
        Flick,
        Button
    }
}