using UnityEngine;
using System.Collections;

public class MinimapCamera : MonoBehaviour {
	public Transform target;

	public int height = 5;
	// Use this for initialization
	void Start () {
		if(!target)
			target = GameObject.FindWithTag("Player").transform;
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(target.position.x, target.position.y + height, target.position.z);
	}
}
