using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Values;

namespace Controllers
{
    public class MainUIController : MonoBehaviour
    {
        [SerializeField] private AudioClip _buttonAudioClip;
        [SerializeField] private AudioClip _menuOpenAudioClip;
        [SerializeField] private AudioClip _menuCloseAudioClip;
        [SerializeField] private GameObject _expText;
        [SerializeField] private GameObject _hpText;
        [SerializeField] private GameObject _hpGrowText;
        [SerializeField] private GameObject _attackText;
        [SerializeField] private GameObject _attackGrowText;
        [SerializeField] private GameObject _defenceText;
        [SerializeField] private GameObject _defenceGrowText;
        [SerializeField] private GameObject _skillText;
        [SerializeField] private GameObject _skillGrowText;
        [SerializeField] private GameObject _playerLevelText;

        private int _growHp;
        private int _growAttackPower;
        private int _growDefencePower;
        private int _growSkill;
        private int _cacheExp;
        private int _playerLevel;

        private int _needExp;


        private GameObject[] _homes;
        private GameObject[] _stageSelects;
        private GameObject[] _customs;

        private GameObject _menu;
        private GameObject _menuWindow;

        private GameObject _timeUI;
        private GameObject _backGroundMorning;
        private GameObject _backGroundAfternoon;
        private GameObject _backGroundNight;

        private GameObject _chapterSelect;

        private AudioSource _audioSource;

        private GameObject _jumpPatternButtonText;
        private GameObject _debugModeButtonText;

        private RoomState _room;

        // Use this for initialization
        void Start()
        {
            RoomController.instance.Initialize(Room.Home);
            
            _audioSource = GetComponent<AudioSource>();

            _timeUI = GameObject.Find("TimeText@Home");
            _backGroundMorning = GameObject.Find("BackGroundMorning");
            _backGroundAfternoon = GameObject.Find("BackGroundAfternoon");
            _backGroundNight = GameObject.Find("BackGroundNight");

            //_stageSelectScrolView = GameObject.Find("StageSelectView@StageSelect");
            _chapterSelect = GameObject.Find("ChapterSelectView@StageSelect");

            _menu = GameObject.Find("MenuPanel");
            _menuWindow = GameObject.Find("MenuWindow");
            _jumpPatternButtonText = GameObject.Find("JumpPatternSettingButton/Text");
            _jumpPatternButtonText.GetComponent<Text>().text = OverallController.instance.JumpPattern.ToString();
            _debugModeButtonText = GameObject.Find("IsDebugModeSettingButton/Text");
            _debugModeButtonText.GetComponent<Text>().text =
                $"{(OverallController.instance.IsDebugMode ? "DebugMode:On" : "DebugMode:Off")}";
            _menuWindow.transform.localScale = Vector3.zero;
            _menu.SetActive(false);

            _homes = GameObject.FindGameObjectsWithTag(Tag.UiHome);
            _stageSelects = GameObject.FindGameObjectsWithTag(Tag.UiStageSelect);
            _customs = GameObject.FindGameObjectsWithTag(Tag.UiCustom);
            Time.timeScale = 1f;

            _growHp = 0;
            _growAttackPower = 0;
            _growDefencePower = 0;
            _growSkill = 0;
            _cacheExp = OverallController.instance.ExperiencePoint;
            _playerLevel = OverallController.instance.PlayerLevel;
            SetNeedExp();
            DrawExp();

            //ChangeHome
            _room = RoomState.Home;
            ChangeActive(_homes, true);
            ChangeActive(_stageSelects, false);
            ChangeActive(_customs, false);
        }

        // Update is called once per frame
        void Update()
        {
            float hour = System.DateTime.Now.Hour;
            float minute = System.DateTime.Now.Minute;
            if (hour >= 6 && hour < 16)
            {
                _backGroundMorning.SetActive(true);
                _backGroundAfternoon.SetActive(false);
                _backGroundNight.SetActive(false);
            }
            else if (16 <= hour && hour < 19)
            {
                _backGroundMorning.SetActive(false);
                _backGroundAfternoon.SetActive(true);
                _backGroundNight.SetActive(false);
            }
            else
            {
                _backGroundMorning.SetActive(false);
                _backGroundAfternoon.SetActive(false);
                _backGroundNight.SetActive(true);
            }

            if (_room == RoomState.Home)
            {
                _timeUI.GetComponent<Text>().text =
                    String.Format("{0} : {1:00}", hour, minute);
            }
            else if (_room == RoomState.Custom)
            {
                DrawExp();
                DrawState();
            }
        }

        void ChangeActive(GameObject[] objects, bool isActive)
        {
            foreach (GameObject obGameObject in objects)
            {
                obGameObject.SetActive(isActive);
            }
        }

        void SetNeedExp()
        {
            int playerLevelCache =
                _playerLevel + _growHp / 20 + _growAttackPower / 10 + _growDefencePower / 5 + _growSkill / 1;
            int count = playerLevelCache / 10;
            _needExp = 10 + count * 5;
        }

        void DrawExp()
        {
            _expText.GetComponent<Text>().text = $"{_cacheExp:0000}";
        }

        void DrawState()
        {
            _playerLevelText.GetComponent<Text>().text = $"Lv.{_playerLevel}";

            _hpText.GetComponent<Text>().text = $"HP：{OverallController.instance.MaxPlayerHp + _growHp}";
            _attackText.GetComponent<Text>().text =
                $"攻撃力：{OverallController.instance.AttackPlayerPower + _growAttackPower}";
            _defenceText.GetComponent<Text>().text =
                $"防御力：{OverallController.instance.DefencePlayerPower + _growDefencePower}";
            _skillText.GetComponent<Text>().text = $"スキルLv：{1}";

            _hpGrowText.GetComponent<Text>().text = $"{_growHp:0000}";
            _attackGrowText.GetComponent<Text>().text = $"{_growAttackPower:0000}";
            _defenceGrowText.GetComponent<Text>().text = $"{_growDefencePower:0000}";
            _skillGrowText.GetComponent<Text>().text = $"{_growSkill:0000}";
        }

