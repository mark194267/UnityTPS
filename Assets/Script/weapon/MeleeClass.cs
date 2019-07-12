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
                if (collision.GetComponentInParent<MeleeClass>() != GetComponentInParent<MeleeClass>())
                {
                    var hit = collision.GetComponentInParent<AvaterMain>();
                    hit.OnHit(0, 999);
                }
            }

                //print("OnTriggerEnter " + collision.name);
                if (collision.GetComponentInParent<AvaterMain>() != GetComponentInParent<AvaterMain>())
            {
                var hit = collision.GetComponentInParent<AvaterMain>();
                print(GetComponentInParent<AvaterMain>().gameObject.name + " OnMeleeHit " + hit.gameObject.name);

                //執行"被打中"
                hit.OnHit(damage + motionDamage, stun + motionStun);
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
