using Assets.Script.ActionControl;
using UnityEngine;

namespace Assets.Script.ActionList
{
    class SkeletonAction : ActionBasic
    {
        public void Before_shoot(ActionStatus actionStatus)
        {
            gun.ChangeWeapon("MG");
        }
        public override void Before_slash(ActionStatus actionStatus)
        {
            gun.ChangeWeapon("katana");
            base.Before_slash(actionStatus);
        }
    }
}
