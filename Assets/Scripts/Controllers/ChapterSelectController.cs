using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;

public class ChapterSelectController : MonoBehaviour
{
    [SerializeField] private GameObject _chapterPrefab;

    private OverallController _overallController;

    private Transform _content;

    // Use this for initialization
    void Start()
    {
        _overallController = GameObject.Find("OverallManager").GetComponent<OverallController>();
        SetChapter();
    }

    void SetChapter()
    {
        _content = this.gameObject.transform.Find("Viewport/Content");
        for (int i = 0; i < _overallController.CreatedChapter; i++)
        {
            GameObject instanceChapter = Instantiate(_chapterPrefab, _content);
            Debug.Log(instanceChapter.transform.Find("StageSelectView@StageSelect").name);

            instanceChapter.transform.Find("StageSelectView@StageSelect").GetComponent<StageSelectViewController>()
                    .Chapter = i + 1;
        }
    }
}