using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Controllers
{
    public class LoadSceneController : MonoBehaviour
    {
        private AsyncOperation _async;
        [SerializeField] private Image loadBar = null;

        // Update is called once per frame
        private void Start()
        {
            RoomController.instance.Initialize(Room.HomeToGame);
            RoomController.instance.GotoRoom(Room.Gaming, loadAnim: true, loadImage: loadBar);
        }
    }
}