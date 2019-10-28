using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;
using Values;

public class Grappling : MonoBehaviour
{
    [SerializeField] private GameObject _parent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Tag.Player))
        {
            if (_parent.CompareTag(Tag.LargeEnemy))
                other.gameObject.GetComponent<Player>().Damaged(_parent.GetComponent<LargeEnemy>().AttackPower);
            else if (_parent.CompareTag(Tag.Enemy))
                other.gameObject.GetComponent<Player>().Damaged(_parent.GetComponent<Enemy.Enemy>().AttackPower);
        }
        else if (other.CompareTag(Tag.Enemy))
        {
            if (_parent.CompareTag(Tag.Player))
                other.gameObject.GetComponent<Enemy.Enemy>().Damaged(_parent.GetComponent<Player>().AttackPower);
        }
        else if (other.CompareTag(Tag.LargeEnemy))
        {
            if (_parent.CompareTag(Tag.Player))
                other.gameObject.GetComponent<LargeEnemy>().Damaged(_parent.GetComponent<Player>().AttackPower);
        }
    }
}