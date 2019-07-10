using System;
using System.Collections;
using Assets.Script.ActionControl;
using Assets.Script.AIGroup;
using Assets.Script.weapon;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Script.Avater
{
    public class AIAvaterMainNavgator : AvaterMain
    {
        public GameObject Navgator { get; set; }
        public AIBase AIBase { get; set; }
        public TargetInfo targetInfo { get; set; }
        //public Vector3 formationPoint;
        public Collider HotThing { get; set; }
        private MotionStatusBuilder statusBuilder = new MotionStatusBuilder();
        public bool IsDecided;
        public bool IsAwake;

        public bool HasMotionStatus = false;

        public string NowCommand { get; set; }

        public AllAmmoType ammoType = new AllAmmoType();

        void Start()
        {
            WeaponFactory weaponFactory = new WeaponFactory();
            weaponFactory.Init();
            var GunDic = weaponFactory.AllWeaponDictionary;
            
            //簡單的初始化，等待改寫

            gameObject.GetComponent<Gun>().AddWeapon(GunDic["basicgun"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["fist"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["MG"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["katana"]);
            gameObject.GetComponent<Gun>().AddWeapon(GunDic["kick"]);

            gameObject.GetComponent<Gun>().CreateWeaponByList();
            //gameObject.GetComponent<Gun>().ChangeWeapon("MG");
            //Animator = this.gameObject.GetComponent<Animator>();
            if(HasMotionStatus)
                motionStatusDir = statusBuilder.GetMotionList(gameObject.name);
            Navgator = transform.Find("Navgator").gameObject;
            ActionScript.Agent = Navgator.GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (IsAwake)
            {
                Animator.SetBool("AI_IsAwake",true);
                StopCoroutine(CheckSight(5));
                transform.position = 
                    new Vector3(Navgator.transform.position.x,transform.position.y,Navgator.transform.position.z);
            }
            else
                StartCoroutine(CheckSight(5));
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

        private void OnTriggerEnter(Collider other) {
            if(other.tag == "heat")
            {
                HotThing = other;
                //ActionScript.ChangeHeat();
            }
        }

        public IEnumerator CheckSight(float time)
        {
            //Debug.Log("Check");
            //這裡用測距，之後會改良為觸發盒
            if (targetInfo.TargetDis < 50)
            {
                //Debug.Log("<50");
                if (Vector3.Angle(transform.TransformVector(Vector3.forward), targetInfo.Target.transform.position) < 120)
                {
                    var height = GetComponent<NavMeshAgent>().height*Vector3.up;
                    var Layer = ~LayerMask.GetMask("AI");
                    //Debug.Log("Angle");
                    RaycastHit hits;
                    //檢查是否看的到
                    if (Physics.Raycast(transform.position, targetInfo.Target.transform.position - transform.position, out hits, 50,Layer,QueryTriggerInteraction.Ignore))
                    {
                        //print(hits.transform.name);
                        if (hits.transform.CompareTag("Player"))
                        {
                            IsAwake = true;
                        }
                    }
                }
            }            
            yield return new WaitForSeconds(time);
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
