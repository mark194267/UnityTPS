using Assets.Script.ActionControl;
using Assets.Script.AIGroup;
using Assets.Script.Avater;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerStateMachine : StateMachine
{
    public PlayerAvater PlayerAvater { get; set; }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator Animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (layerIndex == 0)
        {
            PlayerAvater.moveflag = 0;
            PlayerAvater.anim_flag = 0;
            PlayerAvater.IsRotChest = false;
            PlayerAvater.IsRotChestH = false;
            PlayerAvater.IsRotChestV = false;

            Animator.applyRootMotion = false;

            if (AvaterMain.motionStatusDir != null)
            {
                //Debug.Log(me.name);
                foreach (var motionStatus in AvaterMain.motionStatusDir)
                {
                    //Debug.Log(motionStatus.Key);
                    //Debug.Log(motionStatus.Value.String);
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

}
