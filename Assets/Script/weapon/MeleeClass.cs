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
        public int damage;
        public double stun;
        public float blast;

        public int motionDamage { get; set; }
        public double motionStun { get; set; }
        public float motionBlast { get; set; }

        void OnTriggerEnter(Collider collision)
        {
            print("meleehit");
            if (collision.gameObject.tag != tag/*子彈不會打中子彈*/ && collision.gameObject.GetComponent<AvaterMain>()/*是演員*/)
            {
                var hit = collision.gameObject;
                //執行"被打中"
                hit.GetComponent<AvaterMain>().OnHit(damage + motionDamage, stun + motionStun);
                //Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            if (blast > 0)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, blast);
                foreach (Collider hit in colliders)
                {
                    var rb = hit.attachedRigidbody;
                    if (rb != null||!rb.CompareTag("Player"))
                        rb.AddExplosionForce(5f, transform.position, blast, 0, ForceMode.VelocityChange);
                }
            }
        }
    }
}
