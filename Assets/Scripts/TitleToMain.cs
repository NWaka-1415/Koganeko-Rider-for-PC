using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleToMain : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        else if (Input.anyKeyDown) MoveScene();
    }

    void MoveScene()
    {
        RoomController.instance.GotoRoom(Room.Home);
    }
}