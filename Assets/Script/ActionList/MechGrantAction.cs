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

        public override bool move(ActionStatus actionStatus)
        {
            UpdateWSAD_ToAnimator();
            Agent.SetDestination(Target.transform.position);

            var MyPos = Me.transform.position + Vector3.up;
            var TargetPos = Me.transform.TransformDirection(Vector3.forward);
            RaycastHit hit;
            Physics.BoxCast(MyPos, Vector3.one * .1f, TargetPos, out hit, Me.transform.rotation);
            if (hit.rigidbody != null)
            {
                Debug.Log(hit.transform.name);
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
            UpdateWSAD_ToAnimator();

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
                Physics.BoxCast(MyPos, Vector3.one*.1f, TargetPos - MyPos, out hit, Me.transform.rotation);
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
            Agent.updateRotation = false;
            NowVecter = Vector3.zero;
            var MyPos = Me.transform.position + Vector3.up;
            var TargetPos = Target.transform.position + Vector3.up;
            RaycastHit hit;
            Physics.BoxCast(MyPos, Vector3.one*.1f, TargetPos - MyPos, out hit, Me.transform.rotation);

            /*
            //得到障礙和自身的夾角
            var r = Vector3.SignedAngle(Target.transform.position - Me.transform.position, hit.point - Me.transform.position,Vector3.up);
            var rot = Me.transform.TransformDirection(Vector3.forward);
            //隨機散開角度由.5-3倍，其實角度會和距離成反比 可用自然數改良
            var fin  = Quaternion.AngleAxis(Random.Range(.5f,3f)*-r, Vector3.up) * rot;
            var randDirection = Me.transform.position+fin.normalized*3;//NowVector已經是正規化的向量了    
            */
            //遮蔽物離目標越近，移動幅度要越大，越遠角度要越開
            var step = hit.distance / Vector3.Distance(MyPos,TargetPos);

            Debug.Log(step);

            var randDirection = 
                SetSpreadOutPoint(Target.transform.position, hit.point,
                //Random.Range(3, 7), Random.Range(3f*step, 5f*step)
                //12f * step+10, 5f * step+2f
                Random.Range(18f * step+10f, 20f * step+12f), Random.Range(1f * step +2f, 1f * step+8f)
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
            Physics.BoxCast(MyPos, Vector3.one*.1f, TargetPos - MyPos, out hit, Me.transform.rotation);

            if (hit.transform.CompareTag("Player") || Agent.remainingDistance < 1f)
            {
                //Debug.Log(hit.transform.name);
                return false;
            }
            return true;
        }
        public void After_SpreadOut(ActionStatus actionStatus)
        {

        }
    }
}
