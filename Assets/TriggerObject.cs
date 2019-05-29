using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObject : MonoBehaviour
{
    public GameObject TriggerTarget;
    public bool IsTrigger = false;
    public bool IsRig = false;
    public enum TriggerType { OnTriggerEnter, OnTriggerExit, OnTriggerStay, OnCollisionEnter, OnCollisionExit, OnCollisionStay };
    public TriggerType Type;

    private Vector3 _oriPos { get; set; }
    private Vector3 _oriQua { get; set; }

    public Vector3 TargetPos;
    public Vector3 TargetRot;

    private Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        if (IsRig)
        {
            rigidbody = TriggerTarget.GetComponent<Rigidbody>();
            _oriPos = rigidbody.position;
            _oriQua = rigidbody.rotation.eulerAngles;
        }
        else
        {
            _oriPos = TriggerTarget.transform.localPosition;
            _oriQua = TriggerTarget.transform.localRotation.eulerAngles;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsTrigger)
        {
            if (IsRig)
                StartRig();
            else
                StartMove();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (Type == TriggerType.OnTriggerEnter)
        {
            if (!IsRig)
                IsTrigger = true;
            else
            {
                if(other != TriggerTarget.GetComponent<Collider>())
                    IsTrigger = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (Type == TriggerType.OnTriggerExit)
        {
            if (!IsRig)
                IsTrigger = true;
            else
            {
                if (other != TriggerTarget.GetComponent<Collider>())
                    IsTrigger = true;
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (Type == TriggerType.OnTriggerStay)
        {
            if (!IsRig)
                IsTrigger = true;
            else
            {
                if (other != TriggerTarget.GetComponent<Collider>())
                    IsTrigger = true;
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (Type == TriggerType.OnCollisionEnter)
        {
            if (!IsRig)
                IsTrigger = true;
            else
            {
                if (collision.gameObject != TriggerTarget)
                    IsTrigger = true;
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (Type == TriggerType.OnCollisionExit)
        {
            if (!IsRig)
                IsTrigger = true;
            else
            {
                if (collision.gameObject != TriggerTarget)
                    IsTrigger = true;
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (Type == TriggerType.OnCollisionStay)
        {
            if (!IsRig)
                IsTrigger = true;
            else
            {
                if (collision.gameObject != TriggerTarget)
                    IsTrigger = true;
            }
        }
    }

    public void StartMove()
    {
        TriggerTarget.transform.localPosition = Vector3.Lerp(TriggerTarget.transform.localPosition, TargetPos + _oriPos, Time.deltaTime);
        TriggerTarget.transform.localRotation = Quaternion.Lerp(
            TriggerTarget.transform.localRotation,Quaternion.Euler(_oriQua+TargetRot), Time.deltaTime);
    }
    public void StartRig()
    {
        rigidbody.MovePosition(Vector3.Lerp(_oriPos, TargetPos + _oriPos, Time.deltaTime));
        rigidbody.MoveRotation(Quaternion.Lerp(
            rigidbody.rotation, Quaternion.Euler(_oriQua + TargetRot), Time.deltaTime));
    }
}