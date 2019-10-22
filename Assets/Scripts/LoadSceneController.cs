using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneController : MonoBehaviour
{
    private float _delta = 0f;

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.Log("<color=red>Delta=" + _delta + "</color>");
        if (_delta >= 0.25f)
        {
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            _delta += Time.deltaTime;
        }
    }
}