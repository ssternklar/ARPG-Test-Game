using UnityEngine;
using System.Collections;

public class MyCamera : MonoBehaviour {
	
	public Transform target;
	
	//distance the camera is at, default 10
	float distance;
	float correctedDistance;
	//max and min distance the camera will snap to assuming it crosses the respective thresholds.
	public float maxDistance = 10f;
	public float minDistance = 1f;
	
	//speed of rotation along x and y
	public float xSpeed = 250f;
	public float ySpeed = 120f;
	
	//max and min angles for y
	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;
	
	//the angle rotation of the camera along x and y
	public float x;
	public float y;
	
	RaycastHit hitInfo;
	
	// Use this for initialization
	void Start () {
		x = transform.eulerAngles.x;
		y = transform.eulerAngles.y + 20;
		distance = maxDistance;

		if(rigidbody)
		{
			rigidbody.freezeRotation = true;
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(target)
		{
			float tx = (float)Input.GetAxis("Mouse X") * xSpeed * 0.1f;
			float ty = (float)Input.GetAxisRaw("Mouse Y") * ySpeed * 0.1f;
			
			x = Mathf.Lerp(x, x + tx, 10 * Time.deltaTime);
			y = Mathf.Lerp(y, y+ty, 10 * Time.deltaTime);
			
			y = ClampAngle(y, yMinLimit, yMaxLimit);
			
			//set the offset to our target. If we wanted it to be off to the left or right we'd modify the X variable.
			Vector3 targetOffset = new Vector3(0f,-1.6f,0f);
			
			//calculate current rotation
			Quaternion rotation = Quaternion.Euler(y, x, 0);
			
			// Calculate desired camera position
			Vector3 position = target.position - (rotation * Vector3.forward * maxDistance + targetOffset);
			correctedDistance = maxDistance;
			
			//calculate target position
			Vector3 targetPosition = new Vector3(target.position.x, target.position.y + 1.6f, target.position.z);
			
			
			bool isCorrected = false;
			if(Physics.Linecast(targetPosition, position, out hitInfo, -1))
			{
				//If we hit a wall, correct the distance. Otherwise, corrected distance will remain at maximum
				correctedDistance = Vector3.Distance(target.position, hitInfo.point) - 1f;
				isCorrected = true;
			}
			
			
			if(!isCorrected || correctedDistance > distance)
				distance = Mathf.Lerp (distance, correctedDistance, Time.deltaTime * 3.0f);
			else
				distance = correctedDistance;
			//distance = !isCorrected || correctedDistance > distance ? Mathf.Lerp (distance, correctedDistance, Time.deltaTime * 3.0f) : correctedDistance;
				
			Mathf.Clamp(distance, minDistance, maxDistance);
			
			position = target.position - (rotation * Vector3.forward * distance + targetOffset);
        
        	transform.rotation = rotation;
        	transform.position = position;
		}
	}
	
	float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360)
			angle += 360;

		if (angle > 360)
			angle -= 360;

		return Mathf.Clamp (angle, min, max);
	}
}
