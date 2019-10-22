using UnityEngine;

namespace Controllers
{
    public class AudioController : MonoBehaviour
    {
        private static AudioController _instance = null;

        public static AudioController instance => _instance;

        [SerializeField] private AudioClip[] _clips = new AudioClip[10];

        private AudioSource _audioSource;

        private void Awake()
        {
            if (_instance == null) _instance = this;
            else if (_instance != this) Destroy(gameObject);
        }

        // Use this for initialization
        void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            switch (OverallController.instance.UserSelectChapter)
            {
                case 1:
                    switch (OverallController.instance.UserSelectStage)
                    {
                        case 1:
                            _audioSource.clip = _clips[0];
                            break;
                        case 2:
                            _audioSource.clip = _clips[1];
                            break;
                        case 3:
                            _audioSource.clip = _clips[2];
                            break;
                        case 4:
                            _audioSource.clip = _clips[3];
                            break;
                        case 5:
                            _audioSource.clip = _clips[4];
                            break;
                        case 6:
                            _audioSource.clip = _clips[5];
                            break;
                        case 7:
                            _audioSource.clip = _clips[6];
                            break;
                        case 8:
                            _audioSource.clip = _clips[7];
                            break;
                        case 9:
                            _audioSource.clip = _clips[8];
                            break;
                        case 10:
                            _audioSource.clip = _clips[9];
                            break;
                    }

                    break;
            }

            _audioSource.loop = true;
        }

        public void PlayBgm()
        {
            _audioSource.Play();
        }
    }
}