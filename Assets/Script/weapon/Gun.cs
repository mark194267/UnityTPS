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
        public WeaponBasic NowWeapon;

        public string weaponname;

        public int ammo;
        public int bulletinmag;

        public GameObject Weapon;
        public GameObject Bullet;
        public GameObject target;

        public MouseOrbitImproved cam;

        public bool canshoot = true;

        public void AddWeapon(WeaponBasic weapon)
        {
            weapon.BulletInMag = weapon.MagSize;
            this.WeaponSlot.Add(weapon);
        }

        public void CreateWeaponByList()
        {
            foreach (var weaponBasic in WeaponSlot)
            {
                foreach (var item in gameObject.GetComponentsInChildren(typeof(Transform),true))
                {
                    if(item.name == weaponBasic.type)
                    {
                        weaponBasic.weapon = Instantiate(weaponBasic.weapon, item.transform);
                        weaponBasic.weapon.SetActive(false);
                    }
                }
                //var weaponPosition = gameObject.transform.Find(weaponBasic.type);
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

        public void ChangeWeapon(string weaponName)
        {            
            //關閉上個武器模組
            if (NowWeapon != null)
            {
                if (NowWeapon.weapon != null)
                {
                    NowWeapon.weapon.SetActive(false);
                }
            }            
            //找到新武器
            NowWeapon = WeaponSlot.Find(x => x.name == weaponName);
            NowWeapon.weapon.SetActive(true);

            //射擊武器
            if (NowWeapon.acc > 0)
            {
                NowWeapon.bullet = Resources.Load("Prefabs/" + NowWeapon.type) as GameObject;
                var bullet = NowWeapon.bullet.GetComponent<BulletClass>();
                bullet.damage = NowWeapon.Damage;
                bullet.stun = NowWeapon.stun;
            }
            //肉搏武器
            else
            {
                var melee = NowWeapon.weapon.GetComponent<MeleeClass>();
                melee.damage = NowWeapon.Damage;
                melee.stun = NowWeapon.stun;
            }
            //canshoot = true;
        }
        public virtual bool fire()
        {
            if (canshoot && NowWeapon.BulletInMag - NowWeapon.BulletUsedPerShot >= 0)
            {
                //發射一發
                //StartCoroutine(ShootBullet());
                StartCoroutine(ShootShotGunBullet());
                //後座力區塊
                //cam.Rotate(Random.RandomRange(-100f,100f),Random.RandomRange(0,100f),0f,Space.Self);<<轉為攝影機統一控管
                //如果現在後座力為 < 目標後座力
                    //前三發後坐力
                //如果現在後座力 > 目標後座力
                    //後N發後坐力
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual bool reload()
        {
            //如果彈夾內的子彈 < 彈夾大小，或是剩下彈藥大於 0
            if (NowWeapon.BulletInMag <= NowWeapon.MagSize && NowWeapon.nowammo > 0)
            {
                //如果現在彈量 >= 彈夾容量
                if (NowWeapon.nowammo >= NowWeapon.MagSize)
                {
                    //總彈藥 += 彈夾殘彈
                    NowWeapon.nowammo += NowWeapon.BulletInMag;
                    //現在彈量 -= 彈夾容量
                    NowWeapon.nowammo -= NowWeapon.MagSize;
                    //彈夾殘彈 = 彈夾容量
                    NowWeapon.BulletInMag = NowWeapon.MagSize;
                }
                //如果持有彈量 < 彈夾容量
                if (NowWeapon.nowammo <= NowWeapon.MagSize)
                {
                    //彈夾殘彈 = 彈夾容量
                    NowWeapon.BulletInMag = NowWeapon.nowammo;
                    //持有彈量歸零
                    NowWeapon.nowammo = 0;
                }

                weaponname = NowWeapon.name;
                ammo = NowWeapon.nowammo;
                bulletinmag = NowWeapon.BulletInMag;
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
            if (timeflag == 1) NowWeapon.weapon.GetComponent<Collider>().enabled = true;
            else NowWeapon.weapon.GetComponent<Collider>().enabled = false;
        }

        IEnumerator ShootShotGunBullet()
        {
            if (NowWeapon.BulletInMag - NowWeapon.BulletUsedPerShot < 0) yield break;
            canshoot = false;
            //找到目前"槍口"的方向
            for(int i = 0;i< NowWeapon.BulletUsedPerShot/*散彈數*/;i++)
            {
                var bullet = (GameObject)Instantiate(NowWeapon.bullet,
                NowWeapon.weapon.transform.position,NowWeapon.weapon.transform.rotation);
                
                Quaternion q = UnityEngine.Random.rotationUniform;
                var qv = bullet.transform.TransformVector(Vector3.forward) +
                bullet.transform.TransformDirection(q.eulerAngles*.00001f/* 擴散係數 */);
                //Debug.Log(bullet.transform.TransformDirection(q.eulerAngles));
                bullet.transform.rotation = Quaternion.LookRotation(qv);
                
                //tag來找尋子彈的"陣營"
                bullet.tag = gameObject.tag;
                var b = bullet.GetComponent<BulletClass>();
                //初始化子彈的數值--待改進
                b.damage = NowWeapon.Damage;
                b.blast = NowWeapon.blast;
                //三秒之後移除
                Destroy(bullet,3f);
            }
            //後座力
            /*
            if (gameObject.CompareTag("Player"))
            {
                cam.Vrecoil += 3f;
                cam.Hrecoil = Random.Range(-.5f, .5f);
                cam.FireTime = 1f;
            }
            */
            //可能是雙管之類的
            NowWeapon.BulletInMag = NowWeapon.BulletInMag - NowWeapon.BulletUsedPerShot;
            //print(gameObject.name+" 用 "+name+" 射擊! 而彈量為 "+ NowWeapon.BulletInMag);
            yield return new WaitForSeconds(NowWeapon.rof);
            canshoot = true;
        }
    }
}
