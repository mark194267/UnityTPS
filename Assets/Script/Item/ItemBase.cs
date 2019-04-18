using UnityEngine;

class ItemBase:MonoBehaviour
{
    public string ItemName{get;set;}
    public string pickUpMessage{get;set;}
    void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.tag == "player")
        {
            
        }    
    }
}