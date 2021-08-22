using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetection : MonoBehaviour
{
	private Player player;
	
	void Start()
	{
		player = transform.parent.GetComponent<Player>();
	}
	
	void OnCollisionEnter2D(Collision2D c)
	{
		player.ResetYVelocity();
	}
	
	void OnCollisionStay2D(Collision2D c)
	{
		player.SetGround(true);
	}
	
	void OnCollisionExit2D(Collision2D c)
	{
		player.SetGround(false);
	}
}
