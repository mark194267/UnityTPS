using Assets.Script.ActionControl;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.ActionList
{
    class Crossbow_WarriorAction : ActionScript
    {
        private float _timer;

        public override bool move(ActionStatus actionStatus)
        {
            Agent.SetDestination(Target.transform.position);

            var MyPos = Me.transform.position + Vector3.up * .5f;
            var TargetPos = Target.transform.position + Vector3.up * .5f;
            RaycastHit hit;
            int layermask = ~LayerMask.GetMask("Ignore Raycast");
            Physics.SphereCast(MyPos, .1f, TargetPos - MyPos, out hit, 100f, layermask, QueryTriggerInteraction.Ignore);
            if (hit.transform.CompareTag("Player"))
            {
                return false;
            }
            return true;
        }

        public void Before_shoot(ActionStatus actionStatus)
        {
            //Agent.SetDestination(Target.transform.position);

            Gun.ChangeWeapon("MG");
        }

        public override bool shoot(ActionStatus actionStatus)
        {
            RotateTowardSlerp(Target.transform.position, 3f);
            var angle = Vector3.Angle(Me.transform.TransformDirection(Vector3.forward),
                    Target.transform.position - Me.transform.position);

            if (angle < 10)
            {
                var MyPos = Me.transform.position + Vector3.up*.5f;
                var TargetPos = Target.transform.position + Vector3.up * .5f;
                RaycastHit hit;
                int layermask = ~LayerMask.GetMask("Ignore Raycast");
                Physics.SphereCast(MyPos, .1f, TargetPos - MyPos, out hit, 100f, layermask, QueryTriggerInteraction.Ignore);
                if (hit.transform.CompareTag("Player"))
                {
                    Gun.NowWeapon[0].BulletInMag = 1;
                    Gun.fire(0);
                }
                else
                {
                    Debug.Log(hit.transform.name);
                    return false;
                }
            }
            else
            {
                Animator.SetFloat("AI_angle", 0);
                _timer = 0;
            }

            return true;
        }
        public void Before_SpreadOut(ActionStatus actionStatus)
        {
            NowVecter = Vector3.zero;
            var MyPos = Me.transform.position;
            var TargetPos = Target.transform.position;
            RaycastHit hit;
            Physics.BoxCast(MyPos, Vector3.one * .1f, TargetPos - MyPos, out hit, Me.transform.rotation);
            var step = hit.distance / Vector3.Distance(MyPos, TargetPos);

            var randDirection =
                SetSpreadOutPoint(Target.transform.position, hit.point,
                Random.Range(1f * step + 1f, 1f * step + 5f)
                );


            NavMeshHit navMeshHit;
            //檢查是否能移動到上面
            if (NavMesh.SamplePosition(randDirection, out navMeshHit, 5f, -1))
            {
                Agent.SetDestination(navMeshHit.position);
                //用於檢查                
                NowVecter = navMeshHit.position;
            }
        }
        public bool SpreadOut(ActionStatus actionStatus)
        {
            //散開腳本
            var MyPos = Me.transform.position;
            var TargetPos = Target.transform.position;
            RaycastHit hit;
            int layermask = LayerMask.GetMask("Ignore Raycast");
            layermask = ~layermask;
            Physics.SphereCast(MyPos, .1f, TargetPos - MyPos, out hit, 100f, layermask, QueryTriggerInteraction.Ignore);
            if (!hit.transform.CompareTag("AI") || Agent.remainingDistance < 1f)
            {
                //Debug.Log(hit.transform.name);
                Agent.ResetPath();
                return false;
            }
            return true;
        }
    }
}
