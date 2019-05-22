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
        private float _timer;

        public void Before_equip(ActionStatus actionStatus)
        {
            AvaterMain.anim_flag = 0;
        }
        public bool equip(ActionStatus actionStatus)
        {
            if (AvaterMain.anim_flag == 1)
            {
                Debug.Log(AvaterMain.MotionStatus.String);
                Gun.ChangeWeapon(AvaterMain.MotionStatus.String);
            }
            return true;
        }
        public void Before_Mstrafe(ActionStatus actionStatus)
        {
        }
        public bool Mstrafe(ActionStatus actionStatus)
        {
            FPSLikeRigMovement(3f, 10f);
            return true;
        }
        public void Before_kick(ActionStatus actionStatus)
        {
            //Gun.ChangeWeapon("kick");
        }

        public bool kick(ActionStatus actionStatus)
        {
            var camPos = Camera.transform.TransformDirection(Vector3.forward);
            RotateTowardlerp(Me.transform.position + camPos, 7f);

            if (AvaterMain.anim_flag == 2)
            {
                Rig.velocity = NowVecter;
                Rig.velocity += Me.transform.TransformDirection(Vector3.forward * 2f);
            }
            Gun.Swing(AvaterMain.anim_flag, (int)Convert.ToDouble(actionStatus.Vector3.x), actionStatus.Vector3.y);
            return true;
        }
        public void After_kick(ActionStatus actionStatus)
        {
            //Gun.ChangeWeapon("kick");
        }
        public override void Before_slash(ActionStatus actionStatus)
        {
            _timer = 0;
        }
        public override bool slash(ActionStatus actionStatus)
        {
            var camPos = Camera.transform.TransformDirection(Vector3.forward);
            RotateTowardlerp(Me.transform.position + camPos, 7f);

            _timer += Time.deltaTime;
            var mb = AvaterMain.MotionStatus;
            Rig.velocity = Me.transform.TransformVector(new Vector3(mb.camX, mb.camY, mb.camZ))*_timer;

            Gun.Swing(AvaterMain.anim_flag,(int)Convert.ToDouble(actionStatus.Vector3.x),actionStatus.Vector3.y);
            return true;
        }

        #region idle
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
        #endregion

        #region move
        public override void Before_move(ActionStatus actionStatus)
        {
            Animator.SetBool("avater_can_jump", true);
        }

        public override bool move(ActionStatus actionStatus)
        {
            var camPos = Camera.transform.TransformDirection(Vector3.back * InputManager.ws + Vector3.left * InputManager.ad);
            RotateTowardSlerp(Me.transform.position - camPos, 5f);
            var endspeed = Me.transform.TransformDirection(Vector3.forward*InputManager.maxWSAD).normalized * actionStatus.f1;
            Rig.velocity = Vector3.Lerp(Rig.velocity,endspeed,.3f);
            NowVecter = Rig.velocity;
            return true;
        }
        public override void After_move(ActionStatus actionStatus)
        {
        }
        #endregion

        #region strafe
        public void Before_strafe(ActionStatus actionStatus)
        {
            //Gun.ChangeWeapon("AK-47");
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
        #endregion

        #region jump
        public void Before_jump(ActionStatus actionStatus)
        {
            Animator.SetBool("avater_can_jump",false);
            _timer = .3f;
        }
        public bool jump(ActionStatus actionStatus)
        {
            var camPos = Camera.transform.TransformDirection(Vector3.back * InputManager.ws + Vector3.left * InputManager.ad);
            RotateTowardSlerp(Me.transform.position - camPos, 5f);
            var fwd = Me.transform.TransformVector(Vector3.forward*2f);
            
            if (AvaterMain.anim_flag == 1)
            {
                _timer += Time.deltaTime;
                Rig.velocity  = (Vector3.up* .7f/(_timer*_timer)+fwd/_timer*_timer);
            }
            else
                Rig.velocity = fwd/_timer*_timer;
            return true;
        }
        public void After_jump(ActionStatus actionStatus)
        {
            //NowVecter = Rig.velocity;
        }
#endregion

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

        #region tranglejump
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
        #endregion

        #region falling
        public void Before_falling(ActionStatus actionStatus)
        {
            //Rig.velocity = NowVecter;
        }
        public bool falling(ActionStatus actionStatus)
        {
            var camPos = Camera.transform.TransformDirection(Vector3.back * InputManager.ws + Vector3.left * InputManager.ad);
            RotateTowardSlerp(Me.transform.position - camPos, 5f);
            Rig.AddRelativeForce(Vector3.forward*2f);
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
        #endregion

        #region land
        public void Before_softland(ActionStatus actionStatus)
        {
            AvaterMain.anim_flag = 0;
            //Rig.AddRelativeForce(Vector3.forward * 7, ForceMode.VelocityChange);
        }

        public bool softland(ActionStatus actionStatus)
        {
            if (AvaterMain.anim_flag == 1)
            {
                //AvaterMain.anim_flag = 0;
                //Rig.AddForce(Vector3.up * 10f, ForceMode.Impulse);
                Rig.velocity = Me.transform.TransformVector(Vector3.forward*5);
                //Rig.AddRelativeForce(Vector3.forward * 15, ForceMode.VelocityChange);
            }
            else
                Rig.velocity = Me.transform.TransformVector(Vector3.forward*1);
            return true;
        }

        public void Before_hardland(ActionStatus actionStatus)
        {
            _timer = 0.15f;
        }

        public bool hardland(ActionStatus actionStatus)
        {
            _timer +=Time.deltaTime;
            var fwd = Me.transform.TransformVector(Vector3.forward)/(_timer*_timer);
            Rig.velocity = fwd;
            return true;
        }
        #endregion

        #region slide
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
        #endregion

        #region reload
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
        #endregion
    }
}
