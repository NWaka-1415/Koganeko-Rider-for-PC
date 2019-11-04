using UnityEngine;

namespace Controllers
{
    public class ChapterSelectController : MonoBehaviour
    {
        [SerializeField] private GameObject _chapterPrefab;

        private OverallController _overallController;

        private Transform _content;

        // Use this for initialization
        void Start()
        {
            _overallController = OverallController.instance;
            SetChapter();
        }

        void SetChapter()
        {
            _content = this.gameObject.transform.Find("Viewport/Content");
            for (int i = 0; i < _overallController.CreatedChapter; i++)
            {
                GameObject instanceChapter = Instantiate(_chapterPrefab, _content);
                Debug.Log(instanceChapter.transform.Find("StageSelectView@StageSelect").name);

                instanceChapter.transform.Find("StageSelectView@StageSelect").GetComponent<StageSelectViewController>()
                        .Chapter = i + 1;
            }
        }
    }
}