using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	private Rigidbody2D rb2d;
	
	public float dashDistance = 4.05f;
	public float dashTime = 0.43f;
	private float dashTimer;
	
	enum Facing
	{
		Left,
		Right
	};
	private Facing facing;
	
	private bool jumpBuffer;
	public float jumpBufferTime;
	private float jumpBufferTimer;
	
	private Vector2 dashTarget;
	private Vector2 dashStart;
	
	private bool onGround;
	
	private float yVelocity;
	private float xVelocity;
	
	private float moveSpeed;
	public float runSpeed = 5.125f;
	public float stepSpeed;
	public float jumpSpeed = 16f;
	public float hurtSpeed;
	
	public float gravity = 45f;
	
	private bool shooting;
	public float shootTime = 0.3f;
	private float shootTimer;
	
	public float walkStartTime = 0.075f;
	private float walkStartTimer;
	
	private Animator animator;
	private SpriteRenderer sprite;
	
	public AnimationClip walkAnimation;
	public AnimationClip teleport;
	public AnimationClip idleAnimation;
	public AnimationClip hurtAnimation;
	
	private float walkTimer;
	private float teleportTimer;
	private float idleTimer;
	
	private float hurtTimer;
	
	public float projectileSpeed;
	public ObjectPoolClass shotPool;
	
	public float chargeShot1Speed;
	public ObjectPoolClass chargeShot1Pool;
	
	public float chargeShot2Speed;
	public ObjectPoolClass chargeShot2Pool;
	
	private float chargeTimer;
	
	public float charge0 = 20f;
	public float charge1 = 34f;
	public float charge2 = 50f;
	public float charge3 = 66f;
	public float charge4 = 82f;
	
	/*public float charge0 = 0.3f;
	public float charge1 = 0.567f;
	public float charge2 = 0.833f;
	public float charge3 = 1.1f;
	public float charge4 = 1.367f;*/
	
	
	enum State
	{
		Teleport,
		Default,
		Jump,
		Slide,
		WalkStart,
		Walk,
		Hurt,
		Climb
	};
	
	private State state;
	
	enum WeaponState
	{
		Default,
		Charge0,
		Charge1,
		Charge2,
		Charge3,
		Charge4
	}
	
	private WeaponState weaponState;
	
	// Things I need to do
	/*
	- You can released charged shots while sliding
	- Climbing
	- Hurt
	- Slide particles
	- Hurt particles
	- Terminal velocity
	- Charge shot speed (both kinds)
	- Hurt speed
	*/
	
	
	
    // Start is called before the first frame update
    void Start()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
		animator = gameObject.GetComponent<Animator>();
		sprite = gameObject.GetComponent<SpriteRenderer>();
		
		facing = Facing.Right;
		
		state = State.Teleport;
		teleportTimer = teleport.length;
		animator.Play("Teleport", -1, 0);
    }

    // Update is called once per frame
    void Update()
    {
		// Pause while time is stopped
		if(Time.timeScale == 0)
		{
			return;
		}
		
		switch(state)
		{
			case State.Teleport:
				sprite.enabled = false; // Mega Man's actual sprite is invisible while teleporting in
				if(teleportTimer > 0)
				{
					teleportTimer -= Time.deltaTime;
				}
				else
				{
					sprite.enabled = true;
					state = State.Default;
					animator.Play("MegaManIdle", 0, 0);
				}
				break;
			case State.Default:
				Shoot();
				
				// Idle animation is a bit buggy (seems to be a lot shorter in the animator than the animation length indicates), so fix it later
				idleTimer += Time.deltaTime;
				idleTimer = idleTimer%idleAnimation.length;
			
				if(shooting)
				{
					animator.Play("MegaManIdleShoot", 0, 0);
				}
				else
				{
					animator.Play("MegaManIdle", 0, idleTimer);
				}
			
				// Movement
				if(Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Horizontal") < 0)
				{
					state = State.WalkStart;
					walkStartTimer = walkStartTime;
					animator.Play("MegaManShortWalk", 0, 0);
				}
				else // I know this looks pointless but if you walk for exactly 1 frame, your x velocity doesn't get reset
				{
					xVelocity = 0;
				}
				
				// Sliding
				if(Input.GetButtonDown("Jump") && Input.GetAxis("Vertical") < 0)
				{
					state = State.Slide;
					animator.Play("MegaManSlide", 0, 0);
					StartSlide();
				}
				//else if(jumpBuffer && onGround) // Jumping
				else if(Input.GetButtonDown("Jump") && onGround) // Jumping
				{
					yVelocity = jumpSpeed;
					state = State.Jump;
					animator.Play("MegaManJump", 0, 0);
				}
				//else if(Input.GetButtonUp("Jump") && yVelocity > 0)
				else if(!onGround)
				{
					state = State.Jump;
					animator.Play("MegaManJump", 0, 0);
				}
				
				break;
			case State.Jump:
				moveSpeed = runSpeed;
				Move(); // Allow horizontal movement while jumping
				Shoot();
				
				// Cancels jumps when the jump button is released
				// Unfortunately, it has to be GetButton because GetButtonUp sometimes gets eaten
				if(!Input.GetButton("Jump"))
				{
					StopJump();
				}
				
				if(shooting)
				{
					animator.Play("MegaManJumpShoot", 0, animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
				}
				else
				{
					animator.Play("MegaManJump", 0, 0);
				}
				
				if(onGround)
				{
					state = State.Walk;
					walkTimer = 0;
					animator.Play("MegaManWalk", 0, 0);
				}
				
				break;
			case State.Slide:
				moveSpeed = 0;
				if(dashTimer > 0)
				{
					// Moving in the opposite direction of a slide cancels it
					if(facing == Facing.Left && Input.GetAxis("Horizontal") > 0)
					{
						state = State.Default;
						animator.Play("MegaManIdle", 0, 0);
					}
					else if (facing == Facing.Right && Input.GetAxis("Horizontal") < 0)
					{
						state = State.Default;
						animator.Play("MegaManIdle", 0, 0);
					}
					
					// Jumping while sliding cancels it
					if(Input.GetButtonDown("Jump") && onGround && Input.GetAxis("Vertical") >= 0)
					{
						yVelocity = jumpSpeed;
						state = State.Jump;
						animator.Play("MegaManJump", 0, 0);
					}
					
					dashTimer -= Time.deltaTime/dashTime;
					rb2d.MovePosition(Vector2.Lerp(dashStart, dashTarget, 1-dashTimer));
				}
				else
				{
					state = State.Default;
					animator.Play("MegaManIdle", 0, 0);
				}
				break;
			case State.WalkStart:
				moveSpeed = stepSpeed;
				Move();
				Shoot();
				
				// Go back to default state if you stop moving
				if(Input.GetAxis("Horizontal") == 0)
				{
					state = State.Default;
					animator.Play("MegaManIdle", 0, 0);
				}
				// Sliding
				if(Input.GetButtonDown("Jump") && Input.GetAxis("Vertical") < 0)
				{
					state = State.Slide;
					animator.Play("MegaManSlide", 0, 0);
					StartSlide();
				}
				// Jumping
				else if(Input.GetButtonDown("Jump") && onGround)
				{
					yVelocity = jumpSpeed;
					state = State.Jump;
					animator.Play("MegaManJump", 0, 0);
				}
				// Falling
				else if(!onGround)
				{
					state = State.Jump;
					animator.Play("MegaManJump", 0, 0);
				}
				
				if(walkStartTimer > 0)
				{
					walkStartTimer -= Time.deltaTime;
				}
				else
				{
					state = State.Walk;
					walkTimer = 0;
					animator.Play("MegaManWalk", 0, 0);
				}
				
				break;
			case State.Walk:
				moveSpeed = runSpeed;
				Move();
				Shoot();
				
				// To track animation time (normalised time doesn't do it properly when you're constantly switching states)
				walkTimer += Time.deltaTime;
				walkTimer = walkTimer%walkAnimation.length;
				
				if(shooting)
				{
					animator.Play("MegaManShootWalk", 0, walkTimer/walkAnimation.length);
				}
				else
				{
					animator.Play("MegaManWalk", 0, walkTimer/walkAnimation.length);
				}
				
				// Go back to default state if you stop moving
				if(Input.GetAxis("Horizontal") == 0)
				{
					state = State.Default;
					animator.Play("MegaManIdle", 0, 0);
				}
				// Sliding
				if(Input.GetButtonDown("Jump") && Input.GetAxis("Vertical") < 0)
				{
					state = State.Slide;
					animator.Play("MegaManSlide", 0, 0);
					StartSlide();
				}
				// Jumping
				else if(Input.GetButtonDown("Jump") && onGround)
				{
					yVelocity = jumpSpeed;
					state = State.Jump;
					animator.Play("MegaManJump", 0, 0);
				}
				// Falling
				else if(!onGround)
				{
					state = State.Jump;
					animator.Play("MegaManJump", 0, 0);
				}
				
				break;
			case State.Hurt: // For some reason, knockback isn't working
				if(facing == Facing.Left)
				{
					moveSpeed = hurtSpeed;
				}
				else
				{
					moveSpeed = -hurtSpeed;
				}
				if(hurtTimer > 0)
				{
					hurtTimer -= Time.deltaTime;
				}
				else
				{
					state = State.Default;
					animator.Play("MegaManIdle", 0, 0);
					
					//Reset charged shot
					animator.Play("MegaManNoCharge", 1, 0);
					weaponState = WeaponState.Default;
				}
				break;
			case State.Climb:
				break;
		}
		
		switch(weaponState)
		{
			case WeaponState.Default:
				if(chargeTimer > charge0)
				{
					animator.Play("MegaManNoCharge", 1, 0);
					weaponState = WeaponState.Charge0;
				}
				if(Input.GetButtonUp("Shoot"))
				{
					chargeTimer = 0;
				}
				break;
			case WeaponState.Charge0:
				if(chargeTimer > charge1)
				{
					animator.Play("MegaManCharge1", 1, 0);
					weaponState = WeaponState.Charge1;
				}
				if(Input.GetButtonUp("Shoot"))
				{
					animator.Play("MegaManNoCharge", 1, 0);
					chargeTimer = 0;
					weaponState = WeaponState.Default;
					if(shotPool.poolCount() > 0 && chargeShot2Pool.poolCount() > 0 && chargeShot1Pool.poolCount() > 0)
					{
						CreateShot(shotPool, projectileSpeed);
					}
				}
				break;
			case WeaponState.Charge1:
				if(chargeTimer > charge2)
				{
					animator.Play("MegaManCharge2", 1, 0);
					weaponState = WeaponState.Charge2;
				}
				
				if(!Input.GetButton("Shoot"))
				{
					animator.Play("MegaManNoCharge", 1, 0);
					chargeTimer = 0;
					weaponState = WeaponState.Default;
					if(chargeShot1Pool.poolCount() > 0)
					{
						CreateShot(chargeShot1Pool, chargeShot1Speed);
					}
				}
				break;
			case WeaponState.Charge2:
				if(chargeTimer > charge3)
				{
					animator.Play("MegaManCharge3", 1, 0);
					weaponState = WeaponState.Charge3;
				}
				
				if(!Input.GetButton("Shoot"))
				{
					animator.Play("MegaManNoCharge", 1, 0);
					chargeTimer = 0;
					weaponState = WeaponState.Default;
					if(chargeShot1Pool.poolCount() > 0)
					{
						CreateShot(chargeShot1Pool, chargeShot1Speed);
					}
				}
				break;
			case WeaponState.Charge3:
				if(chargeTimer > charge4)
				{
					animator.Play("MegaManCharge4", 1, 0);
					weaponState = WeaponState.Charge4;
				}
				
				if(!Input.GetButton("Shoot"))
				{
					animator.Play("MegaManNoCharge", 1, 0);
					chargeTimer = 0;
					weaponState = WeaponState.Default;
					if(chargeShot1Pool.poolCount() > 0)
					{
						CreateShot(chargeShot1Pool, chargeShot1Speed);
					}
				}
				break;
			case WeaponState.Charge4:
				if(!Input.GetButton("Shoot"))
				{
					animator.Play("MegaManNoCharge", 1, 0);
					chargeTimer = 0;
					weaponState = WeaponState.Default;
					if(chargeShot2Pool.poolCount() > 0)
					{
						CreateShot(chargeShot2Pool, chargeShot2Speed);
					}
				}
				break;
		}
		
		if(!onGround)
		{
			EndDash(); // Cancels slides if you slide off an edge
			yVelocity -= gravity * Time.deltaTime; // Gravity
		}
		
		// Control the sprite orientation
		if(facing == Facing.Left)
		{
			sprite.flipX = false;
		}
		else
		{
			sprite.flipX = true;
		}
		
		// Shooting Timer
		if(shootTimer > 0)
		{
			shootTimer -= Time.deltaTime;
		}
		else
		{
			shooting = false;
		}
		
		//jumpBuffering();
		
		// For now, you can charge at any time
		Charge();
		
		// Note: I don't need to scale xVelocity by deltaTime because velocity is already scaled with time.
		rb2d.velocity = new Vector2(xVelocity, yVelocity);
    }
	
	void OnTriggerEnter2D(Collider2D c)
	{
		if(c.gameObject.tag == "Enemy")
		{
			animator.Play("MegaManHurt", 0, 0);
			weaponState = WeaponState.Default;
			state = State.Hurt;
			chargeTimer = 0;
			xVelocity = 0;
			hurtTimer = hurtAnimation.length;
			StopJump();
		}
	}
	
	public void EndDash()
	{
		dashTimer = 0;
	}
	
	public void SetGround(bool ground)
	{
		onGround = ground;
	}
	
	public void StopJump()
	{
		if(yVelocity > 0)
		{
			yVelocity = 0;
		}
	}
	
	public void ResetYVelocity()
	{
		yVelocity = 0;
	}
	
	private void Move()
	{
		if(Input.GetAxis("Horizontal") > 0)
		{
			xVelocity = moveSpeed;
			facing = Facing.Right;
		}
		else if(Input.GetAxis("Horizontal") < 0)
		{
			xVelocity = -moveSpeed;
			facing = Facing.Left;
		}
		else
		{
			xVelocity = 0;
		}
	}
	
	private void StartSlide()
	{
		dashTimer = 1; // needs to be 1 because I'm using lerp for dashing
		dashStart = transform.position;
		
		if(facing == Facing.Left)
		{
			dashTarget = new Vector2(transform.position.x - dashDistance, transform.position.y);
		}
		else
		{
			dashTarget = new Vector2(transform.position.x + dashDistance, transform.position.y);
		}
	}
	
	private void Shoot()
	{
		if(Input.GetButtonDown("Shoot") && shotPool.poolCount() > 0 && chargeShot2Pool.poolCount() > 0 && chargeShot1Pool.poolCount() > 0)
		{
			CreateShot(shotPool, projectileSpeed);
		}
		
		// This is just to reset the idle animation after shooting
		if(Input.GetButtonUp("Shoot"))
		{
			idleTimer = 0;
		}
	}
	
	private void Charge()
	{
		if(Input.GetButton("Shoot"))
		{
			chargeTimer += Time.deltaTime * 60;
		}
	}
	
	private void CreateShot(ObjectPoolClass objectPool, float speed)
	{
		// Start Mega Man's shooting animation
		shooting = true;
		shootTimer = shootTime;
		
		//instantiate
		MegaMan.Projectile projectile = objectPool.deploy().GetComponent<MegaMan.Projectile>();
		
		// Set initial variables
		if(facing == Facing.Left)
		{
			projectile.projectileSpeed = -speed;
			projectile.transform.position = new Vector2(transform.position.x - 0.9f, transform.position.y);
			projectile.direction = "Left";
		}
		else
		{
			projectile.projectileSpeed = speed;
			projectile.transform.position = new Vector2(transform.position.x + 0.9f, transform.position.y);
			projectile.direction = "Right";
		}
	}
}