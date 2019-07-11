using UnityEngine;
using System.Collections;
using Assets.Script.Config;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbitImproved : MonoBehaviour {

	public Transform target;
    public bool IsAutoTransparent = false;
    public bool IsAutoClose = false;
    public float distance = 5.0f;
	public float xSpeed = 120.0f;
	public float ySpeed = 120.0f;

	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;

    public float cameraHeight = 1.3f;
	public float distanceMin = .5f;
	public float distanceMax = 15f;

	public float Vrecoil = 0;
	public float Hrecoil = 0;
	public float FireTime = 0;

	public Rigidbody rb;

	public float x = 0.0f;
	public float y = 0.0f;

    // Use this for initialization

    //透明化參數
    public float DistanceToPlayer = 5.0f;
    public Material TransparentMaterial = null;
    public float FadeInTimeout = 0.6f;
    public float FadeOutTimeout = 0.2f;
    public float TargetTransparency = 0.3f;

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

            x = ClampAngle(x, -360, 360);

            y = ClampAngle(y - Vrecoil, yMinLimit, yMaxLimit);
			
			//後座力主要超載這邊
			Quaternion rotation = Quaternion.Euler(y , x + Hrecoil, 0);

			distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel")*5, distanceMin, distanceMax);


            #region 透明化

            if (IsAutoTransparent)
            {
                RaycastHit[] hits; // you can also use CapsuleCastAll() 
                                   // TODO: setup your layermask it improve performance and filter your hits. 
                int layermask = LayerMask.GetMask("Player", "AI");
                hits = Physics.RaycastAll(transform.position, transform.forward, distance, ~layermask);
                foreach (RaycastHit hit in hits)
                {
                    Renderer R = hit.collider.GetComponent<Renderer>();
                    if (R == null)
                    {
                        continue;
                    }
                    // no renderer attached? go to next hit 
                    // TODO: maybe implement here a check for GOs that should not be affected like the player
                    AutoTransparent AT = R.GetComponent<AutoTransparent>();
                    if (AT == null) // if no script is attached, attach one
                    {
                        AT = R.gameObject.AddComponent<AutoTransparent>();
                        AT.TransparentMaterial = TransparentMaterial;
                        AT.FadeInTimeout = FadeInTimeout;
                        AT.FadeOutTimeout = FadeOutTimeout;
                        AT.TargetTransparency = TargetTransparency;
                    }
                    AT.BeTransparent(); // get called every frame to reset the falloff
                }
            }
            #endregion

            if (IsAutoClose)
            {
                RaycastHit hit;

                if (Physics.Linecast(target.position, transform.position, out hit, 1))
                {
                    distance -= hit.distance;
                }
            }
            
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
			//要去的位置
			Vector3 position = rotation * negDistance + target.position;
			//移動，動作偏量
			//加上一個偏量，應該是目標物件動量，加上一個很小的加算值
			//後座力為一角度，用 rot += 某個值 然後在同時更新為加算前的值

			transform.rotation = rotation;
			transform.position = position+Vector3.up* cameraHeight;
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
    public static float ClampAngle180(float angle, float min, float max)
    {
        if (angle < -180F)
            angle = 180F;
        if (angle > 180F)
            angle = 180F;
        return Mathf.Clamp(angle, min, max);
    }
}