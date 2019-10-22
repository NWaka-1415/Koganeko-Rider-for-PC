using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Values;

namespace Controllers
{
    public class ResultSceneController : MonoBehaviour
    {
        [SerializeField] private GameObject _expText;
        private Animator _animator;

        private OverallController _overallController;
        private Text _clearTimeText;

        private int _view;

        // Use this for initialization
        void Start()
        {
            _view = 0;

            _animator = GetComponent<Animator>();
            _animator.SetInteger("View", _view);

            _overallController = GameObject.Find("OverallManager").GetComponent<OverallController>();
            _expText.GetComponent<Text>().text = String.Format("獲得Exp:{0:0000}\n\n合計Exp:{1:0000}",
                _overallController.GettingExp, _overallController.ExperiencePoint);
            _clearTimeText = GameObject.Find("ClearTimeText").GetComponent<Text>();
            _clearTimeText.text = String.Format(" クリアタイム：{0:0000}", _overallController.ClearTime);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (_view == 0)
                {
                    _view = 1;
                    _animator.SetInteger("View", _view);
                }
                else if (_view == 1)
                {
                    _view = 2;
                    _animator.SetInteger("View", _view);
                }
                else
                {
                    SceneManager.LoadScene(Scenes.Home);
                }
            }
        }
    }
}