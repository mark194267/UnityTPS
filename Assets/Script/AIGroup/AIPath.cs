using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 負責路線，巡路管理
/// </summary>
public class AIPath : MonoBehaviour {
    
    public GameObject navBlocker;
    public GameObject nav1Blocker;
    public GameObject nav2Blocker;

    public int MaxHotArea{get;set;}
    private NavMeshSurface navMeshSurface;
    private NavMeshData navMeshData;
    private List<HotArea> hotAreaList; 

    void Start() {
        //GetNavMeshBuilder
        //navBlocker = (GameObject)Resources.Load("Prefabs/Heat");
        navMeshSurface = GetComponent<NavMeshSurface>();
        navBlocker = Resources.Load("Prefabs/Heat") as GameObject;
        hotAreaList = new List<HotArea>();
        MaxHotArea = 100;
    }
    /// <summary>
    /// 事件發生時，更新該區域的MASK寫入HotAreaList
    /// </summary>
    /// <param name="pos"></param>
    public void BurnGround(int hot,int areasize,Vector3 pos)
    {
        //檢查位置上是否已有事件了
        /*if (CheckCollsion(hot, areasize, pos))
        {
            return;
        }
        else */
        if (hotAreaList.Count < MaxHotArea)
        {
            NavMeshData navMeshData = new NavMeshData();
            

            navMeshSurface.UpdateNavMesh(navMeshData);
            var heat = Instantiate(navBlocker, pos, Quaternion.LookRotation(Vector3.forward));
            hotAreaList.Add(heat.GetComponent<HotArea>());
        }
        else
        {
            //可能移除掉存在時間最短的事件區
        }
    }
}