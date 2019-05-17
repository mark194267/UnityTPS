using UnityEngine;
using Assets.Script.ActionControl;

namespace Assets.Script.Avater.Addon
{    
    class ParkourCollision : MonoBehaviour 
    {
        //public Collision contactThing;
        //public Collider triggerThing;

        public Animator Animator;
        public RaycastHit hit;
        private void Start() 
        {
            Animator = GetComponent<Animator>();
        }
        void OnCollisionEnter(Collision collision)
        {   
            //print("Enter!");
            //print(collision.gameObject.name);
            if(collision.transform.tag == "item")
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
            //print(collider.gameObject.tag);
            if(collider.gameObject.tag == "wall")
            {
                RaycastHit temphit;
                //如果碰撞點不是在腳下就可以跑庫
                //向碰撞點射出雷射
                //triggerThing = collider;
                //忽略掉自己
                int layermask = LayerMask.GetMask("Player","AI");
                layermask = ~layermask;
                //忽略掉自己+觸發物件
                if(Physics.Raycast(transform.position+Vector3.up*.5f,transform.TransformVector(Vector3.forward),out temphit,2f,layermask,QueryTriggerInteraction.Ignore))
                {             
                    //取得法線
                    hit = temphit;
                    //print(hit.point-transform.position);
                    //print(hit.transform.name);
                    //射線轉90度--找夾角
                    var q = Quaternion.AngleAxis(90,Vector3.up)*hit.normal;

                    var front = transform.TransformVector(Vector3.forward);
                    var angle = Vector3.Angle(
                        new Vector3(front.x,0,front.z),new Vector3(q.x,0,q.z));
                    
                    Animator.SetFloat("avater_AngleBetweenWall",angle);
                    //Animator.SetTrigger("avater_parkour");//將動畫導向
                    Animator.SetBool("avater_can_parkour",true);
                    //print("Hitpoint"+collider.ClosestPoint(transform.position));
                } 
                else
                    Animator.SetBool("avater_can_parkour", false);
                //print(temphit.normal);
            }
        }
        private void OnTriggerExit(Collider collider) {
            if(collider.gameObject.tag == "wall")
            {
                //Animator.SetTrigger("avater_exit");
            }
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