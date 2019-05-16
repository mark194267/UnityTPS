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
    class UnityChanAction : ActionScript
    {
        public bool ChangeWeapon(ActionStatus actionStatus)
        {
            //InputManager 得到現在按鈕的參照
            //將玩家武器Dictionary內部的索引值對應InputManager的參照
            return true;
        }

        public void Before_meleeready(ActionStatus actionStatus)
        {
            Animator.SetBool("avater_can_jump", true);
            Gun.ChangeWeapon("Wakizashi");
        }

        public bool meleeready(ActionStatus actionStatus)
        {
            FPSLikeRigMovement(5f, 10f);

            //FPSLikeRigMovement(5f/*AvaterMain.MotionStatus.motionSpd*/, 10f);
            return true;
        }
        public void Before_MoveNslash(ActionStatus actionStatus)
        {
            Gun.ChangeWeapon("katana");
        }

        public bool MoveNslash(ActionStatus actionStatus)
        {
            var camPos = Camera.transform.TransformDirection(Vector3.forward);
            RotateTowardlerp(Me.transform.position + camPos, 7f);
            var myVec = Me.transform.TransformVector(Vector3.forward);
            
            if (Physics.Raycast(Me.transform.position,myVec,3f))
            {
                return false;
            }
            Rig.velocity = Me.transform.TransformDirection(Vector3.forward*7f);
            return true;
        }

        public void After_MoveNslash(ActionStatus actionStatus)
        {
            NowVecter = Rig.velocity;
        }

        public override void Before_slash(ActionStatus actionStatus)
        {
            Rig.velocity = NowVecter;
            Rig.velocity += Me.transform.TransformDirection(Vector3.up * 5f);

        }

        public override bool slash(ActionStatus actionStatus)
        {
            //Debug.Log("lal111");
            
            var camPos = Camera.transform.TransformDirection(Vector3.forward);
            RotateTowardlerp(Me.transform.position + camPos, 7f);

            Gun.Swing(AvaterMain.anim_flag, (int)Convert.ToDouble(actionStatus.Vector3.x), actionStatus.Vector3.y);
            return true;
        }

        public void Before_slash1(ActionStatus actionStatus)
        {
            Rig.velocity = Vector3.zero;
        }

        public bool slash1(ActionStatus actionStatus)
        {
            var camPos = Camera.transform.TransformDirection(Vector3.forward);
            RotateTowardlerp(Me.transform.position + camPos, 7f);

            Gun.Swing(AvaterMain.anim_flag,(int)Convert.ToDouble(actionStatus.Vector3.x),actionStatus.Vector3.y);
            return true;
        }

        public void Before_slash2(ActionStatus actionStatus)
        {
            var vec = Me.transform.TransformDirection(Vector3.forward * 7f);
            vec.y = 0;
            Rig.velocity = vec;
        }

        public bool slash2(ActionStatus actionStatus)
        {
            var camPos = Camera.transform.TransformDirection(Vector3.forward);
            RotateTowardlerp(Me.transform.position + camPos, 3f);

            Gun.Swing(AvaterMain.anim_flag, (int)Convert.ToDouble(actionStatus.Vector3.x), actionStatus.Vector3.y);
            return true;
        }

        public void Before_slash3(ActionStatus actionStatus)
        {
            var vec = Me.transform.TransformDirection(Vector3.forward * 10f);
            vec.y = 0;
            Rig.velocity = vec;
        }

        public bool slash3(ActionStatus actionStatus)
        {
            var camPos = Camera.transform.TransformDirection(Vector3.forward);
            RotateTowardlerp(Me.transform.position + camPos, 3f);

            Gun.Swing(AvaterMain.anim_flag, (int)Convert.ToDouble(actionStatus.Vector3.x), actionStatus.Vector3.y);
            return true;
        }

        public void Before_jumpAtk(ActionStatus actionStatus)
        {
            Gun.ChangeWeapon("katana");
        }

        public bool jumpAtk(ActionStatus actionStatus)
        {
            Gun.Swing(AvaterMain.anim_flag, (int)Convert.ToDouble(actionStatus.Vector3.x), actionStatus.Vector3.y);
            return true;
        }
        public void Before_heavyslash(ActionStatus actionStatus)
        {
            Gun.NowWeapon.charge = Animator.GetFloat("charge");
        }

        public bool heavyslash(ActionStatus actionStatus)
        {
            /*
            if (doOnlyOnce)
            {
                Agent.velocity = Me.transform.TransformDirection
                    (Vector3.forward*5f+Vector3.up*100); 
                Debug.Log("your power is "+Gun.NowWeapon.charge); 
                doOnlyOnce = false;   
            }
            */
            return true;
        }

        public void Before_meleeDodge(ActionStatus actionStatus)
        {
            var ws = Animator.GetFloat("input_ws");
            var ad = Animator.GetFloat("input_ad");
            //如果直接按下就會往前
            if (ws * ws + ad * ad <= 0) ws = -1;
            var vec = Me.transform.TransformDirection(new Vector3(ad, 0, ws));
            Rig.velocity = Vector3.ClampMagnitude(vec * 15, 10f);
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
            //var camPos = Camera.transform.TransformDirection(Vector3.back *InputManager.ws+Vector3.left*InputManager.ad);
            //RotateTowardlerp(Me.transform.position-camPos);
            //Rig.velocity = Vector3.Lerp(Rig.velocity, Vector3.zero, 0.5f);
            return true;
        }

        public override void Before_move(ActionStatus actionStatus)
        {
            Animator.SetBool("avater_can_jump", true);
        }

        public override bool move(ActionStatus actionStatus)
        {
            var camPos = Camera.transform.TransformDirection(Vector3.back * InputManager.ws + Vector3.left * InputManager.ad);
            RotateTowardSlerp(Me.transform.position - camPos, 5f);
            var endspeed = Me.transform.TransformDirection(Vector3.forward*InputManager.maxWSAD).normalized * actionStatus.f1;
            Rig.velocity = Vector3.Lerp(Rig.velocity,endspeed,.1f);
            return true;
        }
        public override void After_move(ActionStatus actionStatus)
        {
            NowVecter = Rig.velocity;
        }

        public void Before_strafe(ActionStatus actionStatus)
        {
            Gun.ChangeWeapon("AK-47");
        }

        public bool strafe(ActionStatus actionStatus)
        {
            FPSLikeRigMovement(3f,10f);
            if(Input.GetButton("Fire1"))
            {
                return Gun.fire();
            }
            return true;
        }

        public void Before_jump(ActionStatus actionStatus)
        {
            Animator.SetBool("avater_can_jump",false);
            //Animator.SetBool("avater_IsLanded",false);

            /*
            var ws = Animator.GetFloat("input_ws");
            var ad = Animator.GetFloat("input_ad");
            var camPos = Camera.transform.TransformDirection(new Vector3(ad, 0, ws));
            RotateTowardlerp(Me.transform.position + camPos, 100f);
            //Rig.AddForce(Me.transform.TransformDirection(Vector3.forward * 3f), ForceMode.Impulse);
            Rig.AddForce(Vector3.up * 7f,ForceMode.Impulse);
            */
            //Rig.AddRelativeForce(Vector3.forward * 5f);

        }
        public bool jump(ActionStatus actionStatus)
        {
            //應該適用AddForce故不能用FpsLike，或是只更新他的x,z軸
            //FPSLikeRigMovement(.2f,.1f);
            
            //Rig.velocity = NowVecter*10;
            if (AvaterMain.anim_flag == 1)
            {
                AvaterMain.anim_flag = 0;
                Rig.AddRelativeForce(/*Vector3.forward * 5+*/Vector3.up* 5,ForceMode.VelocityChange);
            }
            return true;
        }
        public void After_jump(ActionStatus actionStatus)
        {
            //NowVecter = Rig.velocity;
        }

        public void Before_dodge(ActionStatus actionStatus)
        {
            var col = Me.GetComponent<Collider>();
            col.enabled = false;
        }

        public bool After_dodge(ActionStatus actionStatus)
        {
            var col = Me.GetComponent<Collider>();
            col.enabled = true;
            return true;
        }
        public bool magnet_melee(ActionStatus actionStatus)
        {
            //參考:https://blog.csdn.net/u013700908/article/details/52888792
            //球體碰撞器來返回目標，夾角小於X時就會自動追尾
            float range = 10f;
            Collider[] colliders = Physics.OverlapSphere(Me.transform.position,range,-1/*LayerMask*/);
            Debug.Log(colliders[0].name);//找到物件
            return true;
        }

        #region 跑牆
        /// <summary>
        /// 2019-01-18新增
        /// </summary>
        /// <param name="actionStatus"></param>
        /// <returns></returns>
        public void Before_wallrun(ActionStatus actionStatus)
        {
            float angle;
            if(Animator.GetFloat("avater_AngleBetweenWall") > 90)
            {
                angle = -90;
            }
            else
            {
                angle = 90;
            }

            var hit = Me.GetComponent<ParkourCollision>().hit;
            //轉為世界向量
            var rot = hit.normal;
            //沿著Y軸轉90度    
            NowVecter = Quaternion.AngleAxis(angle,Vector3.up)*rot;

            Rig.rotation = Quaternion.LookRotation(NowVecter);
        }

        public bool wallrun(ActionStatus actionStatus)
        {
            //給定速度
            Rig.velocity = NowVecter.normalized*6+Vector3.up*3;//NowVector已經是正規化的向量了
            //轉過去
            float pos;
            if(Animator.GetFloat("avater_AngleBetweenWall") > 90)
            {
                pos = -.5f;
            }
            else
            {
                pos = .5f;
            }
            
            if(!Physics.CheckBox(Me.transform.TransformPoint(pos,.7f,.2f),Vector3.one*.05f,Me.transform.rotation,-1,QueryTriggerInteraction.Ignore))
            {
                //如果踩空牆壁...目前全面停用
                Debug.Log("wall");
                //Rig.isKinematic = true;
                return false;
            }            
            return true;
        }
        
        public bool After_wallrun(ActionStatus actionStatus)
        {
            float pos;
            if(Animator.GetFloat("avater_AngleBetweenWall") > 90)
            {
                pos = 1.5f;
            }
            else
            {
                pos = -1.5f;
            }

            Rig.AddForce(Me.transform.TransformVector(Vector3.right*pos)*3,ForceMode.VelocityChange);
            Animator.SetBool("avater_can_parkour", false);
            return true;
        }
        #endregion

        public void Before_tranglejump(ActionStatus actionStatus)
        {
            //Rig.useGravity = true;
            //Rig.isKinematic = true;勿打開!!打開後Rigibody的任何動量相關皆會失效
            //Animator.SetBool("avater_can_jump",true);
            var hit = Me.GetComponent<ParkourCollision>().hit;
            var q = Quaternion.AngleAxis(180,Vector3.up)*hit.normal;
            //轉為世界向量
            NowVecter = q;
            //沿著Y軸轉90度    
            //Rig.velocity = NowVecter*2+Vector3.up*10;//NowVector已經是正規化的向量了 
        }

        public bool tranglejump(ActionStatus actionStatus)
        {
            Rig.velocity = Vector3.up*3;//NowVector已經是正規化的向量了

            if(!Physics.CheckBox(Me.transform.TransformVector(new Vector3(0,.7f,.5f)),Vector3.one*.05f,Me.transform.rotation))
            {
                //Rig.isKinematic = true;
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
            //Rig.velocity = NowVecter;
        }
        public bool falling(ActionStatus actionStatus)
        {
            /*
            var camPos = Camera.transform.TransformDirection(Vector3.back * InputManager.ws + Vector3.left * InputManager.ad);
            RotateTowardSlerp(Me.transform.position - camPos, 5f);
            var endspeed = Me.transform.TransformDirection(Vector3.forward * InputManager.maxWSAD).normalized * actionStatus.f1;
            var NoneYspeed = Vector3.Lerp(Rig.velocity, endspeed, .7f);
            NoneYspeed.y = 0;
            Rig.velocity = NoneYspeed+Vector3.down * 9.8f;
            */
            return true;
        }
        public void After_falling(ActionStatus actionStatus)
        { 
        }

        public void Before_softland(ActionStatus actionStatus)
        {
            Rig.AddRelativeForce(Vector3.forward * 7, ForceMode.VelocityChange);
        }

        public bool softland(ActionStatus actionStatus)
        {
            if (AvaterMain.anim_flag == 1)
            {
                AvaterMain.anim_flag = 0;
                //Rig.AddForce(Vector3.up * 10f, ForceMode.Impulse);
                Rig.AddRelativeForce(Vector3.forward * 15, ForceMode.VelocityChange);
            }
            return true;
        }

        public void Before_hardland(ActionStatus actionStatus)
        {
            //var vec_old = NowVecter; 
            //Rig.velocity = vec*10;

            var vec = Me.transform.TransformDirection(Vector3.forward);
            vec.y = 0;

            Rig.AddForce(vec * 10f,ForceMode.VelocityChange);
        }

        public bool hardland(ActionStatus actionStatus)
        {
            //Rig.velocity = Vector3.zero;
            return true;
        }

        public void Before_slide(ActionStatus actionStatus)
        {
            var vec = Me.transform.TransformDirection(Vector3.forward);
            vec.y = 0;
            Rig.AddForce(vec * 5f, ForceMode.VelocityChange);
        }
        public bool slide(ActionStatus actionStatus)
        {
            if (AvaterMain.anim_flag == 1)
            {
                var camPos = Camera.transform.TransformDirection(Vector3.back * InputManager.ws + Vector3.left * InputManager.ad);
                RotateTowardSlerp(Me.transform.position - camPos, 5f);
                var endspeed = Me.transform.TransformDirection(Vector3.forward * InputManager.maxWSAD).normalized * actionStatus.f1;
                Rig.velocity = Vector3.Lerp(Rig.velocity, endspeed, 5f);
            }
            return true;
        }
        public void After_slide(ActionStatus actionStatus)
        {
            AvaterMain.anim_flag = 0;
        }

        public override bool reload(ActionStatus actionStatus)
        {
            var camPos = Camera.transform.TransformDirection(Vector3.back * InputManager.ws + Vector3.left * InputManager.ad);
            RotateTowardSlerp(Me.transform.position - camPos, 5f);
            var endspeed = Me.transform.TransformDirection(Vector3.forward * InputManager.maxWSAD).normalized * actionStatus.f1;
            Rig.velocity = Vector3.Lerp(Rig.velocity, endspeed, .1f);

            return true;
        }
        public void After_reload(ActionStatus actionStatus)
        {
            Gun.reload();
        }
    }
}
