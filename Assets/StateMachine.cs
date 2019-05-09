using Assets.Script.ActionControl;
using Assets.Script.AIGroup;
using Assets.Script.Avater;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : StateMachineBehaviour
{
    public GameObject me { get; set; }
    public ActionScript action { get; set; }
    public AvaterMain AvaterMain { get; set; }
    public AIBase AIBase { get; set; }

    private ActionStatus _actionStatus { get; set; }
    private MotionStatus _motionStatus { get; set; }
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator Animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (layerIndex == 0)
        {
            foreach (var actionStatuse in AvaterMain.actionStatusDictionary.AllActionStatusDictionary)
            {
                if (stateInfo.IsTag(actionStatuse.Key))
                {
//                    Debug.Log("OnStateEnter" + actionStatuse.Value.ActionName);
                    action.BeforeCustomAction(actionStatuse.Value);
                    _actionStatus = actionStatuse.Value;
                }
            }
            if (AvaterMain.motionStatusDir != null)
            {
                foreach (var motionStatus in AvaterMain.motionStatusDir)
                {
                    if (stateInfo.IsName(motionStatus.Key))
                    {
                        AvaterMain.MotionStatus = motionStatus.Value;
                        //Debug.Log(AvaterMain.MotionStatus.motionSpd);
                    }
                }
            }
            //目前AI還是每一禎重讀一次
            //如果是AI
            if (AIBase != null)
            {
                Animator.SetTrigger("AI_" + AIBase.DistanceBasicAI(AIBase.TargetInfo.GetTargetDis(), 3, 50));
            }
        }
    }
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator Animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (layerIndex == 0)
        {
            //Debug.Log("update");
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
//                    Debug.Log("OnStateExit" + actionStatuse.Value.ActionName);
                    action.AfterCustomAction(actionStatuse.Value);
                }
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
