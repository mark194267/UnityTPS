using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Script.AIGroup;
using Assets.Script.Avater;
using Assets.Script.Config;
using Assets.Script.weapon;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Script.ActionControl
{
    public class ActionBasicBuilder
    {
        public ActionBasic GetActionBaseByName(string ActionName)
        {
            Type t = Type.GetType("Assets.Script.ActionList." + ActionName+"Action");
            ActionBasic actionBase = new ActionBasic();
            actionBase = (ActionBasic)Activator.CreateInstance(t);
            return actionBase;
        }
    }
    /// <summary>
    /// 這裡負責關於人物的面向，移動，動作
    /// </summary>
    public class ActionBasic
    {
        public Animator animator;
        public AIPath aiPathManager;
        public GameObject my;
        public GameObject target;
        public NavMeshPath myPath;

        protected Rigidbody myRig;
        protected NavMeshAgent myAgent;
        protected Vector3 targetPos;
        protected Vector3 NowVecter;
        protected InputManager input;

        protected Camera camera;

        protected Gun gun;

        protected float actionElapsedTime;
        protected AvaterMain main;
        public bool doOnlyOnce;

        public void Init(GameObject my)
        {
            this.my = my;
            //this.myAgent = this.my.GetComponent<NavMeshAgent>();
            this.myAgent = this.my.GetComponentInChildren<NavMeshAgent>();
            this.gun = my.GetComponent<Gun>();
            this.animator = my.GetComponent<Animator>();
            this.myRig = my.GetComponent<Rigidbody>();
            this.main = my.GetComponent<AvaterMain>();
            if (my.GetComponent<InputManager>())
            {
                input = my.GetComponent<InputManager>();
            }

            if(my.transform.Find("Camera"))
            {
                camera = my.transform.Find("Camera").GetComponent<Camera>();
            }
            aiPathManager = GameObject.Find("Vulcan").GetComponent<AIPath>();
            actionElapsedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
        }
        
        #region 自訂狀態機
        public void BeforeCustomAction(ActionStatus actionStatus)
        {
            Type actionbaseType = GetType();
            MethodInfo methodInfo = actionbaseType.GetMethod("Before_"+actionStatus.ActionName);
            if (methodInfo != null)
                methodInfo.Invoke(this, new object[] { actionStatus }/*放入actionStatus*/);
        }
        /// <summary>
        /// 自訂狀態機，預設的狀態機在ActionBasic裡面
        /// </summary>
        /// <param name="actionStatus">狀態參數</param>
        /// <returns></returns>
        public bool CustomAction(ActionStatus actionStatus)
        {
            Type actionbaseType = GetType();
            MethodInfo methodInfo = actionbaseType.GetMethod(actionStatus.ActionName);
            return (bool)methodInfo.Invoke(this, new object[] { actionStatus }/*放入actionStatus*/);
        }

        public void AfterCustomAction(ActionStatus actionStatus)
        {
            Type actionbaseType = GetType();
            MethodInfo methodInfo = actionbaseType.GetMethod("After_"+actionStatus.ActionName);
            if (methodInfo != null)
                methodInfo.Invoke(this, new object[] { actionStatus }/*放入actionStatus*/);
        }
        public bool CustomAction(string actionName)
        {
            //var actionStatus = FindByNameDic(action, type);
            Type actionbaseType = GetType();
            MethodInfo methodInfo = actionbaseType.GetMethod(actionName);
            return (bool)methodInfo.Invoke(this, null/*放入actionStatus*/);
        }

        #endregion

        #region 多型緩和迴轉

        /// <summary>
        /// 緩和旋轉，預設速度為 1.7f*deltatime
        /// </summary>
        /// <param name="target">轉向目標，方法為lookRotation</param>
        protected void RotateTowardSlerp(Transform target)
        {
            Vector3 direction = (target.position - my.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3
            my.transform.rotation = Quaternion.Slerp(my.transform.rotation, lookRotation, Time.deltaTime * 1.7f/*rotationSpeed*/);
        }

        /// <summary>
        /// 緩和旋轉，預設速度為 1.7f*deltatime
        /// </summary>
        /// <param name="target">轉向目標，方法為lookRotation</param>
        protected void RotateTowardSlerp(Vector3 target)
        {
            Vector3 direction = (target - my.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3
            my.transform.rotation = Quaternion.Slerp(my.transform.rotation, lookRotation, Time.deltaTime * 1.7f/*rotationSpeed*/);
        }

        /// <summary>
        /// 緩和旋轉，自訂速度
        /// </summary>
        /// <param name="target">轉向目標，方法為lookRotation</param>
        /// <param name="speed">轉向速度</param>
        protected void RotateTowardSlerp(Vector3 target,float speed)
        {
            Vector3 direction = (target - my.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3
            my.transform.rotation = Quaternion.Slerp(my.transform.rotation, lookRotation, Time.deltaTime * speed/*rotationSpeed*/);
        }

        /// <summary>
        /// 緩和旋轉，大多可用SLerp取代，預設速度為 1.7f*deltatime
        /// </summary>
        /// <param name="target">轉向目標，方法為lookRotation</param>
        protected void RotateTowardlerp(Transform target)
        {
            Vector3 direction = (target.position - my.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3
            my.transform.rotation = Quaternion.Lerp(my.transform.rotation, lookRotation, Time.deltaTime * 1.7f/*rotationSpeed*/);
        }

        /// <summary>
        /// 緩和旋轉，大多可用SLerp取代，預設速度為 1.7f*deltatime
        /// </summary>
        /// <param name="target">轉向目標，方法為lookRotation</param>
        /// 
        protected void RotateTowardlerp(Vector3 target)
        {
            Vector3 direction = (target - my.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3
            my.transform.rotation = Quaternion.Lerp(my.transform.rotation, lookRotation, Time.deltaTime * 3f/*rotationSpeed*/);
        }

        protected void RotateTowardlerp(Vector3 target, float speed)
        {
            Vector3 direction = (target - my.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3
            my.transform.rotation = Quaternion.Lerp(my.transform.rotation, lookRotation, Time.deltaTime * speed/*rotationSpeed*/);
        }
        #endregion

        #region 轉角檢測+路線
        /// <summary>
        /// 只做轉角檢測的進攻路線
        /// </summary>
        /// <returns></returns>
        protected Vector3 takecover()
        {
            NavMeshPath path = new NavMeshPath();
            myAgent.CalculatePath(target.transform.position, path);

            int i = path.corners.Length;
            while ( i-3 > 0)
            {
                //得到倒數2個轉角的位置
                Vector3 point1 = path.corners[i - 2];
                Vector3 point2 = path.corners[i - 3];
                //得到最後一條往轉角的路徑
                Vector3 ToLastCorner = point1 - point2;
                //在獲取目標到轉角的向量
                Vector3 TargetToCorner = target.transform.position - point1;
                //如果兩向量夾角太小，距離太近，就不會過第二個轉角
                if (Vector2.Angle(
                        new Vector2(ToLastCorner.x, ToLastCorner.z),
                        new Vector2(TargetToCorner.x, TargetToCorner.z)) > 45
                    && Vector3.Distance(point1, point2) > .5)
                {
                    //myAgent.SetDestination(point1);
                    return point1;
                }
                else
                {
                    i--;
                }
            }
            return path.corners[path.corners.Length - 3];
        }

        #endregion

        /// <summary>
        /// AI通用的切換目標，保持用此函式來切換目標
        /// </summary>
        /// <param name="targetGameObject"></param>
        public void ChangeTarget(GameObject targetGameObject)
        {
            this.target = targetGameObject;
        }

        #region 基礎，通用動作

        public virtual void Before_idle(ActionStatus actionStatus)
        {
            myAgent.ResetPath();
        }
        
        public virtual bool idle(ActionStatus actionStatus)
        {
            return true;
        }
        
        public virtual void Before_move(ActionStatus actionStatus)
        {
            myAgent.SetDestination(target.transform.position);
        }
        
        public virtual bool move(ActionStatus actionStatus)
        {
            return true;
        }
        
        public virtual void Before_shoot(ActionStatus actionStatus)
        {
            myAgent.ResetPath();
        }

        public virtual bool shoot(ActionStatus actionStatus)
        {
            gun.fire();
            return true;
        }

        public virtual void Before_reload(ActionStatus actionStatus)
        {
            myAgent.ResetPath();
        }
        
        public virtual bool reload(ActionStatus actionStatus)
        {
            return true;
        }
        
        public virtual void Before_dead(ActionStatus actionStatus)
        {
            AddCostArea();
            myAgent.enabled = false;
            myRig.isKinematic = false;
            gun.NowWeapon.weapon.GetComponent<Collider>().enabled = false;
        }

        public virtual bool dead(ActionStatus actionStatus)
        {
            return true;
        }
        
        #endregion

        #region 第一/第三人稱式移動

        /// <summary>
        /// 第一人稱式移動，移動的主體是NavAgent，若沒Agent請改用FPSLikeRigMovement
        /// </summary>
        /// <param name="moveSpeed"></param>
        /// <param name="rotSpeed"></param>
        public void FPSLikeMovement(float moveSpeed,float rotSpeed)
        {
            if(Input.anyKey)
            {
                var dir = 
                    camera.transform.TransformDirection(Vector3.right*input.ad+Vector3.forward*input.ws);
                myAgent.velocity = Vector3.ClampMagnitude(dir.normalized*7f,moveSpeed);
            }
            var camPos = camera.transform.TransformDirection(Vector3.back);
            RotateTowardlerp(my.transform.position-camPos,rotSpeed);
        }
        /// <summary>
        /// 第一人稱式移動，移動的主體是NavAgent，若沒Agent請改用FPSLikeRigMovement
        /// </summary>
        /// <param name="baseSpeed"></param>
        /// <param name="maxSpeed"></param>
        /// <param name="rotSpeed"></param>
        public void FPSLikeMovement(float baseSpeed,float maxSpeed,float rotSpeed)
        {
            if(Input.anyKey)
            {
                var dir = 
                    camera.transform.TransformDirection(Vector3.right*input.ad+Vector3.forward*input.ws);
                myAgent.velocity = Vector3.ClampMagnitude(dir.normalized*baseSpeed,maxSpeed);
            }
            var camPos = camera.transform.TransformDirection(Vector3.back);
            RotateTowardlerp(my.transform.position-camPos,rotSpeed);
        }
        /// <summary>
        /// 第一人稱式移動，預設速度為7f
        /// </summary>
        /// <param name="maxSpeed"></param>
        /// <param name="rotSpeed"></param>
        public void FPSLikeRigMovement(float maxSpeed, float rotSpeed)
        {
            if (Input.anyKey)
            {
                var dir =
                    camera.transform.TransformDirection(Vector3.right * input.ad + Vector3.forward * input.ws);
                dir.y = 0;
                myRig.velocity = Vector3.ClampMagnitude(dir.normalized * 7f, maxSpeed);
            }
            var camPos = camera.transform.TransformDirection(Vector3.back);
            RotateTowardlerp(my.transform.position - camPos, rotSpeed);
        }
        /// <summary>
        /// 第一人稱式移動，自訂速度
        /// </summary>
        /// <param name="baseSpeed"></param>
        /// <param name="maxSpeed"></param>
        /// <param name="rotSpeed"></param>
        public void FPSLikeRigMovement(float baseSpeed, float maxSpeed, float rotSpeed)
        {
            if (Input.anyKey)
            {
                var dir =
                    camera.transform.TransformDirection(Vector3.right * input.ad + Vector3.forward * input.ws);
                myRig.velocity = Vector3.ClampMagnitude(dir.normalized * baseSpeed, maxSpeed);
            }
            var camPos = camera.transform.TransformDirection(Vector3.back);
            RotateTowardlerp(my.transform.position - camPos, rotSpeed);
        }
        #endregion

        public void AddCostArea()
        {
            aiPathManager.BurnGround(10, 5, myRig.position);
        }

    }


}
