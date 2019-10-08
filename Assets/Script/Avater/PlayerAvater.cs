using UnityEngine;

using Assets.Script.ActionControl;
using Assets.Script.weapon;
using Assets.Script.StaticFunction;
using Assets.Script.Config;
using System.Collections.Generic;

namespace Assets.Script.Avater
{
    /// <summary>
    /// 此為核心元件，非必要時別動這邊
    /// </summary>
    public class PlayerAvater : AvaterMain
    {
        //武器欄
        //public Dictionary<int,string> PlayerWeaponDictionary;
        public PlayerStateMachine stateMachine { get; set; }
        public AvaterDataLoader avaterDataLoader = new AvaterDataLoader();
        private MotionStatusBuilder statusBuilder = new MotionStatusBuilder();
        public GameObject GroundCheck;
        public float CheckRadius;
        public string Type;
        public int NowAmmo;
        public int MaxAmmo;
        public int WeaponSlotNumber;
        public AllAmmoType ammoType = new AllAmmoType();
        public GameObject camera { get; set; }

        public bool IsRotGunHand = false;
        public bool IsRotChest = false;
        public bool IsRotChestV = false;
        public bool IsRotChestH = false;

        private List<chestValue> _chestValues = new List<chestValue>();
        private List<camValue> _camValues = new List<camValue>();

        public Transform Hip;
        public Transform chestTransform;
        public Vector3 chestOffSet;
        public float chestMaxRot;

        public Transform GunHandRoot;
        public Transform GunHand;

        public RaycastHit hit { get; set; }

        public enum Guns { Great_Sword,Cross_Sword, Handgun ,Shotgun ,AK47 , SMAW }
        public Guns myguns;

        public float slowMoDur;
        public float slowMoCool;
        public float timer = 0; 

