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
            float distance = TargetInfo.TargetDis;
            //Debug.Log(TargetInfo.TargetDis);             
            //TargetInfo.ToTargetSight();
            var hit = TargetInfo.Me.GetComponent<Avater.AIAvaterMain>().hit;
            //如果在距離內，又看的到目標，衝刺攻擊也算遠距離攻擊。
            if (hit.rigidbody != null)
            {
                //如果沒有彈藥就優先換彈
                // 2019-05-03 改用Gun.fire 回傳false在Animator內觸發
                /*
                if (Me.GetComponent<Gun>().NowWeapon.BulletInMag <= 0)
                {
                    return "reload";
                }
                */
                if (distance < meleerange)
                {
                    //近距離有可能因起始範圍而打不到，回傳值為NULL
                    //或是回傳值為牆壁.

                    //Debug.Log(TargetInfo.TargetSightHit.transform.name);
                    return "close";
                }
                if (hit.transform.GetComponentInParent<Avater.AvaterMain>().CompareTag("Player"))
                {
                    return "long";
                }
                else
                {
                    //Debug.Log(TargetInfo.TargetSightHit.transform.name);
                    return "SpreadOut";
                }
            }
            //Debug.Log(hit.point);
            //雖說是move卻指的是在射擊距離外
            return "move";
        }
    }

    class ZombieAI:AIBase
    {
    }
}
