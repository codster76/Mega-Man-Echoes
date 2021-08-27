using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public GameObject camera;
	
	void OnTriggerStay2D(Collider2D c)
	{
		if(c.gameObject.tag == "Follow Zone")
		{
			camera.transform.position = new Vector3(transform.position.x,camera.transform.position.y,camera.transform.position.z);
		}
	}
	
	void OnTriggerEnter2D(Collider2D c)
	{
		// Might potentially cause collision issues, but it's here for a bit of optimisation.
		if(c.gameObject.tag == "Static Zone")
		{
			camera.transform.position = c.transform.position;
		}
	}
}
