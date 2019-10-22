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
        private void Awake()
        {
            StartCoroutine(LoadScene());
        }

        IEnumerator LoadScene()
        {
            _async = RoomController.instance.GotoRoom(Room.Gaming);
            while (!_async.isDone)
            {
                loadBar.fillAmount = _async.progress;
                yield return null;
            }
        }
    }
}