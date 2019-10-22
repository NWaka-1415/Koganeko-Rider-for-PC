using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;

public class ContinuityAttackBehaviour : StateMachineBehaviour
{
    private Player _player;
    private GameSceneController _gameSceneController;
    private int _count;
    private int _attackCount;
    private float _delta;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("isAttack00");
        _player = animator.gameObject.GetComponent<Player>();
        _gameSceneController = GameObject.Find("GameManager").GetComponent<GameSceneController>();
        if (!_gameSceneController.GameClear && !_gameSceneController.GameOver)
        {
            _player.Attack(0);
        }

        _count = 0;
        _attackCount = 0;
        _delta = 0f;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _delta += Time.deltaTime;
        if (_count == 0)
        {
            if (_player.TapCount > 0)
            {
                _count++;
                _player.TapCount -= 1;
                animator.SetBool("isAttack00", true);
            }
        }

        if (stateInfo.IsName("Attack00"))
        {
            if (_attackCount == 0)
            {
                _attackCount++;
                _player.PlayAudio(0);
            }
        }
        else if (stateInfo.IsName("Attack01"))
        {
            if (_attackCount == 0)
            {
                _attackCount++;
                _player.PlayAudio(1);
            }
        }
        else if (stateInfo.IsName("Attack02"))
        {
            if (_attackCount == 0)
            {
                _attackCount++;
                _player.PlayAudio(2);
                _player.GetComponent<Rigidbody2D>().MovePosition(new Vector2(
                    _player.transform.localPosition.x + 5f * _player.IsRight, _player.transform.localPosition.y));
            }
        }
        else if (stateInfo.IsName("Attack03"))
        {
            if (_delta >= 40f / 60f)
            {
                if (_attackCount == 1)
                {
                    _attackCount++;
                    _player.ShotRabbitTankBullet();
                }
            }
            else if (_delta >= 10f / 60f)
            {
                if (_attackCount == 0)
                {
                    _attackCount++;
                    _player.ShotRabbitTankBullet();
                }
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Attack03"))
        {
            animator.ResetTrigger("isAttack00");
            _player.TapCount = 0;
        }

        if (!_gameSceneController.GameClear && !_gameSceneController.GameOver) _player.Attack(1);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}