using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class CreateOverall : MonoBehaviour
{
    [SerializeField] private GameObject _overallManagerPrefab;

    private GameObject _overallManager = null;

    private void Awake()
    {
        _overallManager = GameObject.Find("OverallManager");
        if (_overallManager == null)
        {
            GameObject instanceOverallManager = Instantiate(_overallManagerPrefab);
            instanceOverallManager.name = "OverallManager";
        }
    }
}