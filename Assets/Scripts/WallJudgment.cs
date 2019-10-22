using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Values;

public class WallJudgment : MonoBehaviour
{
    private bool _judg;
    private bool _isTrriger;

    void Awake()
    {
        _judg = false;
        _isTrriger = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isTrriger)
        {
            _judg = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _isTrriger = true;
        if (other.CompareTag(Tag.GroundAndWall))
        {
            Debug.Log("isWall");
            _judg = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _isTrriger = false;
        if (other.CompareTag(Tag.GroundAndWall))
        {
            _judg = false;
        }
    }

    public bool Judg
    {
        get { return _judg; }
        set { _judg = value; }
    }
}