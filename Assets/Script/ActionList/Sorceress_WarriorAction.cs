using Assets.Script.ActionControl;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.ActionList
{
    class Sorceress_WarriorAction : ActionScript
    {
        public float offsetHeight = 5f;
        public Vector3 destination;

        public bool IsAgentInPos()
        {
            if (!Agent.pathPending)
            {
                if (Agent.remainingDistance <= Agent.stoppingDistance)
                {
                    if (!Agent.hasPath || Agent.velocity.sqrMagnitude == 0f)
                    {
                        return true;
                    }
                }
            }
            return false;
        } 

        public Vector3 GetCanFlyNode(NavMeshPath _path)
        {
            Vector3 canFlyNode = Vector3.zero;
            Vector3 myPos = new Vector3( Me.transform.position.x, offsetHeight, Me.transform.position.z);

            //找到可以直線前進的目標位置，到了之後再重畫路徑
            
            for (int i = 0; i < _path.corners.Length; i++)
            {
                Vector3 airPath = new Vector3(_path.corners[i].x, offsetHeight, _path.corners[i].z);
                RaycastHit hit;
                Physics.Linecast(myPos, airPath, out hit, LayerMask.GetMask("Default", "Parkour"));
                var point = hit.transform;
                if (point == null)
                {
                    canFlyNode = airPath;
                }
            }
            //Debug.Log("Choose Point: "+canFlyNode);
            return canFlyNode;
        }


        public override void Before_move(ActionStatus actionStatus)
        {
            Agent.ResetPath();
            NavMeshPath path = new NavMeshPath();
            if (Agent.CalculatePath(Target.transform.position, path))
            {
                destination = GetCanFlyNode(path);
            }
            else
                Debug.Log("Pathing fail");
        }
        public override bool move(ActionStatus actionStatus)
        {
            Vector3 myPos = new Vector3(Me.transform.position.x, offsetHeight, Me.transform.position.z);

            if (destination == Vector3.zero)//如果有路徑正在執行中，就先追隨路徑
            {
                //Debug.Log("Agent");
                Agent.SetDestination(Target.transform.position);
                Rig.transform.position = new Vector3(Agent.transform.position.x,offsetHeight, Agent.transform.position.z);

                if (IsAgentInPos())
                {
                    Debug.Log("here!");
                    return false;
                }
            }
            else
            {
                //Debug.Log("NoAgent");
                //Vector3 toDes = destination - myPos;//取得往目標方向的向量
                //Rig.AddForce(toDes.normalized*1f*(Vector3.Distance(myPos, destination))*Time.deltaTime, ForceMode.VelocityChange);
                Vector3 toDes = destination - Me.transform.position;
                toDes.y = 0;
                var afterLerp = Vector3.Slerp(Rig.velocity, toDes.normalized * 5f, Time.deltaTime*1.5f);
                Rig.velocity = afterLerp;

                
                //Rig.transform.position = Vector3.Slerp(Me.transform.position, destination, Time.deltaTime * .7f);


                var warp2Pos = new Vector3(Me.transform.position.x,0, Me.transform.position.z);

                Agent.Warp(warp2Pos);
                /*
                if (Vector3.Distance(Me.transform.position, destination) < 2f)
                {
                    return false;
                }
                */
            }
            RotateTowardSlerp(Target.transform.position,2f);

            /*
            if (IsAgentInPos())
            {
                return false;
            }
            */

            //Agent.Warp(Me.transform.position);

            //Rig.transform.position = Vector3.Lerp(Me.transform.position, destination, Time.deltaTime * 1f);
            
            var hit = AI.hit.transform;
            if (hit != null)
            {
                if (hit.CompareTag("Player")) return false;
            }
            else
            {
                return true;
            }

            return true;
        }
        public override void After_move(ActionStatus AS)
        {
        }

        public void Before_shoot(ActionStatus actionStatus)
        {
            Agent.ResetPath();
            //Gun.ChangeWeapon("MG");

            //AI.AddHotArea();
        }

        public override bool shoot(ActionStatus actionStatus)
        {
            Vector3 meVec = new Vector3(Me.transform.position.x, offsetHeight, Me.transform.position.z);
            Vector3 tarVec = new Vector3(Agent.transform.position.x, offsetHeight, Agent.transform.position.z);

            Vector3 toDes = tarVec - Me.transform.position;
            toDes.y = 0;
            var afterLerp = Vector3.Slerp(Rig.velocity, toDes.normalized * 10f, Time.deltaTime * 1.5f);
            Rig.velocity = afterLerp;

            //Rig.transform.position = tarVec;
            Agent.SetDestination(Target.transform.position);

            //Rig.transform.position = Vector3.Lerp(meVec, tarVec, Time.deltaTime*1f);
            RotateTowardSlerp(Target.transform.position, 3f);

            var hit = AI.hit.transform;
            if (hit != null)
            {
                if (hit.CompareTag("Player")) return true;
            }
            else
                return false;

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
