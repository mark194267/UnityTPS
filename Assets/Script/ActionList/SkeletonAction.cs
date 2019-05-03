using Assets.Script.ActionControl;
using UnityEngine;

namespace Assets.Script.ActionList
{
    class SkeletonAction : ActionScript
    {
        public void Before_shoot(ActionStatus actionStatus)
        {
            Gun.ChangeWeapon("MG");
        }
        public override void Before_slash(ActionStatus actionStatus)
        {
            Gun.ChangeWeapon("katana");
            base.Before_slash(actionStatus);
        }
    }
}
