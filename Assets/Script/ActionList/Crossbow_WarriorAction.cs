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

        public override void Before_move(ActionStatus actionStatus)
        {
            var coverPos = takecover(Target.transform.position, 45f, .3f);
            var hasPeople = Physics.CheckSphere(coverPos, 1f, LayerMask.GetMask("AI"));//檢查點上是否有人
            if (coverPos != Vector3.zero && !hasPeople)
            {
                Agent.SetDestination(coverPos);
            }
            else
                Agent.SetDestination(Target.transform.position);
        }
        public override bool move(ActionStatus actionStatus)
        {
            //var coverPos = takecover(Target.transform.position, 45f, .5f);
            /*
            if (NowVecter != Vector3.zero)
            {
                Agent.SetDestination(NowVecter);
            }
            else
                Agent.SetDestination(Target.transform.position);
            */


            var hit = Targetinfo.TargetSightHit;
            var hitrans = hit.transform.GetComponentInParent<Avater.AvaterMain>();
            if (hitrans != null && hitrans.CompareTag("Player"))
            {
                return false;
            }

            if (!Agent.pathPending)
            {
                if (Agent.remainingDistance <= Agent.stoppingDistance)
                {
                    if (!Agent.hasPath || Agent.velocity.sqrMagnitude == 0f)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void Before_shoot(ActionStatus actionStatus)
        {
            //Agent.SetDestination(Target.transform.position);
            Agent.ResetPath();
            Gun.ChangeWeapon("MG");
            var num = NavMesh.GetAreaFromName("H1");
            hotArea.nowHeat = num;
            hotArea.IsOn = true;

        }

        public override bool shoot(ActionStatus actionStatus)
        {
            RotateTowardSlerp(Target.transform.position, 3f);
            var angle = Vector3.Angle(Me.transform.TransformDirection(Vector3.forward),
                    Target.transform.position - Me.transform.position);

            if (angle < 10)
            {
                //var MyPos = Me.transform.position + Vector3.up*.5f;
                //var TargetPos = Target.transform.position + Vector3.up * .5f;
                //RaycastHit hit;
                //int layermask = ~LayerMask.GetMask("Ignore Raycast");
                //Physics.SphereCast(MyPos, .1f, TargetPos - MyPos, out hit, 100f, layermask, QueryTriggerInteraction.Ignore);

                var hit = Targetinfo.TargetSightHit;
                var hitrans = hit.transform.GetComponentInParent<Avater.AvaterMain>();
                if (hitrans != null)
                {
                    if (hitrans.CompareTag("Player"))
                    {
                        if (AvaterMain.anim_flag == 1)
                        {
                            Gun.NowWeapon[0].BulletInMag = 1;
                            Gun.fire(0);
                        }
                    }
                    else
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
        public void After_shoot(ActionStatus actionStatus)
        {
            //var num = NavMesh.GetAreaFromName("walkable");
            //hotArea.nowHeat = num;
            //hotArea.IsOn = false;
        }
        public void Before_SpreadOut(ActionStatus actionStatus)
        {
            var num = NavMesh.GetAreaFromName("H1");
            hotArea.nowHeat = num;
            hotArea.IsOn = true;

            NowVecter = Vector3.zero;
            var MyPos = Me.transform.position;
            var TargetPos = Target.transform.position;
            RaycastHit hit;
            //Physics.BoxCast(MyPos, Vector3.one * .1f, TargetPos - MyPos, out hit, Me.transform.rotation);
            //var step = hit.distance / Vector3.Distance(MyPos, TargetPos);

            hit = Targetinfo.TargetSightHit;
            //Debug.Log(hit.transform.GetComponentInParent<Avater.AvaterMain>().name);
            var randDirection =
                SetSpreadOutPoint(TargetPos,
                hit.transform.GetComponentInParent<Avater.AvaterMain>().transform.position,
                Random.Range(60f, 90f),
                //Random.Range(1f * step + 1f, 1f * step + 5f)
                Random.Range(15f,30f));


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
            //var MyPos = Gun.NowWeapon[0].weapon.transform.position;
            //var TargetPos = Target.transform.position + Vector3.up*.5f;
            //RaycastHit hit;
            //int layermask = LayerMask.GetMask("Ignore Raycast");
            //layermask = ~layermask;
            //var hit = Targetinfo.ToTargetSight();

            //Physics.SphereCast(MyPos, .2f, TargetPos - MyPos, out hit, 100f, -1, QueryTriggerInteraction.Ignore);

            if (!Agent.pathPending)
            {
                if (Agent.remainingDistance <= Agent.stoppingDistance)
                {
                    if (!Agent.hasPath || Agent.velocity.sqrMagnitude == 0f)
                    {
                        return false;
                    }
                }
            }
            /*
            if (hit.transform.CompareTag("Player") )
            {
                return false;
            }
            */
            return true;
        }
        public void Before_kick(ActionStatus actionStatus)
        {
            Gun.ChangeWeapon("kick");
        }
        public bool kick(ActionStatus actionStatus)
        {
            RotateTowardlerp(Target.transform);
            Gun.Swing(AvaterMain.anim_flag, 1, 1);
            return true;
        }
    }
}
