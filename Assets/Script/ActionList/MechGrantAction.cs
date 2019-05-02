using Assets.Script.ActionControl;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.ActionList
{
    class MechGrantAction :ActionBasic
    {
        public override bool move(ActionStatus actionStatus)
        {
            var dir = my.transform.InverseTransformDirection(myAgent.velocity.normalized);
            animator.SetFloat("AI_ws",dir.z);
            animator.SetFloat("AI_ad", dir.x);

            return true;
        }
    }
}
