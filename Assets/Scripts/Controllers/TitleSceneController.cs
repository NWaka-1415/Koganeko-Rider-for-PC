using UnityEngine;

namespace Controllers
{
    public class TitleSceneController : MonoBehaviour
    {
        private void Start()
        {
            RoomController.instance.Initialize(Room.Title);
        }

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
}