        void Start()
        {
            #region 暫時初始化
            camera = gameObject.transform.Find("Camera").gameObject;
            avaterStatus = avaterDataLoader.LoadStatus("UnityChan");
            
            //暫時，初始化到時會交出去
            Init_Avater();

            stateMachine = Animator.GetBehaviour<PlayerStateMachine>();
            stateMachine.me = gameObject;
            stateMachine.action = ActionScript;
            stateMachine.AvaterMain = this;
            stateMachine.PlayerAvater = this;

            //獲取腳色動作值
            motionStatusDir = statusBuilder.GetMotionList("UnityChan");
            //GetAnimaterParameter();

            //ActionScript.ChangeTarget(GameObject.Find("CommandCube").transform.Find("Imp").gameObject);
            WeaponFactory weaponFactory = new WeaponFactory();
            var ammo = ammoType.GetAmmoType();
            weaponFactory.Init(ammo);

            Type = ammoType.Type;
            NowAmmo = ammoType.NowAmmo;
            MaxAmmo = ammoType.MaxAmmo;

            var GunDic = weaponFactory.AllWeaponDictionary;

            GetComponent<Gun>().SetPlayerAvater(this);
            GetComponent<Gun>().AddWeapon(GunDic["basicgun"]);
            GetComponent<Gun>().AddWeapon(GunDic["MG"]);
            GetComponent<Gun>().AddWeapon(GunDic["SMAW"]);
            GetComponent<Gun>().AddWeapon(GunDic["Great_Sword"]);
            GetComponent<Gun>().AddWeapon(GunDic["Cross_Sword"]);
            GetComponent<Gun>().AddWeapon(GunDic["AK47"]);
            GetComponent<Gun>().AddWeapon(GunDic["Handgun"]);
            GetComponent<Gun>().AddWeapon(GunDic["Wakizashi"]);
            GetComponent<Gun>().AddWeapon(GunDic["Shotgun"]);
            GetComponent<Gun>().AddWeapon(GunDic["kick"]);

            GetComponent<Gun>().CreateWeaponByList();
            GetComponent<Gun>().cam = gameObject.transform.Find("Camera").GetComponent<MouseOrbitImproved>();

            GetComponent<Gun>().LoadSingleWeapon("kick");
            GetComponent<Gun>().LoadSingleWeapon("AK47");
            GetComponent<Gun>().LoadSingleWeapon("Handgun");
            GetComponent<Gun>().LoadSingleWeapon("Shotgun");
            GetComponent<Gun>().LoadSingleWeapon("Great_Sword");
            GetComponent<Gun>().LoadSingleWeapon("SMAW");
            GetComponent<Gun>().InactiveAllWeapon();
            GetComponent<Gun>().ActiveWeapon("kick");
            #endregion

#region 上半身轉動，攝影機限制
            /// 新增上半身轉動方法:
            /// 1.先在StatusByAniName新增該動畫名稱，並填入String該動畫名
            /// 2.新增ChestValue類別，並加入列表
            /// 3.(選擇性)新增CamValue類別，並加入列表
            /// 4.如果以上失效...記得檢查ActionList的Before_Action是否呼叫ChestLookByName
            /// 5.如果 name 相同或是MotionStatus相同參數也會錯誤

            chestValue none = new chestValue { name = "none", maxDegress = 60, chestOffSet = new Vector3(0, 0, 0) };
            chestValue chestSlash = new chestValue { name = "slash", maxDegress = 60, chestOffSet = new Vector3(0, 0, 0) };
            chestValue chestPistol = new chestValue { name = "pistol", maxDegress = 60, chestOffSet = new Vector3(-40, 0, 0) };
            chestValue chestWallrun = new chestValue { name = "wallrun", maxDegress = 120, chestOffSet = new Vector3(0, 0, 0) };
            chestValue chestIdle = new chestValue { name = "idle", maxDegress = 60, chestOffSet = new Vector3(10,-8,0) };
            chestValue chestMStrafe = new chestValue { name = "strafe", maxDegress = 0, chestOffSet = new Vector3(10, 50, 0) };
            chestValue chestDash = new chestValue { name = "dash", maxDegress = 0, chestOffSet = new Vector3(20, 0, 0) };
            chestValue chestDash4way = new chestValue { name = "dash4way", maxDegress = 0, chestOffSet = new Vector3(0, 40, 0) };

            chestValue chestPistolsilde = new chestValue { name = "jumpOutMidR", maxDegress = 75, chestOffSet = new Vector3(40, 0, 0) };
            chestValue chestPistolsildeL = new chestValue { name = "jumpOutMidL", maxDegress = 75, chestOffSet = new Vector3(-30, 0, 0) };
            chestValue chestjumpout = new chestValue { name = "GunHandjumpOutF", maxDegress = 60, chestOffSet = new Vector3(30, 0, 0) };
            chestValue chestDualPistolSilde = new chestValue { name = "dualSideDodgeR", maxDegress = 60, chestOffSet = new Vector3(10, 0, 0) };
            chestValue chestDualPistolSildeL = new chestValue { name = "dualSideDodgeL", maxDegress = 60, chestOffSet = new Vector3(10, 0, 0) };
            chestValue chestDualPistolSildeF = new chestValue { name = "dualSideDodgeF", maxDegress = 60, chestOffSet = new Vector3(10, 0, 0) };
            chestValue chestDualslide = new chestValue { name = "dualslide", maxDegress = 60, chestOffSet = new Vector3(10, 0, 0) };
            chestValue chestjumpOutMidR_AR = new chestValue { name = "jumpOutMidR_AR", maxDegress = 60, chestOffSet = new Vector3(40, 0, 0) };
            chestValue chestjumpOutMidL_AR = new chestValue { name = "jumpOutMidL_AR", maxDegress = 60, chestOffSet = new Vector3(-30, 0, 0) };
            chestValue chestjumpOutAir = new chestValue { name = "jumpOutAir", maxDegress = 60, chestOffSet = new Vector3(40, 0, 0) };

            chestValue chestwallrunR = new chestValue { name = "wallrunR_P", maxDegress = 60, chestOffSet = new Vector3(40, 0, 0) };
            chestValue chestwallrunL = new chestValue { name = "wallrunL_P", maxDegress = 60, chestOffSet = new Vector3(-40, 0, 0) };
            chestValue chestwallrunR_AR = new chestValue { name = "wallrunR_AR", maxDegress = 60, chestOffSet = new Vector3(40, 0, 0) };
            chestValue chestwallrunL_AR = new chestValue { name = "wallrunL_AR", maxDegress = 60, chestOffSet = new Vector3(-30, 0, 0) };

            _chestValues.Add(none);
            _chestValues.Add(chestIdle);
            _chestValues.Add(chestMStrafe);
            _chestValues.Add(chestDash);
            _chestValues.Add(chestDash4way);
            _chestValues.Add(chestSlash);
            _chestValues.Add(chestPistol);
            _chestValues.Add(chestWallrun);
            _chestValues.Add(chestPistolsilde);
            _chestValues.Add(chestPistolsildeL);
            _chestValues.Add(chestjumpout);
            _chestValues.Add(chestDualPistolSilde);
            _chestValues.Add(chestDualPistolSildeL);
            _chestValues.Add(chestDualPistolSildeF);
            _chestValues.Add(chestDualslide);
            _chestValues.Add(chestwallrunR);
            _chestValues.Add(chestwallrunL);
            _chestValues.Add(chestwallrunR_AR);
            _chestValues.Add(chestwallrunL_AR);
            _chestValues.Add(chestjumpOutMidR_AR);
            _chestValues.Add(chestjumpOutMidL_AR);
            _chestValues.Add(chestjumpOutAir);

            #region 攝影機限制

            camValue camNormal = new camValue { name = "none",IsLimitX = false, Max_x = 360, Min_x = -360, Max_y = 80, Min_y = -70 };
            camValue camSidedodgeR = new camValue { name = "jumpOutMidR", IsLimitX = true, Max_x = 30, Min_x = -91, Max_y = 60, Min_y = -50 };
            camValue camSidedodgeL = new camValue { name = "jumpOutMidL", IsLimitX = true, Max_x = 91, Min_x = -30, Max_y = 60, Min_y = -50 };
            //camValue camSidedodgeR = new camValue { name = "sidedodgeR", IsLimitX = true, Max_x = 30, Min_x = -180, Max_y = 60, Min_y = -50 };
            //camValue camSidedodgeL = new camValue { name = "sidedodgeL", IsLimitX = true, Max_x = 180, Min_x = -30, Max_y = 60, Min_y = -50 };

            camValue camjumpOutAir = new camValue { name = "jumpOutAir", IsLimitX = true, Max_x = 30, Min_x = -30, Max_y = 15, Min_y = -15 };

            camValue camjumpOutMidF = new camValue { name = "GunHandjumpOutMidF", IsLimitX = true, Max_x = 11, Min_x = -61, Max_y = 15, Min_y = -15 };
            camValue camAerial_Evade = new camValue { name = "GunHandAerial_Evade", IsLimitX = true, Max_x = 30, Min_x = -15, Max_y = 45, Min_y = -15 };
            camValue camjumpOutStartB = new camValue { name = "jumpOutStartB", IsLimitX = true, Max_x = 15, Min_x = -15, Max_y = 15, Min_y = -15 };
            camValue camDualSidedodgeR = new camValue { name = "dualSideDodgeR", IsLimitX = true, Max_x = 10, Min_x = -100, Max_y = -10, Min_y = -50 };
            //camValue camDualSidedodgeR = new camValue { name = "dualSideDodgeR", IsLimitX = true, Max_x = 360, Min_x = -360, Max_y = 80, Min_y = -70 };

            camValue camDualSidedodgeL = new camValue { name = "dualSideDodgeL", IsLimitX = true, Max_x = 100, Min_x = -10, Max_y = -10, Min_y = -50 };
            camValue camDualSidedodgeF = new camValue { name = "dualSideDodgeF", IsLimitX = true, Max_x = 10, Min_x = -100, Max_y = -10, Min_y = -50 };
            camValue camWallrunR = new camValue { name = "wallrunR_P", IsLimitX = true, Max_x = 100, Min_x = -10, Max_y = -10, Min_y = -50 };
            camValue camWallrunL = new camValue { name = "wallrunL_P", IsLimitX = true, Max_x = 10, Min_x = -100, Max_y = -10, Min_y = -50 };
            camValue camWallrunR_AR = new camValue { name = "wallrunR_AR", IsLimitX = true, Max_x = 100, Min_x = -10, Max_y = -10, Min_y = -50 };
            camValue camWallrunL_AR = new camValue { name = "wallrunL_AR", IsLimitX = true, Max_x = 10, Min_x = -100, Max_y = -10, Min_y = -50 };
            camValue camjumpOutMidR_AR = new camValue { name = "jumpOutMidR_AR", IsLimitX = true, Max_x = 30, Min_x = -91, Max_y = 60, Min_y = -50 };
            camValue camjumpOutMidL_AR = new camValue { name = "jumpOutMidL_AR", IsLimitX = true, Max_x = 91, Min_x = -30, Max_y = 60, Min_y = -50 };

            _camValues.Add(camNormal);
            _camValues.Add(camSidedodgeR);
            _camValues.Add(camSidedodgeL);
            _camValues.Add(camjumpOutMidF);
            _camValues.Add(camjumpOutStartB);
            _camValues.Add(camAerial_Evade);
            _camValues.Add(camDualSidedodgeR);
            _camValues.Add(camDualSidedodgeL);
            _camValues.Add(camDualSidedodgeF);
            _camValues.Add(camWallrunR);
            _camValues.Add(camWallrunL);
            _camValues.Add(camWallrunR_AR);
            _camValues.Add(camWallrunL_AR);
            _camValues.Add(camjumpOutMidR_AR);
            _camValues.Add(camjumpOutMidL_AR);
            _camValues.Add(camjumpOutAir);

            #endregion

            #endregion

            /// 未來可能在此增加射線管理員
        }

