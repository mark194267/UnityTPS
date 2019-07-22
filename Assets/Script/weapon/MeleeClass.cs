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
                var hit = collision.GetComponentInParent<AvaterMain>();
                if (collision.GetComponent<MeleeClass>() )//&& collision.GetComponent<MeleeClass>() != GetComponent<MeleeClass>())
                {
                    hit.OnHit(0, 999, transform.rotation.eulerAngles);
                    collision.GetComponent<Collider>().enabled = false;
                    GetComponentInParent<Animator>().SetTrigger("weapon_parry");
                    print(collision.gameObject.name + "is been blocked");

                }
            }
            else
            {
                //print("OnTriggerEnter " + collision.tag + " hit tag " + transform.tag);
                var main = collision.GetComponentInParent<AvaterMain>();
                if (main && !main.CompareTag(tag) )
                {
                    var mymain = GetComponentInParent<AvaterMain>();
                    if (mymain != main)
                    {
                        print(GetComponentInParent<AvaterMain>().gameObject.name + "'s " + gameObject.name + " OnMeleeHit " + main.gameObject.name);
                        //執行"被打中"
                        main.OnHit(damage + motionDamage, stun + motionStun, transform.rotation.eulerAngles);
                    }
                }
            }
        }        
    }
}
