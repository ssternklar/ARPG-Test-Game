using UnityEngine;
using System.Collections;

public class CharController : MonoBehaviour
{
	enum CharacterState {
		Idle = 0,
		Moving = 1,
		Jumping = 2,
		Slashing1 = 3,
		Slashing2 = 4,
		Slashing3 = 5,
		AirSlash1 = 6,
		AirSlash2 = 7,
	}
	
	public AnimationClip idleAnim;
	public AnimationClip moveAnim;
	public AnimationClip jumpAnim;
	public AnimationClip slash1Anim, slash2Anim, slash3Anim;
	
	private Animation anim;

	bool hasCollided = false;
	
	private float jumpAnimationSpeed = 1.15f;
	private float landAnimationSpeed = 1.0f;
	
	private CharacterState characterState;
	private CharacterState prevCharacterState;
	
	public float speed = 2.0f;
	public float jumpHeight = 0.5f;
	public float gravity = 20.0f;

	private float speedSmoothing = 10.0f;
	private float rotateSpeed = 500.0f;
	
	public bool canJump = true;
	
	private bool isAttacking = false;
	
	//movement stuff
	private Vector3 moveDirection = Vector3.zero;

	public Vector3 velocity = Vector3.zero;

	private float verticalSpeed = 0.0f;
		//horizontal speed (x-z)
	private float moveSpeed = 0.0f;
	
	private bool movingBack = false;
	private bool isMoving = false;
	
	public int health = 10;
	
	CharacterController controller;
	private CollisionFlags collisionFlags;
	
	//jumping stuff
	private bool jumping = false;

	//is a controllable person
	private bool isControllable = true;
	
	private float oldAnimTime = 0f;
	
	// Use this for initialization
	void Start ()
	{
		//sets the current direction to forward
		moveDirection = transform.TransformDirection(Vector3.forward);
		
		//sets the animation to the base animation
		anim = (Animation)GetComponent("Animation");
		
		//Checks that all animations are present
		if(!anim)
		{
			//stops all animation
			anim = null;
			Debug.Log("No base animation!!!");
		}
		if(!idleAnim)
		{
			anim = null;
			Debug.Log("No idle animation!!!");
		}
		if(!slash1Anim || !slash2Anim || !slash3Anim)
		{
			anim = null;
			Debug.Log("Missing slash animation!!!");
		}
		if(!jumpAnim && canJump)
		{
			anim = null;
			Debug.Log("No jump animation found and the character has canJump enabled. Turning off animations.");
		}
		controller = (CharacterController)GetComponent("CharacterController");
	}

