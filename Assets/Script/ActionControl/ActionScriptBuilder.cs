﻿using System;
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
    public class ActionScriptBuilder
    {
        public ActionScript GetActionBaseByName(string ActionName)
        {
            Type t = Type.GetType("Assets.Script.ActionList." + ActionName + "Action");
            ActionScript actionBase = new ActionScript();
            actionBase = (ActionScript)Activator.CreateInstance(t);
            return actionBase;
        }
    }
    /// <summary>
    /// 這裡負責關於人物的面向，移動，動作
    /// </summary>
    public class ActionScript
    {
        public Animator Animator { get; set; }
        public GameObject Me { get; set; }
        public GameObject Target { get; set; }
        protected Camera Camera { get; set; }
        protected Rigidbody Rig { get; set; }
        public NavMeshAgent Agent { get; set; }
        protected Vector3 NowVecter { get; set; }
        protected Vector3 velocity { get; set; }
        protected InputManager InputManager { get; set; }
        protected AvaterMain AvaterMain { get; set; }
        protected Gun Gun { get; set; }
        public AIPath aiPathManager { get; set; }
        protected TargetInfo Targetinfo { get; set; }

        public HotArea hotArea { get; set; }


        public void Init(GameObject Me)
        {
            this.Me = Me;
            this.Agent = this.Me.GetComponent<NavMeshAgent>();
            this.Gun = Me.GetComponent<Gun>();
            this.Animator = Me.GetComponent<Animator>();
            this.Rig = Me.GetComponent<Rigidbody>();
            this.AvaterMain = Me.GetComponent<AvaterMain>();
            InputManager = Me.GetComponent<InputManager>();

            if (Me.GetComponent<AIAvaterMain>())
            {
                this.Targetinfo = Me.GetComponent<AIAvaterMain>().targetInfo;
                this.Target = Me.GetComponent<AIAvaterMain>().targetInfo.Target;
            }

            if (Me.transform.Find("Camera"))
            {
                Camera = Me.transform.Find("Camera").GetComponent<Camera>();
            }
            var vol = Me.GetComponent<NavMeshModifierVolume>();
            if (vol != null)
            {
                hotArea = new HotArea();
                hotArea.Nv = vol;
                hotArea.nowHeat = 0;
            }
            //aiPathManager = GameObject.Find("Vulcan").GetComponent<AIPath>();
        }

        #region 自訂狀態機
        public void BeforeCustomAction(ActionStatus actionStatus)
        {
            MethodInfo methodInfo;
            Type actionbaseType = GetType();
            if (actionbaseType.GetMethod("Before_" + actionStatus.ActionName) != null)
            {
                methodInfo = actionbaseType.GetMethod("Before_" + actionStatus.ActionName);
                methodInfo.Invoke(this, new object[] { actionStatus }/*放入actionStatus*/);
            }

        }
        /// <summary>
        /// 自訂狀態機，預設的狀態機在ActionScript裡面
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
            MethodInfo methodInfo;
            Type actionbaseType = GetType();
            if (actionbaseType.GetMethod("After_" + actionStatus.ActionName) != null)
            {
                methodInfo = actionbaseType.GetMethod("After_" + actionStatus.ActionName);
                methodInfo.Invoke(this, new object[] { actionStatus }/*放入actionStatus*/);
            }
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
        /// <param name="Target">轉向目標，方法為lookRotation</param>
        protected void RotateTowardSlerp(Transform Target)
        {
            Vector3 direction = (Target.position - Me.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3
            Me.transform.rotation = Quaternion.Slerp(Me.transform.rotation, lookRotation, Time.deltaTime * 1.7f/*rotationSpeed*/);
        }

        /// <summary>
        /// 緩和旋轉，預設速度為 1.7f*deltatime
        /// </summary>
        /// <param name="Target">轉向目標，方法為lookRotation</param>
        protected void RotateTowardSlerp(Vector3 Target)
        {
            Vector3 direction = (Target - Me.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3
            Me.transform.rotation = Quaternion.Slerp(Me.transform.rotation, lookRotation, Time.deltaTime * 1.7f/*rotationSpeed*/);
        }

        /// <summary>
        /// 緩和旋轉，自訂速度
        /// </summary>
        /// <param name="Target">轉向目標，方法為lookRotation</param>
        /// <param name="speed">轉向速度</param>
        protected void RotateTowardSlerp(Vector3 Target, float speed)
        {
            Vector3 direction = (Target - Me.transform.position).normalized;
            var dir = new Vector3(direction.x, 0, direction.z);
            if (dir != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(dir);    // flattens the vector3
                Me.transform.rotation = Quaternion.Slerp(Me.transform.rotation, lookRotation, Time.deltaTime * speed/*rotationSpeed*/);
            }
        }
        /// <summary>
        /// 緩和旋轉，大多可用SLerp取代，預設速度為 1.7f*deltatime
        /// </summary>
        /// <param name="Target">轉向目標，方法為lookRotation</param>
        protected void RotateTowardlerp(Transform Target)
        {
            Vector3 direction = (Target.position - Me.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3
            Me.transform.rotation = Quaternion.Lerp(Me.transform.rotation, lookRotation, Time.deltaTime * 1.7f/*rotationSpeed*/);
        }

        /// <summary>
        /// 緩和旋轉，大多可用SLerp取代，預設速度為 1.7f*deltatime
        /// </summary>
        /// <param name="Target">轉向目標，方法為lookRotation</param>
        /// 
        protected void RotateTowardlerp(Vector3 Target)
        {
            Vector3 direction = (Target - Me.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3
            Me.transform.rotation = Quaternion.Lerp(Me.transform.rotation, lookRotation, Time.deltaTime * 3f/*rotationSpeed*/);
        }

        protected void RotateTowardlerp(Vector3 Target, float speed)
        {
            Vector3 direction = (Target - Me.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3
            Me.transform.rotation = Quaternion.Lerp(Me.transform.rotation, lookRotation, Time.deltaTime * speed/*rotationSpeed*/);
        }
        #endregion

        #region 轉角檢測+路線
        /// <summary>
        /// 只做轉角檢測的進攻路線
        /// </summary>
        /// <returns></returns>
        protected Vector3 takecover( Vector3 targetPos,float maxConnerAngle,float maxConnerDis)
        {
            NavMeshPath path = new NavMeshPath();
            Agent.CalculatePath(targetPos, path);

            int i = path.corners.Length;
            while (i - 3 > 0)
            {
                //得到倒數2個轉角的位置
                Vector3 point1 = path.corners[i - 2];
                Vector3 point2 = path.corners[i - 3];
                //得到最後一條往轉角的路徑
                Vector3 ToLastCorner = point1 - point2;
                //在獲取目標到轉角的向量
                Vector3 TargetToCorner = Target.transform.position - point1;
                //如果兩向量夾角太小，距離太近，就不會過第二個轉角
                if (Vector2.Angle(
                        new Vector2(ToLastCorner.x, ToLastCorner.z),
                        new Vector2(TargetToCorner.x, TargetToCorner.z)) > maxConnerAngle
                    && Vector3.Distance(point1, point2) > maxConnerDis)
                {
                    return point1;
                }
                else
                {
                    i--;
                }
            }
            return Vector3.zero;
        }

        #endregion

        /// <summary>
        /// AI通用的切換目標，保持用此函式來切換目標
        /// </summary>
        /// <param name="targetGameObject"></param>
        public void ChangeTarget(GameObject targetGameObject)
        {
            this.Target = targetGameObject;
        }

        #region 基礎，通用動作

        public virtual void Before_idle(ActionStatus actionStatus)
        {
            Agent.ResetPath();
        }

        public virtual bool idle(ActionStatus actionStatus)
        {
            return true;
        }

        public virtual void Before_move(ActionStatus actionStatus)
        {
            //Debug.Log("BeforeMove");
            //Agent.isStopped = false;

            //2019-05-02 官方建議每秒更新
            //Agent.SetDestination(Target.transform.position);
        }

        public virtual bool move(ActionStatus actionStatus)
        {
            Agent.SetDestination(Target.transform.position);

            //2019-05-02 官方建議每秒更新
            /*
            if (!Agent.pathPending && !Agent.hasPath)
            {
                Agent.SetDestination(Target.transform.position);
            }
            */
            //if (Animator.GetBehaviour<StateMachine>().maxMeleeRange > Vector3.Distance(Target.transform.position,Me.transform.position))
            if(Targetinfo.ToTargetSight(Animator.GetBehaviour<StateMachine>().maxMeleeRange) != null)
            {                
                return false;
            }
            
            return true;
        }
        public virtual void After_move(ActionStatus actionStatus)
        {
            //Debug.Log("AfterMove");
            //Agent.isStopped = true;
            Agent.ResetPath();
        }

        public virtual bool shoot(ActionStatus actionStatus)
        {
            if (Targetinfo.TargetAngle < 5)
            {
                Gun.fire(0);
            }
            RotateTowardSlerp(Target.transform);
            return true;
        }

        public virtual void Before_slash(ActionStatus actionStatus)
        {
        }

        public virtual bool slash(ActionStatus actionStatus)
        {
            if (AvaterMain.anim_flag == 0)//還沒揮刀時可以轉
            {
                RotateTowardSlerp(Target.transform);
            }
            else
            {
                Gun.Swing(AvaterMain.anim_flag, 1, 1);
            }
            return true;
        }

        public virtual bool reload(ActionStatus actionStatus)
        {
            return true;
        }

        public virtual void Before_stun(ActionStatus actionStatus)
        {
            if (Agent != null) { Agent.ResetPath(); }

            Me.GetComponent<Animator>().applyRootMotion = true;
            Animator.SetBool("avatermain_stun", false);
            Animator.SetBool("avater_can_stun", false);
        }
        public virtual bool stun(ActionStatus actionStatus)
        {
            return true;
        }
        public virtual void After_stun(ActionStatus actionStatus)
        {
            Me.GetComponent<Animator>().applyRootMotion = false;

        }

        public virtual void Before_dead(ActionStatus actionStatus)
        {
            //AddCostArea();
            Agent.enabled = false;
            Rig.isKinematic = false;
            Animator.enabled = false;
            Me.GetComponent<Collider>().enabled = false;

            Gun.NowWeapon[0].weapon.GetComponent<Collider>().enabled = false;
        }

        public virtual bool dead(ActionStatus actionStatus)
        {
            return true;
        }
        public virtual void After_dead(ActionStatus actionStatus)
        {

        }

        #endregion

        #region 第一/第三人稱式移動

        /// <summary>
        /// 第一人稱式移動，移動的主體是NavAgent，若沒Agent請改用FPSLikeRigMovement
        /// </summary>
        /// <param name="moveSpeed"></param>
        /// <param name="rotSpeed"></param>
        public void FPSLikeMovement(float moveSpeed, float rotSpeed)
        {

            var dir =
                Camera.transform.TransformDirection(Vector3.right * InputManager.ad + Vector3.forward * InputManager.ws);
            //Agent.velocity = Vector3.ClampMagnitude(dir.normalized * 7f, moveSpeed);
            Agent.velocity = Vector3.Lerp(Agent.velocity, dir.normalized * moveSpeed, 1f);

            var camPos = Camera.transform.TransformDirection(Vector3.back);
            RotateTowardSlerp(Me.transform.position - camPos, rotSpeed);
        }
        /// <summary>
        /// 第一人稱式移動，移動的主體是NavAgent，若沒Agent請改用FPSLikeRigMovement
        /// </summary>
        /// <param name="baseSpeed"></param>
        /// <param name="maxSpeed"></param>
        /// <param name="rotSpeed"></param>
        public void FPSLikeMovement(float baseSpeed, float maxSpeed, float rotSpeed)
        {

            var dir =
                Camera.transform.TransformDirection(Vector3.right * InputManager.ad + Vector3.forward * InputManager.ws);
            Agent.velocity = Vector3.ClampMagnitude(dir.normalized * baseSpeed, maxSpeed);

            var camPos = Camera.transform.TransformDirection(Vector3.back);
            RotateTowardSlerp(Me.transform.position - camPos, rotSpeed);
        }
        /// <summary>
        /// 第一人稱式移動，預設速度為7f
        /// </summary>
        /// <param name="maxSpeed"></param>
        /// <param name="rotSpeed"></param>
        public void FPSLikeRigMovement(float maxSpeed, float rotSpeed)
        {
            var dir =
                Camera.transform.TransformDirection(Vector3.right * InputManager.ad + Vector3.forward * InputManager.ws);
            dir.y = 0;
            //Rig.velocity = Vector3.ClampMagnitude(dir.normalized * 7f, maxSpeed);
            Rig.velocity = Vector3.Lerp(Rig.velocity, dir.normalized * InputManager.maxWSAD * maxSpeed, 1f);

            var camPos = Camera.transform.TransformDirection(Vector3.back);
            RotateTowardSlerp(Me.transform.position - camPos, rotSpeed);
        }
        /// <summary>
        /// 第一人稱式移動，自訂速度
        /// </summary>
        /// <param name="baseSpeed"></param>
        /// <param name="maxSpeed"></param>
        /// <param name="rotSpeed"></param>
        public void FPSLikeRigMovement(float baseSpeed, float maxSpeed, float rotSpeed)
        {
            var dir =
                Camera.transform.TransformDirection(Vector3.right * InputManager.ad + Vector3.forward * InputManager.ws);
            //Rig.velocity = Vector3.ClampMagnitude(dir.normalized * baseSpeed, maxSpeed);
            Rig.velocity = Vector3.Lerp(Rig.velocity, dir.normalized * InputManager.maxWSAD * maxSpeed, baseSpeed);


            var camPos = Camera.transform.TransformDirection(Vector3.back);
            RotateTowardSlerp(Me.transform.position - camPos, rotSpeed);
        }
        #endregion

        #region 散開腳本
        /// <summary>
        /// 散開，面對某個方向
        /// </summary>
        /// <param name="TargetPos"></param>
        /// <param name="barrier"></param>
        /// <param name="AxisMultiper"></param>
        /// <param name="StepLenght"></param>
        /// <returns></returns>
        public Vector3 SetSpreadOutPoint(Vector3 TargetPos,Vector3 barrier,float angle,float StepLenght)
        {
            var me2target = TargetPos - Me.transform.position;
            var bar2target = barrier - Target.transform.position;
            var bar2targetDis = bar2target.magnitude;
            //得到障礙和自身的夾角
            var r = Vector3.SignedAngle(me2target, bar2target, Vector3.up);
            var rot = Me.transform.TransformDirection(Vector3.forward);
            Vector3 fin;
            if(r > 0)
                fin = Quaternion.AngleAxis(-angle, Vector3.up) * me2target;
            else
                fin = Quaternion.AngleAxis(angle, Vector3.up) * me2target;
//            fin = Quaternion.AngleAxis(AxisMultiper * -r, Vector3.up) * rot;

            //角度會和距離成反比
            return Me.transform.position + fin.normalized * (StepLenght/bar2targetDis);    
        }

        #endregion

        #region 更新人物的移動狀態
        /// <summary>
        /// 更新人物的移動狀態，請先於Animator加入AI_ws, AI_ad, AI_speed
        /// </summary>
        public void UpdateWSAD_ToAnimator()
        {
            var dir = Me.transform.InverseTransformDirection(Agent.velocity.normalized);
            Animator.SetFloat("AI_ws", dir.z);
            Animator.SetFloat("AI_ad", dir.x);

            var Max = Mathf.Max(Mathf.Abs(dir.z), Mathf.Abs(dir.x));
            Animator.SetFloat("AI_speed", Max);
        }
        #endregion
        public void AddCostArea()
        {
        }
    }


}
