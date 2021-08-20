using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleCollision : MonoBehaviour
{
    private Collider2D collider;
	
	void Start()
	{
		collider = gameObject.GetComponent<BoxCollider2D>();
	}
	
	public void disableCollider()
	{
		collider.enabled = false;
	}
	
	public void enableCollider()
	{
		collider.enabled = true;
	}
}
