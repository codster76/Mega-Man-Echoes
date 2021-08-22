using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingCollision : MonoBehaviour
{
    private Player player;
	
	void Start()
	{
		player = transform.parent.GetComponent<Player>();
	}
	
	void OnCollisionEnter2D(Collision2D c)
	{
		player.StopJump();
	}
}
