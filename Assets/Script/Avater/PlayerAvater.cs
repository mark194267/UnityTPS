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
                //actionBasic.SetupBeforeAction(this.name,NowActionStatus.ActionName);
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

        void GetAnimationFlag(int anim_flag)
        {
            animator.SetInteger("anim_flag",anim_flag);
        }
    }
}
