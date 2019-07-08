using Assets.Script.weapon;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Script.AIGroup
{
    class AIConstructer
    {
        public AIBase GetAI(GameObject Me,GameObject Target)
        {
            return new ZombieAI() {
                TargetInfo = new TargetInfo() { Me = Me,Target= Target}
            };
        }
    }

    public class AIBase
    {
        public TargetInfo TargetInfo { get; set; }

        /// <summary>
        /// 以距離為主的AI，返回簡單的策略，如果返回並非遠中近就會執行指定字串
        /// </summary>
        /// <param name="meleerange">肉搏最遠距離</param>
        /// <returns></returns>
        public string DistanceBasicAI(float targetDis,float meleerange,float shootrange)
        {
            float distance = TargetInfo.GetTargetDis();
            TargetInfo.ToTargetSight(shootrange);
            //Debug.Log(TargetInfo.TargetSightHit.transform.name);
            //如果在距離內，又看的到目標，衝刺攻擊也算遠距離攻擊。
            if (TargetInfo.TargetSightHit.rigidbody != null)
            {
                //如果沒有彈藥就優先換彈
                // 2019-05-03 改用Gun.fire 回傳false在Animator內觸發
                /*
                if (Me.GetComponent<Gun>().NowWeapon.BulletInMag <= 0)
                {
                    return "reload";
                }
                */
                if (TargetInfo.TargetSightHit.transform.GetComponentInParent<Avater.AvaterMain>().CompareTag("Player"))
                {
                    if (distance < meleerange)
                    {
                        return "close";
                    }
                    else
                    {
                        return "long";
                    }
                }
                else
                {
                    //Debug.Log(TargetInfo.TargetSightHit.transform.name);
                    return "SpreadOut";
                }
            }
            //雖說是move卻指的是在射擊距離外
            return "move";
        }
    }

    class ZombieAI:AIBase
    {
    }

    public class TargetInfo
    {
        public GameObject Me { get; set; }
        public GameObject Target { get; set; }
        public float TargetDis { get; set; }
        public RaycastHit TargetSightHit { get; set; }
        public float TargetAngle { get; set; }

        public float GetTargetDis()
        {
            return Vector3.Distance(Me.transform.position, Target.transform.position);
        }
        public float GetTargetAngle()
        {
            return Vector3.Angle(Me.transform.TransformDirection(Vector3.forward),
                        Target.transform.position - Me.transform.position);
        }
        public Transform ToTargetSight(float range)
        {
            RaycastHit TargetHit = new RaycastHit();
            int mask = ~LayerMask.GetMask("Player","AI");
            var MyPos = Me.transform.position + Vector3.up * .5f;
            var TargetPos = Target.transform.position+ Vector3.up * .5f;

            //Physics.Raycast(Me.transform.position, Target.transform.position - Me.transform.position, out hit, range,-1,QueryTriggerInteraction.Ignore);
            //Physics.BoxCast(MyPos,Vector3.one*.1f, TargetPos - MyPos, out hit,Me.transform.rotation, range, -1, QueryTriggerInteraction.Ignore);
            var AllHit = Physics.SphereCastAll(MyPos, .3f, TargetPos - MyPos, range, -1, QueryTriggerInteraction.Ignore);
            foreach (var hit in AllHit)
            {
                if(hit.transform.GetComponentInParent<Avater.AvaterMain>() != Me.GetComponentInParent<Avater.AvaterMain>())
                {
                    TargetHit = hit;
                    break;
                }
            }
            TargetSightHit = TargetHit;
            return TargetHit.transform;
        }
        public Transform ToTargetSight()
        {
            RaycastHit TargetHit = new RaycastHit();
            int mask = ~LayerMask.GetMask("Player", "AI");
            var MyPos = Me.transform.position + Vector3.up * .5f;
            var TargetPos = Target.transform.position + Vector3.up * .5f;
            var range = Me.GetComponent<Animator>().GetBehaviour<StateMachine>().TargetDis;

            //Physics.Raycast(Me.transform.position, Target.transform.position - Me.transform.position, out hit, range,-1,QueryTriggerInteraction.Ignore);
            //Physics.BoxCast(MyPos,Vector3.one*.1f, TargetPos - MyPos, out hit,Me.transform.rotation, range, -1, QueryTriggerInteraction.Ignore);
            var AllHit = Physics.SphereCastAll(MyPos, .3f, TargetPos - MyPos, range, -1, QueryTriggerInteraction.Ignore);
            foreach (var hit in AllHit)
            {
                if (hit.transform.GetComponentInParent<Avater.AvaterMain>() != Me.GetComponentInParent<Avater.AvaterMain>())
                {
                    TargetHit = hit;
                    break;
                }
            }
            TargetSightHit = TargetHit;
            return TargetHit.transform;
        }
    }
}
