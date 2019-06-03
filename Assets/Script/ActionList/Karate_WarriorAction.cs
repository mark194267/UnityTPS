using Assets.Script.ActionControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.ActionList
{
    class Karate_WarriorAction:ActionScript
    {
        public override void Before_slash(ActionStatus actionStatus)
        {
            Agent.ResetPath();
            Gun.ChangeWeapon("fist");
        }
    }
}
