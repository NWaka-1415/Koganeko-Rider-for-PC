using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Values;

public class ResultSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject _expText;
    private Animator _animator;

    private OverallManager _overallManager;
    private Text _clearTimeText;

    private int _view;

    // Use this for initialization
    void Start()
    {
        _view = 0;

        _animator = GetComponent<Animator>();
        _animator.SetInteger("View", _view);

        _overallManager = GameObject.Find("OverallManager").GetComponent<OverallManager>();
        _expText.GetComponent<Text>().text = String.Format("獲得Exp:{0:0000}\n\n合計Exp:{1:0000}",
            _overallManager.GettingExp, _overallManager.ExperiencePoint);
        _clearTimeText = GameObject.Find("ClearTimeText").GetComponent<Text>();
        _clearTimeText.text = String.Format(" クリアタイム：{0:0000}", _overallManager.ClearTime);
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