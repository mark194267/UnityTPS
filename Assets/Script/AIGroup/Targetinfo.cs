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
        public float maxDistance = 100f;
        public float meSightHset = .5f;
        public float targetSightHset = .7f;

        public GameObject Me { get; set; }
        public GameObject Target { get; set; }
        public float TargetDis
        {
            get
            {
                return GetTargetDis();
            }
        }
        public RaycastHit TargetSightHit
        {
            get
            {
                return GetTargetSight(); 
            }
        }
        public float TargetAngle
        {
            get
            {
                return GetTargetAngle();
            }
        }

        private float GetTargetDis()
        {
            return Vector3.Distance(Me.transform.position, Target.transform.position);
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
            var MyPos = Me.transform.position + Vector3.up * meSightHset;
            var TargetPos = Target.transform.position + Vector3.up * targetSightHset;
            //var TargetPos = Target.transform.position;

            var AllHit = Physics.SphereCastAll(MyPos, .05f, TargetPos - MyPos, range, -1, QueryTriggerInteraction.Ignore);
            System.Array.Sort(AllHit, (x, y) => x.distance.CompareTo(y.distance));
            foreach (var hit in AllHit)
            {
                //Debug.Log(hit.transform.name);
                //可依照看到的物件減掉權重值來決定是否被發現
                if (hit.transform.GetComponentInParent<Avater.AvaterMain>())
                {
                    var avaterHit = hit.transform.GetComponentInParent<Avater.AvaterMain>();
                    if (avaterHit != Me.GetComponent<Avater.AvaterMain>())//以防打到自己
                    {
                        TargetHit = hit;
                        //Debug.Log("Founded! " + TargetHit.transform.name);
                        break;
                    }
                }
                else
                {
                    //Debug.Log("final get " + hit.transform.name);
                    break;
                }
            }
            //TargetSightHit = TargetHit;
            return TargetHit.transform;
        }
        public Transform ToTargetSight()
        {
            RaycastHit TargetHit = new RaycastHit();

            var MyPos = Me.transform.position + Vector3.up * meSightHset;
            var TargetPos = Target.transform.position + Vector3.up * targetSightHset;
            //var range = Me.GetComponent<Animator>().GetBehaviour<StateMachine>().TargetDis;
            //var AllHit = Physics.RaycastAll(Me.transform.position, Target.transform.position - Me.transform.position, range,-1,QueryTriggerInteraction.Ignore);
            //Physics.BoxCast(MyPos,Vector3.one*.1f, TargetPos - MyPos, out hit,Me.transform.rotation, range, -1, QueryTriggerInteraction.Ignore);
            var AllHit = Physics.SphereCastAll(MyPos, .05f, TargetPos - MyPos, 100f, -1, QueryTriggerInteraction.Ignore);
            System.Array.Sort(AllHit, (x, y) => x.distance.CompareTo(y.distance));

            foreach (var hit in AllHit)
            {
                //Debug.Log("Founded! " + hit.transform.name);

                if (hit.transform.GetComponentInParent<Avater.AvaterMain>())
                {
                    var avaterHit = hit.transform.GetComponentInParent<Avater.AvaterMain>();
                    if (avaterHit != Me.GetComponent<Avater.AvaterMain>())//以防打到自己
                    {
                        TargetHit = hit;
                        //Debug.Log("Founded! " + TargetHit.transform.name);
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
            //TargetSightHit = TargetHit;
            return TargetHit.transform;
        }
        private RaycastHit GetTargetSight()
        {
            RaycastHit TargetHit = new RaycastHit();

            var MyPos = Me.transform.position + Vector3.up * meSightHset;
            var TargetPos = Target.transform.position + Vector3.up * targetSightHset;
            var AllHit = Physics.SphereCastAll(MyPos, .05f, TargetPos - MyPos, maxDistance, -1, QueryTriggerInteraction.Ignore);
            System.Array.Sort(AllHit, (x, y) => x.distance.CompareTo(y.distance));

            foreach (var hit in AllHit)
            {
                //Debug.Log("Founded! " + hit.transform.name);

                if (hit.transform.GetComponentInParent<Avater.AvaterMain>())
                {
                    var avaterHit = hit.transform.GetComponentInParent<Avater.AvaterMain>();
                    if (avaterHit != Me.GetComponent<Avater.AvaterMain>())//以防打到自己
                    {
                        TargetHit = hit;
                        //Debug.Log("Founded! " + TargetHit.transform.name);
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
            //TargetSightHit = TargetHit;
            return TargetHit;
        }
    }
}
