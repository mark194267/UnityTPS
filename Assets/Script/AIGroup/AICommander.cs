using System.Collections;
using Assets.Script.ActionControl;
using Assets.Script.Avater;
using Assets.Script.weapon;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Script.AIGroup
{
    public class AICommander:MonoBehaviour
    {
        private GameObject target;
        public GameObject hot;

        public bool IsAwake;
        public NavGrid NavGrid = new NavGrid();
        public NavMeshPath path;

        private ActionStatusDictionary actionStatusDictionary = new ActionStatusDictionary();
        private ActionScriptBuilder actionConstructer = new ActionScriptBuilder();
        private AIConstructer aiConstructer = new AIConstructer();
        private WeaponFactory weaponFactory = new WeaponFactory();
        private AvaterDataLoader avaterDataLoader = new AvaterDataLoader();
        private AvaterStatus avaterStatus = new AvaterStatus();
        private HotAreaManager HotAreaManager = new HotAreaManager();

        void Awake()
        {
            //初期化
            path = new NavMeshPath();
            //GameObject[] allMyAi = GameObject.FindGameObjectsWithTag("AI");
            AIAvaterMain[] allMyAi = GetComponentsInChildren<AIAvaterMain>();
            target = GameObject.FindGameObjectWithTag("Player");
            weaponFactory.Init();
            avaterStatus = avaterDataLoader.LoadStatus("Imp");
            //Debug.Log("hihi");

            foreach (var ai in allMyAi)
            {
                //Debug.Log("2");
                ai.avaterStatus = avaterStatus;
                //Debug.Log("3");
                ai.IsAwake = IsAwake;
                //Debug.Log("4");
                ai.AIBase = aiConstructer.GetAI(ai.gameObject, target);
                ai.targetInfo = ai.AIBase.TargetInfo;
                //Debug.Log("5");
                ai.Init_Avater();

                //Debug.Log("7");
                ai.hot = hot;
                ai.HotAreaManager = HotAreaManager;
                ai.Commander = this;
            }

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

        public void AddHotArea(Transform trans)
        {
            RaycastHit hit;
            Physics.BoxCast(trans.position+Vector3.up*5f, new Vector3(2, 1, 2), Vector3.down,out hit, trans.rotation, 5f, LayerMask.GetMask("hot"), QueryTriggerInteraction.Collide);

            if (hit.transform != null)
            {
                var ht = hit.transform.GetComponent<HotArea>();
                //ht.size += 1;
                ht.transform.position = (ht.transform.position + trans.position) / 2;
                ht.setHot = ht.hot + 1;
                //Debug.Log("HotGround Founded");
            }
            else
            {
                GameObject hotobject = Instantiate(hot, transform);
                var hotArea = hotobject.GetComponent<HotArea>();
                hotArea.size = 4; hotArea.setHot = NavMesh.GetAreaFromName("H1");
                hotArea.transform.position = trans.position;
                hotArea.transform.rotation = trans.rotation;
                HotAreaManager.AddArea(hotArea);
            }
        }
    }
}
