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

        public enum Guns { Wakizashi, Handgun ,Shotgun , AK47,}
        public Guns myguns;
        public 
        void Start()
        {
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
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["bazooka"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["katana"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["AK47"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["Handgun"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["Wakizashi"]);
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
                        myguns = Guns.Wakizashi;
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
