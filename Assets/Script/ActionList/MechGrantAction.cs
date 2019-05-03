using Assets.Script.ActionControl;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.ActionList
{
    class MechGrantAction :ActionScript
    {
        public override void Before_move(ActionStatus actionStatus)
        {
            Agent.updateRotation = false;
        }

        public override bool move(ActionStatus actionStatus)
        {
            Agent.SetDestination(Target.transform.position);
            var dir = Me.transform.InverseTransformDirection(Agent.velocity.normalized);
            Animator.SetFloat("AI_ws",dir.z);
            Animator.SetFloat("AI_ad", dir.x);

            RotateTowardSlerp(Target.transform.position);

            return true;
        }
        public void Before_shoot(ActionStatus actionStatus)
        {
            Gun.ChangeWeapon("MG");
        }

        public override bool shoot(ActionStatus actionStatus)
        {
            //Agent.SetDestination(Target.transform.position);
            var dir = Me.transform.InverseTransformDirection(Agent.velocity.normalized);
            Animator.SetFloat("AI_ws", dir.z);
            Animator.SetFloat("AI_ad", dir.x);

            RotateTowardSlerp(Target.transform.position,.5f);

            var angle = Vector3.Angle(Me.transform.TransformDirection(Vector3.forward),
                    Target.transform.position - Me.transform.position);

            if (angle < 10)
            {
                Animator.SetFloat("AI_angle", Mathf.Clamp(3 / angle, 0, 1));
                return Gun.fire();
            }

            return true;
        }
    }
}
