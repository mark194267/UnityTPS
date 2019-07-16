using Assets.Script.ActionControl;
using Assets.Script.AIGroup;
using Assets.Script.Avater;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateMachine : StateMachineBehaviour
{
    public float maxShootRange = 50f;
    public float maxMeleeRange = 3f;

    public GameObject me { get; set; }
    public ActionScript action { get; set; }
    public AvaterMain AvaterMain { get; set; }
    public AIBase AIBase { get; set; }

    private ActionStatus _actionStatus { get; set; }
    private MotionStatus _motionStatus { get; set; }

    private string _latestCommand;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator Animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (layerIndex == 0)
        {
            if (AvaterMain.motionStatusDir != null)
            {
                //Debug.Log(me.name);
                foreach (var motionStatus in AvaterMain.motionStatusDir)
                {
                    //                    Debug.Log(motionStatus.Key);
                    //                    Debug.Log(motionStatus.Value.String);
                    if (stateInfo.IsName(motionStatus.Key))
                    {
                        AvaterMain.MotionStatus = motionStatus.Value;
                    }
                }
            }

            foreach (var actionStatuse in AvaterMain.actionStatusDictionary.AllActionStatusDictionary)
            {
                if (stateInfo.IsTag(actionStatuse.Key))
                {
                    action.BeforeCustomAction(actionStatuse.Value);
                    _actionStatus = actionStatuse.Value;
                }
            }


            foreach (var cando in AvaterMain.candolist)
            {
                Animator.SetBool(cando, true);
                if (_actionStatus.ignorelist != null)
                {
                    foreach (var i in _actionStatus.ignorelist)
                    {
                        if ("avater_can_" + i == cando)
                            Animator.SetBool("avater_can_" + i, false);
                    }
                }
            }
        }
    }
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator Animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (layerIndex == 0)
        {
            Animator.SetBool("avater_IsEndNormal", action.CustomAction(_actionStatus));

        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator Animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (layerIndex == 0)
        {
            foreach (var actionStatuse in AvaterMain.actionStatusDictionary.AllActionStatusDictionary)
            {
                if (stateInfo.IsTag(actionStatuse.Key))
                {
                    action.AfterCustomAction(actionStatuse.Value);
                }
            }
            //如果是AI
            if (AIBase != null)
            {
                Animator.SetBool("AI_" + _latestCommand, false);
                _latestCommand = AIBase.DistanceBasicAI(
                    AIBase.TargetInfo.GetTargetDis(), maxMeleeRange, maxShootRange);
                Animator.SetBool("AI_" + _latestCommand, true);
            }
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator Animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator Animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
