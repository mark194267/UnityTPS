using System;
using Assets.Script.ActionControl;
using Assets.Script.Avater;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Script.ActionList
{
    class ImpAction: ActionBasic
    {
        public override void Before_idle(ActionStatus actionStatus)
        {
            base.Before_idle(actionStatus);
            gun.ChangeWeapon("MG");
        }

        public void Before_fireball(ActionStatus actionStatus)
        {
            myAgent.ResetPath();
            gun.ChangeWeapon("MG");
        }

        public bool fireball(ActionStatus actionStatus)
        {
            if (gun.NowWeapon.BulletInMag > 0)
            {
                if (Vector3.Angle(my.transform.TransformDirection(Vector3.forward),
                        target.transform.position - my.transform.position) < 5)
                {
                    gun.fire();
                }
            }
            else
            {
                return false;
            }
            RotateTowardlerp(target.transform);
            return true;
        }

        public bool jumpslash(ActionStatus actionStatus)
        {

            //RotateTowardlerp(target.transform);
            return true;
        }
    }
}
