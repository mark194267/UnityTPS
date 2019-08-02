using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;

namespace Assets.Script.weapon
{
    class WeaponFactory
    {
        public Dictionary<string, WeaponBasic> AllWeaponDictionary;
        public Dictionary<string, WeaponBasic> RangeDictionary;
        public Dictionary<string, WeaponBasic> ShotgunDictionary;
        public Dictionary<string, WeaponBasic> SidearmDictionary;
        public Dictionary<string, WeaponBasic> RocketDictionary;
        public Dictionary<string, WeaponBasic> CloseDictionary;
        public Dictionary<string, WeaponBasic> DualDictionary;
        public Dictionary<string, WeaponBasic> KickDictionary;


        public void Init()
        {
            RangeDictionary = Loadweapon("range", "rifle");
            ShotgunDictionary = Loadweapon("range", "shotgun");
            SidearmDictionary = Loadweapon("range", "sidearm");
            RocketDictionary = Loadweapon("range", "rocket");
            CloseDictionary = Loadweapon("close", "sword");
            DualDictionary = Loadweapon("close", "dual");
            KickDictionary = Loadweapon("close","kick");
            AllWeaponDictionary = 
                RangeDictionary.Concat(CloseDictionary).Concat(RocketDictionary).Concat(DualDictionary).Concat(KickDictionary).Concat(SidearmDictionary).Concat(ShotgunDictionary)
                .GroupBy(d => d.Key).ToDictionary(d => d.Key, d => d.First().Value);
        }

        public void Init(List<Ammo> ammo)
        {
            SidearmDictionary = Loadweapon("range", "sidearm",ammo);
            RangeDictionary = Loadweapon("range", "rifle",ammo);
            ShotgunDictionary = Loadweapon("range", "shotgun",ammo);
            RocketDictionary = Loadweapon("range", "rocket", ammo);
            CloseDictionary = Loadweapon("close", "sword", ammo);
            AllWeaponDictionary = RangeDictionary
                .Concat(CloseDictionary)
                .Concat(RocketDictionary)
                .Concat(ShotgunDictionary)
                .Concat(SidearmDictionary)
                .GroupBy(d => d.Key)
                .ToDictionary(d => d.Key, d => d.First().Value);
        }

