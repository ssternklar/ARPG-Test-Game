using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour {
	public int health = 5;
	public Transform target;
	public float moveSpeed = 30f;
	public float rotationSpeed = 30f;
	public float damageOnHit = 5f;

	public float range;
	public float chaseRange = 200f;
	public float dashRange = 10f;

	private float startY;

	private float currentTime;
	public float dashChargeTime = 2f;
	public float dashTime = 2f;
	public string type = "Enemy";
	RaycastHit hitInfo;

	enum EnemyState
	{
		Idle = 0,
		Moving = 1,
		ChargeUp = 2,
		Dashing = 3,
	}
	
	EnemyState enemyState;
	// Use this for initialization
	void Start () {
		target = GameObject.FindWithTag("Player").transform;
		startY = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		//look at player
		/*transform.rotation = Quaternion.Slerp(transform.rotation,
		                                      Quaternion.LookRotation(target.position - transform.position),
		                                      rotationSpeed * Time.deltaTime);*/
		range = Vector3.Distance(target.position, transform.position);
		
		AI();
		transform.position = new Vector3(transform.position.x, startY, transform.position.z);
	}
	void AI()
	{
		if(!Physics.Raycast(transform.position, target.position - transform.position, out hitInfo, (target.position - transform.position).magnitude))
		{
			if(range <= chaseRange && range >= dashRange)
			{
				//chase
				enemyState = EnemyState.Moving;
				transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
				var moveDir = transform.TransformDirection(Vector3.forward);
				transform.position += moveDir * moveSpeed * Time.deltaTime;
				currentTime = Time.time;
			}
			else if(range <= dashRange + 1 && Time.time < currentTime + dashChargeTime)
			{
				//charge up
				enemyState = EnemyState.ChargeUp;
				transform.LookAt(new Vector3(target.position.x, transform.position.y , target.position.z));
				var moveDir = transform.TransformDirection(Vector3.forward);
				transform.position += moveDir * Time.deltaTime;
			}
			else if(Time.time >= currentTime + dashChargeTime && Time.time < currentTime + dashChargeTime + dashTime)
			{
				//dash
				var moveDir = transform.TransformDirection(Vector3.forward);
				transform.position += moveDir * moveSpeed * 3 * Time.deltaTime;
				enemyState = EnemyState.Dashing;
			}
		}
		else
			Debug.DrawRay(transform.position, (target.position - transform.position));
	}
	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.CompareTag("Sword"))
		{	
			//reset the dash timer
			currentTime = Time.time;
			
			GameObject.FindWithTag("Player").SendMessage("SendMSGForward", gameObject);
		}

		if(collider.gameObject.CompareTag("Player"))
		{
			collider.gameObject.SendMessage("DamagePlayer", new Vector4(transform.forward.x, 0, transform.forward.z, damageOnHit));
			currentTime = Time.time;
		}
	}
	
	void GetHit(float[] hitDirStuff)
	{
		//0 = x translation
		//1 = y translation
		//2 = z translation
		//3 = damage (1 by default, 3 on last hit)
		//4 = knockback multiplier (6 by default, 12 otherwise)
		transform.position += new Vector3(hitDirStuff[0], hitDirStuff[1] + 0.1f, hitDirStuff[2]) * hitDirStuff[4];
		health -= (int)hitDirStuff[3];
		if(health <= 0)
		{
			if(type == "BOSS")
			{
				Screen.lockCursor = false;
				Application.LoadLevel("Victory");
			}
			GameObject.Destroy(gameObject);
		}
	}
}