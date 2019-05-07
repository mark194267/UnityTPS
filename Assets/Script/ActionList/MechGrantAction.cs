using Assets.Script.ActionControl;
using UnityEngine;
using UnityEngine.AI;
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
            Agent.stoppingDistance = .5f;
        }

        public override bool move(ActionStatus actionStatus)
        {
            //之後改用路徑請求
            Agent.SetDestination(Target.transform.position);

            var dir = Me.transform.InverseTransformDirection(Agent.velocity.normalized);
            Animator.SetFloat("AI_ws",dir.z);
            Animator.SetFloat("AI_ad", dir.x);

            RotateTowardSlerp(Target.transform.position,3f);


            return true;
        }

        public void Before_shoot(ActionStatus actionStatus)
        {
            //Agent.SetDestination(Target.transform.position);

            Gun.ChangeWeapon("MG");
        }

        public override bool shoot(ActionStatus actionStatus)
        {
            var dir = Me.transform.InverseTransformDirection(Agent.velocity.normalized);
            Animator.SetFloat("AI_ws", dir.z);
            Animator.SetFloat("AI_ad", dir.x);

            var Max = Mathf.Max(Mathf.Abs(dir.z), Mathf.Abs(dir.x));
            Animator.SetFloat("AI_speed", Max);

            RotateTowardSlerp(Target.transform.position,3f);

            var angle = Vector3.Angle(Me.transform.TransformDirection(Vector3.forward),
                    Target.transform.position - Me.transform.position);

            if (angle < 10)
            {
                var f = Mathf.Lerp(Animator.GetFloat("AI_angle"), Mathf.Clamp(3 / angle, 0, 1), Time.deltaTime*.3f/*轉速加速度*/);
                Animator.SetFloat("AI_angle", f);
                //格林機槍轉速
                //用的是自然數 1/N 
                /*
                Debug.Log(f / Gun.NowWeaponOrign.rof);
                Gun.NowWeapon.rof = f/Gun.NowWeaponOrign.rof;
                */
                var MyPos = Me.transform.position + Vector3.up;
                var TargetPos = Target.transform.position + Vector3.up;
                RaycastHit hit;
                Physics.BoxCast(MyPos, Vector3.one*.5f, TargetPos - MyPos, out hit, Me.transform.rotation);
                if (hit.transform.CompareTag("Player"))
                {
                    Gun.fire();
                }
                else
                    return false;
            }
            else Animator.SetFloat("AI_angle", 0);

            return true;
        }
        public void Before_SpreadOut(ActionStatus actionStatus)
        {
            NowVecter = Vector3.zero;
            var MyPos = Me.transform.position + Vector3.up;
            var TargetPos = Target.transform.position + Vector3.up;
            RaycastHit hit;
            Physics.BoxCast(MyPos, Vector3.one*.5f, TargetPos - MyPos, out hit, Me.transform.rotation);

            //得到障礙和自身的夾角
            //var randDirection = Vector3.Reflect(hit.transform.position, Me.transform.TransformVector(Vector3.forward));
            //Debug.Log(randDirection);
            var r = Vector3.SignedAngle(Target.transform.position - Me.transform.position, hit.point - Me.transform.position,Vector3.up);
            //Debug.Log(r);
            var rot = Me.transform.TransformDirection(Vector3.forward);
            var fin  = Quaternion.AngleAxis(r, Vector3.up) * rot;
            var randDirection = Me.transform.position+fin.normalized*3;//NowVector已經是正規化的向量了    
            //var randDirection = Random.onUnitSphere * Vector3.Distance(hit.point, Me.transform.position);
            //randDirection += Me.transform.position;

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
            //動畫參數
            var dir = Me.transform.InverseTransformDirection(Agent.velocity.normalized);
            Animator.SetFloat("AI_ws", dir.z);
            Animator.SetFloat("AI_ad", dir.x);
            
            var Max = Mathf.Max(Mathf.Abs(dir.z), Mathf.Abs(dir.x));
            Animator.SetFloat("AI_speed", Max);

            //散開腳本
            var MyPos = Me.transform.position + Vector3.up;
            var TargetPos = Target.transform.position + Vector3.up;
            RaycastHit hit;
            Physics.BoxCast(MyPos, Vector3.one*.5f, TargetPos - MyPos, out hit, Me.transform.rotation);
            /*
             * 確認前面有隊友之後
             * 隨機往左或右邊移動
             */
            if (!Me.transform.CompareTag(hit.transform.tag) || Agent.remainingDistance < 1f)
            {
                Debug.Log(hit.transform.name);
                return false;
            }
            return true;
        }
        public void After_SpreadOut(ActionStatus actionStatus)
        {

        }
    }
}
