using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private AudioClip _audio;

    [SerializeField] private string _loadScene;
    [SerializeField] private string _destroyScene;

    private OverallManager _overallManager;
    private SceneController _sceneController;
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
            _text.text = String.Format("ステージ{0:00}", _stage);
            _overallManager = GameObject.Find("OverallManager").GetComponent<OverallManager>();
            _sceneController = GameObject.Find("OverallManager").GetComponent<SceneController>();
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
        _overallManager.UserSelectChapter = _chapter;
        _overallManager.UserSelectStage = _stage;
        _sceneController.MoveScene(_loadScene);
    }

    void OnClickedNormalButton()
    {
        SceneManager.LoadScene(_loadScene);
        StartCoroutine(unloadScene());
    }

    private IEnumerator unloadScene()
    {
        yield return SceneManager.UnloadSceneAsync(_destroyScene);
    }
    
    public int Chapter
    {
        get { return _chapter; }
        set { _chapter = value; }
    }

    public int Stage
    {
        get { return _stage; }
        set { _stage = value; }
    }
}