        public void SaveGrow()
        {
            _audioSource.clip = _buttonAudioClip;
            _audioSource.Play();
            _playerLevel += (_growHp / 20 + _growAttackPower / 10 + _growDefencePower / 5 + _growSkill / 1);
            OverallController.instance.SetGrowing(_growHp, _growAttackPower, _growDefencePower, _growSkill, _cacheExp,
                _playerLevel);
            _growHp = 0;
            _growAttackPower = 0;
            _growDefencePower = 0;
            _growSkill = 0;
            _cacheExp = OverallController.instance.ExperiencePoint;
        }

        public void PlusHpPoint()
        {
            if (_cacheExp - _needExp >= 0)
            {
                _audioSource.clip = _buttonAudioClip;
                _audioSource.Play();
                _growHp += 20;
                _cacheExp -= _needExp;
                SetNeedExp();
            }
        }

        public void PlusAttackPoint()
        {
            if (_cacheExp - _needExp >= 0)
            {
                _audioSource.clip = _buttonAudioClip;
                _audioSource.Play();
                _growAttackPower += 10;
                _cacheExp -= _needExp;
                SetNeedExp();
            }
        }

        public void PlusDefencePoint()
        {
            if (_cacheExp - _needExp >= 0)
            {
                _audioSource.clip = _buttonAudioClip;
                _audioSource.Play();
                _growDefencePower += 5;
                _cacheExp -= _needExp;
                SetNeedExp();
            }
        }

        public void PlusSkillLv()
        {
            if (_cacheExp - _needExp >= 0)
            {
                _audioSource.clip = _buttonAudioClip;
                _audioSource.Play();
                _growSkill += 1;
                _cacheExp -= _needExp;
                SetNeedExp();
            }
        }

        public void MinusHpPoint()
        {
            if (_growHp - 20 >= 0)
            {
                _audioSource.clip = _buttonAudioClip;
                _audioSource.Play();
                _growHp -= 20;
                SetNeedExp();
                _cacheExp += _needExp;
            }
        }

        public void MinusAttackPoint()
        {
            if (_growAttackPower - 10 >= 0)
            {
                _audioSource.clip = _buttonAudioClip;
                _audioSource.Play();
                _growAttackPower -= 10;
                SetNeedExp();
                _cacheExp += _needExp;
            }
        }

        public void MinusDefencePoint()
        {
            if (_growDefencePower - 5 >= 0)
            {
                _audioSource.clip = _buttonAudioClip;
                _audioSource.Play();
                _growDefencePower -= 5;
                SetNeedExp();
                _cacheExp += _needExp;
            }
        }

        public void MinusSkillLv()
        {
            if (_growSkill - 1 >= 0)
            {
                _audioSource.clip = _buttonAudioClip;
                _audioSource.Play();
                _growSkill -= 1;
                SetNeedExp();
                _cacheExp += _needExp;
            }
        }

        public void ChangeHome()
        {
            _audioSource.clip = _buttonAudioClip;
            _audioSource.Play();
            _room = RoomState.Home;
            ChangeActive(_homes, true);
            ChangeActive(_stageSelects, false);
            ChangeActive(_customs, false);
        }

        public void ChangeStageSelect()
        {
            _audioSource.clip = _buttonAudioClip;
            _audioSource.Play();
            _room = RoomState.Battle;
            ChangeActive(_homes, false);
            ChangeActive(_stageSelects, true);
            ChangeActive(_customs, false);
        }

        public void ChangeCustom()
        {
            _audioSource.clip = _buttonAudioClip;
            _audioSource.Play();
            _room = RoomState.Custom;
            ChangeActive(_homes, false);
            ChangeActive(_stageSelects, false);
            ChangeActive(_customs, true);
        }

        public void SettingJump()
        {
            _audioSource.clip = _buttonAudioClip;
            _audioSource.Play();
            OverallController.instance.ChangeJumpPattern();
            _jumpPatternButtonText.GetComponent<Text>().text = OverallController.instance.JumpPattern.ToString();
        }

        public void SettingDebugMode()
        {
            _audioSource.clip = _buttonAudioClip;
            _audioSource.Play();
            OverallController.instance.ChangeDebugMode();
            _debugModeButtonText.GetComponent<Text>().text =
                $"{(OverallController.instance.IsDebugMode ? "DebugMode:On" : "DebugMode:Off")}";
        }

        public void ShowMenu()
        {
            _audioSource.clip = _menuOpenAudioClip;
            _audioSource.Play();
            _menu.SetActive(true);
            Debug.Log("MenuWin Start");
            _menuWindow.transform.DOScale(1f, 0.3f); //scale:1f, time:0.3f
            Debug.Log("MenuWin End");
        }

        public void CloseMenu()
        {
            _audioSource.clip = _menuCloseAudioClip;
            _audioSource.Play();
            _menuWindow.transform.DOScale(0f, 0.3f).OnComplete(() => _menu.SetActive(false));
        }

        public void DataDeleteButton()
        {
            OverallController.instance.ResetGameData();
            Destroy(GameObject.Find("OverallManager"));
            //削除後タイトルに戻る(時間を若干開けて戻るようにしようね)
            RoomController.instance.GotoRoom(Room.Title);
            SceneManager.UnloadSceneAsync(Scenes.Home);
        }

        enum RoomState
        {
            Home,
            Battle,
            Custom
        }
    }
}