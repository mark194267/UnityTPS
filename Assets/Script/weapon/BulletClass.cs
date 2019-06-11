using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Script.Avater;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Script.weapon
{
    class BulletClass:MonoBehaviour
    {        
        public int damage;
        public double stun;
        public float blast;

        void Start()
        {
            gameObject.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward*20,ForceMode.VelocityChange);
        }
        
        void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.tag != tag/*子彈不會打中子彈*/ && collision.GetComponentInParent<AvaterMain>()/*是演員*/)
            {
                var hit = collision.GetComponentInParent<AvaterMain>();

                //var dir = hit.GetComponentInParent<Transform>().TransformPoint();

                //執行"被打中"
                hit.OnHit(damage,stun);
                
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {        
            if (blast > 0)
            {
                /*
                Collider[] colliders = Physics.OverlapSphere(transform.position, blast);
                if (colliders == null)
                    return;
                foreach (Collider hit in colliders)
                {
                    var rb = hit.GetComponentInParent<Rigidbody>();
                    if (rb != null || !hit.transform.CompareTag("Player"))
                        rb.AddExplosionForce(5f, transform.position, blast,0,ForceMode.VelocityChange);
                }
                */
            }
        }
    }
}
