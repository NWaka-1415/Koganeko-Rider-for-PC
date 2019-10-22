using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Values;

public class SearchArea : MonoBehaviour
{
    private GameObject _playerInSearchArea;
    private LargeEnemy _largeEnemy;

    private bool _isPlayerFound = false;

    private void Start()
    {
        _largeEnemy = GetComponentInParent<LargeEnemy>();
    }

    private void Update()
    {
        //Debug.Log("isFound:" + _isPlayerFound.ToString());
        if (!_isPlayerFound && !_largeEnemy.IsAttackState)
        {
            float rand = Random.Range(0, 10);
            //Debug.Log("Rand:" + rand.ToString());
            if (rand < 5f)
            {
                if (!_largeEnemy.IsPatrol)
                {
                    _largeEnemy.States = LargeEnemy.State.Stop;
                }
            }
            else
            {
                _largeEnemy.States = LargeEnemy.State.Patrol;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        //Debug.Log("other:" + other.tag);
        if (!_largeEnemy.IsDown)
        {
            float rand = Random.Range(0, 10);
            //Debug.Log("Rand:" + rand.ToString());
            if (other.CompareTag(Tag.Player))
            {
                _isPlayerFound = true;
                _playerInSearchArea = other.gameObject;
                float distX = this.gameObject.transform.position.x - _playerInSearchArea.transform.position.x;
                Debug.Log("distX:" + distX.ToString());
                Debug.Log(_largeEnemy.name + "'s IsAttackState:" + _largeEnemy.IsAttackState.ToString());
                if (_largeEnemy.IsAttackState) return;
                if (Mathf.Abs(distX) < 15f)
                {
                    _largeEnemy.States = LargeEnemy.State.ShortDistAttack;
                }
                else
                {
                    /*
                        if (rand < 4f)
                        {
                            if (!_largeEnemy.IsAttackState)
                            {
                                _largeEnemy.States = LargeEnemy.State.Walk;
                            }
                        }
                        else
                        {*/
                    _largeEnemy.States = LargeEnemy.State.LongDistAttack;
                    //}
                }

                if (distX > 0)
                {
                    _largeEnemy.IsRight = -1;
                }
                else if (distX < 0)
                {
                    _largeEnemy.IsRight = 1;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(Tag.Player))
        {
            _largeEnemy.States = LargeEnemy.State.Stop;
            _isPlayerFound = false;
        }
    }
}