using System.Collections;
using Assets.Script.ActionControl;
using Assets.Script.Avater;
using Assets.Script.weapon;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Script.AIGroup
{
    class AICommander:MonoBehaviour
    {
        private GameObject[] allMyAi;
        private GameObject target;

        public bool IsAwake;
        public NavGrid NavGrid = new NavGrid();
        public NavMeshPath path;

        private ActionStatusDictionary actionStatusDictionary = new ActionStatusDictionary();
        private ActionScriptBuilder actionConstructer = new ActionScriptBuilder();
        private AIConstructer aiConstructer = new AIConstructer();
        private WeaponFactory weaponFactory = new WeaponFactory();
        private AvaterDataLoader avaterDataLoader = new AvaterDataLoader();
        private AvaterStatus avaterStatus = new AvaterStatus();

        void Awake()
        {
            //初期化
            path = new NavMeshPath();
            allMyAi = GameObject.FindGameObjectsWithTag("AI");
            target = GameObject.Find("UnityChan");
            weaponFactory.Init();
            avaterStatus = avaterDataLoader.LoadStatus("Imp");
            foreach (GameObject ai in allMyAi)
            {
                var aimain = ai.gameObject.GetComponent<AIAvaterMain>();
                aimain.avaterStatus = avaterStatus;
                aimain.Init_Avater();
                aimain.IsAwake = true;
                aimain.targetInfo = new TargetInfo() { Me = ai, Target = target };
                aimain.stateMachine.AIBase = aiConstructer.GetAI(aimain.targetInfo);
                //Debug.Log(aimain.stateMachine.AIBase);
            }
        }

        void Start()
        {
            //是否有巡邏路線
            //var agent = GetComponent<NavMeshAgent>();
            //agent.CalculatePath(target.transform.position, path);
        }

        void Update()
        {
            //調整時間後再重新思考
            //StartCoroutine(ChangeTactic(30));


        }

        void OnDrawGizmos()
        {
            NavGrid.DrawGizmos();
        }

        IEnumerator ChangeTactic(float waittime)
        {
            var agent = GetComponent<NavMeshAgent>();
            agent.CalculatePath(target.transform.position, path);
            //print("Change!");
            /*
            for (int i = 0; i < allMyAi.Length; i++)
            {
                if (!IsAwake)
                {
                    var nodeAi = allMyAi[i].GetComponent<AIAvaterMain>();
                    nodeAi.formationPoint = gameObject.transform.TransformVector(Vector3.right*i);//offsetPoint
                }

                if (IsAwake)
                {
                    
                    var RandPoint = RandomPosition(
                                        target.transform.position,
                                            Vector2.Distance(
                                                new Vector2(transform.position.x,transform.position.z),
                                                new Vector2(target.transform.position.x,target.transform.position.z)
                                            )
                                        );
                        
                    allMyAi[i].GetComponent<NavMeshAgent>().SetDestination(RandPoint);
                    
                }
            }
            */
            yield return new WaitForSeconds(waittime);
        }

        public Vector3 RandomPosition(Vector3 origin, float radius)
        {
            //var randDirection = UnityEngine.Random.insideUnitSphere * radius;
            var randDirection = Random.onUnitSphere * radius;
            randDirection += origin;
            NavMeshHit navHit;
            NavMesh.SamplePosition(randDirection, out navHit, radius, -1);
            return navHit.position;
        }
    }
}
