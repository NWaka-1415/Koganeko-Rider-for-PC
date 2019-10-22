using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Stageオブジェクトにアタッチすること
public class GameStageManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _phasesObj;
    private Phase[] _phases;

    private OverallManager _overallManager;
    private GameSceneManager _gameSceneManager;
    private int _nowPhase = 0;

    // Use this for initialization
    void Start()
    {
        Init();
    }

    private void Update()
    {
        if (_gameSceneManager.GameClear) return;
        if (isEndPhase(_phases[_nowPhase].Enemys))
        {
            _nowPhase += 1;
        }

        //Debug.Log("Phase " + _nowPhase.ToString());
        if (!(_nowPhase >= _phases.Length))
        {
            //Debug.Log("if Root");
            if (!isEndPhase(_phases[_nowPhase].Enemys))
            {
                SetActives(_phases[_nowPhase].Enemys);
            }
        }
        else
        {
            _gameSceneManager.GameClear = true;
        }
    }

    void Init()
    {
        _overallManager = GameObject.Find("OverallManager").GetComponent<OverallManager>();
        _gameSceneManager = GameObject.Find("GameManager").GetComponent<GameSceneManager>();
        _nowPhase = 0;
        _phases = new Phase[_phasesObj.Length];
        int i = 0;
        foreach (GameObject phase in _phasesObj)
        {
            _phases[i] = phase.GetComponent<Phase>();
            i++;
        }

        foreach (Phase phase in _phases)
        {
            SetActives(phase.Enemys, false);
        }
    }

    void SetActives(GameObject[] gameObjects, bool isSet = true)
    {
        foreach (GameObject obj in gameObjects)
        {
            if (obj != null)
            {
                obj.SetActive(isSet);
            }

            //Debug.Log("Setting Object:" + obj);
        }
    }

    bool isEndPhase(GameObject[] gameObjects)
    {
        foreach (GameObject obj in gameObjects)
        {
            //Debug.Log("Object:" + obj);
            if (obj != null)
            {
                return false;
            }
        }

        return true;
    }
}