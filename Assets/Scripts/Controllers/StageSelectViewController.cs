using UnityEngine;

namespace Controllers
{
    public class StageSelectViewController : MonoBehaviour
    {
        [SerializeField] private GameObject _stageSelectButton;

        private OverallController _overallController;

        private Transform _content;

        private int _chapter;
        private int _stageNum;

        // Use this for initialization
        void Start()
        {
            _overallController = OverallController.instance;
            SetStageButton();
        }

        void SetStageButton()
        {
            if (_chapter != _overallController.MaxChapter)
            {
                _stageNum = _overallController.StagePerChapter;
            }
            else
            {
                _stageNum = _overallController.MaxStage;
            }

            _content = this.gameObject.transform.Find("Viewport/Content");
            for (int i = 0; i < _stageNum; i++)
            {
                GameObject instanceSelectButton = Instantiate(_stageSelectButton, _content);
                instanceSelectButton.GetComponent<ButtonController>().Chapter = _chapter;
                instanceSelectButton.GetComponent<ButtonController>().Stage = i + 1;
            }
        }

        public int Chapter
        {
            get => _chapter;
            set => _chapter = value;
        }
    }
}