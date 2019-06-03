using Assets.Script.Avater;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.weapon
{
    /// <summary>
    /// 這裡負責槍枝的基礎功能
    /// </summary>
    public class Gun : MonoBehaviour
    {
        public List<WeaponBasic> WeaponSlot = new List<WeaponBasic>();

        public List<WeaponBasic> EquipmentSlot = new List<WeaponBasic>();

        public List<WeaponBasic> NowWeapon;
        public WeaponBasic NowWeaponOrign;

        public string weaponname;

        public int ammo;
        public int bulletinmag;

        public GameObject Weapon;
        public GameObject Bullet;
        public GameObject target;

        PlayerAvater PlayerAvater;

        public MouseOrbitImproved cam;

        public bool canshoot = true;
        public void SetPlayerAvater(PlayerAvater avater)
        {
            PlayerAvater = avater;
        }

        public void AddWeapon(WeaponBasic weapon)
        {
            weapon.BulletInMag = weapon.MagSize;
            //Debug.Log(weapon.weapon.name);
            EquipmentSlot.Add(weapon);
        }

        public void CreateWeaponByList()
        {
            foreach (var item in gameObject.GetComponentsInChildren(typeof(Transform), true))
            {
                foreach (var weaponBasic in EquipmentSlot)
                {

                    if (item.name == weaponBasic.weapontype)
                    {
                        //後面的疊道前面了
                        WeaponBasic weapon = new WeaponBasic(){
                            name = weaponBasic.name,
                            weapontype = weaponBasic.weapontype,
                            ammotype = weaponBasic.ammotype,
                            Damage = weaponBasic.Damage,
                            MagSize = weaponBasic.MagSize,
                            BulletInMag = weaponBasic.BulletInMag,
                            BulletUsedPerShot = weaponBasic.BulletUsedPerShot,
                            charge = weaponBasic.charge,
                            acc = weaponBasic.acc,
                            rof = weaponBasic.rof,
                            dropoff = weaponBasic.dropoff,
                            speed = weaponBasic.speed,
                            recoil = weaponBasic.recoil,
                            blast = weaponBasic.blast,
                            stun = weaponBasic.stun,
                            weapon = Instantiate(weaponBasic.weapon, item.transform),
                            };
                            weapon.weapon.tag = gameObject.tag;
                            weapon.weapon.SetActive(false);
                        //print(weaponBasic.name + " " + weaponBasic.weapon.name);
                        WeaponSlot.Add(weapon);
                    }
                }
                //var weaponPosition = gameObject.transform.Find(weaponBasic.type);
            }
            
            foreach(var gun in WeaponSlot)
            {
                 print(gun.name);
                 print(gun.name+" "+gun.weapon.name);
            }
            
            /*
            var guns = WeaponSlot.FindAll(x => x.name == "AK-47");
            foreach(var gun in guns)
            {
                print(gun.weapon.name);
                gun.weapon.SetActive(true);
            } 
            */
        }


        public void ExchangeNewWeapon(WeaponBasic weapon)
        {
            //如果是肉搏
            if(weapon.acc < 0)
            {
                //找到武器欄內的肉搏
            }
            //如果不是肉搏
            //獲得新武器
        }

        public void ChangeWeapon(string weaponName)
        {
            //關閉上個武器模組
            //foreach (var Weapon in NowWeapon) Weapon.weapon.SetActive(false);
            //找到新武器
            NowWeapon = WeaponSlot.FindAll(x => x.name == weaponName);
            //Debug.Log(NowWeapon[0].name);
//            Debug.Log(NowWeapon[1].name);

            foreach (var Weapon in NowWeapon)
            {
                Weapon.weapon.SetActive(true);
            } 
            //用來改變狀態
            NowWeaponOrign = NowWeapon[0];

            foreach (var Weapon in NowWeapon)
            {
                //射擊武器
                if (Weapon.acc > 0)
                {
                    Debug.Log(Weapon.name + Weapon.ammotype);

                    Weapon.bullet = Resources.Load("Prefabs/" + Weapon.ammotype.Type) as GameObject;
                    var bullet = Weapon.bullet.GetComponent<BulletClass>();
                    bullet.damage = Weapon.Damage;
                    bullet.stun = Weapon.stun;
                }
                //肉搏武器
                else
                {
                    var melee = Weapon.weapon.GetComponent<MeleeClass>();
                    melee.damage = Weapon.Damage;
                    melee.stun = Weapon.stun;
                }
            }
        }
        public virtual bool fire(int BarrelIndex)
        {
            if (NowWeapon[BarrelIndex].BulletInMag - NowWeapon[BarrelIndex].BulletUsedPerShot >= 0)
            {
                if (canshoot)
                {
                    //發射一發
                    StartCoroutine(ShootShotGunBullet(BarrelIndex));
                    //後座力區塊
                    //cam.Rotate(Random.RandomRange(-100f,100f),Random.RandomRange(0,100f),0f,Space.Self);<<轉為攝影機統一控管
                    //如果現在後座力為 < 目標後座力
                    //前三發後坐力
                    //如果現在後座力 > 目標後座力
                    //後N發後坐力
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 換彈，請在"完成換彈"後使用
        /// </summary>
        /// <returns></returns>
        public virtual bool reload(int WeaponIndex)
        {
            var Weapon = NowWeapon[WeaponIndex];
            //如果彈夾內的子彈 < 彈夾大小，或是剩下彈藥大於 0
            if (Weapon.BulletInMag <= Weapon.MagSize && PlayerAvater.NowAmmo > 0)
            {
                //如果現在彈量 >= 彈夾容量
                if (PlayerAvater.NowAmmo >= Weapon.MagSize)
                {
                    //總彈藥 += 彈夾殘彈
                    PlayerAvater.NowAmmo += Weapon.BulletInMag;
                    //現在彈量 -= 彈夾容量
                    PlayerAvater.NowAmmo -= Weapon.MagSize;
                    //彈夾殘彈 = 彈夾容量
                    Weapon.BulletInMag = Weapon.MagSize;
                }
                //如果持有彈量 < 彈夾容量
                if (PlayerAvater.NowAmmo <= Weapon.MagSize)
                {
                    //彈夾殘彈 = 彈夾容量
                    Weapon.BulletInMag = PlayerAvater.NowAmmo;
                    //持有彈量歸零
                    PlayerAvater.NowAmmo = 0;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 開啟肉搏判定的HITBOX,
        /// 請在動作動畫內加入 AnimationEvent 輸入至 GetAnimationFlag
        /// </summary>
        /// <param name="timeflag"></param>
        public void Swing(int timeflag,int motionDamage,double motionStun)
        {
            if (timeflag == 1) NowWeapon[0].weapon.GetComponentInChildren<Collider>().enabled = true;
            else NowWeapon[0].weapon.GetComponentInChildren<Collider>().enabled = false;
        }
        public void Swing(int WeaponIndex,int timeflag, int motionDamage, double motionStun)
        {
            if (timeflag == 1) NowWeapon[WeaponIndex].weapon.GetComponentInChildren<Collider>().enabled = true;
            else NowWeapon[WeaponIndex].weapon.GetComponentInChildren<Collider>().enabled = false;
        }
        /// <summary>
        /// 開啟肉搏判定的HITBOX,
        /// 請在動作動畫內加入 AnimationEvent 輸入至 GetAnimationFlag
        /// </summary>
        /// <param name="timeflag">動作旗標，在動畫修改旗標時間</param>
        /// <param name="motionDamage">動作傷害補正</param>
        /// <param name="motionStun">動作氣絕值補正</param>
        /// <param name="force">力道方向*相對於腳色</param>

        public void Swing(int timeflag, int motionDamage, double motionStun, Vector3 force)
        {
            if (timeflag == 1)
            {
                NowWeapon[0].weapon.GetComponent<Collider>().enabled = true;
                this.GetComponentInParent<Rigidbody>().AddRelativeForce(force,ForceMode.VelocityChange);
            }
            else NowWeapon[0].weapon.GetComponent<Collider>().enabled = false;
        }
        /// <summary>
        /// 開啟肉搏判定的HITBOX,
        /// 請在動作動畫內加入 AnimationEvent 輸入至 GetAnimationFlag
        /// </summary>
        /// <param name="timeflag">動作旗標，在動畫修改旗標時間</param>
        /// <param name="motionDamage">動作傷害補正</param>
        /// <param name="motionStun">動作氣絕值補正</param>
        /// <param name="force">力道方向*相對於腳色</param>
        /// <param name="mode">力道模式</param>
        public void Swing(int timeflag, int motionDamage, double motionStun, Vector3 force,ForceMode mode)
        {
            if (timeflag == 1)
            {
                NowWeapon[0].weapon.GetComponent<Collider>().enabled = true;
                this.GetComponentInParent<Rigidbody>().AddRelativeForce(force, mode);
            }
            else NowWeapon[0].weapon.GetComponent<Collider>().enabled = false;
        }

        IEnumerator ShootShotGunBullet(int BarrelIndex)
        {
            if (NowWeapon[BarrelIndex].BulletInMag - NowWeapon[BarrelIndex].BulletUsedPerShot < 0) yield break;
            canshoot = false;

            //得到攝影機的正中央
            Quaternion Qua;
            if (transform.CompareTag("Player"))
            {
                var MainCam = GetComponentInChildren<Camera>();
                var MainCamPos = MainCam.ScreenToWorldPoint(
                    new Vector3(MainCam.pixelWidth / 2, MainCam.pixelHeight / 2, MainCam.nearClipPlane * 100)
                    );
                Qua = Quaternion.LookRotation(MainCamPos - transform.position);
            }
            else
                Qua = NowWeapon[BarrelIndex].weapon.transform.rotation;

                //找到目前"槍口"的方向
            for (int i = 0;i< NowWeapon[BarrelIndex].BulletUsedPerShot/*散彈數*/;i++)
            {
                var bullet = Instantiate(NowWeapon[BarrelIndex].bullet,
                NowWeapon[BarrelIndex].weapon.transform.position, Qua);
                
                Quaternion q = UnityEngine.Random.rotationUniform;
                var qv = bullet.transform.TransformVector(Vector3.forward) +
                bullet.transform.TransformDirection(q.eulerAngles*.00001f/* 擴散係數 */);
                bullet.transform.rotation = Quaternion.LookRotation(qv);
                
                //tag來找尋子彈的"陣營"
                bullet.tag = gameObject.tag;
                var b = bullet.GetComponent<BulletClass>();
                //初始化子彈的數值--待改進
                b.damage = NowWeapon[BarrelIndex].Damage;
                b.blast = NowWeapon[BarrelIndex].blast;
                //三秒之後移除
                Destroy(bullet,3f);
            }
            //後座力
            
            if (gameObject.CompareTag("Player"))
            {
                cam.Vrecoil += 3f;
                cam.Hrecoil = Random.Range(-.5f, .5f);
                cam.FireTime = 1f;
            }
            
            //可能是雙管之類的
            NowWeapon[BarrelIndex].BulletInMag = NowWeapon[BarrelIndex].BulletInMag - NowWeapon[BarrelIndex].BulletUsedPerShot;
            yield return new WaitForSeconds(NowWeapon[BarrelIndex].rof);
            canshoot = true;
        }
    }
}
