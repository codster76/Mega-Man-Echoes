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
	
	private float walkTimer;
	private float teleportTimer;
	private float idleTimer;
	
	public float projectileSpeed;
	public ObjectPoolClass shotPool;
	//public Projectile projectile;
	
	private int frames;
	private float chargeTimer;
	private int chargeColour;
	private int chargeLevel;
	
	// timers based on frames
	public int charge1 = 34;
	public int charge2 = 50;
	public int charge3 = 66;
	public int charge4 = 82;
	
	public Material chargeMaterial1;
	public Material chargeMaterial2;
	public Material chargeMaterial3;
	public Material chargeMaterial4;
	public Material chargeMaterial5;
	
	
	enum State
	{
		Teleport,
		Default,
		Jump,
		Slide,
		WalkStart,
		Walk
	};
	
	private State state;
	
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
		switch(state)
		{
			case State.Teleport:
				sprite.enabled = false;
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
				if(!Input.GetButton("Jump") && yVelocity > 0)
				{
					StopFall();
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
				
				/*if(shooting)
				{
					animator.Play("MegaManIdleShoot", -1, animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
				}*/
				/*else
				{
					animator.Play("MegaManShortWalk", -1, 0);
				}*/
				
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
		}
		
		if(!onGround)
		{
			EndDash();
			yVelocity -= gravity * Time.deltaTime;
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
	
	public void EndDash()
	{
		dashTimer = 0;
	}
	
	public void SetGround(bool ground)
	{
		onGround = ground;
	}
	
	public void StopFall()
	{
		yVelocity = 0;
	}
	
	/*void jumpBuffering()
	{
		if(Input.GetButtonDown("Jump"))
		{
			jumpBufferTimer = jumpBufferTime;
		}
		if(jumpBufferTimer > 0)
		{
			jumpBuffer = true;
			jumpBufferTimer -= Time.deltaTime;
		}
		else
		{
			jumpBuffer = false;
		}
	}*/
	
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
		if(Input.GetButtonDown("Shoot") && shotPool.poolCount() > 0)
		{
			shooting = true;
			shootTimer = shootTime;
			
			//instantiate
			// I need access to the Projectile object type to run this bit
			MegaMan.Projectile projectile = shotPool.deploy().GetComponent<MegaMan.Projectile>();
			projectile.projectileSpeed = projectileSpeed;
			
			if(facing == Facing.Left)
			{
				projectile.transform.position = new Vector2(transform.position.x - 0.9f, transform.position.y);
				projectile.direction = "Left";
			}
			else
			{
				projectile.transform.position = new Vector2(transform.position.x + 0.9f, transform.position.y);
				projectile.direction = "Right";
			}
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
			frames = (int)chargeTimer;
		}
		else
		{
			chargeTimer = 0;
			frames = 0;
			chargeLevel = 0;
		}
		
		switch(chargeLevel)
		{
			case 0:
				if(frames > charge1)
				{
					
				}
				break;
			case 1:
				
				break;
			case 2:
				
				break;
			case 3:
				
				break;
			case 4:
				
				break;
		}
		
		if(frames > 0 && frames < charge1)
		{
			
		}
		else if (frames > charge1 && frames < charge2)
		{
			if(frames%3 == 0)
			{
				chargeColour++;
			}
			
			if(chargeColour >= 7)
			{
				chargeColour = 0;
			}
		}
		else if (frames > charge2 && frames < charge3)
		{
			if(frames%3 == 0)
			{
				
			}
		}
		else if (frames > charge3 && frames < charge4)
		{
			if(frames%3 == 0)
			{
				
			}
		}
		else if (frames > charge4)
		{
			if(frames%3 == 0)
			{
				
			}
		}
	}
}