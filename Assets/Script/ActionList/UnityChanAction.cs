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
    class UnityChanAction : PlayerActionScript
    {
        private float _timer;
        private Vector3 _velocity;
        private Vector3 _climbVector;
        PlayerAvater PA;
        PlayerAvater.Guns g;
        string lastWeapon;
        string lastAction;

        public void Before_equip(ActionStatus actionStatus)
        {
            AvaterMain.anim_flag = 0;
            
        }
        public bool equip(ActionStatus actionStatus)
        {
            if(Animator.GetBool("avater_IsLanded"))
                FPSLikeRigMovement(3f, 10f);

            if (AvaterMain.anim_flag == 1)
            {
                PA.ChangeWeapon(InputManager.weaponSlot);
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
            //Gun.SpecialWeaponBasic = PA.weaponSlotList[100];
            PA.maxSpd = 10f;
            PA.minSpd = 0;
        }
        public bool Mstrafe(ActionStatus actionStatus)
        {
            //Me.GetComponent<PlayerAvater>().IsRotChestV = true;
            //var camPos = Camera.transform.TransformDirection(Vector3.forward);
            //Vector3 direction = (Me.transform.position + camPos - Me.transform.position).normalized;
            //Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3

            //Me.GetComponent<Animator>().rootRotation = lookRotation;

            //Gun.Swing(AvaterMain.anim_flag, 10, 10);

            FPSLikeRigMovement(10f, 10f);
            _velocity = Rig.velocity;

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
            PA.weaponSlotList[1].weapon.GetComponent<MeleeClass>().IsBlocking = true;
            PA.weaponSlotList[1].weapon.GetComponent<Collider>().enabled = true;

            //Gun.MainWeaponBasic.weapon.GetComponent<MeleeClass>().IsBlocking = true;
            PA.weaponSlotList[PA.weaponSlotNumber].weapon.GetComponent<Collider>().enabled = true;
        }
        public bool Block(ActionStatus actionStatus)
        {
            FPSLikeRigMovement(7f, 10f);

            return true;
        }
        public void After_Block(ActionStatus actionStatus)
        {
            //Me.GetComponent<Animator>().applyRootMotion = false;
            PA.weaponSlotList[1].weapon.GetComponent<MeleeClass>().IsBlocking = false;
            PA.weaponSlotList[1].weapon.GetComponent<Collider>().enabled = false;
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

        #region slash
        public override void Before_slash(ActionStatus actionStatus)
        {
            //Gun.InactiveWeapon(lastWeapon);
            //Rig.velocity = Vector3.zero;
            Me.GetComponent<PlayerAvater>().myguns = PlayerAvater.Guns.Great_Sword;
            //var g = Me.GetComponent<PlayerAvater>().myguns;
            //Gun.ChangeWeapon(g.ToString());

            //紀錄上個武器確保能被換掉
            lastWeapon = Me.GetComponent<PlayerAvater>().myguns.ToString();

            //Gun.MainWeaponBasic = Gun.ActiveWeapon("Great_Sword")[0];
            //Gun.MainWeaponBasic = PA.weaponSlotList[1];
            //Gun.MainWeaponBasic.weapon.SetActive(true);
            //PA.weaponSlotList[1].weapon.SetActive(true);

            PA.ChangeWeapon(1);

            //Me.GetComponent<PlayerAvater>().WeaponSlotNumber = 1;
            //Animator.SetInteger("avater_weaponslot", 1);
            Me.GetComponent<Animator>().applyRootMotion = true;

            Me.GetComponent<PlayerAvater>().ChangeRotOffSet("slash");
            Me.GetComponent<PlayerAvater>().IsRotChestH = AvaterMain.MotionStatus.IsRotH;
            Me.GetComponent<PlayerAvater>().IsRotChestV = AvaterMain.MotionStatus.IsRotV;

            _velocity = Camera.transform.TransformVector(AvaterMain.MotionStatus.camX,
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

                _velocity = Vector3.Slerp(_velocity, Vector3.zero, Time.deltaTime);
                //Rig.AddRelativeForce(_vecter, ForceMode.VelocityChange);
                Rig.velocity = _velocity;
            }
            else
            {
                Animator.SetBool("avater_can_dodge", true);
            }
            //Gun.Swing(AvaterMain.anim_flag,(int)Convert.ToDouble(actionStatus.Vector3.x),actionStatus.Vector3.y);
            Gun.Swing(PA.weaponSlotList[PA.weaponSlotNumber]);

            return true;
        }
        public void After_slash(ActionStatus actionStatus)
        {
            //Me.GetComponent<Animator>().applyRootMotion = false;
        }


        public void Before_slashRoot(ActionStatus actionStatus)
        {
            //Gun.InactiveWeapon(lastWeapon);

            //Me.GetComponent<PlayerAvater>().myguns = PlayerAvater.Guns.Great_Sword;
            //紀錄上個武器確保能被換掉
            //lastWeapon = Me.GetComponent<PlayerAvater>().myguns.ToString();
            //Gun.ChangeWeapon(g.ToString());
            //Me.GetComponent<PlayerAvater>().WeaponSlotNumber = 1;
            //Gun.MainWeaponBasic = Gun.ActiveWeapon("Great_Sword")[0];
            //Gun.MainWeaponBasic = PA.weaponSlotList[1];
            //Gun.MainWeaponBasic.weapon.SetActive(true);

            //PA.weaponSlotList[1].weapon.SetActive(true);
            //Debug.Log(PA.weaponSlotList[1].weapon.name);
            PA.ChangeWeapon(1);
            Animator.SetInteger("avater_weaponslot", 1);

            Rig.velocity = Vector3.zero;
            Me.GetComponent<Animator>().applyRootMotion = true;

            Me.GetComponent<PlayerAvater>().IsRotChestH = AvaterMain.MotionStatus.IsRotH;
            Me.GetComponent<PlayerAvater>().IsRotChestV = AvaterMain.MotionStatus.IsRotV;
        }
        public bool slashRoot(ActionStatus actionStatus)
        {
            //Me.GetComponent<Animator>().applyRootMotion = true;

            //Gun.Swing(AvaterMain.anim_flag, (int)Convert.ToDouble(actionStatus.Vector3.x), actionStatus.Vector3.y);
            Gun.Swing(PA.weaponSlotList[PA.weaponSlotNumber]);

            return true;
        }
        public void After_slashRoot(ActionStatus actionStatus)
        {

        }

        public void Before_slashAir(ActionStatus actionStatus)
        {
            //Gun.InactiveWeapon(lastWeapon);
            //Me.GetComponent<PlayerAvater>().myguns = PlayerAvater.Guns.Great_Sword;

            //紀錄上個武器確保能被換掉
            //lastWeapon = Me.GetComponent<PlayerAvater>().myguns.ToString();

            //Gun.MainWeaponBasic = Gun.ActiveWeapon("Great_Sword")[0];

            PA.ChangeWeapon(1);

            //Me.GetComponent<PlayerAvater>().weaponSlotNumber = 1;
            //Animator.SetInteger("avater_weaponslot", 1);
            //Me.GetComponent<Animator>().applyRootMotion = true;

            Me.GetComponent<PlayerAvater>().ChangeRotOffSet("slash");
            Me.GetComponent<PlayerAvater>().IsRotChestH = AvaterMain.MotionStatus.IsRotH;
            Me.GetComponent<PlayerAvater>().IsRotChestV = AvaterMain.MotionStatus.IsRotV;

            _velocity = Rig.velocity.magnitude * Me.transform.forward;

            //抓字串判斷是否無敵
            if (PA.MotionStatus.String.Contains("God"))
            {
                var cols = Me.GetComponents<Collider>();
                foreach (var col in cols) col.enabled = false;
                _velocity = _velocity + Vector3.up * .15f;
            }

            //_velocity = Camera.transform.TransformVector(AvaterMain.MotionStatus.camX,
            //AvaterMain.MotionStatus.camY, AvaterMain.MotionStatus.camZ);
            //_velocity = Vector3.ClampMagnitude(_velocity, 20f);
        }
        public bool slashAir(ActionStatus actionStatus)
        {
            if (AvaterMain.anim_flag > 0)
            {
                Animator.SetBool("avater_can_dodge", false);

                var camPos = Camera.transform.TransformDirection(Vector3.forward);
                RotateTowardSlerp(Me.transform.position + camPos, 3f);
                //Me.GetComponent<Animator>().applyRootMotion = false;

            }
            else
            {
                //Animator.SetBool("avater_can_dodge", true);
            }
            _velocity = Vector3.Slerp(_velocity, Vector3.zero, Time.deltaTime);
            Rig.velocity = _velocity + Rig.velocity.y * Vector3.up;

            Gun.Swing(PA.weaponSlotList[PA.weaponSlotNumber]);
            return true;
        }
        public void After_slashAir(ActionStatus AS)
        {
            if (PA.MotionStatus.String.Contains("God"))
            {
                var cols = Me.GetComponents<Collider>();
                foreach (var col in cols)
                {                    
                    col.enabled = true;
                } 
            }
        }

        public void Before_slashRootAir(ActionStatus actionStatus)
        {
            //Gun.InactiveWeapon(lastWeapon);

            //Me.GetComponent<PlayerAvater>().myguns = PlayerAvater.Guns.Great_Sword;
            //紀錄上個武器確保能被換掉
            //lastWeapon = Me.GetComponent<PlayerAvater>().myguns.ToString();
            //Me.GetComponent<PlayerAvater>().weaponSlotNumber = 1;
            //Gun.MainWeaponBasic = Gun.ActiveWeapon("Great_Sword")[0];
            //Animator.SetInteger("avater_weaponslot", 1);
            PA.ChangeWeapon(1);

            Rig.velocity = Vector3.zero;
            Me.GetComponent<Animator>().applyRootMotion = true;

            Me.GetComponent<PlayerAvater>().IsRotChestH = AvaterMain.MotionStatus.IsRotH;
            Me.GetComponent<PlayerAvater>().IsRotChestV = AvaterMain.MotionStatus.IsRotV;
        }
        public bool slashRootAir(ActionStatus actionStatus)
        {
            Gun.Swing(PA.weaponSlotList[PA.weaponSlotNumber]);
            return true;
        }
        #endregion

        public void Before_backJump(ActionStatus AS)
        {
            //Gun.InactiveWeapon(lastWeapon);

            //Me.GetComponent<PlayerAvater>().myguns = PlayerAvater.Guns.Handgun;
            //紀錄上個武器確保能被換掉
            //lastWeapon = Me.GetComponent<PlayerAvater>().myguns.ToString();
            //Gun.ChangeWeapon(g.ToString());
            //Gun.MainWeaponBasic = Gun.ActiveWeapon("Handgun")[0];
            //Animator.SetInteger("avater_weaponslot", 2);

            PA.ChangeWeapon(2);
            //得到攝影機的Z軸轉動，並轉動向量
            _velocity = Vector3.ProjectOnPlane(Me.transform.TransformVector(Vector3.back), Vector3.up).normalized*20f;
            //保持人物轉動放在計算動量之後
            Vector3 direction = _velocity;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(-direction.x, 0, -direction.z));    // flattens the vector3
            Me.transform.rotation = lookRotation;

            //換手槍
        }
        public bool backJump(ActionStatus AS)
        {
            if (PA.moveflag == 1)
            {
                _velocity = Vector3.Slerp(_velocity, Vector3.zero, Time.deltaTime);
                Rig.velocity = _velocity;
            }
            return true;
        }
        public void Before_frontflip(ActionStatus AS)
        {
            //Rig.AddRelativeForce(Vector3.up*5+Vector3.back * 5f, ForceMode.VelocityChange);
        }
        public bool frontflip(ActionStatus AS)
        {
            Rig.velocity = Rig.transform.TransformVector(Vector3.up * 4 + Vector3.back*.2f);
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
            PA = Me.GetComponent<PlayerAvater>();
            g = Me.GetComponent<PlayerAvater>().myguns;
            //var ms = PA.MotionStatus;
            //PA.chestOffSet = new Vector3(ms.camX, ms.camY, ms.camZ);
            
            Me.GetComponent<PlayerAvater>().ChangeRotOffSet("idle");
            PA.maxSpd = 10f;
            PA.minSpd = 0f;
            //PA.IsRotChestH = true;
            //PA.IsRotChestV = true;
        }

        public override bool move(ActionStatus actionStatus)
        {
            Vector3 dir = Camera.transform.TransformDirection(Vector3.forward * InputManager.ws * 3 + Vector3.right * InputManager.ad * 3);
            Vector3 afterProject = Vector3.ProjectOnPlane(dir, Vector3.up).normalized;
            _velocity = Vector3.ProjectOnPlane(_velocity, Vector3.up);
            if (InputManager.maxWSAD > 0)
            {
                Vector3 afterLerp = Vector3.Slerp(Me.transform.forward, afterProject, Time.deltaTime*7f);
                Me.transform.rotation = Quaternion.LookRotation(afterLerp, Vector3.up);

                if (_velocity.magnitude < 6f)//最小速度
                    _velocity = afterProject * 6f;//拿的是地面的投影
                else
                    _velocity = Vector3.Lerp(_velocity, afterProject * 10f, Time.deltaTime*2.5f/*加速度*/);//最大速度

                Rig.velocity = _velocity;
            }
            else
            {
                Rig.velocity = Vector3.zero;
                _timer = 0;
            }

            return true;
        }
        public override void After_move(ActionStatus actionStatus)
        {
            //Me.GetComponent<PlayerAvater>().IsRotChestH = false;
            //Me.GetComponent<PlayerAvater>().IsRotChestV = false;
        }
        #endregion

        #region jump
        public void Before_jump(ActionStatus actionStatus)
        {
            _timer = .3f;
        }
        public bool jump(ActionStatus actionStatus)
        {
            var camPos = Camera.transform.TransformDirection(Vector3.back * InputManager.ws + Vector3.left * InputManager.ad);
            RotateTowardSlerp(Me.transform.position - camPos, 4f);
            var fwd = Me.transform.TransformVector(Vector3.forward * 5f);

            if (AvaterMain.anim_flag == 1)
            {
                _timer += Time.deltaTime;
                _velocity = (Vector3.up * 1f / (_timer * _timer) + fwd / _timer * _timer);
            }
            else
                _velocity = fwd / _timer * _timer;
            Rig.velocity = _velocity;
            return true;
        }
        public void After_jump(ActionStatus actionStatus)
        {
            //_vecter = Rig.velocity;
            //Animator.SetBool("action_can_fall", true);
        }
        #endregion

        
        #region strafe
        public void Before_strafe(ActionStatus actionStatus)
        {
            //Gun.ChangeWeapon("AK47");
            //Gun.MainWeaponBasic = Gun.ActiveWeapon(PA.myguns.ToString())[0];

            var ms = PA.MotionStatus;
            PA.chestOffSet = new Vector3(ms.camX, ms.camY, ms.camZ);
            //PA.ChangeRotOffSet("strafe");
            PA.maxSpd = 10f;
            PA.minSpd = 0f;
            //PA.IsRotChest = true;
            PA.IsRotChestH = true;
            PA.IsRotChestV = true;
        }

        public bool strafe(ActionStatus actionStatus)
        {
            _velocity =  FPSLikeRigMovement(10f,10f);
            if(Input.GetButton("Fire1"))
            {
                return Gun.fire(PA.weaponSlotList[PA.weaponSlotNumber]);
            }
            return true;
        }
        #endregion

        #region Pistolstrafe
        public void Before_Pistolstrafe(ActionStatus actionStatus)
        {
            //PA.weaponSlotList[2].weapon.SetActive(true);
            PA.ChangeWeapon(2);
            Me.GetComponent<PlayerAvater>().ChangeCamLimit("none");
            //Gun.MainWeaponBasic = Gun.ActiveWeapon(PA.myguns.ToString())[0];
            var ms = PA.MotionStatus;

            PA.chestOffSet = new Vector3(ms.camX, ms.camY, ms.camZ);
            PA.chestMaxRot = 60f;
            PA.IsRotChest = true;
            PA.maxSpd = 10f;
            PA.minSpd = 0f;
            //PA.IsRotChestH = true;
            //PA.IsRotChestV = true;
        }

        public bool Pistolstrafe(ActionStatus actionStatus)
        {
            FPSLikeRigMovement(10f, 14f);
            if (Input.GetButton("Fire1"))
            {
                return Gun.fire(PA.weaponSlotList[PA.weaponSlotNumber]);
            }
            return true;
        }
        #endregion

        public void Before_dodge(ActionStatus AS)
        {
            var dir =
             Camera.transform.TransformDirection(Vector3.right * InputManager.ad + Vector3.forward * InputManager.ws);
            _velocity = Vector3.ProjectOnPlane(dir, Vector3.up);
            //獲得要迴避的方向
            _velocity = _velocity * 30f + Vector3.up*1f;
        }
        public bool dodge(ActionStatus AS)
        {
            _velocity = Vector3.Slerp(_velocity, Vector3.zero, Time.deltaTime*2f);
            Rig.velocity = _velocity;
            return true;
        }
        #region jumpout
        public void Before_jumpout(ActionStatus AS)
        {
            //Animator.applyRootMotion = true;
            //轉動玩家面向
            //var camPos = Camera.transform.TransformDirection(Vector3.back);
            //RotateTowardSlerp(Me.transform.position - Camera.transform.TransformDirection(Vector3.back), 60f);

            _velocity = Camera.transform.TransformDirection(Vector3.right * PA.MotionStatus.camX + Vector3.forward * PA.MotionStatus.camZ);
            //得到攝影機的Z軸轉動，並轉動向量
            _velocity = Vector3.ProjectOnPlane(_velocity, Vector3.up).normalized;
            _velocity = Vector3.ClampMagnitude(_velocity * 40, 50f);
            //保持人物轉動放在計算動量之後
            Vector3 direction = (Camera.transform.TransformDirection(Vector3.forward));
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3
            Me.transform.rotation = lookRotation;

            if (!String.IsNullOrEmpty(PA.MotionStatus.String))
            {
                //Me.GetComponent<PlayerAvater>().ChangeCamLimit(PA.MotionStatus.String);
                Me.GetComponent<PlayerAvater>().ChangeRotOffSet(PA.MotionStatus.String);
                Me.GetComponent<PlayerAvater>().IsRotChest = true;
            }

        }

        public bool jumpout(ActionStatus AS)
        {

            //只有前跳在翻滾時不能射擊.其他動作沒有旗標
            if (PA.moveflag == 1)
            {
                /*
                if (Input.GetButton("Fire1"))
                    Gun.fire(Gun.MainWeaponBasic);
                */
                _velocity = Vector3.Slerp(_velocity, Vector3.zero, Time.deltaTime*2f);
                Rig.velocity = _velocity;
            }
            return true;
        }

        public void After_jumpout(ActionStatus AS)
        {
        }

        public void Before_leanGround(ActionStatus AS)
        {
            //Debug.Log(Animator.GetFloat("avater_yspeed"));
            if (PA.MotionStatus.String.StartsWith("GunHand"))
            {
                //Me.GetComponent<PlayerAvater>().ChangeCamLimit(PA.MotionStatus.String);
                //Me.GetComponent<PlayerAvater>().ChangeRotOffSet(PA.MotionStatus.String);
                Me.GetComponent<PlayerAvater>().IsRotGunHand = true;
            }
            else if (!String.IsNullOrEmpty(PA.MotionStatus.String))
            {
                //Me.GetComponent<PlayerAvater>().ChangeCamLimit(PA.MotionStatus.String);
                Me.GetComponent<PlayerAvater>().ChangeRotOffSet(PA.MotionStatus.String);
                Me.GetComponent<PlayerAvater>().IsRotChest = true;
            }
            _velocity.y = Rig.velocity.y;
        }

        public bool leanGround(ActionStatus AS)
        {
            if (Input.GetButton("Fire1"))
                Gun.fire(PA.weaponSlotList[PA.weaponSlotNumber]);
            _velocity = Vector3.Slerp(_velocity, Vector3.zero, Time.deltaTime);

            Rig.velocity = _velocity+Rig.velocity.y*Vector3.up;
            return true;
        }

        public void After_leanGround(ActionStatus AS)
        {
        }

        #endregion

        #region shooterJump
        public void Before_shooterJump(ActionStatus actionStatus)
        {
            //_velocity = Camera.transform.TransformDirection(Vector3.right * InputManager.ad + Vector3.forward * InputManager.ws);
            //得到攝影機的Z軸轉動，並轉動向量
            _velocity = Vector3.ProjectOnPlane(_velocity, Vector3.up);
            _velocity = Vector3.ClampMagnitude(_velocity + Vector3.up * 10, 30f);
            //_timer = .3f;
        }
        public bool shooterJump(ActionStatus actionStatus)
        {
            //var camPos = Camera.transform.TransformDirection(Vector3.back * InputManager.ws + Vector3.left * InputManager.ad);
            //RotateTowardSlerp(Me.transform.position - camPos, 4f);
            FPSLikeRigMovement(10f, 10f);
            //var fwd = Me.transform.TransformVector(Vector3.right * InputManager.ad*5f + Vector3.forward * InputManager.ws * 5f);

            if (AvaterMain.anim_flag == 1)
            {
                //_timer += Time.deltaTime;
                //Rig.velocity = ( _velocity / _timer * _timer);
                Rig.velocity = Vector3.Slerp(_velocity, Vector3.zero, 1 / (120 * Time.deltaTime));
            }
            /*
            else
                Rig.velocity = _velocity / _timer * _timer;
            */
            return true;
        }
        public void After_shooterJump(ActionStatus actionStatus)
        {
            //_vecter = Rig.velocity;
            //Animator.SetBool("action_can_fall", true);
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

            if (!String.IsNullOrEmpty(PA.MotionStatus.String))
            {
                Me.GetComponent<PlayerAvater>().IsRotChest = true;
                Me.GetComponent<PlayerAvater>().ChangeRotOffSet(PA.MotionStatus.String);
                //Me.GetComponent<PlayerAvater>().ChangeCamLimit(PA.MotionStatus.String);
            }

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
            _velocity = Quaternion.AngleAxis(angle,Vector3.up)*rot;

            Me.transform.rotation = Quaternion.LookRotation(_velocity);
        }

        public bool wallrun(ActionStatus actionStatus)
        {
            //給定速度
            Rig.velocity = _velocity.normalized*9+Vector3.up*2;//NowVector已經是正規化的向量了
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

            if (Input.GetButton("Fire1"))
            {
                Gun.fire(PA.weaponSlotList[PA.weaponSlotNumber]);
            }

            if (!Physics.CheckSphere(Me.transform.TransformPoint(.3f*pos,1,0),.7f,LayerMask.GetMask("Parkour"),QueryTriggerInteraction.Ignore))
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

            Rig.AddRelativeForce(Me.transform.TransformVector(Vector3.right*pos)*3,ForceMode.VelocityChange);
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
            _velocity = q;
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

            if (InputManager.maxWSAD > 0)
            {
                Vector3 dir = Camera.transform.TransformDirection(Vector3.forward * InputManager.ws*3 + Vector3.right * InputManager.ad*3);
                Vector3 afterProject = Vector3.ProjectOnPlane(dir, Vector3.up).normalized;
                Vector3 afterLerp = Vector3.Slerp(Me.transform.forward, afterProject, Time.deltaTime * 7f);
                Me.transform.rotation = Quaternion.LookRotation(afterLerp, Vector3.up);

                Rig.AddRelativeForce(Vector3.forward * 2f);
            }
            else
            {
                _timer = 0;
            }
            _velocity = Rig.velocity;
            return true;
        }
        public void After_falling(ActionStatus actionStatus)
        { 
        }
        #endregion

        #region falling
        public void Before_strafeFalling(ActionStatus actionStatus)
        {
            //PA.IsRotChestH = true;
            //PA.IsRotChestV = true;
        }
        public bool strafeFalling(ActionStatus actionStatus)
        {
            //var camPos = Camera.transform.TransformDirection(Vector3.back * InputManager.ws + Vector3.left * InputManager.ad);
            //if (Input.anyKey)
            //{
            //    RotateTowardSlerp(Me.transform.position - camPos, 10f);
            //    Rig.AddRelativeForce(Vector3.forward * 2f);
            //}
            FPSLikeRigMovement(3f, 5f);

            if (Input.GetButton("Fire1"))
            {
                Gun.fire(PA.weaponSlotList[PA.weaponSlotNumber]);
            }
            _velocity = Rig.velocity;
            return true;
        }
        public void After_strafeFalling(ActionStatus actionStatus)
        {
        }
        #endregion

        #region land
        public void Before_softland(ActionStatus actionStatus)
        {
            Animator.applyRootMotion = true;
        }

        public bool softland(ActionStatus actionStatus)
        {
            return true;
        }

        public void Before_hardland(ActionStatus actionStatus)
        {
            //得到向量長度
            var spd = Rig.velocity.magnitude*2f;
            //Debug.Log(spd);
            if (spd < 7f)
                spd = 7f;

            _velocity = Vector3.ProjectOnPlane(_velocity, Vector3.up);
            //防止輸入過小.
            var dir = Me.transform.forward * InputManager.ws + Me.transform.right * InputManager.ad;
            if (dir.magnitude < 1)
                dir = Me.transform.forward;
            _velocity = Vector3.ProjectOnPlane(dir.normalized * spd, Vector3.up);
            //Debug.Log(dir.normalized * spd);

            //避免向量過大
            _velocity = Vector3.ClampMagnitude(_velocity, 20f);
            //保持人物轉動放在計算動量之後
            Vector3 direction = _velocity;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3
            Me.transform.rotation = lookRotation;
            //目標 ->避免腳色山羊腿
            //問題 ->定速率時可以避免物理問題(摩擦力等)，但卻忽略斜坡速率問題
            //方針 ->投影至目標斜率.並加入最高.低斜率
        }

        public bool hardland(ActionStatus actionStatus)
        {
            float ySpeed = Rig.velocity.y;

            if (PA.moveflag == 1)
            {

                //FPSLikeRigMovement(100f, 15f, 10f);
            }
            _velocity = Vector3.Slerp(_velocity, Vector3.up * ySpeed, Time.deltaTime);
            Rig.velocity = _velocity;
            return true;
        }
        #endregion

        #region slide
        public void Before_slide(ActionStatus actionStatus)
        {
            
            var vec = Me.transform.TransformDirection(Vector3.forward);
            vec.y = 0;
            Rig.AddForce(vec * 7f, ForceMode.VelocityChange);

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
            }
            return true;
        }
        public void After_slide(ActionStatus actionStatus)
        {
            Animator.SetBool("input_crouch", false);
        }
        #endregion

        #region Dash

        public void Before_dash(ActionStatus actionStatus)
        {

            //_velocity = Camera.transform.TransformDirection(Vector3.forward * Animator.GetFloat("input_ws") + Vector3.right * Animator.GetFloat("input_ad"));
            //得到攝影機的Z軸轉動，並轉動向量
            _velocity = Vector3.ProjectOnPlane(_velocity, Vector3.up);
            //避免向量過小
            if (_velocity.magnitude < 5f)
            {
                Vector3 minSpd = Me.transform.forward * InputManager.ws + Me.transform.right* InputManager.ad;
                _velocity = Vector3.ProjectOnPlane(minSpd.normalized * 6f, Vector3.up);
            }
            _velocity = _velocity + Vector3.up*_velocity.magnitude/2;
            //避免向量過大
            _velocity = Vector3.ClampMagnitude(_velocity, 20f);
            //保持人物轉動放在計算動量之後
            Vector3 direction = _velocity;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3
            Me.transform.rotation = lookRotation;

            //_velocity = Camera.transform.TransformDirection(Vector3.forward * Animator.GetFloat("input_ws") + Vector3.right * Animator.GetFloat("input_ad"));
            ////得到攝影機的Z軸轉動，並轉動向量
            //_velocity = Vector3.ProjectOnPlane(_velocity, Vector3.up);
            ////Debug.Log(" DashVector =  "+_velocity);
            //_velocity = Vector3.ClampMagnitude(_velocity * 20, 20f);
            //Me.GetComponent<PlayerAvater>().ChangeRotOffSet("dash");
            //Me.GetComponent<PlayerAvater>().IsRotChest = true;
        }
        public bool dash(ActionStatus actionStatus)
        {
            if (PA.moveflag == 1)
            {
                _velocity = Vector3.Slerp(_velocity, Vector3.zero, Time.deltaTime);
                Rig.velocity = _velocity;
                //FPSLikeRigMovement(100f, 15f, 10f);
            }
            return true;
        }
        #endregion

        #region 4WayDash

        public void Before_fourWayDash(ActionStatus actionStatus)
        {
            //Me.GetComponent<PlayerAvater>().ChangeRotOffSet("dash4way");
            //Me.GetComponent<PlayerAvater>().IsRotChestH = false;

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

            if (PA.anim_flag == 1)
            {
                var wpn = PA.weaponSlotList[PA.weaponSlotNumber];
                if (wpn.reloadtype == "single")
                {
                    Gun.reload_shotgun(wpn);
                    PA.anim_flag = 0;
                    //如果子彈還沒滿就繼續換
                    if (wpn.BulletInMag < wpn.MagSize && wpn.ammotype.NowAmmo != 0)
                        return false;
                    return true;
                }
                else
                {
                    Gun.reload(PA.weaponSlotList[PA.weaponSlotNumber]);
                }
                return true;
            }
            return false;
        }

        public bool reload_strafe(ActionStatus actionStatus)
        {
            FPSLikeRigMovement(12f, 15f);
            if (PA.anim_flag == 1)
            {
                var wpn = PA.weaponSlotList[PA.weaponSlotNumber];
                if (wpn.reloadtype == "single")
                {
                    Gun.reload_shotgun(wpn);
                    PA.anim_flag = 0;
                    //如果子彈還沒滿就繼續換
                    if (wpn.BulletInMag < wpn.MagSize && wpn.ammotype.NowAmmo != 0)
                        return false;
                    return true;
                }
                else
                {
                    Gun.reload(PA.weaponSlotList[PA.weaponSlotNumber]);
                }
                return true;
            }
            return false;
        }
        #endregion

        #region climb

        public void Before_climb(ActionStatus AS)
        {
            Me.GetComponent<Animator>().applyRootMotion = true;

            var hit = Me.GetComponent<ParkourCollision>().hit;
            var toPoint = Me.transform.TransformVector(hit.point).normalized;
            var EdgeOffSet = Rig.position + toPoint * .3f;
            EdgeOffSet.y = hit.point.y - .3f;
            //Debug.Log("hitPos: " + hit.point + " toPoint: " + toPoint + " EdgeOffSet " + EdgeOffSet);
            _climbVector = EdgeOffSet;
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
            //var hit = Me.GetComponent<ParkourCollision>().hit;
            //Me.transform.position = Vector3.Lerp(Me.transform.position, _climbVector, 5f * Time.deltaTime);

            return true;
        }
        public void After_climb(ActionStatus AS)
        {
            var hit = Me.GetComponent<ParkourCollision>().hit;
            //Me.transform.position = new Vector3(Me.transform.position.x, hit.point.y, Me.transform.position.z);
            //Me.transform.position = hit.point;
            //Me.GetComponent<Animator>().SetBool("avater_IsLanded", true);
            //Debug.Log(hit.point);
        }

        #endregion
    }
}
