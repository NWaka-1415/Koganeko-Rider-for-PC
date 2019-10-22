using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireChromeAttack : StateMachineBehaviour
{
    private Enemy _enemyFireChrome;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _enemyFireChrome = animator.gameObject.GetComponent<Enemy>();
        _enemyFireChrome.Rigidbody2D.velocity =
            new Vector2(50f * _enemyFireChrome.IsRight, _enemyFireChrome.Rigidbody2D.velocity.y);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _enemyFireChrome.Rigidbody2D.velocity =
            new Vector2(50f * _enemyFireChrome.IsRight, _enemyFireChrome.Rigidbody2D.velocity.y);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _enemyFireChrome.Rigidbody2D.velocity = new Vector2(0f, _enemyFireChrome.Rigidbody2D.velocity.y);
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