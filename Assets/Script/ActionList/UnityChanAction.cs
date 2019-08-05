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
            /*
            var camPos = Camera.transform.TransformDirection(Vector3.back * InputManager.ws + Vector3.left * InputManager.ad);
            RotateTowardSlerp(Me.transform.position - camPos, 5f);
            var endspeed = Me.transform.TransformDirection(Vector3.forward * InputManager.maxWSAD).normalized * actionStatus.f1;
            Rig.velocity = Vector3.Lerp(Rig.velocity, endspeed, .3f);
            NowVecter = Rig.velocity;
            */
            FPSLikeRigMovement(3f, 10f);

            if (AvaterMain.anim_flag == 1)
            {
                //Debug.Log(AvaterMain.MotionStatus.String);
                //Gun.ChangeWeapon(AvaterMain.MotionStatus.String);
                var g = Me.GetComponent<PlayerAvater>().myguns;
                //Debug.Log(g.ToString());
                Gun.ChangeWeapon(g.ToString());
            }
            return true;
        }
        public void After_eqip(ActionStatus AS)
        {
            AvaterMain.anim_flag = 0;
        }
        public void Before_Mstrafe(ActionStatus actionStatus)
        {
            //Me.GetComponent<Animator>().applyRootMotion = true;
        }
        public bool Mstrafe(ActionStatus actionStatus)
        {
            //var camPos = Camera.transform.TransformDirection(Vector3.forward);
            //Vector3 direction = (Me.transform.position + camPos - Me.transform.position).normalized;
            //Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3

            //Me.GetComponent<Animator>().rootRotation = lookRotation;

             //Gun.Swing(AvaterMain.anim_flag, 10, 10);

            if (!Me.GetComponent<PlayerAvater>().IsRotChestH)
            {
                FPSLikeRigMovement(7f, 10f);
            }
            return true;
        }
        public void After_Mstrafe(ActionStatus actionStatus)
        {
            //Me.GetComponent<Animator>().applyRootMotion = false;
        }
        public void Before_kick(ActionStatus actionStatus)
        {
            //Gun.ChangeWeapon("kick");
            Me.GetComponent<Animator>().applyRootMotion = true;
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
            //Gun.Swing(AvaterMain.anim_flag, (int)Convert.ToDouble(actionStatus.Vector3.x), actionStatus.Vector3.y);
            return true;
        }
        public void After_kick(ActionStatus actionStatus)
        {
            //Gun.ChangeWeapon("kick");
            //Me.GetComponent<Animator>().applyRootMotion = false;

        }

        public void Before_block(ActionStatus AS)
        {
            Me.GetComponent<PlayerAvater>().myguns = PlayerAvater.Guns.Great_Sword;
            var g = Me.GetComponent<PlayerAvater>().myguns;
            Gun.ChangeWeapon(g.ToString());

            AvaterMain.anim_flag = 0;
            _timer = 0;
        }

        public bool block(ActionStatus AS)
        {
            Me.GetComponent<Animator>().applyRootMotion = true;
            Me.GetComponent<PlayerAvater>().IsRotChestH = true;
            Me.GetComponent<PlayerAvater>().IsRotChestV = true;
            Gun.Block(AvaterMain.anim_flag,0,0);
            return true;
        }
        public void After_block(ActionStatus AS)
        {
            Me.GetComponent<Animator>().applyRootMotion = false;
            Me.GetComponent<PlayerAvater>().IsRotChestH = false;
            Me.GetComponent<PlayerAvater>().IsRotChestV = false;
            AvaterMain.anim_flag = 0;
            _timer = 0;
        }

        public override void Before_slash(ActionStatus actionStatus)
        {

            Me.GetComponent<PlayerAvater>().myguns = PlayerAvater.Guns.Great_Sword;
            var g = Me.GetComponent<PlayerAvater>().myguns;
            Gun.ChangeWeapon(g.ToString());
            Me.GetComponent<PlayerAvater>().WeaponSlotNumber = 1;
            AvaterMain.anim_flag = 0;
            AvaterMain.moveflag = 0;

            NowVecter = new Vector3(AvaterMain.MotionStatus.camX,
    AvaterMain.MotionStatus.camY, AvaterMain.MotionStatus.camZ);
        }
        public override bool slash(ActionStatus actionStatus)
        {
            Me.GetComponent<Animator>().applyRootMotion = true;
            //Me.GetComponent<PlayerAvater>().IsRotChestH = true;
            //Me.GetComponent<PlayerAvater>().IsRotChestV = true;
            
            if (AvaterMain.moveflag > 0)
            {
                var camPos = Camera.transform.TransformDirection(Vector3.forward);
                RotateTowardSlerp(Me.transform.position + camPos, 7f);
                Me.GetComponent<Animator>().applyRootMotion = false;

                NowVecter = Vector3.Slerp(NowVecter, Vector3.zero,Time.deltaTime*2.5f);
                if (NowVecter.sqrMagnitude > 0)
                {
                    Rig.AddRelativeForce(NowVecter, ForceMode.VelocityChange);
                }
            }


            /*
            var camPos = Camera.transform.TransformDirection(Vector3.forward);
            RotateTowardlerp(Me.transform.position + camPos, 7f);
            var mb = AvaterMain.MotionStatus;
            if (AvaterMain.anim_flag > 0)
            {
                _timer += Time.deltaTime;
                Rig.velocity = Me.transform.TransformVector(new Vector3(mb.camX, mb.camY, mb.camZ)) * _timer;
            }
            */
            Gun.Swing(AvaterMain.anim_flag,(int)Convert.ToDouble(actionStatus.Vector3.x),actionStatus.Vector3.y);
            return true;
        }
        public void After_slash(ActionStatus actionStatus)
        {
            Me.GetComponent<Animator>().applyRootMotion = false;

            Me.GetComponent<PlayerAvater>().IsRotChestH = false;
            Me.GetComponent<PlayerAvater>().IsRotChestV = false;
            AvaterMain.anim_flag = 0;
            AvaterMain.moveflag = 0;
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

            PlayerAvater PA = Me.GetComponent<PlayerAvater>();
            var ms = PA.MotionStatus;
            PA.chestOffSet = new Vector3(ms.camX, ms.camY, ms.camZ);

            PA.IsRotChestH = true;
            PA.IsRotChestV = true;
        }

        public override bool move(ActionStatus actionStatus)
        {
            var camPos = Camera.transform.TransformDirection(Vector3.back * InputManager.ws + Vector3.left * InputManager.ad);
            RotateTowardSlerp(Me.transform.position - camPos, 9f);
            var endspeed = Me.transform.TransformDirection(Vector3.forward*InputManager.maxWSAD).normalized * actionStatus.f1;
            Rig.velocity = Vector3.Lerp(Rig.velocity,endspeed,.1f);
            NowVecter = Rig.velocity;
            return true;
        }
        public override void After_move(ActionStatus actionStatus)
        {
            Me.GetComponent<PlayerAvater>().IsRotChestH = false;
            Me.GetComponent<PlayerAvater>().IsRotChestV = false;
        }
        #endregion

        #region strafe
        public void Before_strafe(ActionStatus actionStatus)
        {
            //Gun.ChangeWeapon("AK47");

            PlayerAvater PA = Me.GetComponent<PlayerAvater>();
            var ms = PA.MotionStatus;
            PA.chestOffSet = new Vector3(ms.camX, ms.camY, ms.camZ);

            PA.IsRotChestH = true;
            PA.IsRotChestV = true;
        }

        public bool strafe(ActionStatus actionStatus)
        {
            FPSLikeRigMovement(7f,10f);
            if(Input.GetButton("Fire1"))
            {
                return Gun.fire(0);
            }
            return true;
        }
        #endregion

        #region jump
        public void Before_jump(ActionStatus actionStatus)
        {
            //Animator.SetBool("avater_can_jump",false);
            //Animator.SetBool("action_can_fall", false);
            _timer = .3f;
        }
        public bool jump(ActionStatus actionStatus)
        {
            var camPos = Camera.transform.TransformDirection(Vector3.back * InputManager.ws + Vector3.left * InputManager.ad);
            RotateTowardSlerp(Me.transform.position - camPos, 4f);
            var fwd = Me.transform.TransformVector(Vector3.forward*5f);
            
            if (AvaterMain.anim_flag == 1)
            {
                _timer += Time.deltaTime;
                Rig.velocity  = (Vector3.up* 1.6f/(_timer*_timer)+fwd/_timer*_timer);
            }
            else
                Rig.velocity = fwd/_timer*_timer;
            return true;
        }
        public void After_jump(ActionStatus actionStatus)
        {
            //NowVecter = Rig.velocity;
            //Animator.SetBool("action_can_fall", true);
        }
        #endregion

        #region jumpout
        public void Before_jumpout(ActionStatus AS)
        {
            //Me.GetComponent<Animator>().applyRootMotion = false;
            //Rig.AddRelativeForce(Vector3.up*5+Vector3.back * 5f, ForceMode.VelocityChange);
        }

        public bool jumpout(ActionStatus AS)
        {
            Rig.velocity = Rig.transform.TransformVector(Vector3.up*4+Vector3.back);
            return true;
        }
        public void After_jumpout(ActionStatus AS)
        {
            //Me.GetComponent<Animator>().applyRootMotion = false;
        }

        #endregion

        #region offwall

        public bool offwall(ActionStatus AS)
        {
            Animator.applyRootMotion = true;
            return true;
        }

        public void After_offwall(ActionStatus AS)
        {
            Animator.applyRootMotion = false;
        }

        #endregion

        #region wallrun
        /// <summary>
        /// 2019-01-18新增
        /// </summary>
        /// <param name="actionStatus"></param>
        /// <returns></returns>
        public void Before_wallrun(ActionStatus actionStatus)
        {
            Me.GetComponent<PlayerAvater>().IsRotChestH = true;

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

            Me.transform.rotation = Quaternion.LookRotation(NowVecter);
        }

        public bool wallrun(ActionStatus actionStatus)
        {
            //給定速度
            Rig.velocity = NowVecter.normalized*9+Vector3.up*2;//NowVector已經是正規化的向量了
            //轉過去
            float pos;
            if(Animator.GetFloat("avater_AngleBetweenWall") > 90)
            {
                pos = -1;
            }
            else
            {
                pos = 1;
            }
            
            if(!Physics.CheckSphere(Me.transform.TransformPoint(.3f*pos,1,0),.7f,LayerMask.GetMask("Parkour"),QueryTriggerInteraction.Ignore))
            {
                //Me.GetComponent<Animator>().applyRootMotion = false;
                return false;
            }            
            return true;
        }
        
        public bool After_wallrun(ActionStatus actionStatus)
        {
            Me.GetComponent<PlayerAvater>().IsRotChestH = false;

            float pos;
            if(Animator.GetFloat("avater_AngleBetweenWall") > 90)
            {
                pos = 1;
            }
            else
            {
                pos = -1;
            }

            Rig.AddRelativeForce(Me.transform.TransformVector(Vector3.right*pos)*2,ForceMode.VelocityChange);
            //Animator.SetBool("action_can_parkour", false);
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
            /*
            if(!Physics.CheckBox(Me.transform.TransformVector(new Vector3(0,.7f,.5f)),Vector3.one*.05f,Me.transform.rotation))
            {
                //Rig.isKinematic = true;
                return false;
            }
            */
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
            if (Input.anyKey)
            {
                RotateTowardSlerp(Me.transform.position - camPos, .2f);
                Rig.AddRelativeForce(Vector3.forward * 2f);
            }

            if (Input.GetButton("Fire1"))
            {
                return Gun.fire(0);
            }

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
            if (AvaterMain.anim_flag == 2)
            {
                Rig.velocity = Me.transform.TransformVector(Vector3.forward * .7f);
            }            
            else if (AvaterMain.anim_flag == 1)
                Rig.velocity = Me.transform.TransformVector(Vector3.forward * 5);
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
            Rig.AddForce(vec * 6f, ForceMode.VelocityChange);
            
            AvaterMain.anim_flag = 0;
        }
        public bool slide(ActionStatus actionStatus)
        {
            if (AvaterMain.anim_flag == 1)
            {
                var camPos = Camera.transform.TransformDirection(Vector3.back * InputManager.ws + Vector3.left * InputManager.ad);
                RotateTowardSlerp(Me.transform.position - camPos, 5f);
                var endspeed = Me.transform.TransformDirection(Vector3.forward * InputManager.maxWSAD).normalized * actionStatus.f1;
                Rig.velocity = Vector3.Lerp(Rig.velocity, endspeed, 5f);

                Gun.fire(0);
            }
            return true;
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
            Gun.reload(0);
        }
        #endregion

        #region climb

        public void Before_climb(ActionStatus AS)
        {
            Me.GetComponent<Animator>().applyRootMotion = true;

            var hit = Me.GetComponent<ParkourCollision>().hit;
            var toPoint = Me.transform.TransformVector(hit.point).normalized;
            var EdgeOffSet = Rig.position + toPoint * .3f;
            EdgeOffSet.y = hit.point.y;
            //Debug.Log("hitPos: " + hit.point + " toPoint: " + toPoint + " EdgeOffSet " + EdgeOffSet);
            velocity = EdgeOffSet;
            Rig.rotation = Quaternion.LookRotation(EdgeOffSet, Vector3.up);

            /*
            var hit = Me.GetComponent<ParkourCollision>().hit;
            //Me.transform.position = new Vector3(Me.transform.position.x, hit.point.y, Me.transform.position.z);
            Me.transform.position = hit.point;
            Debug.Log(hit.point);
            */
        }
        public bool climb(ActionStatus AS)
        {
            var hit = Me.GetComponent<ParkourCollision>().hit;
            var toPoint = Me.transform.position - hit.point;
            
            //Me.transform.position = Vector3.Lerp(Me.transform.position, hit.point, 10f*Time.deltaTime);
            //Me.transform.position = Vector3.Lerp(Me.transform.position, hit.point-toPoint*2f, 10f * Time.deltaTime);

            Me.transform.position = Vector3.Lerp(Me.transform.position, velocity, 5f * Time.deltaTime);

            return true;
        }
        public void After_climb(ActionStatus AS)
        {
            var hit = Me.GetComponent<ParkourCollision>().hit;
            //Me.transform.position = new Vector3(Me.transform.position.x, hit.point.y, Me.transform.position.z);
            Me.transform.position = hit.point;
            Me.GetComponent<Animator>().applyRootMotion = false;
            Me.GetComponent<Animator>().SetBool("avater_IsLanded", true);
            //Debug.Log(hit.point);
        }

        #endregion
    }
}
