using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Values;

public class MainCameraControll : MonoBehaviour
{
    private GameObject _player;

    private float _distY;

    // Use this for initialization
    void Start()
    {
        _player = GameObject.FindWithTag(Tag.Player);
        _distY = 3.084046f;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = _player.transform.position;
        this.transform.position = new Vector3(playerPos.x, playerPos.y + _distY, -10f);
    }
}