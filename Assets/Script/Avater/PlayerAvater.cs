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

        public GameObject col;
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
            gameObject.GetComponent<Gun>().CreateWeaponByList();
            gameObject.GetComponent<Gun>().cam = gameObject.transform.Find("Camera").GetComponent<MouseOrbitImproved>();
            
            /// 未來可能在此增加射線管理員
            /// 先保留於此
            /// StartCoroutine(GetRayCast());
            
            //GrabLedgePoint = transform.Find("GrabLedgePoint").transform;
            //gameObject.GetComponent<Gun>().ChangeWeapon(PlayerWeaponDictionary[0]);
            //gameObject.GetComponent<Gun>().ChangeWeapon("MG");
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
            
            // 跳躍復原
            if (!animator.GetBool("avater_can_jump") && collision.gameObject.layer == 1)
            {
                animator.SetBool("avater_can_jump",true);
                GetComponent<Rigidbody>().useGravity = false;
                GetComponent<Rigidbody>().isKinematic = true;
                GetComponent<NavMeshAgent>().enabled = true;
            }
            
            /// 新增2019/01/16
            /// 跑酷物理量
            /// 在不解除NavMeshAgent的情況下碰不到碰撞，故一定需要跳躍
            if(collision.gameObject.tag == "wall")
            {
                //Debug.Log("hit");                
                animator.SetTrigger("avater_parkour");//將動畫導向
                col = collision.gameObject;
            }
            

            /// 2019/01/17 
            /// 判斷夾角
            var vec = collision.relativeVelocity;
            var front = transform.TransformDirection(transform.forward);
            //var angle = Vector3.Angle(transform.TransformDirection(transform.forward),vec);
             var angle = Vector2.Angle(
                new Vector2(front.x,front.z),new Vector2(vec.x,vec.z));

            animator.SetFloat("avater_AngleBetweenWall",angle);
            //print(angle);
        }

        void OnCollisionStay(Collision collision)
        {
            #region 跑牆
            /// 2019/01/17 跑酷
            /// 改以觸發器偵測
            /*
            if (collision.gameObject.tag == "wall")
            {
                foreach (ContactPoint item in collision)
                {
                    Debug.Log(item.point);
                }
                //DrawPoint = collision.collider.ClosestPoint(transform.TransformPoint(GrabLedgePoint));
                //DrawPoint = transform.TransformPoint(GrabLedgePoint);
                //DrawPoint = collision.collider.ClosestPointOnBounds(transform.position);                
                //DrawPoint = gameObject.GetComponentInChildren<Renderer>().bounds.ClosestPoint(DrawPoint);
                GetComponent<Rigidbody>().isKinematic = true;
            }
            */  
            #endregion

            #region 掛豬肉
            /// 2019/01/17 掛豬肉
            /// 
            if(collision.gameObject.tag == "wall")
            {
                if(!Physics.CheckBox(transform.position+new Vector3(-0.2f,1.7f,0),new Vector3(.05f,.05f,.3f)))
                {
                    if(Physics.CheckBox(transform.position+new Vector3(-0.2f,1.6f,.2f),new Vector3(.05f,.05f,.05f)) &&
                        Physics.CheckBox(transform.position+new Vector3(-0.2f,1.6f,-.2f),new Vector3(.05f,.05f,.05f)))
                    {
                        //可以掛
                    }
                    //GetComponent<Rigidbody>().isKinematic = true;
                }
                //檢查頭頂前的方盒是否有東西
                //檢查頭頂+右邊值向前的方盒是否有東西
                //檢查頭頂+左邊值向前的方盒是否有東西
            }
            #endregion       
            
            #region 踢壁跳
            if(collision.transform.tag == "wall")
            {
                if(Physics.CheckBox(transform.position+new Vector3(-0.2f,1.6f,0),new Vector3(.05f,.05f,.05f)))
                {
                    //可以踢
                }
            }
            #endregion
            
            #region 撿物件
            if(collision.transform.tag == "Pickup")
            {
                if(Input.GetButtonDown("use"))
                {
                    //撿起物品
                }

            }

            if(collision.transform.tag == "weapon")
            {
                //在GUI顯示即將撿起的武器名稱
                if(Input.GetButtonDown("use"))
                {
                    //改變動畫為交換武器
                    //增加武器到清單
                }
            }
            #endregion
        }
        
        //藉由動畫觸發avater_can_parkour來控制跑酷的開啟時段避免黏牆，動作未完成
        void ParkourReciver(bool anim_flag)
        {
            animator.SetBool("anim_flag",anim_flag);
        }
        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            //抓取邊緣的箱子位置
            //Gizmos.DrawWireCube(transform.position+ transform.TransformVector(new Vector3(0,1.7f, 0.2f)), new Vector3(.2f,.1f,.5f));
            //Gizmos.DrawWireCube(transform.position+ transform.TransformVector(new Vector3(0.2f,1.6f,0.2f)), Vector3.one * .1f);
            //Gizmos.DrawWireCube(transform.position+ transform.TransformVector(new Vector3(-0.2f,1.6f,0.2f)), Vector3.one*.1f);
            //Gizmos.DrawCube(gameObject.GetComponent<Rigidbody>().position +transform.TransformVector(Vector3.right*0.5f), Vector3.one * .1f);
            //Gizmos.DrawCube(gameObject.GetComponent<Rigidbody>().position + transform.TransformVector(Vector3.left * 0.3f), Vector3.one * .1f);

        }
    }
}
