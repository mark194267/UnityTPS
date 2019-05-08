﻿using Assets.Script.weapon;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Script.AIGroup
{
    class AIConstructer
    {
        public AIBase GetAI(TargetInfo info)
        {
            return new ZombieAI() { TargetInfo = info};
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
                if (TargetInfo.TargetSightHit.transform.CompareTag("Player"))
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
        public RaycastHit ToTargetSight(float range)
        {
            RaycastHit hit;
            int mask = ~LayerMask.GetMask("PostProcessing");
            var MyPos = Me.transform.TransformPoint(Vector3.up);
            var TargetPos = Target.transform.TransformPoint(Vector3.up);

            //Physics.Raycast(Me.transform.position, Target.transform.position - Me.transform.position, out hit, range,-1,QueryTriggerInteraction.Ignore);
            Physics.BoxCast(MyPos,Vector3.one*.1f, TargetPos - MyPos, out hit,Me.transform.rotation, range, -1, QueryTriggerInteraction.Ignore);

            TargetSightHit = hit;
            return hit;
        }

    }
}
