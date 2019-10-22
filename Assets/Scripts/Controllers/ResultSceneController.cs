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

        private Text _clearTimeText;

        private int _view;

        // Use this for initialization
        void Start()
        {
            RoomController.instance.Initialize(Room.Result);
            _view = 0;

            _animator = GetComponent<Animator>();
            _animator.SetInteger("View", _view);

            _expText.GetComponent<Text>().text =
                $"獲得Exp:{OverallController.instance.GettingExp:0000}\n\n合計Exp:{OverallController.instance.ExperiencePoint:0000}";
            _clearTimeText = GameObject.Find("ClearTimeText").GetComponent<Text>();
            _clearTimeText.text = $" クリアタイム：{OverallController.instance.ClearTime:0000}";
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
                    RoomController.instance.GotoRoom(Room.Home);
                }
            }
        }
    }
}