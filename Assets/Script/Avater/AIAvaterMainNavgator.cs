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
                if (NavMesh.SamplePosition(nextStepPos, out navMeshHit, 3f, -1))
                {
                    //該地點有地板A.K.A找的到合法的地點
                    RaycastHit hit;
                    Physics.Raycast(nextStepPos, Vector3.up, out hit, 10f);
                    var roof = hit.point.y;
                    if (hit.transform != null)
                    {
                        // 2019-11-28 找的到天花板.但是樣本點傳入時不一定是合法的點。
                        // 有可能傳入的點在天花板上
                        Debug.Log("Has Roof");
                        flyHeight = (roof + targetPos.y) / 4;
                    }
                    else
                        flyHeight = targetPos.y + 7f;
                    Debug.Log("Height=" + flyHeight + " Roof=" + roof);
                }
                else
                    Debug.Log("None Pos");//沒有可用的樣本點，呼叫會繼續用舊高度

                yield return new WaitForSeconds(3);
            }
        }
    }
}
