using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public GameObject camera;
	
	void OnTriggerStay2D(Collider2D c)
	{
		camera.transform.position = new Vector3(transform.position.x,camera.transform.position.y,camera.transform.position.z);
		Debug.Log("a");
	}
}
