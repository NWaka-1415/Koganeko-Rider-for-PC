﻿using UnityEngine;

//Stageオブジェクトにアタッチすること
namespace Controllers
{
    public class GameStageController : MonoBehaviour
    {
        [SerializeField] private GameObject[] _phasesObj;
        private Phase[] _phases;

        private OverallController _overallController;
        private GameSceneController _gameSceneController;
        private int _nowPhase = 0;

        // Use this for initialization
        void Start()
        {
            Init();
        }

        private void Update()
        {
            if (_gameSceneController.GameClear) return;
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
                _gameSceneController.GameClear = true;
            }
        }

        void Init()
        {
            _overallController = GameObject.Find("OverallManager").GetComponent<OverallController>();
            _gameSceneController = GameObject.Find("GameManager").GetComponent<GameSceneController>();
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
}