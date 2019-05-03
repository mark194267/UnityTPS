using Assets.Script.ActionControl;
using Assets.Script.weapon;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

namespace Assets.Script.Avater
{
    /// <summary>
    /// 此為核心元件，非必要時別動這邊
    /// </summary>
    class PlayerAvater : AvaterMain
    {
        //武器欄
        //public Dictionary<int,string> PlayerWeaponDictionary;

        public AvaterDataLoader avaterDataLoader = new AvaterDataLoader();
        private MotionStatusBuilder statusBuilder = new MotionStatusBuilder();

        void Start()
        {
            avaterStatus = avaterDataLoader.LoadStatus("UnityChan");
            //暫時，初始化到時會交出去
            Init_Avater();
            //獲取腳色動作值
            motionStatusDir = statusBuilder.GetMotionList("UnityChan");
            //GetAnimaterParameter();
            
            //ActionScript.ChangeTarget(GameObject.Find("CommandCube").transform.Find("Imp").gameObject);
            WeaponFactory weaponFactory = new WeaponFactory();
            weaponFactory.Init();
            var GunDic = weaponFactory.AllWeaponDictionary;

            gameObject.GetComponent<Gun>().AddWeapon(GunDic["basicgun"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["MG"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["bazooka"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["katana"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["AK-47"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["Wakizashi"]);
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
            /*
            //在字典內找尋該動作的數值(待廢除)
            foreach (var actionStatuse in actionStatusDictionary.AllActionStatusDictionary)
            {
                if (Animator.GetCurrentAnimatorStateInfo(0).IsTag(actionStatuse.Key))
                {
                    NowActionStatus = actionStatuse.Value;
                }
            }
            */
            /*
            foreach (var motionStatus in motionStatusDir)
            {
                if (Animator.GetCurrentAnimatorStateInfo(0).IsName(motionStatus.Key))
                {
                    NowMotionStatus = motionStatus.Value;
                }
            }
            */
            /*
            //動作變了
            if (OldActionStatus != NowActionStatus)
            {
                //觸發動作結束
                //Debug.Log(OldActionStatus.ActionName);
                if(OldActionStatus != null)
                    ActionScript.AfterCustomAction(OldActionStatus);
                //觸發下個動作之前
                ActionScript.BeforeCustomAction(NowActionStatus);
                //撥開通用開關(可能會移除)
                //ActionScript.SetupBeforeAction(this.name,NowActionStatus.ActionName);
                //讀取該動作是否可進入其他動畫
                if (NowActionStatus.ignorelist != null)
                {
                    foreach (var cando in NowActionStatus.ignorelist)
                    {
                        Animator.SetBool("avater_can_" + cando, false);
                    }
                }
                //狀態更新+執行新狀態
                OldActionStatus = NowActionStatus;
            }

            IsEndNormal = ActionScript.CustomAction(NowActionStatus);
            Animator.SetBool("avater_IsEndNormal", IsEndNormal);
            */
            //檢查掉落速度
            if (GetComponent<Rigidbody>().velocity.y != 0)
            {
                Animator.SetFloat("avater_yspeed", GetComponent<Rigidbody>().velocity.y*-1f);
            }            
        }

        /// <summary>
        /// 落地檢查
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerStay(Collider other)
        {
            //忽略自己
            int layermask = LayerMask.GetMask("PostProcessing");
            layermask = ~layermask;
            if (/*!Animator.GetBool("avater_IsParkour") &&*/ Physics.CheckBox(transform.position - Vector3.down * .1f, new Vector3(.001f, .2f, .001f), transform.rotation, layermask, QueryTriggerInteraction.Ignore))
            {
                //Debug.Log("Grounded!");  
                //print(other.gameObject.name);              
                Animator.SetBool("avater_IsLanded", true);
            }
            else
            {
                Animator.SetBool("avater_IsLanded", false);
            }
        }

    }
}
