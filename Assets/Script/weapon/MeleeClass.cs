using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Script.Avater;
using UnityEngine;

namespace Assets.Script.weapon
{
    class MeleeClass:MonoBehaviour
    {
        public bool IsBlocking = false;

        public int damage;
        public double stun;
        public float blast;

        public int motionDamage { get; set; }
        public double motionStun { get; set; }
        public float motionBlast { get; set; }

        void OnTriggerEnter(Collider collision)
        {
            if (IsBlocking)
            {
                if (collision.GetComponent<MeleeClass>() /*&& collision.GetComponent<MeleeClass>() != GetComponent<MeleeClass>()*/)
                {
                    var hit = collision.GetComponentInParent<AvaterMain>();
                    hit.OnHit(0, 999);
                    collision.GetComponent<Collider>().enabled = false;
                    GetComponentInParent<Animator>().SetTrigger("weapon_parry");
                    print(collision.gameObject.name + "is been blocked");

                }
            }
            else
            {
                //print("OnTriggerEnter " + collision.name);
                if (collision.GetComponent<AvaterMain>() && !collision.CompareTag(tag) /*collision.GetComponentInParent<AvaterMain>() != GetComponentInParent<AvaterMain>()*/)
                {
                    var hit = collision.GetComponentInParent<AvaterMain>();
                    print(GetComponentInParent<AvaterMain>().gameObject.name + "'s " + gameObject.name + " OnMeleeHit " + hit.gameObject.name);
                    //執行"被打中"
                    hit.OnHit(damage + motionDamage, stun + motionStun);
                }
            }
        }
    }
}
