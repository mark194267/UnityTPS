using Assets.Script.ActionControl;
using UnityEngine;

namespace Assets.Script.ActionList
{
    class SkeletonAction : ActionBasic
    {
        
        public void Before_slash(ActionStatus actionStatus)
        {
            gun.ChangeWeapon("katana");
        }
        
    }
}
