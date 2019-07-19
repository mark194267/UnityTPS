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


    /// <summary>
    /// 檢查事件區域是否已有其他事件，並更新該事件
    /// </summary>
    private bool CheckCollsion(int hot,int areaSize,Vector3 pos)
    {
        RaycastHit hit;
        if (Physics.BoxCast(pos, Vector3.one * areaSize, Vector3.forward,out hit,Quaternion.Euler(0,0,0),0,0, QueryTriggerInteraction.Collide/*Mask還沒寫入*/))
        {
            //如果碰到事件
            var heat = hit.transform.gameObject.GetComponent<HotArea>();
            heat.ChangeTemperature(hot);
            //更新等到畫下一次路線前
            return true;
        }
        return false;
    }
}