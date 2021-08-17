using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	private Rigidbody2D rb2d;
	
	public float dashDistance;
	public float dashTime;
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
	
	public float moveSpeed;
	public float jumpSpeed;
	
	public float gravity;
	
	enum State
	{
		Default,
		Jump,
		Slide
	};
	
	private State state;
	
    // Start is called before the first frame update
    void Start()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
		state = State.Default;
		facing = Facing.Right;
    }

    // Update is called once per frame
    void Update()
    {
		switch(state)
		{
			case State.Default:
				// Movement
				Move();
				
				// Sliding
				if(Input.GetButtonDown("Jump") && Input.GetAxis("Vertical") < 0)
				{
					state = State.Slide;
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
				//else if(jumpBuffer && onGround) // Jumping
				else if(Input.GetButtonDown("Jump") && onGround) // Jumping
				{
					yVelocity = jumpSpeed;
					state = State.Jump;
				}
				else if(!onGround)
				{
					state = State.Jump;
				}
				
				break;
			case State.Jump:
				Move(); // Allow horizontal movement while jumping
				if(onGround)
				{
					Debug.Log("A");
					StopFall();
					state = State.Default;
				}
				
				// Cancels jumps when the jump button is released
				if(Input.GetButtonUp("Jump") && yVelocity > 0)
				{
					StopFall();
				}
				break;
			case State.Slide:
				if(dashTimer > 0)
				{
					// Moving in the opposite direction of a slide cancels it
					if(facing == Facing.Left && Input.GetAxis("Horizontal") > 0)
					{
						state = State.Default;
					}
					else if (facing == Facing.Right && Input.GetAxis("Horizontal") < 0)
					{
						state = State.Default;
					}
					
					// Jumping while sliding cancels it
					if(Input.GetButtonDown("Jump") && onGround && Input.GetAxis("Vertical") >= 0)
					{
						yVelocity = jumpSpeed;
						state = State.Jump;
					}
					
					dashTimer -= Time.deltaTime/dashTime;
					rb2d.MovePosition(Vector2.Lerp(dashStart, dashTarget, 1-dashTimer));
				}
				else
				{
					state = State.Default;
				}
				break;
		}
		
		if(!onGround)
		{
			EndDash();
			yVelocity -= gravity * Time.deltaTime;
		}
		
		jumpBuffering();
		
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
	
	void jumpBuffering()
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
	}
	
	void Move()
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
}
