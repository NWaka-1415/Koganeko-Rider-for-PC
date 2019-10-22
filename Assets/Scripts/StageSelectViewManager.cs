using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectViewManager : MonoBehaviour
{
    [SerializeField] private GameObject _stageSelectButton;

    private OverallManager _overallManager;

    private Transform _content;

    private int _chapter;
    private int _stageNum;

    // Use this for initialization
    void Start()
    {
        _overallManager = GameObject.Find("OverallManager").GetComponent<OverallManager>();
        SetStageButton();
    }

    void SetStageButton()
    {
        if (_chapter != _overallManager.MaxChapter)
        {
            _stageNum = _overallManager.StagePerChapter;
        }
        else
        {
            _stageNum = _overallManager.MaxStage;
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
        get { return _chapter; }
        set { _chapter = value; }
    }
}