using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterSelectController : MonoBehaviour
{
    [SerializeField] private GameObject _chapterPrefab;

    private OverallManager _overallManager;

    private Transform _content;

    // Use this for initialization
    void Start()
    {
        _overallManager = GameObject.Find("OverallManager").GetComponent<OverallManager>();
        SetChapter();
    }

    void SetChapter()
    {
        _content = this.gameObject.transform.Find("Viewport/Content");
        for (int i = 0; i < _overallManager.CleatedChapter; i++)
        {
            GameObject instanceChapter = Instantiate(_chapterPrefab, _content);
            Debug.Log(instanceChapter.transform.Find("StageSelectView@StageSelect").name);

            instanceChapter.transform.Find("StageSelectView@StageSelect").GetComponent<StageSelectViewManager>()
                    .Chapter = i + 1;
        }
    }
}