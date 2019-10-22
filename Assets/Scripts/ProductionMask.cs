using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionMask : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _production;

    private int _count = 0;

    // Update is called once per frame
    void Update()
    {
        if (_player.GetComponent<Player>().OnGround())
        {
            if (_count == 0)
            {
                _production.GetComponent<Animator>().SetTrigger("isOk");
                _count++;
            }
        }

        this.transform.position = new Vector3(_player.transform.localPosition.x + 0.228f,
            _player.transform.localPosition.y + 0.952f, this.transform.position.z);
    }
}