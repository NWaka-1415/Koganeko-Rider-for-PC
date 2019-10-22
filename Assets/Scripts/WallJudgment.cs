using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Values;

public class WallJudgment : MonoBehaviour
{
    private bool _isTrigger;

    void Awake()
    {
        Judgment = false;
        _isTrigger = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isTrigger)
        {
            Judgment = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _isTrigger = true;
        if (other.CompareTag(Tag.GroundAndWall))
        {
            Debug.Log("isWall");
            Judgment = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _isTrigger = false;
        if (other.CompareTag(Tag.GroundAndWall))
        {
            Judgment = false;
        }
    }

    public bool Judgment { get; set; }
}