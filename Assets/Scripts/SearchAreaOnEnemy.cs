using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Values;

public class SearchAreaOnEnemy : MonoBehaviour
{
    private GameObject _playerInSearchArea;
    private Enemy.Enemy _enemy;

    private float _shortDist = 15f;

    private bool _isPlayerFound = false;

    private void Start()
    {
        _enemy = GetComponentInParent<Enemy.Enemy>();
        if (_enemy.Pattern == Enemy.Enemy.EnemyPattern.Skyper)
        {
            _shortDist = 2.5f;
        }
    }

    private void Update()
    {
        Debug.Log("<color=red>" + _enemy.name + "'s isFound:" + _isPlayerFound.ToString() + "</color>");
        if (!_isPlayerFound && !_enemy.IsAttackState)
        {
            float rand = Random.Range(0, 10);
            //Debug.Log("Rand:" + rand.ToString());
            if (rand < 5f)
            {
                if (!_enemy.IsPatrol)
                {
                    _enemy.States = Enemy.Enemy.State.Stop;
                }
            }
            else
            {
                _enemy.States = Enemy.Enemy.State.Patrol;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        //Debug.Log("other:" + other.tag);
        if (!_enemy.IsDown)
        {
            float rand = Random.Range(0, 10);
            //Debug.Log("Rand:" + rand.ToString());
            if (other.CompareTag(Tag.Player))
            {
                _isPlayerFound = true;
                _playerInSearchArea = other.gameObject;
                float distX = this.gameObject.transform.position.x - _playerInSearchArea.transform.position.x;
                //Debug.Log("distX:" + distX.ToString());
                //Debug.Log(_enemy.name + "'s IsAttackState:" + _enemy.IsAttackState.ToString());
                if (_enemy.IsAttackState) return;
                if (Mathf.Abs(distX) < _shortDist)
                {
                    _enemy.States = Enemy.Enemy.State.ShortDistAttack;
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
                    _enemy.States = Enemy.Enemy.State.LongDistAttack;
                    //}
                }

                if (distX > 0)
                {
                    _enemy.IsRight = -1;
                }
                else if (distX < 0)
                {
                    _enemy.IsRight = 1;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(Tag.Player))
        {
            _enemy.States = Enemy.Enemy.State.Stop;
            _isPlayerFound = false;
        }
    }
}