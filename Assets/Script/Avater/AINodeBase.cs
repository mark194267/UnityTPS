﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Script.ActionControl;
using Assets.Script.AIGroup;
using Assets.Script.Test;
using Assets.Script.weapon;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Script.Avater
{
    class AINodeBase:AvaterMain
    {
        public AIBase AiBase;
        public Vector3 formationPoint;
        public Collider HotThing;

        public float NowTime;
        public bool IsDecided;
        public bool IsAwake;
        public string NowCommand;

        void Start()
        {
            WeaponFactory weaponFactory = new WeaponFactory();
            weaponFactory.Init();
            var GunDic = weaponFactory.AllWeaponDictionary;
            actionBasic.target = GameObject.Find("UnityChan");

            gameObject.GetComponent<Gun>().AddWeapon(GunDic["basicgun"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["bazooka"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["MG"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["katana"]);
            gameObject.GetComponent<Gun>().CreateWeaponByList();
            gameObject.GetComponent<Gun>().ChangeWeapon("MG");

            //gameObject.GetComponent<Gun>().ChangeWeapon("bazooka");
            AiBase.target = GameObject.Find("UnityChan");
            animator = this.gameObject.GetComponent<Animator>();
            //有無被叫醒
            //IsAwake = true;
            actionBasic.myPath = transform.GetComponentInParent<AICommander>().path;
        }

        void Update()
        {
            if(!IsAwake)
            {
                //這裡用測距，之後會改良為觸發盒
                if(Vector3.Distance(gameObject.transform.position,AiBase.target.transform.position) < 10)
                {
                    if(Vector3.Angle(gameObject.transform.TransformDirection(Vector3.forward),AiBase.target.transform.position) < 15)
                    {
                        RaycastHit hits;
                        //檢查是否看的到
                        if(Physics.Raycast(gameObject.transform.position,AiBase.target.transform.position-gameObject.transform.position,out hits,10))
                        {
                            IsAwake = true;
                        }
                    }
                }
                return;
            }

            foreach (var actionStatuse in actionStatusDictionary.AllActionStatusDictionary)
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsTag(actionStatuse.Key))
                {
                    NowActionStatus = actionStatuse.Value;
                }
            }
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 >= 0.99)
            {
                //決定方針
                NowCommand = AiBase.DistanceBasicAI(1, 2);
                //觸發動作
                animator.SetTrigger("AI_" + NowCommand);
                //初始化動作數值
                //actionBasic.SetupBeforeAction(this.name,NowActionStatus.ActionName);
                actionBasic.BeforeCustomAction(NowActionStatus);
                //關閉不能進入的狀態
                RefreshAnimaterParameter();
                if (NowActionStatus.ignorelist != null)
                {
                    foreach (var cando in NowActionStatus.ignorelist)
                    {
                        animator.SetBool("avater_can_" + cando, false);
                    }
                }
            }
            OldActionStatus = NowActionStatus;
            IsEndNormal = actionBasic.CustomAction(NowActionStatus);
            animator.SetBool("avater_IsEndNormal", IsEndNormal);
        }

        private void OnTriggerEnter(Collider other) {
            if(other.tag == "heat")
            {
                HotThing = other;
                //actionBasic.ChangeHeat();
            }
        }

        //劃出路線-參考以下
        //https://answers.unity.com/questions/361810/draw-path-along-navmesh-agent-path.html
        public void OnDrawGizmos()
        {        
            var nav = GetComponent<NavMeshAgent>();
            if( nav == null || nav.path == null )
                return;
        
            var line = this.GetComponent<LineRenderer>();
            if( line == null )
            {
                line = this.gameObject.AddComponent<LineRenderer>();
                line.material = new Material( Shader.Find( "Sprites/Default" ) ) { color = Color.yellow };
                line.SetWidth( 0.5f, 0.5f );
                line.SetColors( Color.yellow, Color.yellow );
            }
        
            var path = nav.path;
        
            line.SetVertexCount( path.corners.Length );
        
            for( int i = 0; i < path.corners.Length; i++ )
            {
                line.SetPosition( i, path.corners[ i ] );
            }        
        }

    }
}
