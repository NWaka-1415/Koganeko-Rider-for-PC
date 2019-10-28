using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;

public class ProductionBehaviour : StateMachineBehaviour
{
    private GameSceneController _gameSceneController;
    private GameObject _canvas;
    private AudioController _audioController;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _canvas = GameObject.Find("Canvas");
        _audioController = GameObject.Find("Audio Source").GetComponent<AudioController>();
        _gameSceneController = GameSceneController.instance;
        _gameSceneController.PlayStartAudio();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _canvas.GetComponent<Canvas>().enabled = true;
        _gameSceneController.GameStarted();
        _audioController.PlayBgm();
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