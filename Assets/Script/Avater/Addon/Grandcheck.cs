using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grandcheck : MonoBehaviour
{
    public Animator animator { get; set; }
    public Transform MyObject { get; set; }
    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
        MyObject = gameObject.transform.parent;
    }

    private void OnTriggerStay(Collider other)
    {
        //print(other.gameObject.name);
        animator.SetBool("avater_IsLanded", true);
        //MyObject.parent = other.transform;
    }
    private void OnTriggerExit(Collider other)
    {
        animator.SetBool("avater_IsLanded", false);
        
    }
}
