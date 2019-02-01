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
        private float alarm;    
        public bool cangrab;        
        private Vector3 DrawPoint = new Vector3();
        public Transform GrabLedgePoint{get;set;}

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
            RaycastHit hit;
            Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit,1);

            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right));
            //Debug.DrawRay(hit.point, Quaternion.AngleAxis(90f, Vector3.up)*hit.normal,Color.yellow);
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
            
            if(collision.transform.tag == "item")
            {
                //道具
            }
            if (collision.collider.gameObject.layer == 1)
            {
                animator.SetBool("avater_IsLanded",true);
            }
        }

        private void OnCollisionExit(Collision collision) 
        {
            if (collision.collider.gameObject.layer == 1)
            {
                animator.SetBool("avater_IsLanded",false);
            }
        }
        
        private void OnTriggerStay(Collider collider) 
        {
                        // 跳躍復原

            if(!animator.GetBool("avater_IsLanded")&&collider.gameObject.tag == "wall")
            {
                RaycastHit temphit;
                //如果碰撞點不是在腳下就可以跑庫
                //向碰撞點射出雷射
                if(Physics.Raycast(transform.position,
                collider.ClosestPoint(transform.position)-transform.position,out temphit))
                {             
                    //if(temphit.normal.y == 0) return;       
                    //取得法線
                    hit = temphit;
                    var vec = hit.normal;
                    print(hit.normal);
                    //轉90度--找夾角
                    var q = Quaternion.AngleAxis(90,Vector3.up)*vec;
                    
                    //紅線代表著碰到的點
                    Debug.DrawLine(transform.position,collider.ClosestPoint(transform.position),Color.red);
                    //黃線代表著轉90度後的的向量
                    Debug.DrawRay(transform.position,q,Color.yellow);
                    //綠線代表著法線
                    Debug.DrawRay(hit.point,vec,Color.green);

                    
                    //如果前面有東西就是三角跳--第二優先
                    //側邊有東西就是跑牆
                    var front = transform.TransformVector(Vector3.forward);
                    var angle = Vector3.Angle(
                        new Vector3(front.x,0,front.z),new Vector3(q.x,0,q.z));
                    
                    animator.SetFloat("avater_AngleBetweenWall",angle);
                    animator.SetTrigger("avater_parkour");//將動畫導向
                } 

            }
        }
        
        void GetAnimationFlag(int anim_flag)
        {
            animator.SetInteger("anim_flag",anim_flag);
        }
        void OnDrawGizmos()
        {
        }
    }
}
