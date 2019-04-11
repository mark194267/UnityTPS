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

        void OnTriggerEnter(Collider collision)
        {
            print("hit");
            if (collision.gameObject.tag != tag/*子彈不會打中子彈*/ && collision.gameObject.GetComponent<AvaterMain>()/*是演員*/)
            {
                var hit = collision.gameObject;
                //執行"被打中"
                hit.GetComponent<AvaterMain>().OnHit(damage, stun);
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
                    if (rb != null)
                        rb.AddExplosionForce(5f, transform.position, blast);
                }
            }
        }
    }
}
