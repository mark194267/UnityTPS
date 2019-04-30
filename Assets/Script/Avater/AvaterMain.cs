using Assets.Script.ActionControl;
using Assets.Script.weapon;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Script.Avater
{
    public class AvaterMain : MonoBehaviour
    {
        //2019-04-24 增加動作值字典
        protected Dictionary<string, MotionStatus> motionStatusDir = new Dictionary<string, MotionStatus>();
        protected ActionBasicBuilder actionBasicBuilder = new ActionBasicBuilder();
        public ActionStatusDictionary actionStatusDictionary = new ActionStatusDictionary();
        protected Animator animator;

        public ActionBasic actionBasic = new ActionBasic();
        public AvaterStatus avaterStatus { get; set; }
        //2019-04-24 增加動作值
        public MotionStatus NowMotionStatus;
        public ActionStatus OldActionStatus;
        public ActionStatus NowActionStatus;
        private List<string> candolist = new List<string>();

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
            actionBasic = actionBasicBuilder.GetActionBaseByName(gameObject.name);
            actionBasic.Init(gameObject);
            animator = gameObject.GetComponent<Animator>();

            Hp = avaterStatus.Hp;
            Stun = 0;
            Atk = avaterStatus.Atk;

            stateMachine = animator.GetBehaviour<StateMachine>();
            stateMachine.me = gameObject;
            stateMachine.action = actionBasic;
            stateMachine.main = this;
        }

        public void OnDead()
        {

        }

        public void OnHit(int atk, double stun)
        {
            //先扣血
            Hp -= atk;
            if (Hp < 1)
            {
                print("i`m Dead!");
                animator.SetTrigger("avatermain_dead");

                //死了
                Destroy(gameObject, 3f);
            }

            //增加頓值
            Stun += stun;
            //如果頓值大於可以承受的頓值
            if (Stun >= avaterStatus.Stun /* and 倒下時不會加頓值 */)
            {
                //倒下並且重置頓值
                //重置路徑禁止行動
                GetComponent<NavMeshAgent>().ResetPath();
                animator.SetTrigger("avatermain_stun");
                Stun = 0;
            }
            //print(avaterStatus.Hp);
        }

        public void OnSpecial(string Name, float value)
        {
            animator.CrossFade(Name, 0);
            actionBasic.doOnlyOnce = true;
            NowActionStatus.ActionName = Name;
        }

        public void recover()
        {
            gameObject.GetComponent<Gun>().NowWeapon.weapon.GetComponent<Collider>().enabled = false;
            gameObject.GetComponent<NavMeshAgent>().enabled = true;
        }

        public void GetAnimationFlag(int flag)
        {
            //animator.SetInteger("anim_flag", flag);
            anim_flag = flag;
            //Debug.Log("hi");
        }

        public void GetAnimationIntMatrix(string intstring)
        {
            AnimationString = intstring.Split(',');            
        }
    }
}
