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
        public override void Before_move(ActionStatus actionStatus)
        {
            var coverPos = takecover(Target.transform.position, 45f, .3f);
            var hasPeople = Physics.CheckSphere(coverPos, 1f, LayerMask.GetMask("AI"));//檢查點上是否有人
            if (coverPos != Vector3.zero && !hasPeople)
            {
                Agent.SetDestination(coverPos);
            }
            else
            {
                //檢查到玩家的路徑是否存在
                NavMeshPath path = new NavMeshPath();
                Agent.CalculatePath(Target.transform.position, path);
                //不存在的話就找最近的掩體
                if (path.status == NavMeshPathStatus.PathPartial)
                {
                    //NavMeshHit hit;
                    var cor =  path.corners[path.corners.Length-1];
                    //Debug.Log(cor);
                    Agent.SetDestination(cor);
                    /*
                    if (Agent.FindClosestEdge(out hit))
                    {
                        Agent.SetDestination(hit.position);
                    }
                    */
                }
                else
                    Agent.SetPath(path);
            }
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
            UpdateWSAD_ToAnimator();

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
            var hit = AI.hit.transform;
            if (hit != null)
            {
                if (hit.CompareTag("Player")) return false;
            }            

            return true;
        }

        public void Before_shoot(ActionStatus actionStatus)
        {
            //Agent.SetDestination(Target.transform.position);
            Gun.MainWeaponBasic = Gun.ActiveWeapon("Crossbow_test")[0];
            Agent.ResetPath();
            //Gun.ChangeWeapon("MG");
            //var num = NavMesh.GetAreaFromName("H1");
            //hotArea.nowHeat = num;
            //hotArea.IsOn = true;
            AI.AddHotArea();
        }

        public override bool shoot(ActionStatus actionStatus)
        {
            RotateTowardSlerp(Target.transform.position, 3f);
            var angle = Vector3.Angle(Me.transform.TransformDirection(Vector3.forward),
                    Target.transform.position - Me.transform.position);
            var hit = AI.hit.transform;
            if (hit != null)
            {

                if (hit.CompareTag("Player"))
                {
                    if (AvaterMain.anim_flag == 1)
                    {
                        Gun.target = Target;
                        Gun.NowWeapon[0].BulletInMag = 3;
                        Gun.fire(Gun.MainWeaponBasic);
                        //AvaterMain.anim_flag = 0;
                    }
                }
                else
                    return false;
            }
            //if (angle < 10)
            //{

            //}
            //else
            //{
            //    Animator.SetFloat("AI_angle", 0);
            //}

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
            //UpdateWSAD_ToAnimator();

            var num = NavMesh.GetAreaFromName("H1");
            //hotArea.nowHeat = num;
            //hotArea.IsOn = true;

            //NowVecter = Vector3.zero;
            var MyPos = Me.transform.position;
            var TargetPos = Target.transform.position;
            //Physics.BoxCast(MyPos, Vector3.one * .1f, TargetPos - MyPos, out hit, Me.transform.rotation);
            //var step = hit.distance / Vector3.Distance(MyPos, TargetPos);

            var hit = AI.hit.transform;
            //Debug.Log(hit.transform.GetComponentInParent<Avater.AvaterMain>().name);
            if (hit)
            {
                var randDirection =
                SetSpreadOutPoint(TargetPos,
                hit.position,
                Random.Range(60f, 120f),
                //Random.Range(1f * step + 1f, 1f * step + 5f)
                Random.Range(5f, 5f));
                NavMeshHit navMeshHit;
                //檢查是否能移動到上面
                if (NavMesh.SamplePosition(randDirection, out navMeshHit, 3f, -1))
                {
                    Agent.SetDestination(navMeshHit.position);
                    //用於檢查                
                    //NowVecter = navMeshHit.position;
                }
                else Agent.SetDestination(TargetPos);
            }
            else Agent.SetDestination(TargetPos);
        }
        public bool SpreadOut(ActionStatus actionStatus)
        {
            //散開腳本
            //var MyPos = Gun.NowWeapon[0].weapon.transform.position;
            //var TargetPos = Target.transform.position + Vector3.up*.5f;
            //RaycastHit hit;
            //int layermask = LayerMask.GetMask("Ignore Raycast");
            //layermask = ~layermask;

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

            var hit = AI.hit.transform;
            if (hit != null)
            {
                if (hit.CompareTag("Player")) return false;
            }

            return true;
        }
        public void Before_kick(ActionStatus actionStatus)
        {
            Gun.MainWeaponBasic = Gun.ActiveWeapon("kick")[0];
        }
        public bool kick(ActionStatus actionStatus)
        {
            RotateTowardlerp(Target.transform);
            //Gun.Swing(AvaterMain.anim_flag, 1, 1);
            Gun.Swing(Gun.MainWeaponBasic);
            return true;
        }
    }
}
