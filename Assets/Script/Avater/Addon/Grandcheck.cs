using Assets.Script.Avater;
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
        if (other.gameObject.layer != LayerMask.GetMask("AI","Player"))
        {
            //animator.SetBool("avater_IsLanded", true);
            //print(other.gameObject.name);
            StartCoroutine(CheckSteep());
        }
        /*if(!other.GetComponentInParent<AvaterMain>())
            MyObject.parent = other.transform;
        */
    }
    private void OnTriggerExit(Collider other)
    {
        animator.SetBool("avater_IsLanded", false);        
    }
    IEnumerator CheckSteep()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit, 1f, ~LayerMask.GetMask("Player", "AI"), QueryTriggerInteraction.Ignore);
        var angle2wall = Vector3.Angle(transform.up, Vector3.ProjectOnPlane(hit.normal, transform.right));
        //Debug.Log(angle2wall);
        
        if (60 > angle2wall && angle2wall > -60)
        {
            animator.SetBool("avater_IsLanded", true);
        }
        else
            animator.SetBool("avater_IsLanded", false);

        yield return new WaitForSeconds(0.01f);
    }
}
