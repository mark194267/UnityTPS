using Assets.Script.ActionControl;
using Assets.Script.weapon;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

namespace Assets.Script.Avater
{
    class PlayerAvater : AvaterMain
    {
        public Collision contactThing;
        public Collider triggerThing;
        public Ray ray;
        public RaycastHit hit;
        public Dictionary<int,string> PlayerWeaponDictionary;
        void Start()
        {
            //暫時，初始化到時會交出去
            Init_Avater();
            //GetAnimaterParameter();
            
            //actionBasic.ChangeTarget(GameObject.Find("CommandCube").transform.Find("Imp").gameObject);
            WeaponFactory weaponFactory = new WeaponFactory();
            weaponFactory.Init();
            var GunDic = weaponFactory.AllWeaponDictionary;

            gameObject.GetComponent<Gun>().AddWeapon(GunDic["basicgun"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["MG"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["bazooka"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["katana"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["AK-47"]);
            gameObject.GetComponent<Gun>().CreateWeaponByList();
            gameObject.GetComponent<Gun>().cam = gameObject.transform.Find("Camera").GetComponent<MouseOrbitImproved>();
            
            /// 未來可能在此增加射線管理員
            /// 先保留於此
            /// StartCoroutine(GetRayCast());
            
            //GrabLedgePoint = transform.Find("GrabLedgePoint").transform;
            //gameObject.GetComponent<Gun>().ChangeWeapon(PlayerWeaponDictionary[0]);
        }
        void Update()
        {
            //在字典內找尋該動作的數值(待廢除)
            foreach (var actionStatuse in actionStatusDictionary.AllActionStatusDictionary)
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsTag(actionStatuse.Key))
                {
                    NowActionStatus = actionStatuse.Value;
                }
            }
            //動作變了
            if (OldActionStatus != null && OldActionStatus != NowActionStatus)
            {
                //觸發動作結束
                //Debug.Log(OldActionStatus.ActionName);
                actionBasic.AfterCustomAction(OldActionStatus);
                //觸發下個動作之前
                actionBasic.BeforeCustomAction(NowActionStatus);
                //撥開通用開關(可能會移除)
                actionBasic.SetupBeforeAction();
                //讀取該動作是否可進入其他動畫
                RefreshAnimaterParameter();
                if (NowActionStatus.ignorelist != null)
                {
                    foreach (var cando in NowActionStatus.ignorelist)
                    {
                        animator.SetBool("avater_can_" + cando, false);
                    }
                }
            }
            
            //狀態更新+執行新狀態
            OldActionStatus = NowActionStatus;
            IsEndNormal = actionBasic.CustomAction(NowActionStatus);
            animator.SetBool("avater_IsEndNormal", IsEndNormal);

            //檢查掉落速度
            if (GetComponent<Rigidbody>().velocity.y != 0)
            {
                animator.SetFloat("avater_yspeed", GetComponent<Rigidbody>().velocity.y);
            }
            
        }
        /// <summary>
        /// 主射線，用子程式超載主檔的Ray ray;
        /// </summary>
        /// <returns></returns>
        /*
        IEnumerator GetRayCast()
        {

            yield return new WaitForSeconds(.05f); 
        }
        */
        void OnCollisionEnter(Collision collision)
        {   
            print("Enter!");
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
            if(!animator.GetBool("avater_IsParkour") && Physics.CheckBox(transform.position-Vector3.down*.1f,new Vector3(.001f,.2f,.001f),transform.rotation,-1,QueryTriggerInteraction.Ignore) )
            {
                Debug.Log("Grounded!");  
                print(other.gameObject.name);              
                animator.SetBool("avater_IsLanded",true);
            }
            else
            {
                animator.SetBool("avater_IsLanded",false);
            }
        }
        private void OnTriggerEnter(Collider collider) 
        {
            print(collider.name);
            if(collider.gameObject.tag == "wall")
            {
                ray.origin = transform.position;
                ray.direction = transform.position-collider.ClosestPoint(transform.position);
                RaycastHit temphit;
                //如果碰撞點不是在腳下就可以跑庫
                //向碰撞點射出雷射
                triggerThing = collider;
                if(Physics.Raycast(transform.position,transform.TransformVector(Vector3.forward),out temphit))
                {             
                    //取得法線
                    hit = temphit;
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
                print(temphit.normal);
            } 
        }
        void GetAnimationFlag(int anim_flag)
        {
            animator.SetInteger("anim_flag",anim_flag);
        }
        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position,triggerThing.ClosestPoint(transform.position));
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(triggerThing.ClosestPoint(transform.position),.2f);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(hit.point,Quaternion.AngleAxis(90,Vector3.up)*hit.normal);
            //Gizmos.DrawSphere(hit.point,.3f);
            //Gizmos.DrawSphere(Pos,.3f);
            //Gizmos.DrawSphere(col2,3);
        }
    }
}
