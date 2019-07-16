using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Script.AIGroup
{
    public class TargetInfo
    {
        public GameObject Me { get; set; }
        public GameObject Target { get; set; }
        public float TargetDis { get; set; }
        public RaycastHit TargetSightHit { get; set; }
        public float TargetAngle { get; set; }

        public float GetTargetDis()
        {
            TargetDis = Vector3.Distance(Me.transform.position, Target.transform.position);
            return TargetDis;
        }
        public float GetTargetAngle()
        {
            return Vector3.Angle(Me.transform.TransformDirection(Vector3.forward),
                        Target.transform.position - Me.transform.position);
        }
        public Transform ToTargetSight(float range)
        {
            RaycastHit TargetHit = new RaycastHit();
            //int mask = ~LayerMask.GetMask("Player","AI");
            //var MyPos = Me.transform.position + Me.GetComponent<NavMeshAgent>().height * Vector3.up;
            var TargetPos = Target.transform.position+Vector3.up/*Target.GetComponent<Collider>().bounds.center*/;

            var MyPos = Me.transform.position+Vector3.up;
            //var TargetPos = Target.transform.position;

            var AllHit = Physics.SphereCastAll(MyPos, .05f, TargetPos - MyPos, range, -1, QueryTriggerInteraction.Ignore);
            foreach (var hit in AllHit)
            {
                Debug.Log(hit.transform.name);
                if (hit.transform.GetComponentInParent<Avater.AvaterMain>())
                {
                    var avaterHit = hit.transform.GetComponentInParent<Avater.AvaterMain>();
                    if (avaterHit != Me.GetComponent<Avater.AvaterMain>())//以防打到自己
                    {
                        TargetHit = hit;
                        Debug.Log("Founded! " + TargetHit.transform.name);
                        break;
                    }
                }
                else
                {
                    Debug.Log("only get " + hit.transform.name);
                    break;
                }
            }
            TargetSightHit = TargetHit;
            return TargetHit.transform;
        }
        public Transform ToTargetSight()
        {
            RaycastHit TargetHit = new RaycastHit();

            var MyPos = Me.transform.position+Vector3.up*.5f;
            var TargetPos = Target.transform.position+Vector3.up*.7f;
            //var range = Me.GetComponent<Animator>().GetBehaviour<StateMachine>().TargetDis;
            //var AllHit = Physics.RaycastAll(Me.transform.position, Target.transform.position - Me.transform.position, range,-1,QueryTriggerInteraction.Ignore);
            //Physics.BoxCast(MyPos,Vector3.one*.1f, TargetPos - MyPos, out hit,Me.transform.rotation, range, -1, QueryTriggerInteraction.Ignore);
            var AllHit = Physics.SphereCastAll(MyPos, .05f, TargetPos - MyPos, 100f, -1, QueryTriggerInteraction.Ignore);
            foreach (var hit in AllHit)
            {
                //Debug.Log("Founded! " + hit.transform.name);

                if (hit.transform.GetComponentInParent<Avater.AvaterMain>())
                {
                    var avaterHit = hit.transform.GetComponentInParent<Avater.AvaterMain>();
                    if (avaterHit != Me.GetComponent<Avater.AvaterMain>())//以防打到自己
                    {
                        TargetHit = hit;
                        Debug.Log("Founded! " + TargetHit.transform.name);
                        break;
                    }
                }
                else
                {
                    //不管看到啥都先返回去
                    //Debug.Log("only get " + hit.transform.name);
                    break;
                }
            }
            TargetSightHit = TargetHit;
            return TargetHit.transform;
        }
    }
}
