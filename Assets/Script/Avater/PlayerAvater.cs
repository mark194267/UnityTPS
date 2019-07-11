using UnityEngine;

using Assets.Script.ActionControl;
using Assets.Script.weapon;
using Assets.Script.Config;

namespace Assets.Script.Avater
{
    /// <summary>
    /// 此為核心元件，非必要時別動這邊
    /// </summary>
    public class PlayerAvater : AvaterMain
    {
        //武器欄
        //public Dictionary<int,string> PlayerWeaponDictionary;

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

        public bool IsRotChestV = false;
        public bool IsRotChestH = false;

        public Transform chestTransform;
        public Vector3 chestOffSet;

        public RaycastHit hit { get; set; }

        public enum Guns { Great_Sword,Cross_Sword, Handgun ,Shotgun ,AK47 , SMAW }
        public Guns myguns;
        public 
        void Start()
        {
            camera = gameObject.transform.Find("Camera").gameObject;
            avaterStatus = avaterDataLoader.LoadStatus("UnityChan");
            //暫時，初始化到時會交出去
            Init_Avater();
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

            gameObject.GetComponent<Gun>().SetPlayerAvater(this);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["basicgun"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["MG"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["SMAW"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["Great_Sword"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["Cross_Sword"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["AK47"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["Handgun"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["Wakizashi"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["Shotgun"]);

            gameObject.GetComponent<Gun>().CreateWeaponByList();
            gameObject.GetComponent<Gun>().cam = gameObject.transform.Find("Camera").GetComponent<MouseOrbitImproved>();
            
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
            if (IsRotChestV||IsRotChestH)
            {
                ChestLook(IsRotChestV,IsRotChestH);
            }
        }

        public void ChestLook(bool IsV,bool IsH)
        {
            var cam = camera.GetComponent<MouseOrbitImproved>();
            if (IsH)
            {

                //等待加入限制角度
                var rootrot = transform.rotation.eulerAngles;
                var localcam = rootrot.y - cam.x;

                //目標轉向只能為 母物件轉向+-45度之間
                var targetRot = Mathf.Clamp(localcam /*輸入轉向為攝影機轉向*/, (rootrot.y - 45f)%360, (rootrot.y + 45f)%360)/*此時camx為世界轉軸，需要先改成本地轉軸*/;
                var localRot = transform.rotation.eulerAngles.y-targetRot;

                Debug.Log(localcam + "  " + targetRot + " " + localRot);


                //保留動畫的原轉向所以只能用 加成的方式
                chestTransform.rotation = chestTransform.rotation * /*目前轉向和目標轉向的差值*/Quaternion.AngleAxis(localRot, Vector3.up);
            }
            if (IsV)
            {
                chestTransform.rotation = chestTransform.rotation * Quaternion.AngleAxis(cam.y+chestOffSet.y, Vector3.right);
            }
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
    }
}