        void Update()
        {

        }

        private void LateUpdate()
        {
            if (IsRotChest|| IsRotChestH|| IsRotChestV)
            {
                ChestLook();
            }
            if (IsRotGunHand)
            {
                GunHandAim();
            }
            if (GetComponent<Rigidbody>().velocity.y != 0)
            {
                Animator.SetFloat("avater_yspeed", GetComponent<Rigidbody>().velocity.y * -1f);
            }
        }

        public void ChestLook()
        {
            var cam = camera.GetComponent<MouseOrbitImproved>();
            var came = camera.GetComponent<Camera>();

            //父節點的角度
            var Rootrot = transform.rotation.eulerAngles;
            var root = RotFunction.Clamp180(Rootrot.y);
            var camY = RotFunction.Clamp180(cam.x);

            //取得目標的本地角度
            var TargetRot = RotFunction.Clamp180(camY - root);
            Animator.SetFloat("avater_ChestAngleH", TargetRot);
            //var TargetRot = RotFunction.Clamp360(camY-root);
            //Animator.SetFloat("avater_ChestAngleH", TargetRot);
            Animator.SetFloat("avater_ChestAngleV", cam.y);

            if (IsRotChest)
            {
                Vector3 toScreenCenterPos = came.ScreenToWorldPoint(new Vector3(came.scaledPixelWidth / 2, came.scaledPixelHeight / 2, 100f));
                //取得目前夾角
                //在夾角內就能自由轉動...夾角外的話就在極限角。
                //最大軸度數
                Vector3 toScreenVector = toScreenCenterPos - transform.position;
                float toScreenAngle = Quaternion.Angle(Quaternion.LookRotation(toScreenVector, Hip.up), Hip.rotation);
                //print(toScreenAngle);

                Debug.DrawRay(transform.position + Vector3.up * 1.6f, toScreenVector, Color.green);

                if (toScreenAngle < chestMaxRot)//最大角度
                {
                    //chestTransform.LookAt(toScreenCenterPos, Hip.TransformDirection(Vector3.up));//UP軸為轉動的Y軸
                    Quaternion toScreenQua = Quaternion.LookRotation(toScreenVector, Hip.up);
                    chestTransform.rotation = toScreenQua;

                    //Debug.DrawRay(transform.position + Vector3.up * 1.6f, toScreenQua * chestTransform.forward, Color.blue);
                }
                else
                {
                    //目標:得到一向量為向著攝影機面對的方向轉動至最大角度

                    //得到該方向的垂直轉軸--重要--
                    Vector3 vertical2Screen = Vector3.Cross(Hip.forward, toScreenVector);
                    //
                    var QuaForMaxAngle = Quaternion.AngleAxis(chestMaxRot/*以最大角度取代*/, vertical2Screen)* Hip.rotation;
                    chestTransform.rotation = QuaForMaxAngle;

                    Debug.DrawRay(transform.position + Vector3.up * 1.6f, chestTransform.forward, Color.red);

                    //加入"超出角度不能射擊.準心變紅"
                }
                chestTransform.rotation = chestTransform.rotation * Quaternion.AngleAxis(chestOffSet.x, Vector3.up);
                chestTransform.rotation = chestTransform.rotation * Quaternion.AngleAxis(chestOffSet.y, Vector3.right);
            }
            if (IsRotChestH)
            {
                //子節點的角度
                var ChestRot = chestTransform.rotation.eulerAngles;
                var RotAngle = Mathf.Clamp(TargetRot, -chestMaxRot, chestMaxRot);

                //目前SLerp不能接元轉向
                //因為每一針開始轉向都會被歸位，因此要給的是轉向差

                /*
                var Qrot = Quaternion.Slerp(chestTransform.rotation,
                    chestTransform.rotation * Quaternion.AngleAxis(RotAngle + chestOffSet.x, Vector3.up),
                    Time.deltaTime);
                */

                //Debug.Log("Qrot : "+Qrot.eulerAngles);
                chestTransform.rotation = chestTransform.rotation * Quaternion.AngleAxis(RotAngle + chestOffSet.x, Vector3.up);
            }

            if (IsRotChestV)
            {
                chestTransform.rotation = chestTransform.rotation * Quaternion.AngleAxis(cam.y + chestOffSet.y, Vector3.right);
            }
        }
        public void GunHandAim()
        {
            var came = camera.GetComponent<Camera>();
            var cam = camera.GetComponent<MouseOrbitImproved>();

            var Rootrot = transform.rotation.eulerAngles;
            var root = RotFunction.Clamp180(Rootrot.y);
            var camY = RotFunction.Clamp180(cam.x);

            //取得目標的本地角度
            var TargetRot = RotFunction.Clamp180(camY - root);

            Animator.SetFloat("avater_ChestAngleH", TargetRot);
            Animator.SetFloat("avater_ChestAngleV", cam.y);

            //先得到面相攝影機中間的向量
            Vector3 lookat = came.ScreenToWorldPoint(new Vector3(came.scaledPixelWidth / 2, came.scaledPixelHeight / 2, 100f));
            GunHand.LookAt(lookat, Hip.TransformDirection(Vector3.up));
            GunHand.rotation = GunHand.rotation * Quaternion.AngleAxis(-40, Vector3.up);
            GunHand.rotation = GunHand.rotation * Quaternion.AngleAxis(-15, Vector3.right);

            //將手部面向回傳的向量

        }

