using UnityEngine;
using UnityEngine.AI;

public class Vulcan : MonoBehaviour {
    
    public GameObject navBlocker;
    private NavMeshSurface navMeshSurface;
    private NavMeshData navMeshData;
    private void OnStart() {
        //GetNavMeshBuilder
        //navBlocker = (GameObject)Resources.Load("Prefabs/Heat");
        navMeshSurface = GetComponent<NavMeshSurface>();
    }
    public void BurnGround(Vector3 pos)
    {
        
        Instantiate(navBlocker,pos,Quaternion.LookRotation(Vector3.forward));
        //navMeshSurface.UpdateNavMesh()
    }
}