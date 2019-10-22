using System;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class GameUiController : MonoBehaviour
    {
        [SerializeField] private GameObject _pausePanel;
        [SerializeField] private GameObject _jumpButton;
        [SerializeField] private GameObject[] _paramIcons;

        [SerializeField] private GameObject _debugLog;

        private OverallController _overallController;

        private Player _player;
        private Text _hpText;
        private Image _hpGreenGage;
        private Image _hpRedGage;

        private float _fromHp;
        private float _toHp;
        private float _reduceTimeFrame;
        private float _frameReduceHp;

        private bool _isPause;

        // Use this for initialization
        void Start()
        {
            _overallController = GameObject.Find("OverallManager").GetComponent<OverallController>();

            _player = GameObject.FindWithTag("Player").GetComponent<Player>();
            _hpText = GameObject.Find("HP Text").GetComponent<Text>();

            SetHpText();
            InitImageParam();
            _isPause = false;
            _reduceTimeFrame = 120.0f;
        }

        // Update is called once per frame
        void Update()
        {
            SetHpText();
            SetUI();
            SetIcon();
            float maxHp = _player.GetHP(0);
            float hitPoint = _player.GetHP(1);
            //Debug.Log("ShowingHp:" + (hitPoint / maxHp).ToString());
            _hpGreenGage.fillAmount = hitPoint / maxHp;
            ReduceRedHp();
        }

        void SetUI()
        {
            switch (_overallController.JumpPattern)
            {
                case OverallController.JumpPatterns.Flick:
                    _jumpButton.SetActive(false);
                    break;
                case OverallController.JumpPatterns.Button:
                    _jumpButton.SetActive(true);
                    break;
            }
        }

        void InitImageParam()
        {
            _debugLog.SetActive(_overallController.IsDebugMode);

            //Statusアイコンのセット(不可視化)
            foreach (GameObject paramIcon in _paramIcons)
            {
                paramIcon.GetComponent<Image>().color = new Color(255f, 255f, 255f, 0f);
            }

            //Hpのセット
            _hpGreenGage = GameObject.Find("HP Gage Green").GetComponent<Image>();
            _hpGreenGage.fillAmount = 1;
            _hpRedGage = GameObject.Find("HP Gage Red").GetComponent<Image>();
            _hpRedGage.fillAmount = 1;
            _pausePanel.SetActive(false);
        }

        void ReduceRedHp()
        {
            if (_fromHp > _toHp)
            {
                _hpRedGage.fillAmount = _fromHp / _player.GetHP(0);
                _fromHp -= _frameReduceHp;
            }
            else
            {
                _hpRedGage.fillAmount = _toHp / _player.GetHP(0);
            }
        }

        public void SetReducing()
        {
            float differenceHp = _fromHp - _toHp;
            _frameReduceHp = differenceHp / _reduceTimeFrame;
            Debug.Log(_frameReduceHp.ToString());
        }

        public float FromHp
        {
            get { return _fromHp; }
            set { _fromHp = value; }
        }

        public float ToHp
        {
            get { return _toHp; }
            set { _toHp = value; }
        }

        void SetHpText()
        {
            _hpText.text = String.Format("{0} / {1}", _player.GetHP(1).ToString(), _player.GetHP(0).ToString());
        }

        void SetIcon()
        {
            //AttackUp
            //AttackDown
            //DefenceUp
            if (_player.IsGuard)
            {
                _paramIcons[(int) ParamIcons.DefenceUp].GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);
            }
            else
            {
                _paramIcons[(int) ParamIcons.DefenceUp].GetComponent<Image>().color = new Color(255f, 255f, 255f, 0f);
            }

            //DefenceDown
            //AttackSpeedUp
            //AttackSpeedDown
            //SpeedUp
            //SpeedDown
        }

        /*
     * Debug
     */
        public void ShowDebugMessage(string message)
        {
            if (!_overallController.IsDebugMode) return;
            if (_debugLog.GetComponent<Text>().text.Length > 300)
            {
                _debugLog.GetComponent<Text>().text = "";
            }

            _debugLog.GetComponent<Text>().text += message + "\n";
        }

        /*
     * For Button
     */

        public void PauseButton()
        {
            _isPause = true;
            Time.timeScale = 0f;
            _pausePanel.SetActive(true);
        }

        public void ClosePauseButton()
        {
            _isPause = false;
            Time.timeScale = 1f;
            _pausePanel.SetActive(false);
        }

        enum ParamIcons
        {
            AttackDown = 0,
            AttackUp = 1,
            DefenceDown = 2,
            DefenceUp = 3,
            AttackSpeedDown = 4,
            AttackSpeedUp = 5,
            SpeedDown = 6,
            SpeedUp = 7
        }
    }
}