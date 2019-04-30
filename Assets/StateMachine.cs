using Assets.Script.ActionControl;
using Assets.Script.Avater;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : StateMachineBehaviour
{
    public GameObject me { get; set; }
    public ActionBasic action { get; set; }
    public AvaterMain main { get; set; }
    private ActionStatus _actionStatus { get; set; }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (layerIndex == 0)
        {
            foreach (var actionStatuse in main.actionStatusDictionary.AllActionStatusDictionary)
            {
                if (stateInfo.IsTag(actionStatuse.Key))
                {
                    Debug.Log("OnStateEnter" + actionStatuse.Value.ActionName);
                    action.BeforeCustomAction(actionStatuse.Value);
                    _actionStatus = actionStatuse.Value;
                }
            }
        }
    }
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (layerIndex == 0)
        {
            //Debug.Log("update");
            me.GetComponent<Animator>().SetBool("avater_IsEndNormal", action.CustomAction(_actionStatus));
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (layerIndex == 0)
        {
            foreach (var actionStatuse in main.actionStatusDictionary.AllActionStatusDictionary)
            {
                if (stateInfo.IsTag(actionStatuse.Key))
                {
                    Debug.Log("OnStateExit" + actionStatuse.Value.ActionName);
                    action.AfterCustomAction(actionStatuse.Value);
                }
            }
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
