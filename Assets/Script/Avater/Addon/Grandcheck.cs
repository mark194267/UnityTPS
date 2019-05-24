using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grandcheck : MonoBehaviour
{
    public Animator animator { get; set; }
    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
    }

    private void OnTriggerStay(Collider other)
    {
        //print(other.gameObject.name);
        animator.SetBool("avater_IsLanded", true);
    }
    private void OnTriggerExit(Collider other)
    {
        animator.SetBool("avater_IsLanded", false);
    }
}
