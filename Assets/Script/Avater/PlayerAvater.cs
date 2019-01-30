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
            
            // 跳躍復原
            if (!animator.GetBool("avater_can_jump") && collision.gameObject.layer == 1)
            {
                animator.SetBool("avater_can_jump",true);
                GetComponent<Rigidbody>().useGravity = false;
                GetComponent<Rigidbody>().isKinematic = true;
                GetComponent<NavMeshAgent>().enabled = true;
            }
            /*
            /// 新增2019/01/16
            /// 跑酷物理量
            /// 在不解除NavMeshAgent的情況下碰不到碰撞，故一定需要跳躍
            if(collision.collider.gameObject.tag == "wall"&&!animator.GetBool("avater_can_jump"))
            {
                //Debug.Log("hit");                
                animator.SetTrigger("avater_parkour");//將動畫導向
            }
            

            /// 2019/01/30
            /// 判斷夾角
            if(Physics.Raycast(transform.position,
            collision.contacts[0].point-transform.position,out hit))
            {
                //var vec = collision.relativeVelocity;
                var vec = collision.transform.TransformVector(hit.normal);
                var q = Quaternion.AngleAxis(0,Vector3.up)*vec;

                var front = transform.TransformVector(Vector3.forward);
                var angle = Vector2.Angle(
                    new Vector2(front.x,front.z),new Vector2(q.x,q.z));
                animator.SetFloat("avater_AngleBetweenWall",angle);
                print(angle);
            } 
        */
        }

        private void OnTriggerEnter(Collider collider) 
        {
            if(collider.gameObject.tag == "wall"&&!animator.GetBool("avater_can_jump"))
            {
                if(Physics.Raycast(transform.position,
                collider.transform.position-transform.position,out hit))
                {
                    var vec = collider.transform.TransformVector(hit.normal);
                    var q = Quaternion.AngleAxis(0,Vector3.up)*vec;

                    var front = transform.TransformVector(Vector3.forward);
                    var angle = Vector2.Angle(
                        new Vector2(front.x,front.z),new Vector2(q.x,q.z));
                    
                    animator.SetFloat("avater_AngleBetweenWall",angle);
                    print(angle);
                } 
                animator.SetTrigger("avater_parkour");//將動畫導向
            }
        }
        void GetAnimationFlag(int anim_flag)
        {
            animator.SetInteger("anim_flag",anim_flag);
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
