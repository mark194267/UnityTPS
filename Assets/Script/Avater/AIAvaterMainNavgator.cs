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
        public float flyHeight;
        public Vector3 nextStepPos;

        public override void Start()
        {
            base.Start();
            var Nav = Navgator.GetComponent<NavMeshAgent>();
            StartCoroutine(SetHeight());
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
        public void GetHeight(Vector3 point)
        {
            //StartCoroutine(SetHeight(point));
        }

        public IEnumerator SetHeight()
        {
            while (true)
            {
                var targetPos = targetInfo.Target.transform.position;
                Debug.Log(targetInfo.Target.transform.position);
                NavMeshHit navMeshHit;
                if (NavMesh.SamplePosition(nextStepPos, out navMeshHit, 1f, -1))
                {
                    //該地點有地板A.K.A找的到合法的地點
                    RaycastHit hit;
                    Physics.Raycast(nextStepPos, Vector3.up, out hit, 10f);
                    var roof = hit.point.y;
                    if (hit.transform != null)
                    {
                        // 2019-11-25 未測試
                        Debug.Log("Has Roof");
                        flyHeight = (roof + targetPos.y) / 2;
                    }
                    else
                        flyHeight = targetPos.y + 7f;
                    Debug.Log("Height=" + flyHeight + " Roof=" + roof);
                }
                else
                    Debug.Log("None Pos");

                yield return new WaitForSeconds(3);
            }
        }
    }
}
