using Assets.Script.ActionControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Script.ActionList
{
    class Brute_WarriorAction : ActionScript
    {
        public override void Before_slash(ActionStatus actionStatus)
        {
            Agent.ResetPath();
            Gun.ChangeWeapon("fist");
        }
        public override bool slash(ActionStatus actionStatus)
        {
            Me.GetComponent<Animator>().applyRootMotion = true;
            RotateTowardlerp(Target.transform.position,2f);

            Gun.SwingByIndex(1, 1, 1);
            Gun.SwingByIndex(0, 1, 1);

            return true;
        }
        public void After_slash(ActionStatus AS)
        {
            Me.GetComponent<Animator>().applyRootMotion = false;
        }
        public void Before_kick(ActionStatus actionStatus)
        {
            //Gun.ChangeWeapon(AvaterMain.MotionStatus.String);
        }
        public bool kick(ActionStatus actionStatus)
        {
            Gun.SwingAll(AvaterMain.anim_flag, 1, 1);
            return true;
        }
        public void Before_shoot(ActionStatus actionStatus)
        {
            Agent.ResetPath();
            Gun.ChangeWeapon("MG");
        }
    }
}
