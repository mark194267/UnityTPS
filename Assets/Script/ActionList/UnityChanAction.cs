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
        private Vector3 _vecter;
        private Vector3 _velocity;
        PlayerAvater PA;
        PlayerAvater.Guns g;
        string LastWeapon;

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
            _vecter = Rig.velocity;
            */
            FPSLikeRigMovement(3f, 10f);

            if (AvaterMain.anim_flag == 1)
            {
                //Debug.Log(AvaterMain.MotionStatus.String);
                //Gun.ChangeWeapon(AvaterMain.MotionStatus.String);
                if (!String.IsNullOrEmpty(LastWeapon))
                {
                    Gun.InactiveWeapon(LastWeapon);
                }

                LastWeapon = Me.GetComponent<PlayerAvater>().myguns.ToString();
                //Debug.Log(g.ToString());
                //Gun.ChangeWeapon(g.ToString());
                Gun.MainWeaponBasic = Gun.ActiveWeapon(LastWeapon)[0];
            }
            return true;
        }
        public void After_equip(ActionStatus AS)
        {
            AvaterMain.anim_flag = 0;
        }
        public void Before_Mstrafe(ActionStatus actionStatus)
        {
            Me.GetComponent<PlayerAvater>().ChangeRotOffSet("strafe");

            //Me.GetComponent<Animator>().applyRootMotion = true;
            Me.GetComponent<PlayerAvater>().IsRotChest = true;
            //Me.GetComponent<PlayerAvater>().IsRotChestV = true;
            //Me.GetComponent<PlayerAvater>().IsRotChestH = false;
            Gun.SpecialWeaponBasic = Gun.ActiveWeapon("kick")[0];

        }
        public bool Mstrafe(ActionStatus actionStatus)
        {
            //Me.GetComponent<PlayerAvater>().IsRotChestV = true;
            //var camPos = Camera.transform.TransformDirection(Vector3.forward);
            //Vector3 direction = (Me.transform.position + camPos - Me.transform.position).normalized;
            //Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3

            //Me.GetComponent<Animator>().rootRotation = lookRotation;

            //Gun.Swing(AvaterMain.anim_flag, 10, 10);

            FPSLikeRigMovement(7f, 10f);

            return true;
        }
        public void After_Mstrafe(ActionStatus actionStatus)
        {
            //Me.GetComponent<Animator>().applyRootMotion = false;
        }

        public void Before_Block(ActionStatus actionStatus)
        {
            //Me.GetComponent<PlayerAvater>().ChangeRotOffSet("strafe");
            //Me.GetComponent<PlayerAvater>().IsRotChest = true;
            Gun.MainWeaponBasic.weapon.GetComponent<MeleeClass>().IsBlocking = true;
            Gun.MainWeaponBasic.weapon.GetComponent<Collider>().enabled = true;
        }
        public bool Block(ActionStatus actionStatus)
        {
            FPSLikeRigMovement(7f, 10f);

            return true;
        }
        public void After_Block(ActionStatus actionStatus)
        {
            //Me.GetComponent<Animator>().applyRootMotion = false;
            Gun.MainWeaponBasic.weapon.GetComponent<MeleeClass>().IsBlocking = false;
            Gun.MainWeaponBasic.weapon.GetComponent<Collider>().enabled = false;
        }

        public void Before_kick(ActionStatus actionStatus)
        {
            //Gun.ChangeWeapon("kick");
            Me.GetComponent<Animator>().applyRootMotion = true;
            Gun.SpecialWeaponBasic = Gun.ActiveWeapon("kick")[0];

        }

        public bool kick(ActionStatus actionStatus)
        {
            var camPos = Camera.transform.TransformDirection(Vector3.forward);
            RotateTowardlerp(Me.transform.position + camPos, 7f);
            //Gun.SwingByIndex(weaponindex, 1, 1);
            Gun.Swing(Gun.SpecialWeaponBasic);

            //Gun.Swing(AvaterMain.anim_flag, (int)Convert.ToDouble(actionStatus.Vector3.x), actionStatus.Vector3.y);
            return true;
        }
        public void After_kick(ActionStatus actionStatus)
        {
            //Gun.tempWeaponBasic.weapon.SetActive(false);
            //var g = Me.GetComponent<PlayerAvater>().myguns;
            //Gun.InactiveWeapon("kick");
            //Gun.ChangeWeapon(g.ToString());
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
        }

        public override void Before_slash(ActionStatus actionStatus)
        {
            //Rig.velocity = Vector3.zero;
            Me.GetComponent<PlayerAvater>().myguns = PlayerAvater.Guns.Great_Sword;
            //var g = Me.GetComponent<PlayerAvater>().myguns;
            //Gun.ChangeWeapon(g.ToString());
            Gun.MainWeaponBasic = Gun.ActiveWeapon("Great_Sword")[0];

            Me.GetComponent<PlayerAvater>().WeaponSlotNumber = 1;

            Me.GetComponent<Animator>().applyRootMotion = true;

            Me.GetComponent<PlayerAvater>().ChangeRotOffSet("slash");
            Me.GetComponent<PlayerAvater>().IsRotChestH = AvaterMain.MotionStatus.IsRotH;
            Me.GetComponent<PlayerAvater>().IsRotChestV = AvaterMain.MotionStatus.IsRotV;

            _velocity = Me.transform.TransformVector(AvaterMain.MotionStatus.camX,
            AvaterMain.MotionStatus.camY, AvaterMain.MotionStatus.camZ);
            _velocity = Vector3.ClampMagnitude(_velocity, 20f);

            //_vecter = new Vector3(AvaterMain.MotionStatus.camX,AvaterMain.MotionStatus.camY, AvaterMain.MotionStatus.camZ);
        }
        public override bool slash(ActionStatus actionStatus)
        {
            if (AvaterMain.moveflag > 0)
            {
                Animator.SetBool("avater_can_dodge", false);

                var camPos = Camera.transform.TransformDirection(Vector3.forward);
                RotateTowardSlerp(Me.transform.position + camPos, 3f);
                Me.GetComponent<Animator>().applyRootMotion = false;

                _vecter = Vector3.Slerp(_vecter, Vector3.zero, Time.deltaTime);
                //Rig.AddRelativeForce(_vecter, ForceMode.VelocityChange);
                Rig.velocity = _velocity;
            }
            else
            {
                Animator.SetBool("avater_can_dodge", true);
            }
            //Gun.Swing(AvaterMain.anim_flag,(int)Convert.ToDouble(actionStatus.Vector3.x),actionStatus.Vector3.y);
            Gun.Swing(Gun.MainWeaponBasic);

            return true;
        }
        public void After_slash(ActionStatus actionStatus)
        {
            //Me.GetComponent<Animator>().applyRootMotion = false;
        }


        public void Before_slashRoot(ActionStatus actionStatus)
        {
            Rig.velocity = Vector3.zero;
            Me.GetComponent<PlayerAvater>().myguns = PlayerAvater.Guns.Great_Sword;

            //Gun.ChangeWeapon(g.ToString());
            Me.GetComponent<PlayerAvater>().WeaponSlotNumber = 1;
            Gun.MainWeaponBasic = Gun.ActiveWeapon("Great_Sword")[0];

            Me.GetComponent<Animator>().applyRootMotion = true;

            Me.GetComponent<PlayerAvater>().IsRotChestH = AvaterMain.MotionStatus.IsRotH;
            Me.GetComponent<PlayerAvater>().IsRotChestV = AvaterMain.MotionStatus.IsRotV;

            _vecter = new Vector3(AvaterMain.MotionStatus.camX,
            AvaterMain.MotionStatus.camY, AvaterMain.MotionStatus.camZ);
        }
        public bool slashRoot(ActionStatus actionStatus)
        {
            //Me.GetComponent<Animator>().applyRootMotion = true;

            //Gun.Swing(AvaterMain.anim_flag, (int)Convert.ToDouble(actionStatus.Vector3.x), actionStatus.Vector3.y);
            Gun.Swing(Gun.MainWeaponBasic);

            return true;
        }
        public void After_slashRoot(ActionStatus actionStatus)
        {

        }
        public void Before_frontflip(ActionStatus AS)
        {
            //Rig.AddRelativeForce(Vector3.up*5+Vector3.back * 5f, ForceMode.VelocityChange);
        }
        public bool frontflip(ActionStatus AS)
        {
            Rig.velocity = Rig.transform.TransformVector(Vector3.up * 4 + Vector3.back);
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

            PA = Me.GetComponent<PlayerAvater>();
            g = Me.GetComponent<PlayerAvater>().myguns;
            //var ms = PA.MotionStatus;
            //PA.chestOffSet = new Vector3(ms.camX, ms.camY, ms.camZ);
            Me.GetComponent<PlayerAvater>().ChangeRotOffSet("idle");

            //PA.IsRotChestH = true;
            //PA.IsRotChestV = true;
        }

        public override bool move(ActionStatus actionStatus)
        {
            var camPos = Camera.transform.TransformDirection(Vector3.back * InputManager.ws + Vector3.left * InputManager.ad);
            RotateTowardSlerp(Me.transform.position - camPos, PA.MotionStatus.motionSpd);
            var endspeed = Me.transform.TransformDirection(Vector3.forward*InputManager.maxWSAD).normalized * actionStatus.f1;
            Rig.velocity = Vector3.Lerp(Rig.velocity,endspeed,.1f);
            _vecter = Rig.velocity;
            return true;
        }
        public override void After_move(ActionStatus actionStatus)
        {
            //Me.GetComponent<PlayerAvater>().IsRotChestH = false;
            //Me.GetComponent<PlayerAvater>().IsRotChestV = false;
        }
        #endregion

        #region strafe
        public void Before_strafe(ActionStatus actionStatus)
        {
            //Gun.ChangeWeapon("AK47");
            Gun.MainWeaponBasic = Gun.ActiveWeapon(PA.myguns.ToString())[0];

            var ms = PA.MotionStatus;
            PA.chestOffSet = new Vector3(ms.camX, ms.camY, ms.camZ);
            //PA.ChangeRotOffSet("strafe");

            //PA.IsRotChest = true;
            PA.IsRotChestH = true;
            PA.IsRotChestV = true;
        }

        public bool strafe(ActionStatus actionStatus)
        {
            FPSLikeRigMovement(7f,10f);
            if(Input.GetButton("Fire1"))
            {
                return Gun.fire(Gun.MainWeaponBasic);
            }
            return true;
        }
        #endregion

        #region Pistolstrafe
        public void Before_Pistolstrafe(ActionStatus actionStatus)
        {
            Me.GetComponent<PlayerAvater>().ChangeCamLimit("none");
            //Gun.ChangeWeapon("AK47");
            Gun.MainWeaponBasic = Gun.ActiveWeapon(PA.myguns.ToString())[0];
            var ms = PA.MotionStatus;

            PA.chestOffSet = new Vector3(ms.camX, ms.camY, ms.camZ);

            PA.IsRotChest = true;
            //PA.IsRotChestH = true;
            //PA.IsRotChestV = true;
        }

        public bool Pistolstrafe(ActionStatus actionStatus)
        {
            FPSLikeRigMovement(7f, 10f);
            if (Input.GetButton("Fire1"))
            {
                return Gun.fire(Gun.MainWeaponBasic);
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
            //_vecter = Rig.velocity;
            //Animator.SetBool("action_can_fall", true);
        }
        #endregion

        #region jumpout
        public void Before_jumpout(ActionStatus AS)
        {
            //Animator.applyRootMotion = true;

            Me.GetComponent<PlayerAvater>().ChangeRotOffSet("Pistolsilde");
            Me.GetComponent<PlayerAvater>().IsRotChest = true;
            Me.GetComponent<PlayerAvater>().ChangeCamLimit("sidedodgeR");

            _velocity = Me.transform.TransformVector(Vector3.right * 20f);
            
        }

        public bool jumpout(ActionStatus AS)
        {
            _velocity = Vector3.Slerp(_velocity, Vector3.zero, Time.deltaTime);
            Rig.velocity = _velocity;
            if (Input.GetButton("Fire1"))
                Gun.fire(Gun.MainWeaponBasic);
            return true;
        }
        public void After_jumpout(ActionStatus AS)
        {
            //Me.GetComponent<PlayerAvater>().ChangeCamLimit("none");
        }

        public void Before_jumpoutL(ActionStatus AS)
        {
            //Animator.applyRootMotion = true;

            Me.GetComponent<PlayerAvater>().ChangeRotOffSet("PistolsildeL");
            Me.GetComponent<PlayerAvater>().IsRotChest = true;
            Me.GetComponent<PlayerAvater>().ChangeCamLimit("sidedodgeL");

            _velocity = Me.transform.TransformVector(Vector3.right * Animator.GetFloat("input_ad") * 20f);

        }

        public bool jumpoutL(ActionStatus AS)
        {
            _velocity = Vector3.Slerp(_velocity, Vector3.zero, Time.deltaTime);
            Rig.velocity = _velocity;
            if(Input.GetButton("Fire1"))
                Gun.fire(Gun.MainWeaponBasic);
            return true;
        }
        public void After_jumpoutL(ActionStatus AS)
        {
            //Me.GetComponent<PlayerAvater>().ChangeCamLimit("none");
        }

        public void Before_jumpoutF(ActionStatus AS)
        {
            _velocity = Me.transform.TransformVector(Vector3.forward* Animator.GetFloat("input_ws") * 20f);
        }

        public bool jumpoutF(ActionStatus AS)
        {
            _velocity = Vector3.Slerp(_velocity, Vector3.zero, Time.deltaTime);
            Rig.velocity = _velocity;
            if (PA.anim_flag  == 0)
            {
                if (Input.GetButton("Fire1"))
                    Gun.fire(Gun.MainWeaponBasic);
            }
            return true;
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
            Me.GetComponent<PlayerAvater>().ChangeRotOffSet("wallrun");
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
            _vecter = Quaternion.AngleAxis(angle,Vector3.up)*rot;

            Me.transform.rotation = Quaternion.LookRotation(_vecter);
        }

        public bool wallrun(ActionStatus actionStatus)
        {
            //給定速度
            Rig.velocity = _vecter.normalized*9+Vector3.up*2;//NowVector已經是正規化的向量了
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
            _vecter = q;
            //沿著Y軸轉90度    
            //Rig.velocity = _vecter*2+Vector3.up*10;//NowVector已經是正規化的向量了 
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
            //PA.IsRotChestH = true;
            //PA.IsRotChestV = true;
        }
        public bool falling(ActionStatus actionStatus)
        {
            //var camPos = Camera.transform.TransformDirection(Vector3.back * InputManager.ws + Vector3.left * InputManager.ad);
            //if (Input.anyKey)
            //{
            //    RotateTowardSlerp(Me.transform.position - camPos, 10f);
            //    Rig.AddRelativeForce(Vector3.forward * 2f);
            //}

            var camPos = Camera.transform.TransformDirection(Vector3.back);
            RotateTowardSlerp(Me.transform.position - camPos, 15f);
            Rig.AddRelativeForce(Vector3.forward * 2f);

            if (Input.GetButton("Fire1"))
            {
                return Gun.fire(Gun.MainWeaponBasic);
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
            Rig.AddForce(vec * .5f, ForceMode.VelocityChange);

            _velocity = Me.transform.TransformDirection(Vector3.forward * InputManager.maxWSAD)* actionStatus.f1;
            Me.GetComponent<PlayerAvater>().IsRotChestH = true;
            Me.GetComponent<PlayerAvater>().IsRotChestV = true;
        }
        public bool slide(ActionStatus actionStatus)
        {
            if (AvaterMain.moveflag == 1)
            {
                //var camPos = Camera.transform.TransformDirection(Vector3.back * InputManager.ws + Vector3.left * InputManager.ad);
                //RotateTowardSlerp(Me.transform.position - camPos, 5f);

                _velocity = Vector3.Lerp(_velocity, Vector3.zero, 1.125f*Time.deltaTime);
                Rig.velocity = _velocity;

                Gun.fire(0);
            }
            return true;
        }
        #endregion

        #region Dash

        public void Before_dash(ActionStatus actionStatus)
        {

            _velocity = Camera.transform.TransformDirection(Vector3.forward * Animator.GetFloat("input_ws") + Vector3.right * Animator.GetFloat("input_ad"));
            //得到攝影機的Z軸轉動，並轉動向量
            _velocity = Vector3.ProjectOnPlane(_velocity, Vector3.up);
            //Debug.Log(" DashVector =  "+_velocity);
            _velocity = Vector3.ClampMagnitude(_velocity * 20, 20f);
            Me.GetComponent<PlayerAvater>().ChangeRotOffSet("dash");
            Me.GetComponent<PlayerAvater>().IsRotChest = true;
        }
        public bool dash(ActionStatus actionStatus)
        {
            _velocity = Vector3.Slerp(_velocity, Vector3.zero, Time.deltaTime);
            Rig.velocity = _velocity;
            //FPSLikeRigMovement(100f, 15f, 10f);
            return true;
        }
        #endregion

        #region 4WayDash

        public void Before_fourWayDash(ActionStatus actionStatus)
        {
            Me.GetComponent<PlayerAvater>().ChangeRotOffSet("dash4way");
            Me.GetComponent<PlayerAvater>().IsRotChestH = false;

            _timer = .3f;
        }

        public bool fourWayDash(ActionStatus actionStatus)
        {
            var camPos = Camera.transform.TransformDirection(Vector3.back * InputManager.ws + Vector3.left * InputManager.ad);
            RotateTowardSlerp(Me.transform.position - camPos, 4f);
            var fwd = Me.transform.TransformVector(Vector3.forward * 10f);

            if (AvaterMain.anim_flag == 1)
            {
                _timer += Time.deltaTime;
                Rig.velocity = (fwd / _timer * _timer);
            }
            return true;
        }

        public void After_fourWayDash(ActionStatus actionStatus)
        {

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
            Gun.reload(Gun.MainWeaponBasic);
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
            _velocity = EdgeOffSet;
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

            Me.transform.position = Vector3.Lerp(Me.transform.position, _velocity, 5f * Time.deltaTime);

            return true;
        }
        public void After_climb(ActionStatus AS)
        {
            var hit = Me.GetComponent<ParkourCollision>().hit;
            //Me.transform.position = new Vector3(Me.transform.position.x, hit.point.y, Me.transform.position.z);
            Me.transform.position = hit.point;
            Me.GetComponent<Animator>().SetBool("avater_IsLanded", true);
            //Debug.Log(hit.point);
        }

        #endregion
    }
}
