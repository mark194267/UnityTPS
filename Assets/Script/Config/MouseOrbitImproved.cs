﻿using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbitImproved : MonoBehaviour {

	public Transform target;
	public float distance = 5.0f;
	public float xSpeed = 120.0f;
	public float ySpeed = 120.0f;

	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;

	public float distanceMin = .5f;
	public float distanceMax = 15f;

	public float Vrecoil = 0;
	public float Hrecoil = 0;
	public float FireTime = 0;

	public Rigidbody rb;

	float x = 0.0f;
	float y = 0.0f;

	// Use this for initialization
	void Start () 
	{
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;

		rb = GetComponent<Rigidbody>();

		// Make the rigid body not change rotation
		if (rb != null)
		{
			rb.freezeRotation = true;
		}
	}

	void LateUpdate () 
	{		
		if (target) 
		{
			x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
			y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f ;

			y = ClampAngle(y - Vrecoil, yMinLimit, yMaxLimit);
			
			//後座力主要超載這邊
			Quaternion rotation = Quaternion.Euler(y , x + Hrecoil, 0);

			distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel")*5, distanceMin, distanceMax);

			RaycastHit hit;
			if (Physics.Linecast (target.position, transform.position, out hit,1)) 
			{
				distance -=  hit.distance;
			}
			Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
			//要去的位置
			Vector3 position = rotation * negDistance + target.position;
			//移動，動作偏量
			//加上一個偏量，應該是目標物件動量，加上一個很小的加算值
			//後座力為一角度，用 rot += 某個值 然後在同時更新為加算前的值

			transform.rotation = rotation;
			transform.position = position+Vector3.up*1.7f;
		}

        //後座力回復
        //可能為開槍後N秒內下降，或是滑鼠大幅度動作

        if (Vrecoil > 0 && FireTime > 0)
        {
            Vrecoil = 0f;
            FireTime -= Time.deltaTime;
        }
    }

	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}
}