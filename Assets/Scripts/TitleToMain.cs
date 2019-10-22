using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleToMain : MonoBehaviour
{
    private float _time;

    // Use this for initialization
    void Start()
    {
        _time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;
        if (Input.touchCount > 0 && _time > 1)
        {
            MoveScene();
        }
    }

    void MoveScene()
    {
        SceneManager.LoadScene("HomeScene");
    }
}