        //開始時就載入全列表，被呼叫時返回一個新的武器給該單位
        //近接和遠程分開
        public Dictionary<string, WeaponBasic> Loadweapon(string close_or_range, string weapontype)
        {
            Dictionary<string, WeaponBasic> weapondictionary = new Dictionary<string, WeaponBasic>();
            XmlDocument doc = new XmlDocument();
            doc.Load("Assets\\data\\weapon\\weapon.xml");
            XmlNodeList weapontypeNode = doc.SelectNodes("weapon/" + close_or_range + "/" + weapontype);
            XmlNodeList nodeList = weapontypeNode[0].ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                string name = node.Attributes["name"].Value;
                string ammotype = node.Attributes["type"].Value;

                int damage = Convert.ToInt32(node.Attributes["damage"].Value);
                int magsize = Convert.ToInt32(node.Attributes["magsize"].Value);
                int bulletusedpershot = Convert.ToInt32(node.Attributes["bulletusedpershot"].Value);
                int multi = Convert.ToInt32(node.Attributes["multi"].Value);

                float charge = (float)Convert.ToDouble(node.Attributes["charge"].Value);
                float acc = (float)Convert.ToDouble(node.Attributes["acc"].Value);
                float rof = (float)Convert.ToDouble(node.Attributes["rof"].Value);
                float dropoff = (float)Convert.ToDouble(node.Attributes["dropoff"].Value);
                float speed = (float)Convert.ToDouble(node.Attributes["speed"].Value);
                float recoil = (float)Convert.ToDouble(node.Attributes["recoil"].Value);
                float blast = (float)Convert.ToDouble(node.Attributes["blast"].Value);

                double stun = Convert.ToDouble(node.Attributes["stun"].Value);

                GameObject weaponGameObject = Resources.Load("Prefabs/weapon/"+close_or_range+"/"+weapontype+ "/" + name) as GameObject;

                WeaponBasic weapon = new WeaponBasic()
                {
                    name = name,
                    weapontype = weapontype,
                    ammotype = new Ammo { Type = ammotype },
                    Damage = damage,
                    MagSize = magsize,
                    BulletUsedPerShot = bulletusedpershot,
                    multi = multi
                    ,
                    acc = acc,
                    dropoff = dropoff,
                    speed = speed,
                    recoil = recoil,
                    rof = rof
                    ,
                    stun = stun,
                    charge = charge,
                    blast = blast,
                    weapon = weaponGameObject
                };
                weapondictionary.Add(name, weapon);
            }
            return weapondictionary;
        }
        public Dictionary<string, WeaponBasic> Loadweapon(string close_or_range, string weapontype,List<Ammo> ammo)
        {
            Dictionary<string, WeaponBasic> weapondictionary = new Dictionary<string, WeaponBasic>();
            XmlDocument doc = new XmlDocument();
            doc.Load("Assets\\data\\weapon\\weapon.xml");
            XmlNodeList weapontypeNode = doc.SelectNodes("weapon/" + close_or_range + "/" + weapontype);
            XmlNodeList nodeList = weapontypeNode[0].ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                string name = node.Attributes["name"].Value;
                string ammotype = node.Attributes["type"].Value;
                int damage = Convert.ToInt32(node.Attributes["damage"].Value);
                int magsize = Convert.ToInt32(node.Attributes["magsize"].Value);
                int bulletusedpershot = Convert.ToInt32(node.Attributes["bulletusedpershot"].Value);
                int multi = Convert.ToInt32(node.Attributes["multi"].Value);

                float charge = (float)Convert.ToDouble(node.Attributes["charge"].Value);
                float acc = (float)Convert.ToDouble(node.Attributes["acc"].Value);
                float rof = (float)Convert.ToDouble(node.Attributes["rof"].Value);
                float dropoff = (float)Convert.ToDouble(node.Attributes["dropoff"].Value);
                float speed = (float)Convert.ToDouble(node.Attributes["speed"].Value);
                float recoil = (float)Convert.ToDouble(node.Attributes["recoil"].Value);
                float blast = (float)Convert.ToDouble(node.Attributes["blast"].Value);

                double stun = Convert.ToDouble(node.Attributes["stun"].Value);
                GameObject weaponGameObject = Resources.Load("Prefabs/weapon/" + close_or_range + "/" + weapontype + "/" + name) as GameObject;
                //Debug.Log(weaponGameObject.name);
                WeaponBasic weapon = new WeaponBasic()
                {
                    name = name,
                    weapontype = weapontype,
                    Damage = damage,
                    MagSize = magsize,
                    BulletUsedPerShot = bulletusedpershot,
                    multi = multi
                    ,
                    acc = acc,
                    dropoff = dropoff,
                    speed = speed,
                    recoil = recoil,
                    rof = rof
                    ,
                    stun = stun,
                    charge = charge,
                    blast = blast,
                    weapon = weaponGameObject
                };
                weapon.ammotype = ammo.Find(x => x.Type == ammotype);
                //Debug.Log(weapon.weapon.name + weapon.ammotype);
                weapondictionary.Add(name, weapon);
            }
            return weapondictionary;
        }

    }


    public class WeaponBasic
    {
        public string name;
        public string weapontype;
        public Ammo ammotype;

        public int Damage;
        public int MagSize;
        public int BulletInMag;
        public int BulletUsedPerShot;
        public int multi;

        public float charge;
        public float acc;
        public float rof;
        public float dropoff;
        public float speed;
        public float recoil;
        public float blast;

        public double stun;

        public GameObject weapon;
        public GameObject bullet;
    }

}
