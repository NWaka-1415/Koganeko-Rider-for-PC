using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controllers
{
	public class SceneController : MonoBehaviour
	{
		private string _homeScene = "HomeScene";
		private string _titleScene = "TitleScene";
		private string _gameScene = "GameScene";
		private string _stageSelect = "StageSelect";

		private string _nowScene;
	
		// Use this for initialization
		void Start () {
		
		}
	
		// Update is called once per frame
		void Update () {
		
		}

		public void MoveScene(string scene)
		{
			_nowScene = scene;
			SceneManager.LoadScene(scene);
		}

		public string HomeScene
		{
			get { return _homeScene; }
		}

		public string TitleScene
		{
			get { return _titleScene; }
		}

		public string GameScene
		{
			get { return _gameScene; }
		}
	}
}
