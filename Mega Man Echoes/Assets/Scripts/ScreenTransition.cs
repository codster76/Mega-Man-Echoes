using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransition : MonoBehaviour
{
	// Explanation for screen transitions in Assets/Documentation/Screen Transitions.txt
	
    public Animator cameraAnimator; // The camera's animator
	public Transform camera;
	private ScreenTransitionInfo screenTransition;
	
	void Start()
	{
		cameraAnimator.enabled = false;
	}
	
	void OnTriggerEnter2D(Collider2D c)
	{
		if(c.gameObject.tag == "Screen Transition")
		{
			screenTransition = c.gameObject.GetComponent<ScreenTransitionInfo>();
			cameraAnimator.enabled = true;
			cameraAnimator.Play("ScreenTransitionLeft",0,0);
			Time.timeScale = 0;
		}
	}
	
	public void resumeTime()
	{
		// Move the camera and its origin to the next room
		cameraAnimator.transform.position = screenTransition.cameraPosition.position;
		camera.position = screenTransition.cameraPosition.position;
		Time.timeScale = 1;
		cameraAnimator.enabled = false;
	}
}
