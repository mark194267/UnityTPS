using Assets.Script.Avater;
using System;
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
        public List<WeaponBasic> NowWeapon = new List<WeaponBasic>();
        public WeaponBasic NowWeaponOrign;

        public WeaponBasic MainWeaponBasic { get; set; }
        public WeaponBasic DualWeaponBasic { get; set; }

        public WeaponBasic SpecialWeaponBasic { get; set; }
        public WeaponBasic SkillWeaponBasic { get; set; }

        public string weaponname;

        public int ammo;
        public int bulletinmag;

        public GameObject Weapon;
        public GameObject Bullet;
        public GameObject target;

        public PlayerAvater PlayerAvater { get; set; }

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
                            multi = weaponBasic.multi,
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
            /*
            foreach(var gun in WeaponSlot)
            {
                 print(gun.name);
                 print(gun.name+" "+gun.weapon.name);
            }
            */
        }
        public void CreateWeaponByDic(ref Dictionary<int,WeaponBasic> weaponDic)
        {
            foreach (var item in gameObject.GetComponentsInChildren(typeof(Transform), true))
            {
                foreach (var weaponBasic in weaponDic)
                {
                    //找到放武器的位置.生成武器在指定位置
                    var weapon = weaponBasic.Value;
                    if (item.name == weapon.weapontype)
                    {
                        weapon.weapon = Instantiate(weapon.weapon, item.transform);
                        weapon.weapon.SetActive(false);
                        LoadWeapon(weapon);
                    }
                }
            }
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

        public void LoadSingleWeapon(string weaponName)
        {
            var wpn = WeaponSlot.FindAll(x => x.name == weaponName);
            foreach (var wpnSingle in wpn)
            {
                WeaponBasic wb = new WeaponBasic(wpnSingle);
                wb.weapon = wpnSingle.weapon;
                //先載入武器數值
                LoadWeapon(wb);
                //將預備欄的武器放入目前的武器欄
                NowWeapon.Add(wb);
            }
        }

        public void InactiveAllWeapon()
        {
            //關閉上個武器模組
            if (NowWeapon != null)
                foreach (var Weapon in NowWeapon) Weapon.weapon.SetActive(false);
        }

        public void InactiveAllWeapon(string[] except)
        {
            //關閉上個武器
            if (NowWeapon != null)
            {
                foreach (var Weapon in NowWeapon)
                {
                    if(!except.ToString().Contains(Weapon.name))
                        Weapon.weapon.SetActive(false);
                }
            }
        }

        public void InactiveWeapon(string weaponName)
        {
            var wpnList = NowWeapon.FindAll(x => x.name == weaponName);
            foreach (var wpn in wpnList)
            {
                wpn.weapon.SetActive(false);
            }
        }

        public List<WeaponBasic> ActiveWeapon(string weaponName)
        {
            var wpnList = NowWeapon.FindAll(x => x.name == weaponName);
            //開啟武器
            foreach (var wpn in wpnList)
            {
                //print(wpn.name);
                wpn.weapon.SetActive(true);
            }
            return wpnList;
        }

        public void LoadWeapon(WeaponBasic Weapon)
        {
            //射擊武器
            if (Weapon.acc > 0)
            {
                Weapon.bullet = Resources.Load("Prefabs/" + Weapon.ammotype.Type) as GameObject;
                Weapon.BulletInMag = Weapon.MagSize;

                var bullet = Weapon.bullet.GetComponent<BulletClass>();
                bullet.damage = Weapon.Damage;
                bullet.stun = Weapon.stun;

            }
            //肉搏武器
            else
            {
                //print(Weapon.weapon.name);
                var melee = Weapon.weapon.GetComponent<MeleeClass>();
                melee.damage = Weapon.Damage;
                melee.stun = Weapon.stun;
            }
        }

        public void ChangeWeapon(string weaponName)
        {
            //關閉上個武器模組
            if(NowWeapon != null)
                foreach (var Weapon in NowWeapon) Weapon.weapon.SetActive(false);
            //找到新武器
            NowWeapon = WeaponSlot.FindAll(x => x.name == weaponName);
            //Debug.Log(NowWeapon[0].name);
//            Debug.Log(NowWeapon[1].name);

            foreach (var Weapon in NowWeapon) Weapon.weapon.SetActive(true);

            //用來改變狀態
            NowWeaponOrign = NowWeapon[0];

            foreach (var Weapon in NowWeapon)
            {
                LoadWeapon(Weapon);
            }
        }
        public virtual bool fire(int BarrelIndex)
        {
            //Debug.Log(NowWeapon[BarrelIndex].BulletInMag + " - " + NowWeapon[BarrelIndex].BulletUsedPerShot);

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
        public virtual bool fire(string WeaponName,int LR)
        {
            //Debug.Log(NowWeapon[BarrelIndex].BulletInMag + " - " + NowWeapon[BarrelIndex].BulletUsedPerShot);
            var gun = NowWeapon.FindAll(x => x.name == WeaponName);
            if (gun[LR].BulletInMag - gun[LR].BulletUsedPerShot >= 0)
            {
                if (canshoot)
                {
                    //發射一發
                    StartCoroutine(ShootShotGunBullet(LR));
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
        public virtual bool fire(WeaponBasic wb)
        {
            //Debug.Log(NowWeapon[BarrelIndex].BulletInMag + " - " + NowWeapon[BarrelIndex].BulletUsedPerShot);
            if (wb.BulletInMag - wb.BulletUsedPerShot >= 0)
            {
                if (canshoot)
                {
                    //發射一發
                    StartCoroutine(ShootShotGunBullet(wb));
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

        public virtual bool reload(WeaponBasic weaponBasic)
        {
            //如果彈夾內的子彈 < 彈夾大小，或是剩下彈藥大於 0
            if (weaponBasic.BulletInMag <= weaponBasic.MagSize 
                && weaponBasic.ammotype.NowAmmo > 0)
            {
                //如果現在彈量 >= 彈夾容量
                if (weaponBasic.ammotype.NowAmmo >= weaponBasic.MagSize)
                {
                    //總彈藥 += 彈夾殘彈
                    weaponBasic.ammotype.NowAmmo += weaponBasic.BulletInMag;
                    //現在彈量 -= 彈夾容量
                    weaponBasic.ammotype.NowAmmo -= weaponBasic.MagSize;
                    //彈夾殘彈 = 彈夾容量
                    weaponBasic.BulletInMag = weaponBasic.MagSize;
                }
                //如果持有彈量 < 彈夾容量
                if (weaponBasic.ammotype.NowAmmo <= weaponBasic.MagSize)
                {
                    //彈夾殘彈 = 彈夾容量
                    weaponBasic.BulletInMag = weaponBasic.ammotype.NowAmmo;
                    //持有彈量歸零
                    weaponBasic.ammotype.NowAmmo = 0;
                }
                return true;
            }
            return false;
        }
        public bool reload_shotgun(WeaponBasic weaponBasic)
        {
            //如果彈夾內的子彈 < 彈夾大小，或是剩下彈藥大於 0
            if (weaponBasic.ammotype.NowAmmo > 0 && weaponBasic.BulletInMag < weaponBasic.MagSize)
            {
                //如果現在彈量 >= 彈夾容量
                if (weaponBasic.ammotype.NowAmmo >= weaponBasic.MagSize)
                {
                    //現在彈量 -= 彈夾容量
                    weaponBasic.ammotype.NowAmmo -= 1;
                    //彈夾殘彈 = 彈夾容量
                    weaponBasic.BulletInMag += 1;
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 防禦，在timeflag期間碰撞到的對象的coillder關閉
        /// </summary>
        /// <param name="timeflag"></param>
        /// <param name="motionDamage"></param>
        /// <param name="motionStun"></param>
        public void Block(int timeflag, int motionDamage, double motionStun)
        {
            if (timeflag == 1)
            {
                NowWeapon[0].weapon.GetComponentInChildren<Collider>().enabled = true;
                NowWeapon[0].weapon.GetComponentInChildren<MeleeClass>().IsBlocking = true;
            }
            else
            {
                NowWeapon[0].weapon.GetComponentInChildren<MeleeClass>().IsBlocking = false;
                NowWeapon[0].weapon.GetComponentInChildren<Collider>().enabled = false;
            }
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
        /// <summary>
        /// 在動畫中插入旗標，並依照旗標數字開啟武器碰撞
        /// </summary>
        /// <param name="WeaponIndex">目標旗標</param>
        /// <param name="motionDamage"></param>
        /// <param name="motionStun"></param>
        public void SwingByIndex(int WeaponIndex, int motionDamage, double motionStun)
        {
            var timeflag = GetComponent<AvaterMain>().anim_flag;
            if (timeflag != WeaponIndex) NowWeapon[WeaponIndex].weapon.GetComponentInChildren<Collider>().enabled = false;
            else NowWeapon[WeaponIndex].weapon.GetComponentInChildren<Collider>().enabled = true;
        }
        public void SwingByIndex(int WeaponIndex, int flagTrigger, int motionDamage, double motionStun)
        {
            var timeflag = GetComponent<AvaterMain>().anim_flag;
            if (timeflag != flagTrigger) NowWeapon[WeaponIndex].weapon.GetComponentInChildren<Collider>().enabled = false;
            else NowWeapon[WeaponIndex].weapon.GetComponentInChildren<Collider>().enabled = true;
        }
        public void SwingAll(int WeaponIndex, int motionDamage, double motionStun)
        {
            var timeflag = GetComponent<AvaterMain>().anim_flag;
            if (timeflag != WeaponIndex)
                foreach (var weapon in NowWeapon) weapon.weapon.GetComponentInChildren<Collider>().enabled = false;
            else
                foreach (var weapon in NowWeapon) weapon.weapon.GetComponentInChildren<Collider>().enabled = true;
        }
        public void Swing(WeaponBasic weaponBasic)
        {
            var timeflag = GetComponent<AvaterMain>().anim_flag;
            if (timeflag == 1) weaponBasic.weapon.GetComponentInChildren<Collider>().enabled = true;
            else weaponBasic.weapon.GetComponentInChildren<Collider>().enabled = false;
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
                //Qua = NowWeapon[BarrelIndex].weapon.transform.rotation;
                Qua = Quaternion.LookRotation(target.transform.position - transform.position);


            //找到目前"槍口"的方向
            for (int i = 0;i< NowWeapon[BarrelIndex].multi/*散彈數*/;i++)
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
                b.speed = NowWeapon[BarrelIndex].speed;
                //三秒之後移除
                Destroy(bullet,3f);
            }
            //後座力
            
            if (gameObject.CompareTag("Player"))
            {
                cam.Vrecoil += 3f;
                cam.Hrecoil = UnityEngine.Random.Range(-.5f, .5f);
                cam.FireTime = 1f;
            }
            
            //可能是雙管之類的
            NowWeapon[BarrelIndex].BulletInMag = NowWeapon[BarrelIndex].BulletInMag - NowWeapon[BarrelIndex].BulletUsedPerShot;
            yield return new WaitForSeconds(NowWeapon[BarrelIndex].rof);
            canshoot = true;
        }
        IEnumerator ShootShotGunBullet(WeaponBasic wb)
        {
            if (wb.BulletInMag - wb.BulletUsedPerShot < 0) yield break;
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
                Qua = Quaternion.LookRotation(target.transform.position+Vector3.up*1.5f/*目標高度*/ - wb.weapon.transform.position);

            //找到目前"槍口"的方向
            for (int i = 0; i < wb.multi/*散彈數*/; i++)
            {
                var bullet = Instantiate(wb.bullet,
                wb.weapon.transform.position, Qua);

                Quaternion q = UnityEngine.Random.rotationUniform;
                var qv = bullet.transform.TransformVector(Vector3.forward) +
                bullet.transform.TransformDirection(q.eulerAngles * .00001f/* 擴散係數 */);
                bullet.transform.rotation = Quaternion.LookRotation(qv);

                //tag來找尋子彈的"陣營"
                bullet.tag = gameObject.tag;
                var b = bullet.GetComponent<BulletClass>();
                //初始化子彈的數值--待改進
                b.damage = wb.Damage;
                b.blast = wb.blast;
                b.speed = wb.speed;
                //三秒之後移除
                Destroy(bullet, 3f);
            }
            //後座力

            if (gameObject.CompareTag("Player"))
            {
                cam.Vrecoil += 3f;
                cam.Hrecoil = UnityEngine.Random.Range(-.5f, .5f);
                cam.FireTime = 1f;
            }

            //可能是雙管之類的
            wb.BulletInMag = wb.BulletInMag - wb.BulletUsedPerShot;
            yield return new WaitForSeconds(wb.rof);
            canshoot = true;
        }

    }
}
