using UnityEngine;
using Assets.Script.ActionControl;

namespace Assets.Script.Avater.Addon
{    
    class ParkourCollision : MonoBehaviour 
    {
        //public Collision contactThing;
        //public Collider triggerThing;

        public Animator animator;
        public Ray ray;
        public RaycastHit hit;
        private void Start() 
        {
            animator = GetComponent<Animator>();
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
            if(animator.GetCurrentAnimatorStateInfo(0).IsTag("falling"))
            {
                print(collision.gameObject.name);
            }
            //Debug.Log("foot: "+transform.position.y+" Ground: "+collision.contacts[0].point.y);

        }
        private void OnTriggerStay(Collider other) 
        {
            int layermask = LayerMask.GetMask("PostProcessing");
            layermask = ~layermask;
            if(!animator.GetBool("avater_IsParkour") && Physics.CheckBox(transform.position-Vector3.down*.1f,new Vector3(.001f,.2f,.001f),transform.rotation,layermask,QueryTriggerInteraction.Ignore) )
            {
                //Debug.Log("Grounded!");  
                //print(other.gameObject.name);              
                animator.SetBool("avater_IsLanded",true);
            }
            else
            {
                animator.SetBool("avater_IsLanded",false);
            }
        }
        private void OnTriggerEnter(Collider collider) 
        {
            //print(collider.name);
            if(collider.gameObject.tag == "wall")
            {
                ray.origin = transform.position;
                ray.direction = transform.position-collider.ClosestPoint(transform.position);
                RaycastHit temphit;
                //如果碰撞點不是在腳下就可以跑庫
                //向碰撞點射出雷射
                //triggerThing = collider;
                //忽略掉自己
                int layermask = LayerMask.GetMask("PostProcessing");
                layermask = ~layermask;
                //忽略掉自己+觸發物件
                if(Physics.Raycast(transform.position+Vector3.up*.5f,transform.TransformVector(Vector3.forward),out temphit,5f,layermask,QueryTriggerInteraction.Ignore))
                {             
                    //取得法線
                    hit = temphit;
                    print(hit.point-transform.position);
                    print(hit.transform.name);
                    //射線轉90度--找夾角
                    var q = Quaternion.AngleAxis(90,Vector3.up)*hit.normal;

                    var front = transform.TransformVector(Vector3.forward);
                    var angle = Vector3.Angle(
                        new Vector3(front.x,0,front.z),new Vector3(q.x,0,q.z));
                    
                    animator.SetFloat("avater_AngleBetweenWall",angle);
                    animator.SetTrigger("avater_parkour");//將動畫導向
                    //animator.SetBool("avater_can_parkour",true);
                    //print("Hitpoint"+collider.ClosestPoint(transform.position));
                } 
                //print(temphit.normal);
            }
        }
        private void OnTriggerExit(Collider collider) {
            if(collider.gameObject.tag == "wall")
            {
                //animator.SetTrigger("avater_exit");
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