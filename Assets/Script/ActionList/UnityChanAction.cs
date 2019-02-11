using System;
using Assets.Script.ActionControl;
using Assets.Script.Avater;
using Assets.Script.Config;
using Assets.Script.weapon;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Script.ActionList
{
    class UnityChanAction : ActionBasic
    {
        public bool ChangeWeapon(ActionStatus actionStatus)
        {
            //input 得到現在按鈕的參照
            //將玩家武器Dictionary內部的索引值對應input的參照
            return true;
        }

        public void Before_shoot(ActionStatus actionStatus)
        {
            gun.ChangeWeapon("MG");
        }

        public bool shoot(ActionStatus actionStatus)
        {
            RotateTowardSlerp(target.transform.position);
            gun.fire();
            return true;
        }

        public void Before_slash1(ActionStatus actionStatus)
        {
            gun.ChangeWeapon("katana");
        }

        public void slash1(ActionStatus actionStatus)
        {
            if (actionElapsedTime > actionStatus.Time1)
            {
                if (doOnlyOnce)
                {
                    gun.StartSlash(actionStatus.Time2);
                    doOnlyOnce = false;
                }
            }
        }

        public void Before_heavyslash(ActionStatus actionStatus)
        {
            gun.NowWeapon.charge = animator.GetFloat("charge");
        }

        public bool heavyslash(ActionStatus actionStatus)
        {
            if (doOnlyOnce)
            {
                myAgent.velocity = my.transform.TransformDirection
                    (Vector3.forward*5f+Vector3.up*100); 
                Debug.Log("your power is "+gun.NowWeapon.charge); 
                doOnlyOnce = false;   
            }
            return true;
        }

        public bool idle(ActionStatus actionStatus)
        {
            //var camPos = camera.transform.TransformDirection(Vector3.back *input.ws+Vector3.left*input.ad);
            //RotateTowardlerp(my.transform.position-camPos);
            myAgent.velocity = Vector3.Lerp(myAgent.velocity, Vector3.zero, 0.5f);
            return true;
        }

        public bool move(ActionStatus actionStatus)
        {
            if (Input.anyKey)
            {
                var camPos = camera.transform.TransformDirection(Vector3.back *input.ws+Vector3.left*input.ad);
                RotateTowardlerp(my.transform.position-camPos,5f);
                myAgent.velocity = my.transform.TransformDirection(Vector3.forward).normalized * 5f;
            }

            //gun.fire();
            return true;
        }

        public void Before_strafe(ActionStatus actionStatus)
        {
            gun.ChangeWeapon("AK-47");
        }

        public bool strafe(ActionStatus actionStatus)
        {
            FPSLikeMovement(5f,10f);
            if(Input.GetButton("Fire1"))
            {
                return gun.fire();
            }
            return true;
        }

        public void Before_jump(ActionStatus actionStatus)
        {
            animator.SetBool("avater_can_jump",false);
            animator.SetBool("avater_IsLanded",false);

            NowVecter = myAgent.velocity;

            myAgent.enabled = false;
            myRig.isKinematic = false;
            myRig.useGravity = true;
            myRig.AddForce(NowVecter+Vector3.up * 7f,ForceMode.Impulse);
        }
        public bool jump(ActionStatus actionStatus)
        {
            //應該適用AddForce故不能用FpsLike，或是只更新他的x,z軸
            //FPSLikeRigMovement(.2f,.1f);
            return true;
        }

        public void Before_dodge(ActionStatus actionStatus)
        {
            var col = my.GetComponent<Collider>();
            col.enabled = false;
        }

        public bool After_dodge(ActionStatus actionStatus)
        {
            var col = my.GetComponent<Collider>();
            col.enabled = true;
            return true;
        }
        public bool magnet_melee(ActionStatus actionStatus)
        {
            //參考:https://blog.csdn.net/u013700908/article/details/52888792
            //球體碰撞器來返回目標，夾角小於X時就會自動追尾
            float range = 10f;
            Collider[] colliders = Physics.OverlapSphere(my.transform.position,range,-1/*LayerMask*/);
            Debug.Log(colliders[0].name);//找到物件
            return true;
        }
        #region 跑牆
        /// <summary>
        /// 2019-01-18新增
        /// </summary>
        /// <param name="actionStatus"></param>
        /// <returns></returns>
        public void Before_wallrunR(ActionStatus actionStatus)
        {
            myRig.useGravity = true;
            //myRig.isKinematic = true;勿打開!!打開後Rigibody的任何動量相關皆會失效
            //animator.SetBool("avater_can_jump",true);
            var hit = my.GetComponent<PlayerAvater>().hit;
            //轉為世界向量
            var rot = hit.normal;
            //沿著Y軸轉90度    
            NowVecter = Quaternion.AngleAxis(-90,Vector3.up)*rot;
            
            myRig.velocity = NowVecter.normalized*6+Vector3.up*3;//NowVector已經是正規化的向量了
        }

        public bool wallrunR(ActionStatus actionStatus)
        {
            //轉過去
            var Q = Quaternion.LookRotation(NowVecter);
            myRig.rotation = Quaternion.Lerp(my.transform.rotation,Q,.1f);
            //其實不需要每禎都抓
            /*
            if(Physics.BoxCast(myRig.position+Vector3.right*-.5f+Vector3.up*.7f+Vector3.forward*0,new Vector3(.2f,.5f,.5f)
            ,my.transform.TransformDirection(Vector3.forward)))
            {
                Debug.Log("A wall");
            }
            else
            {
                myRig.isKinematic = true;//Debug用
                return false;
            }
            */
            return true;
        }

        public void Before_wallrunL(ActionStatus actionStatus)
        {            
            myRig.useGravity = true;
            //myRig.isKinematic = true;勿打開!!打開後Rigibody的任何動量相關皆會失效
            //animator.SetBool("avater_can_jump",true);
            var hit = my.GetComponent<PlayerAvater>().hit;
            //轉為世界向量
            var rot = hit.normal;     
            NowVecter = Quaternion.AngleAxis(90,Vector3.up)*rot;      
            myRig.velocity = NowVecter.normalized*6+Vector3.up*3;//NowVector已經是正規化的向量了            
        }
        public bool wallrunL(ActionStatus actionStatus)
        {            
            //轉過去
            var Q = Quaternion.LookRotation(NowVecter);
            myRig.rotation = Quaternion.Lerp(my.transform.rotation,Q,.1f);
            //其實不需要每禎都抓
            /*
            if(Physics.BoxCast(myRig.position+Vector3.right*.5f+Vector3.up*.7f+Vector3.forward*0,new Vector3(.2f,.5f,.5f)
            ,my.transform.TransformDirection(Vector3.forward)))
            {
                //Debug.Log("A wall");
            }
            else
            {
                //myRig.isKinematic = true;//Debug用
                //return false;
            }
            */
            return true;
    
        }
        #endregion

        public void Before_tranglejump(ActionStatus actionStatus)
        {
            myRig.useGravity = true;
            //myRig.isKinematic = true;勿打開!!打開後Rigibody的任何動量相關皆會失效
            //animator.SetBool("avater_can_jump",true);
            var hit = my.GetComponent<PlayerAvater>().hit;
            var q = Quaternion.AngleAxis(180,Vector3.up)*hit.normal;
            //轉為世界向量
            NowVecter = q;
            //沿著Y軸轉90度    
            myRig.velocity = NowVecter*2+Vector3.up*10;//NowVector已經是正規化的向量了 
        }

        public bool tranglejump(ActionStatus actionStatus)
        {
            //Debug.Log("Me!");
            //var Q = Quaternion.LookRotation(NowVecter);
            //myRig.rotation = Quaternion.Lerp(my.transform.rotation,Q,.1f);
            return true;
        }
        
        public bool PanicMelee(ActionStatus actionStatus)
        {

            return true;
        }
        public bool falling(ActionStatus actionStatus)
        {
            FPSLikeMovement(5f,10f);
            return true;
        }
        public bool land(ActionStatus actionStatus)
        {
            return true;
        }
        public void Before_land(ActionStatus actionStatus)
        {
            animator.SetInteger("anim_flag",0);
            my.GetComponent<Rigidbody>().useGravity = false;
            my.GetComponent<Rigidbody>().isKinematic = true;
            my.GetComponent<NavMeshAgent>().enabled = true;
        }

    }
}
