using Assets.Script.ActionControl;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.ActionList
{
    class Sorceress_WarriorAction : AIActionScript
    {
        public float offsetHeight = 5f;
        public Vector3 _velocity;
        public Vector3 destination;
        NavMeshPath _path = new NavMeshPath();        

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

        //找到可以直線前進的目標位置，到了之後再重畫路徑
        public Vector3 GetCanFlyNode(NavMeshPath _path)
        {
            Vector3 canFlyNode = Vector3.zero;
            Vector3 myPos = new Vector3( Me.transform.position.x, AiNav.flyHeight, Me.transform.position.z);
            //傳入路徑陣列._path.corner[1] 為自己的位置+1節點
            //，也就是直線故不會切西瓜
            if (_path.corners.Length > 1)
            {
                for (int i = 2; i < _path.corners.Length; i++)
                {
                    //檢查自己到現在的導航點在空中是否可以連成一直線
                    //無論如何檢查到最後一個點.最後只會返回最遠的點(切最直的路徑)
                    Vector3 airPath = new Vector3(_path.corners[i].x, AiNav.flyHeight, _path.corners[i].z);
                    RaycastHit hit;
                    Physics.Linecast(myPos, airPath, out hit, LayerMask.GetMask("Default", "Parkour"));
                    //射線沒打到代表路徑是通的
                    var point = hit.transform;
                    if (point == null)
                    {
                        canFlyNode = airPath;
                    }
                }
            }
            //Debug.Log("Choose Point: "+canFlyNode);
            //回傳最終點.沒找到點就是空值
            return canFlyNode;
        }



        public override void Before_move(ActionStatus actionStatus)
        {
            Agent.ResetPath();
            //找尋往目標(玩家)的路徑
            NavMeshHit meshHit;
            if (NavMesh.SamplePosition(Target.transform.position,out meshHit,3f,-1))
            {
                if (Agent.CalculatePath(meshHit.position, _path))
                {
                    //把目標點存起來
                    destination = GetCanFlyNode(_path);
                }
            }
            else
                Debug.Log("Pathing fail");//目標現在的位置無法前往
        }
        public override bool move(ActionStatus actionStatus)
        {
            Vector3 myPos = Me.transform.position;
            //Vector3 tarPos = new Vector3(Target.transform.position.x, AiNav.flyHeight, Target.transform.position.z);
            
            if (destination == Vector3.zero)//找不到可以切西瓜的導航點，走向下個轉角
            {
                Debug.Log("NextCorner");
                //導向下個轉角
                Agent.SetDestination(_path.corners[1]);                
                //跟著導航器走
                var endPoint = new Vector3(Agent.transform.position.x, AiNav.flyHeight, Agent.transform.position.z);
                Rig.transform.position = Vector3.Slerp(Rig.transform.position, endPoint, Time.deltaTime * .5f);
                // 2019-11-28 傳入的點並不能判斷正確位置的高度
                //送入下一禎時的位置 
                var nextPos = Vector3.Lerp(Agent.transform.position, Agent.nextPosition, Time.deltaTime * 1f);
                AiNav.nextStepPos = nextPos;
                //如果到達地點就重新呼叫狀態(或是離開) 
                //11-28 Agent和腳色到達目標點的時間不一致.而跟隨的腳色不一定會先到
                //2019-12-02 改大目標半徑來加快結束
                if (IsAgentInPos())
                {
                    return false;
                }
            }
            else //空中切西瓜
            {                
                Debug.Log("destination : "+ destination);
                //得到往目標點的方向
                Vector3 toDes = destination - myPos;
                //改變現有速度朝著目標向量
                var afterLerp = Vector3.Slerp(Rig.velocity, toDes.normalized * 5f, Time.deltaTime*1.5f);
                //用高度差乘上係數得到上升或下降速度
                afterLerp.y = 0f;
                //確認現在速率
                float _height = Mathf.Lerp(Rig.transform.position.y, AiNav.flyHeight, Time.deltaTime);

                Rig.transform.position =new Vector3(Rig.transform.position.x, _height, Rig.transform.position.z);
                Rig.velocity = afterLerp;
                //將導航器傳送到現在位置(不被高低差卡住)
                var warp2Pos = new Vector3(Me.transform.position.x,0, Me.transform.position.z);
                NavMeshHit warphit;
                if (NavMesh.SamplePosition(warp2Pos,out warphit, 5f, -1))
                {
                    Agent.Warp(warphit.position);
                }

                //送入下一禎時的位置
                var nextPos = Vector3.Lerp(Me.transform.position, destination, Time.deltaTime * 1.5f);
                AiNav.nextStepPos = nextPos;
                //到達位置--目前並不可靠
                //myPos.y = destination.y;
                //if (Vector3.Distance(myPos, destination) < 2.5f)
                //{
                //    return false;
                //}
                var hit = AiNav.hit.transform;
                if (hit != null)
                {
                    if (hit.CompareTag("Player")) return false;
                }
            }
            RotateTowardSlerp(Target.transform.position,2f);
            return true;
        }
        public override void After_move(ActionStatus AS)
        {
        }

        public void Before_shoot(ActionStatus actionStatus)
        {
            Agent.ResetPath();
            if(destination == Vector3.zero)
                destination = Me.transform.position;
            //Gun.ChangeWeapon("MG");
            AiNav.GetHeight(destination);
            //AI.AddHotArea();
            Gun.MainWeaponBasic = Gun.ActiveWeapon("Crossbow_test")[0];
        }

        public override bool shoot(ActionStatus actionStatus)
        {
            //取得目標點的高度

            //取得方向.移動
            Vector3 meVec = Me.transform.position;
            Vector3 tarVec = new Vector3(Target.transform.position.x, AiNav.flyHeight, Target.transform.position.z);
            Vector3 toTarget = Vector3.Slerp(meVec, tarVec, Time.deltaTime * 1.5f);
            AiNav.nextStepPos = toTarget;
            
            //速度
            var afterLerp = Vector3.Slerp(Rig.velocity, (tarVec-meVec).normalized * 6f, Time.deltaTime * .5f);
            var upSpd = (Target.transform.position + Vector3.up * 7f - Me.transform.position).y;
            
            //高度重算
            afterLerp.y = upSpd;
            Rig.velocity = afterLerp;
            RotateTowardSlerp(Target.transform.position, 5f);

            //使導航器追蹤腳色
            var warp2Pos = new Vector3(Me.transform.position.x, 0, Me.transform.position.z);
            NavMeshHit warphit;
            if (NavMesh.SamplePosition(warp2Pos, out warphit, 5f, -1))
            {
                Agent.Warp(warphit.position);
            }

            //射線檢查
            var hit = AiNav.hit.transform;
            if (hit != null)
            {
                if (hit.CompareTag("Player"))
                {
                    Gun.target = Target;
                    Gun.NowWeapon[0].BulletInMag = 3;
                    Gun.fire(Gun.MainWeaponBasic);
                    //AvaterMain.anim_flag = 0;
                }
                else
                    return false;
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
