using Assets.Script.ActionControl;
using Assets.Script.weapon;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Script.Avater
{
    public class AvaterMain : MonoBehaviour
    {
        //2019-04-24 增加動作值字典x
        public Dictionary<string, MotionStatus> motionStatusDir = new Dictionary<string, MotionStatus>();
        protected ActionScriptBuilder ActionScriptBuilder = new ActionScriptBuilder();
        public ActionStatusDictionary actionStatusDictionary = new ActionStatusDictionary();
        protected Animator Animator;

        public ActionScript ActionScript = new ActionScript();
        public AvaterStatus avaterStatus { get; set; }
        //2019-04-24 增加動作值
        public MotionStatus MotionStatus;
        public List<string> candolist = new List<string>();

        public float ActionElapsedTime;
        public bool IsEndNormal = true;
        public int anim_flag;
        public StateMachine stateMachine { get; set; }

        protected int Hp { get; set; }
        protected double Stun { get; set; }
        protected int Atk { get; set; }

        protected string[] AnimationString { get; set; }

        public void Init_Avater()
        {
            actionStatusDictionary.Init(gameObject.name);
            ActionScript = ActionScriptBuilder.GetActionBaseByName(gameObject.name);
            ActionScript.Init(gameObject);
            Animator = gameObject.GetComponent<Animator>();

            Hp = avaterStatus.Hp;
            Stun = 0;
            Atk = avaterStatus.Atk;

            stateMachine = Animator.GetBehaviour<StateMachine>();
            stateMachine.me = gameObject;
            stateMachine.action = ActionScript;
            stateMachine.AvaterMain = this;

            foreach (var animaterparameter in Animator.parameters)
            {
                if (animaterparameter.name.StartsWith("avater_can"))
                {
                    candolist.Add(animaterparameter.name);
                }
            }
        }

        public void OnDead()
        {

        }

        public void OnHit(int atk, double stun)
        {
            //print(gameObject.name + " say:i`m hit!");

            //先扣血
            Hp -= atk;

            //增加頓值
            Stun += stun;
            if (Hp < 1)
            {
                //print("i`m Dead!");
                //Animator.SetTrigger("avatermain_dead");

                Animator.enabled = false;
                GetComponent<NavMeshAgent>().enabled = false;
                GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<Gun>().NowWeapon[0].weapon.SetActive(false);
                //死了
                Destroy(gameObject, 30f);
            }
            //如果頓值大於可以承受的頓值
            else if (Stun >= avaterStatus.Stun /* and 倒下時不會加頓值 */)
            {
                //倒下並且重置頓值
                //重置路徑禁止行動
                //print("i`m stun!");
                /*
                if(GetComponent<NavMeshAgent>() != null)
                    GetComponent<NavMeshAgent>().ResetPath();
                */
                Animator.SetTrigger("avatermain_stun");
                //Animator.enabled = false;
                Stun = 0;
            }
            //print(avaterStatus.Hp);
        }

        public void recover()
        {
            gameObject.GetComponent<Gun>().NowWeapon[0].weapon.GetComponent<Collider>().enabled = false;
            gameObject.GetComponent<NavMeshAgent>().enabled = true;
        }

        public void GetAnimationFlag(int flag)
        {
            //Animator.SetInteger("anim_flag", flag);
            anim_flag = flag;
            //Debug.Log("hi");
        }

        public void GetAnimationIntMatrix(string intstring)
        {
            AnimationString = intstring.Split(',');            
        }
    }
}
