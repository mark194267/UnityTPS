using System;
using System.Collections;
using Assets.Script.ActionControl;
using Assets.Script.AIGroup;
using Assets.Script.weapon;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Script.Avater
{
    public class AIAvaterMain:AvaterMain
    {
        public AIBase AIBase { get; set; }
        public TargetInfo targetInfo { get; set; }
        public RaycastHit hit { get; set; }
        public GameObject hot { get; set; }
        public StateMachine stateMachine { get; set; }
        //public Vector3 formationPoint;
        //public Collider HotThing { get; set; }
        public AICommander Commander { get; set; }
        public HotAreaManager HotAreaManager { get; set; }

        private MotionStatusBuilder statusBuilder = new MotionStatusBuilder();

        public float sightDis;
        public float sightStartHoffset;
        public float sightEndHoffset;
        public float sightRadius;

        public bool IsAwake;

        public bool IsUpdateSight = false;

        public bool HasMotionStatus = false;

        public string NowCommand { get; set; }

        public AllAmmoType ammoType = new AllAmmoType();

        public virtual void Start()
        {
            stateMachine = Animator.GetBehaviour<StateMachine>();
            stateMachine.me = gameObject;
            stateMachine.action = ActionScript;
            stateMachine.AvaterMain = this;
            stateMachine.AIBase = AIBase;
            var aibase = stateMachine.AIBase;

            targetInfo = aibase.TargetInfo;

            WeaponFactory weaponFactory = new WeaponFactory();
            weaponFactory.Init();
            var GunDic = weaponFactory.AllWeaponDictionary;
            
            //簡單的初始化，等待改寫

            //gameObject.GetComponent<Gun>().AddWeapon(GunDic["basicgun"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["fist"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["Crossbow"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["Crossbow_test"]);

            //gameObject.GetComponent<Gun>().AddWeapon(GunDic["katana"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["kick"]);
            gameObject.GetComponent<Gun>().CreateWeaponByList();

            //GetComponent<Gun>().LoadSingleWeapon("Crossbow");
            GetComponent<Gun>().LoadSingleWeapon("Crossbow_test");
            GetComponent<Gun>().LoadSingleWeapon("kick");

            if (HasMotionStatus)
                motionStatusDir = statusBuilder.GetMotionList(gameObject.name);
            targetInfo.checkRadius = sightRadius;
            targetInfo.maxDistance = sightDis;
            targetInfo.targetSightHset = sightEndHoffset;
            targetInfo.meSightHset = sightStartHoffset;

            StartCoroutine(CheckSight(.1f));
        }

        public virtual void Update()
        {
            if (IsAwake)
            {
                Animator.SetBool("AI_IsAwake",true);
                //StopCoroutine(CheckSight(5));
            }
        }
        #region Update()

        /*
        void Update()
        {
            
            TargetDis = Vector3.Distance(gameObject.transform.position, AiBase.target.transform.position);

            if (!IsAwake)
            {
                //這裡用測距，之後會改良為觸發盒
                if(TargetDis < 10)
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
            NowCommand = AiBase.DistanceBasicAI(TargetDis, 0, 20);
            Animator.SetTrigger("AI_" + NowCommand);
          
            if (Animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 > 0.99 || OldActionStatus != NowActionStatus)
            //if(Animator.GetNextAnimatorStateInfo(0).IsTag(NowActionStatus.ActionName))
            {
                Debug.Log("tick");
                //決定方針
                NowCommand = AiBase.DistanceBasicAI(TargetDis,3, 7);
                //擲骰子,觸發動作
                var num = UnityEngine.Random.Range(0, 100);
                Animator.SetInteger("AI_Dice", num);
                Animator.SetTrigger("AI_" + NowCommand);
                //初始化動作數值
                //ActionScript.SetupBeforeAction(this.name,NowActionStatus.ActionName);
                if (OldActionStatus != null)
                {
                    ActionScript.AfterCustomAction(OldActionStatus);
                }
                ActionScript.BeforeCustomAction(NowActionStatus);
                //關閉不能進入的狀態
                if (NowActionStatus.ignorelist != null)
                {
                    foreach (var cando in NowActionStatus.ignorelist)
                    {
                        Animator.SetBool("avater_can_" + cando, false);
                    }
                }
                OldActionStatus = NowActionStatus;
            }
            IsEndNormal = ActionScript.CustomAction(NowActionStatus);
            Animator.SetBool("avater_IsEndNormal", IsEndNormal);
            
        }
        */
        #endregion

        public override void OnHit(int atk, double stun,Vector3 vector)
        {
            Animator.SetBool("AI_IsAwake", true);
            base.OnHit(atk, stun, vector);
        }

        public IEnumerator CheckSight(float time)
        {
            while (true)
            {
                if (!IsAwake)
                {
                    if (targetInfo.TargetAngle < 120)
                    {
                        //檢查是否看的到
                        hit = targetInfo.TargetSightHit;
                        if (hit.transform != null)
                        {
                            if (hit.transform.CompareTag("Player"))
                            {
                                IsAwake = true;
                                IsUpdateSight = true;
                            }
                        }
                    }
                }

                if (IsUpdateSight)
                {
                    hit = targetInfo.TargetSightHit;
                    //print(hit);
                    //帶加入戰鬥.閒置不同的timer
                    //yield return new WaitForSeconds(time);
                }

                yield return new WaitForSeconds(time);
            }
        }

        public void AddHotArea()
        {
            Commander.AddHotArea(transform);
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
