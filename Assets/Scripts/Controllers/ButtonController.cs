using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class ButtonController : MonoBehaviour
    {
        [SerializeField] private AudioClip _audio;

        [SerializeField] private Room loadScene;

        private Button _button;
        private AudioSource _audioSource;

        private int _chapter;
        private int _stage;

        // Use this for initialization
        void Start()
        {
            _button = GetComponent<Button>();
            _audioSource = GetComponent<AudioSource>();
            if (this.gameObject.name == "StageSelectButton@StageSelect(Clone)")
            {
                Text _text = GetComponentInChildren<Text>();
                _text.text = $"ステージ{_stage:00}";
                _button.onClick.AddListener(OnClickedSttageButton);
            }
            else
            {
                _button.onClick.AddListener(OnClickedNormalButton);
            }
        }

        void OnClickedSttageButton()
        {
            _audioSource.clip = _audio;
            _audioSource.Play();
            OverallController.instance.UserSelectChapter = _chapter;
            OverallController.instance.UserSelectStage = _stage;
            
        }

        void OnClickedNormalButton()
        {
            RoomController.instance.GotoRoom(loadScene);
        }
    
        public int Chapter
        {
            get => _chapter;
            set => _chapter = value;
        }

        public int Stage
        {
            get => _stage;
            set => _stage = value;
        }
    }
}