using System;
using Assets.Script.ActionControl;
using Assets.Script.Avater;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Script.ActionList
{
    class ImpAction: ActionScript
    {
        public override void Before_idle(ActionStatus actionStatus)
        {
            base.Before_idle(actionStatus);
            Gun.ChangeWeapon("MG");
        }

        public void Before_fireball(ActionStatus actionStatus)
        {
            Agent.ResetPath();
            Gun.ChangeWeapon("MG");
        }

        public bool fireball(ActionStatus actionStatus)
        {
            if (Gun.NowWeapon.BulletInMag > 0)
            {
                if (Vector3.Angle(Me.transform.TransformDirection(Vector3.forward),
                        Target.transform.position - Me.transform.position) < 5)
                {
                    Gun.fire();
                }
            }
            else
            {
                return false;
            }
            RotateTowardlerp(Target.transform);
            return true;
        }

        public bool jumpslash(ActionStatus actionStatus)
        {

            //RotateTowardlerp(Target.transform);
            return true;
        }
    }
}
