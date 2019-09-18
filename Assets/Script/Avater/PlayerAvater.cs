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

        public bool IsRotChest = false;
        public bool IsRotChestV = false;
        public bool IsRotChestH = false;

        private List<chestValue> _chestValues = new List<chestValue>();
        private List<camValue> _camValues = new List<camValue>();

        public Transform Hip;
        public Transform chestTransform;
        public Vector3 chestOffSet;
        public float chestMaxRot;

        public RaycastHit hit { get; set; }

        public enum Guns { Great_Sword,Cross_Sword, Handgun ,Shotgun ,AK47 , SMAW }
        public Guns myguns;

        public float slowMoDur;
        public float slowMoCool;
        public float timer = 0; 

        void Start()
        {
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

            chestValue none = new chestValue { name = "none", maxDegress = 60, chestOffSet = new Vector3(0, 0, 0) };
            chestValue chestSlash = new chestValue { name = "slash", maxDegress = 60, chestOffSet = new Vector3(0, 0, 0) };
            chestValue chestPistol = new chestValue { name = "pistol", maxDegress = 60, chestOffSet = new Vector3(-40, 0, 0) };
            chestValue chestWallrun = new chestValue { name = "wallrun", maxDegress = 120, chestOffSet = new Vector3(0, 0, 0) };
            chestValue chestIdle = new chestValue { name = "idle", maxDegress = 60, chestOffSet = new Vector3(10,-8,0) };
            chestValue chestMStrafe = new chestValue { name = "strafe", maxDegress = 0, chestOffSet = new Vector3(20, 0, 0) };
            chestValue chestDash = new chestValue { name = "dash", maxDegress = 0, chestOffSet = new Vector3(20, 0, 0) };
            chestValue chestDash4way = new chestValue { name = "dash4way", maxDegress = 0, chestOffSet = new Vector3(0, 40, 0) };
            chestValue chestPistolsilde = new chestValue { name = "sidedodgeR", maxDegress = 60, chestOffSet = new Vector3(40, 0, 0) };
            chestValue chestPistolsildeL = new chestValue { name = "sidedodgeL", maxDegress = 60, chestOffSet = new Vector3(-30, 0, 0) };

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

            camValue camNormal = new camValue { name = "none",IsLimitX = false, Max_x = 360, Min_x = -360, Max_y = 80, Min_y = -70 };
            camValue camSidedodgeR = new camValue { name = "sidedodgeR",IsLimitX = true, Max_x = 10, Min_x = -100, Max_y = -10, Min_y = -50 };
            camValue camSidedodgeL = new camValue { name = "sidedodgeL", IsLimitX = true, Max_x = 100, Min_x = -10, Max_y = -10, Min_y = -50 };

            _camValues.Add(camNormal);
            _camValues.Add(camSidedodgeR);
            _camValues.Add(camSidedodgeL);

            /// 未來可能在此增加射線管理員
        }

        void Update()
        {
            if (GetComponent<Rigidbody>().velocity.y != 0)
            {
                Animator.SetFloat("avater_yspeed", GetComponent<Rigidbody>().velocity.y*-1f);
            }
        }

        private void LateUpdate()
        {
            if (IsRotChest|| IsRotChestH|| IsRotChestV)
            {
                ChestLook();
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

            if (IsRotChest)
            {
                chestTransform.LookAt(came.ScreenToWorldPoint(new Vector3(came.scaledPixelWidth / 2, came.scaledPixelHeight / 2, 100f)), Hip.TransformDirection(Vector3.up));
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
        /*
        public float Clamp180(float Num)
        {
            if (Num < -180)
            {
                Num += 360;
            }
            if (Num > 180)
            {
                Num -= 360;
            }
            return Num;
        }
        */

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

        public void ChangeWeapon(int slotNum)
        {
            if (WeaponSlotNumber != slotNum)
            {
                bool canchange = true;
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
                        canchange = false;
                        break;
                }
                if (canchange)
                {
                    WeaponSlotNumber = slotNum;
                    Animator.SetInteger("avater_weaponslot", slotNum);
                    Animator.SetTrigger("avater_changeweapon");
                }
            }
        }
        public void StartSlowMo()
        {
            
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
