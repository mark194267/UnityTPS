using UnityEngine;
using Assets.Script.ActionControl;

namespace Assets.Script.Avater.Addon
{
    class ParkourCollision : MonoBehaviour
    {
        //public Collision contactThing;
        //public Collider triggerThing;
        public bool canparkour;
        public bool canclimb;

        public float limitedDegree = 30;

        public Transform _climbChecker;
        public float climbHeight;
        public Vector3 climbVec;
        public float climbFwdDis;

        public Animator Animator;
        public RaycastHit hit { get; set; }
        private void Start()
        {
            Animator = GetComponent<Animator>();
        }
        void OnCollisionEnter(Collision collision)
        {
            //print("Enter!");
            //print(collision.gameObject.name);
            if (collision.transform.tag == "item")
            {
                //道具
            }
            if (collision.collider.gameObject.layer == 1)
            {

            }
            //Debug.Log("foot: "+transform.position.y+" Ground: "+collision.contacts[0].point.y);

        }

        private void OnTriggerStay(Collider collider)
        {
            if (canparkour)
            {
                RaycastHit temphit;
                int layermask = LayerMask.GetMask("Parkour");
                //layermask = ~layermask;
                if (Physics.Raycast(transform.position + Vector3.up * 1f, transform.TransformVector(Vector3.forward), out temphit, .6f, layermask, QueryTriggerInteraction.Ignore))
                {
                    //取得法線
                    hit = temphit;
                    //找垂直夾角...如果大於上下N度就不能跑庫
                    //射線轉90度--找夾角
                    var q = Quaternion.AngleAxis(90, Vector3.up) * hit.normal;
                    var front = transform.TransformVector(Vector3.forward);
                    var angle = Vector3.Angle(
                        new Vector3(front.x, 0, front.z), new Vector3(q.x, 0, q.z));

                    Animator.SetFloat("avater_AngleBetweenWall", angle);
                    Animator.SetBool("action_parkour", true);
                }
                else
                    Animator.SetBool("action_parkour", false);

            }

            #region 爬牆

            if (canclimb)
            {
                RaycastHit temphit;
                if (Physics.Raycast(_climbChecker.transform.position, Vector3.down, out temphit, climbHeight, ~LayerMask.GetMask("Player"),QueryTriggerInteraction.Ignore))
                {
                    if (Physics.Raycast(new Vector3(transform.position.x, temphit.point.y, transform.position.z), transform.TransformVector( Vector3.forward), out temphit, 1f, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore))
                    {
                        //Debug.Log(temphit.point);
                        Animator.SetBool("action_climb", true);
                        hit = temphit;

                    }
                }
                else
                    Animator.SetBool("action_climb", false);
            }

            #endregion

        }
        private void OnTriggerExit(Collider collider)
        {
            if (collider.gameObject.tag == "wall")
            {
                //Animator.SetTrigger("avater_exit");
            }
            Animator.SetBool("action_parkour", false);

        }
        /*
        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position+Vector3.up*.3f,transform.TransformVector(Vector3.forward*10f));
            //Gizmos.color = Color.blue;
            //Gizmos.DrawLine(transform.position,triggerThing.ClosestPoint(transform.position));
            //Gizmos.color = Color.red;
            //Gizmos.DrawSphere(triggerThing.ClosestPoint(transform.position),.2f);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(hit.point,Quaternion.AngleAxis(90,Vector3.up)*hit.normal);
            //Gizmos.DrawSphere(hit.point,.3f);
            //Gizmos.DrawSphere(Pos,.3f);
            //Gizmos.DrawSphere(col2,3);
        }
        */
    }
}