	// Update is called once per frame
	void Update ()
	{
		if (!isControllable)
		{
			// kill all inputs if not controllable.
			Input.ResetInputAxes();
		}

		ApplyGravity ();

		if (Input.GetButtonDown ("Jump"))
		{
			ApplyJumping();
		}


		ApplyHorizontalMovement();

		
		ApplyAttacking(controller);

		velocity += moveSpeed * moveDirection + new Vector3(0, verticalSpeed, 0);

		if(hasCollided)
			velocity = Vector3.ClampMagnitude(velocity, 80);
		else
			velocity = Vector3.ClampMagnitude(velocity, 20);

		var movement = velocity;
		movement *= Time.deltaTime;

		if(IsGrounded())
		{
			velocity *= .9f;
			hasCollided =false;
		}
		
		// Move the controller
		if(!isAttacking)
		{
			collisionFlags = controller.Move(movement);
			transform.position += new Vector3(0,0,0);
			
			transform.rotation = Quaternion.LookRotation(moveDirection);
		}
		else
		{
			collisionFlags = controller.Move(new Vector3(0, verticalSpeed, 0) * Time.deltaTime);
		}
		
		//STATE CHANGE
		if (IsGrounded() && !isAttacking && !jumping)
		{
			if(isMoving)
				characterState = CharacterState.Moving;
			else
				characterState = CharacterState.Idle;
		}
		
		//ANIMATION start
		ApplyAnimations();
		
		// We are in jump mode but just became grounded
		prevCharacterState = characterState;
	}
	void ApplyHorizontalMovement()
	{
		var cameraTransform = Camera.main.transform;
		var grounded = IsGrounded();
		
		// Forward vector relative to the camera along the x-z plane	
		var forward = cameraTransform.TransformDirection(Vector3.forward);
		forward.y = 0;
		forward = forward.normalized;
		
		// Right vector relative to the camera
		// Always orthogonal to the forward vector
		var right = new Vector3(forward.z, 0, -forward.x);
		
		//Vertical axis controls
		var v = Input.GetAxisRaw("Vertical");
		//Horizontal axiz controls
		var h = Input.GetAxisRaw("Horizontal");
		
		//moving back vs looking back
		if (v < -0.2)
			movingBack = true;
		else
			movingBack = false;
		
		var wasMoving = isMoving;
		isMoving = Mathf.Abs(h) > 0.1 || Mathf.Abs(v) > 0.1;
		
		// Target direction relative to the camera
		var targetDirection = h * right + v * forward;
		
		// We store speed and direction seperately,
		// so that when the character stands still we still have a valid forward direction
		// moveDirection is always normalized, and we only update it if there is user input.
		if (targetDirection != Vector3.zero)
		{
			// If we are really slow, just snap to the target direction
			if (moveSpeed < speed * 0.9 && grounded)
			{
				moveDirection = targetDirection.normalized;
			}
			// Otherwise smoothly turn towards it
			else
			{
				moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
				
				moveDirection = moveDirection.normalized;
			}
		}
		
		// Smooth the speed based on the current target direction
		var curSmooth = speedSmoothing * Time.deltaTime;
		
		// Choose target speed
		var targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);
		
		targetSpeed *= speed;
		
		moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, curSmooth);
	}
	
	void ApplyJumping()
	{	
		if (IsGrounded() && canJump)
		{
			jumping = true;
			verticalSpeed = CalculateJumpVerticalSpeed (jumpHeight);
			characterState = CharacterState.Jumping;
		}
	}
	
	void ApplyGravity()
	{
		if (isControllable)	// don't move player at all if not controllable.
		{
			if (IsGrounded ())
			{
				verticalSpeed = -0.01f;
				jumping = false;
			}
			else
				verticalSpeed -= gravity * Time.deltaTime;
		}
	}
	
	void ApplyAttacking(CharacterController controller)
	{
		if(Input.GetButtonDown("Fire1") && IsGrounded())
		{
			if(characterState == CharacterState.Slashing1 && animation[slash1Anim.name].time > 13f/24f)
			{
				characterState = CharacterState.Slashing2;
				isAttacking = true;
				oldAnimTime = -1f;
				canJump = false;
			}
			else if(characterState == CharacterState.Slashing2 && animation[slash2Anim.name].time > 13f/24f)
			{
				characterState = CharacterState.Slashing3;
				isAttacking = true;
				oldAnimTime = -1f;
				canJump = false;
			}
			else if((characterState != CharacterState.Slashing1) && (characterState != CharacterState.Slashing2) && (characterState != CharacterState.Slashing3))
			{
				characterState = CharacterState.Slashing1;
				isAttacking = true;
				oldAnimTime = -1f;
				canJump = false;
			}
		}
		if(characterState == CharacterState.Slashing1)
		{
			if(animation[slash1Anim.name].time < oldAnimTime)
			{
				isAttacking = false;
				characterState = CharacterState.Idle;
				oldAnimTime = -1f;
				canJump = true;
			}
			else
			{
				if(animation[slash1Anim.name].time < 13f/24f) controller.Move(transform.forward / 5);
				oldAnimTime = animation[slash1Anim.name].time;
			}
		}
		if(characterState == CharacterState.Slashing2)
		{
			if(animation[slash2Anim.name].time < oldAnimTime)
			{
				isAttacking = false;
				characterState = CharacterState.Idle;
				oldAnimTime = -1f;
				canJump = true;
			}
			else
			{
				if(animation[slash2Anim.name].time < 15f/24f) controller.Move(transform.forward / 5);
				oldAnimTime = animation[slash2Anim.name].time;
			}
		}
		if(characterState == CharacterState.Slashing3)
		{
			if(animation[slash3Anim.name].time < oldAnimTime)
			{
				isAttacking = false;
				characterState = CharacterState.Idle;
				oldAnimTime = -1f;
				canJump = true;
			}
			else
			{
				if(animation[slash3Anim.name].time < 12f/24f) controller.Move(transform.forward / 5);
				oldAnimTime = animation[slash3Anim.name].time;
			}
		}
	}

	public void ApplyAnimations()
	{
		if(animation)
		{
			if(characterState == CharacterState.Jumping)
			{
				if(velocity.y > 0)
				{
					animation[jumpAnim.name].speed = (float)jumpAnimationSpeed;
					animation[jumpAnim.name].wrapMode = WrapMode.ClampForever;
					animation.Play(jumpAnim.name);
				}
				else
				{
					animation[jumpAnim.name].speed = -landAnimationSpeed;
					animation[jumpAnim.name].wrapMode = WrapMode.ClampForever;
					animation.Play(jumpAnim.name);
				}
			}
			else
			{
				if(characterState == CharacterState.Idle)
				{
					animation.Play(idleAnim.name);
				}
				if(characterState == CharacterState.Moving)
				{
					animation.Play(moveAnim.name);
				}
				if(characterState == CharacterState.Slashing1)
				{
					animation.Play(slash1Anim.name);
				}
				if(characterState == CharacterState.Slashing2)
				{
					animation.Play(slash2Anim.name);
				}
				if(characterState == CharacterState.Slashing3)
				{
					animation.Play(slash3Anim.name);
				}
				/*if(characterState == CharacterState.AirSlash1)
				{
					animation.CrossFade(aSlash1Anim.name);
				}
				if(characterState == CharacterState.AirSlash2)
				{
					animation.CrossFade(aSlash2Anim.name);
				}*/
			}
		}
		//ANIMATION end
	}
	
	float CalculateJumpVerticalSpeed(float targetJumpHeight)
	{
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * targetJumpHeight * gravity);
	}
	
	public bool IsGrounded()
	{
		return (collisionFlags & CollisionFlags.CollidedBelow) != 0	;
	}
	public float GetSpeed()
	{
		return moveSpeed;
	}
	public bool IsJumping()
	{
		return jumping;	
	}
	public Vector3 GetDirection()
	{
		return moveDirection;
	}
	public void SendMSGForward(GameObject gObj)
	{
		if(isAttacking)
		{
			Vector3 v = GetDirection();
			float damage, knockback;
			if(characterState == CharacterState.Slashing3)
			{
				damage = 3f;
				knockback = 16f;
			}
			else
			{
				damage = 1f;
				knockback = 8f;
			}
			float[] array = new float[5];
			array[0] = v.x;
			array[1] = v.y;
			array[2] = v.z;
			array[3] = damage;
			array[4] = knockback;
			gObj.SendMessage("GetHit", array);
		}
	}
	public bool IsMovingBackwards()
	{
		return movingBack;
	}
	public bool IsMoving()
	{
		return Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5;
	}
	public void Reset ()
	{
		gameObject.tag = "Player";
	}
	public void DamagePlayer(Vector4 moveDir)
	{
		//controller.Move(new Vector3(moveDir.x, moveDir.y, moveDir.z) * 30);
		velocity += new Vector3(moveDir.x, moveDir.y + 1, moveDir.z) * 30;
		health -= (int)moveDir.w;
		if(health <= 0)
		{
			Application.LoadLevel("Game Over");
		}
	}
}