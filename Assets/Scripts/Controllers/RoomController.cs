using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Values;

namespace Controllers
{
    public class RoomController : MonoBehaviour
    {
        private static RoomController _instance = null;

        public static RoomController instance => _instance;

        private Room _currentRoom;

        private Dictionary<Room, String> _rooms;

        private void Awake()
        {
            if (_instance == null) _instance = this;
            else if (_instance != this) Destroy(gameObject);
            SetRooms();
        }

        private void SetRooms()
        {
            _rooms = new Dictionary<Room, string>();
            _rooms.Add(Room.Title, Scenes.Title);
            _rooms.Add(Room.Home, Scenes.Home);
            _rooms.Add(Room.Gaming, Scenes.Game);
            _rooms.Add(Room.Result, Scenes.Result);
            _rooms.Add(Room.HomeToGame, Scenes.Load);
        }

        public void GotoRoom(Room room)
        {
            _currentRoom = room;
            SceneManager.LoadSceneAsync(_rooms[room]);
        }
    }
}