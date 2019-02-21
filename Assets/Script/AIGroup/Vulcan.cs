using UnityEngine;

public class Vulcan : MonoBehaviour {
    
    public GameObject navBlocker;
    private void OnEnable() {
        //GetNavMeshBuilder
        navBlocker = (GameObject)Resources.Load("Prefabs/" + name);
    }
    public void BurnGround(Vector3 pos)
    {
        Instantiate(Navblocker,pos,)
    }
}