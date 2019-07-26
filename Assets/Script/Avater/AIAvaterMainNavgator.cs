using System;
using System.Collections;
using Assets.Script.ActionControl;
using Assets.Script.AIGroup;
using Assets.Script.weapon;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Script.Avater
{
    public class AIAvaterMainNavgator : AIAvaterMain
    {
        public Transform Navgator;
        public override void Start()
        {
            base.Start();
            //GetComponent<NavMeshAgent>().updateUpAxis = false;
            Navgator =  GetComponentInChildren<NavMeshAgent>().transform;
        }

        public override void Update()
        {
            base.Update();
            transform.position = Navgator.position;
            transform.rotation = Navgator.rotation;
        }
    }
}
