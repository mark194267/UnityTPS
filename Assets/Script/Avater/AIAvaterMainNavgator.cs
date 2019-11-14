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
        public bool IsLateApplyRot = false;

        public Transform Navgator;
        public override void Start()
        {
            base.Start();
    
            var Nav = Navgator.GetComponent<NavMeshAgent>();
        }
        public void LateUpdate()
        {
            if (IsLateApplyRot == true)
            {
                var Nav = Navgator.GetComponent<NavMeshAgent>();
                //transform.position = new Vector3(Nav.transform.position.x, 3, Nav.transform.position.z);
                transform.LookAt(Nav.steeringTarget);
                
            }
        }
        public IEnumerator SetHeight()
        {

            yield return new WaitForSeconds(1f);
        }
    }
}
