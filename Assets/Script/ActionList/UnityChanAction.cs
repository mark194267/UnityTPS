using System;
using Assets.Script.ActionControl;
using Assets.Script.Avater;
using Assets.Script.Avater.Addon;
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

        public void Before_meleeready(ActionStatus actionStatus)
        {
            gun.ChangeWeapon("katana");
        }

        public bool meleeready(ActionStatus actionStatus)
        {
            FPSLikeRigMovement(main.NowMotionStatus.motionSpd, 10f);
            return true;
        }
        public void Before_MoveNslash(ActionStatus actionStatus)
        {
            gun.ChangeWeapon("katana");
        }

        public bool MoveNslash(ActionStatus actionStatus)
        {
            var camPos = camera.transform.TransformDirection(Vector3.forward);
            RotateTowardlerp(my.transform.position + camPos, 7f);
            var myVec = my.transform.TransformVector(Vector3.forward);
            
            if (Physics.Raycast(my.transform.position,myVec,3f))
            {
                Debug.Log(myVec);
                return false;
            }
            myRig.velocity = my.transform.TransformDirection(Vector3.forward*7f);
            return true;
        }

        public void After_MoveNslash(ActionStatus actionStatus)
        {
            NowVecter = myRig.velocity;
        }

        public void Before_slash(ActionStatus actionStatus)
        {
            myRig.velocity = NowVecter;
            myRig.velocity += my.transform.TransformDirection(Vector3.up * 5f);

        }

        public bool slash(ActionStatus actionStatus)
        {
            //Debug.Log("lal111");
            
            var camPos = camera.transform.TransformDirection(Vector3.forward);
            RotateTowardlerp(my.transform.position + camPos, 7f);

            gun.Swing(main.anim_flag, (int)Convert.ToDouble(actionStatus.Vector3.x), actionStatus.Vector3.y);
            return true;
        }

        public void Before_slash1(ActionStatus actionStatus)
        {
            myRig.velocity = Vector3.zero;
        }

        public bool slash1(ActionStatus actionStatus)
        {
            var camPos = camera.transform.TransformDirection(Vector3.forward);
            RotateTowardlerp(my.transform.position + camPos, 7f);

            gun.Swing(main.anim_flag,(int)Convert.ToDouble(actionStatus.Vector3.x),actionStatus.Vector3.y);
            return true;
        }

        public void Before_slash2(ActionStatus actionStatus)
        {
            var vec = my.transform.TransformDirection(Vector3.forward * 7f);
            vec.y = 0;
            myRig.velocity = vec;
        }

        public bool slash2(ActionStatus actionStatus)
        {
            var camPos = camera.transform.TransformDirection(Vector3.forward);
            RotateTowardlerp(my.transform.position + camPos, 3f);

            gun.Swing(main.anim_flag, (int)Convert.ToDouble(actionStatus.Vector3.x), actionStatus.Vector3.y);
            return true;
        }

        public void Before_slash3(ActionStatus actionStatus)
        {
            var vec = my.transform.TransformDirection(Vector3.forward * 10f);
            vec.y = 0;
            myRig.velocity = vec;
        }

        public bool slash3(ActionStatus actionStatus)
        {
            var camPos = camera.transform.TransformDirection(Vector3.forward);
            RotateTowardlerp(my.transform.position + camPos, 3f);

            gun.Swing(main.anim_flag, (int)Convert.ToDouble(actionStatus.Vector3.x), actionStatus.Vector3.y);
            return true;
        }

        public void Before_jumpAtk(ActionStatus actionStatus)
        {
            gun.ChangeWeapon("katana");
        }

        public bool jumpAtk(ActionStatus actionStatus)
        {
            gun.Swing(main.anim_flag, (int)Convert.ToDouble(actionStatus.Vector3.x), actionStatus.Vector3.y);
            return true;
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

        public void Before_meleeDodge(ActionStatus actionStatus)
        {
            var ws = animator.GetFloat("input_ws");
            var ad = animator.GetFloat("input_ad");
            //如果直接按下就會往前
            if (ws * ws + ad * ad <= 0) ws = -1;
            var vec = my.transform.TransformDirection(new Vector3(ad, 0, ws));
            myRig.velocity = Vector3.ClampMagnitude(vec * 15, 10f);
        }

        public bool meleeDodge(ActionStatus actionStatus)
        {
            return true;
        }

        public override void Before_idle(ActionStatus actionStatus)
        {
        }

        public override bool idle(ActionStatus actionStatus)
        {
            //var camPos = camera.transform.TransformDirection(Vector3.back *input.ws+Vector3.left*input.ad);
            //RotateTowardlerp(my.transform.position-camPos);
            //myRig.velocity = Vector3.Lerp(myRig.velocity, Vector3.zero, 0.5f);
            return true;
        }

        public override void Before_move(ActionStatus actionStatus)
        {
        }

        public override bool move(ActionStatus actionStatus)
        {
            var camPos = camera.transform.TransformDirection(Vector3.back * input.ws + Vector3.left * input.ad);
            RotateTowardSlerp(my.transform.position - camPos, 5f);
            myRig.velocity = my.transform.TransformDirection(Vector3.forward).normalized * 5f;

            return true;
        }
        public override void After_move(ActionStatus actionStatus)
        {
            
        }

        public void Before_strafe(ActionStatus actionStatus)
        {
            gun.ChangeWeapon("AK-47");
        }

        public bool strafe(ActionStatus actionStatus)
        {
            FPSLikeRigMovement(3f,10f);
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

            NowVecter = myRig.velocity;

            var ws = animator.GetFloat("input_ws");
            var ad = animator.GetFloat("input_ad");
            var camPos = camera.transform.TransformDirection(new Vector3(ad, 0, ws));
            RotateTowardlerp(my.transform.position + camPos, 100f);
            myRig.AddForce(my.transform.TransformDirection(Vector3.forward * 3f), ForceMode.Impulse);
            myRig.AddForce(NowVecter+Vector3.up * 7f,ForceMode.Impulse);
        }
        public bool jump(ActionStatus actionStatus)
        {
            //應該適用AddForce故不能用FpsLike，或是只更新他的x,z軸
            //FPSLikeRigMovement(.2f,.1f);

            return true;
        }
        public void After_jump(ActionStatus actionStatus)
        {
            NowVecter = myRig.velocity;
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
            //myRig.useGravity = true;
            //myRig.isKinematic = true;勿打開!!打開後Rigibody的任何動量相關皆會失效
            var hit = my.GetComponent<ParkourCollision>().hit;
            //轉為世界向量
            var rot = hit.normal;
            //沿著Y軸轉90度    
            NowVecter = Quaternion.AngleAxis(-90,Vector3.up)*rot;

            myRig.rotation = Quaternion.LookRotation(NowVecter);
            animator.SetBool("avater_IsParkour",true);
        }

        public bool wallrunR(ActionStatus actionStatus)
        {
            //給定速度
            myRig.velocity = NowVecter.normalized*3+Vector3.up*1;//NowVector已經是正規化的向量了
            //轉過去
            //var Q = Quaternion.LookRotation(NowVecter);
            //myRig.rotation = Quaternion.Lerp(my.transform.rotation,Q,.1f);
            //myRig.rotation = Q;
            
            if(!Physics.CheckBox(my.transform.TransformPoint(-.8f,.7f,0),Vector3.one*.1f,my.transform.rotation,-1,QueryTriggerInteraction.Ignore))
            {
                //如果踩空牆壁...目前全面停用
                //Debug.Log("Hit");
                //myRig.isKinematic = true;
                return false;
            }            
            return true;
        }
        
        public bool After_wallrunR(ActionStatus actionStatus)
        {
            myRig.AddForce(my.transform.TransformVector(Vector3.right)*3,ForceMode.VelocityChange);
            animator.SetBool("avater_can_parkour", false);
            return true;
        }
        
        public void Before_wallrunL(ActionStatus actionStatus)
        {            
            //myRig.useGravity = true;
            //myRig.isKinematic = true;勿打開!!打開後Rigibody的任何動量相關皆會失效
            var hit = my.GetComponent<ParkourCollision>().hit;
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
          
            myRig.velocity = NowVecter.normalized*6+Vector3.up*3;//NowVector已經是正規化的向量了

            if(!Physics.CheckBox(my.transform.TransformPoint(new Vector3(.5f,.8f,.2f)),Vector3.one*.05f,my.transform.rotation))
            {
                //myRig.isKinematic = true;
                return false;
            }              
            return true;
        }
        public void After_wallrunL(ActionStatus actionStatus)
        {
            animator.SetBool("avater_can_parkour", false);
        }
        #endregion

        public void Before_tranglejump(ActionStatus actionStatus)
        {
            //myRig.useGravity = true;
            //myRig.isKinematic = true;勿打開!!打開後Rigibody的任何動量相關皆會失效
            //animator.SetBool("avater_can_jump",true);
            var hit = my.GetComponent<ParkourCollision>().hit;
            var q = Quaternion.AngleAxis(180,Vector3.up)*hit.normal;
            //轉為世界向量
            NowVecter = q;
            //沿著Y軸轉90度    
            //myRig.velocity = NowVecter*2+Vector3.up*10;//NowVector已經是正規化的向量了 
        }

        public bool tranglejump(ActionStatus actionStatus)
        {
            myRig.velocity = Vector3.up*3;//NowVector已經是正規化的向量了

            if(!Physics.CheckBox(my.transform.TransformPoint(new Vector3(0,.7f,.5f)),Vector3.one*.05f,my.transform.rotation))
            {
                //myRig.isKinematic = true;
                return false;
            }       
            return true;
        }
        
        public bool PanicMelee(ActionStatus actionStatus)
        {

            return true;
        }
        public void Before_falling(ActionStatus actionStatus)
        {
            //myRig.velocity = NowVecter;
        }
        public bool falling(ActionStatus actionStatus)
        {
            //FPSLikeMovement(5f,.5f);
            return true;
        }
        public void After_falling(ActionStatus actionStatus)
        {
        }
        public void Before_land(ActionStatus actionStatus)
        {
            //var vec_old = NowVecter; 
            var vec = my.transform.TransformDirection(Vector3.forward);
            myRig.velocity = vec*10;
            //落地檢查
            //Debug.Log(vec);
            //my.GetComponent<NavMeshAgent>().enabled = true;
        }

        public bool land(ActionStatus actionStatus)
        {
            //myRig.velocity = Vector3.zero;
            return true;
        }

        public void Before_slide(ActionStatus actionStatus)
        {
            var vec = my.transform.TransformDirection(Vector3.forward);
            vec.y = 0;
            myRig.AddForce(vec*300f);
        }
        public void After_slide(ActionStatus actionStatus)
        {
            animator.SetBool("input_dodge", false);
        }

        public override bool reload(ActionStatus actionStatus)
        {
            if (input.ad != 0 || input.ws != 0)
            {
                var camPos = camera.transform.TransformVector(new Vector3(input.ad, 0, input.ws));
                RotateTowardSlerp(my.transform.position + camPos, 5f);
                var vec = Vector3.ClampMagnitude(Vector3.forward * new Vector3(input.ad, 0, input.ws).magnitude * 5f, 10f);
                myRig.velocity = my.transform.TransformDirection(vec);
            }

            return true;
        }
        public void After_reload(ActionStatus actionStatus)
        {
            gun.reload();
        }
    }
}