        public void ChangeAniRoot(bool IsRoot)
        {
            if (moveflag > 0 || IsRoot)
            {
                Animator.applyRootMotion = true;
            }
            else
                Animator.applyRootMotion = false;
        }

        public void ChangeRotOffSet(string name)
        {
            chestValue chest = _chestValues.Find(x => x.name == name);

            chestOffSet = chest.chestOffSet;
            chestMaxRot = chest.maxDegress;
        }

        public void ChangeCamLimit(string name)
        {
            camValue camv = _camValues.Find(x => x.name == name);

            var cam = camera.GetComponent<MouseOrbitImproved>();
            cam.IsLimitX = camv.IsLimitX;
            cam.xMaxLimit = camv.Max_x;
            cam.xMinLimit = camv.Min_x;
            cam.yMaxLimit = camv.Max_y;
            cam.yMinLimit = camv.Min_y;
        }

        public void CheckCanChangeWeapon(int slotNum)
        {
            //如果武器欄位不同才換
            if (WeaponSlotNumber != slotNum)
            {
                //WeaponSlotNumber = slotNum;
                Animator.SetTrigger("avater_changeweapon");
            }
        }
        public void ChangeWeapon(int slotNum)
        {
                //依照得到的slotNum切換武器參照
                switch (slotNum)
                {
                    case 0:
                        break;

                    case 1:
                        myguns = Guns.Great_Sword;
                        break;
                    case 2:
                        myguns = Guns.Handgun;
                        break;
                    case 3:
                        myguns = Guns.Shotgun;
                        break;
                    case 4:
                        myguns = Guns.AK47;
                        break;
                    case 5:
                        myguns = Guns.SMAW;
                        break;

                    default:
                        break;
                }
            WeaponSlotNumber = slotNum;
            Animator.SetInteger("avater_weaponslot", slotNum);
        }
        public void SlowMo()
        {
            if (Time.timeScale == 1.0f)
                Time.timeScale = 0.7f;
            else
                Time.timeScale = 1.0f;
            // Adjust fixed delta time according to timescale
            // The fixed delta time will now be 0.02 frames per real-time second
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
    }

    public struct chestValue
    {
        public string name { get; set; }
        public Vector3 chestOffSet { get; set; }
        public float maxDegress { get; set; }
    }
    public struct camValue
    {
        public string name { get; set; }

        public bool IsLimitX;
        public float Max_x;
        public float Min_x;
        public float Max_y;
        public float Min_y;
    }
}
