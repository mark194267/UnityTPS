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
            var frontTransformDirection = gameObject.transform.TransformDirection(Vector3.forward);
            gameObject.GetComponent<Rigidbody>().AddForce(frontTransformDirection.normalized*50,ForceMode.VelocityChange);
        }
        
        void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.tag != tag/*子彈不會打中子彈*/ && collision.gameObject.GetComponentInParent<AvaterMain>()/*是演員*/)
            {
                var hit = collision.gameObject;
                //執行"被打中"
                hit.GetComponentInParent<AvaterMain>().OnHit(damage,stun);
                Destroy(gameObject);
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
                    if (rb != null || !rb.CompareTag("Player"))
                        rb.AddExplosionForce(5f, transform.position, blast,0,ForceMode.VelocityChange);
                }
            }        
        }
    }
}
