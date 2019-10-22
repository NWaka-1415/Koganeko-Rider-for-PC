using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FloorContorol : MonoBehaviour
{
    private GameObject _player;
    private TilemapCollider2D _tilemapCollider2D;

    // Use this for initialization
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _tilemapCollider2D = GetComponent<TilemapCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}