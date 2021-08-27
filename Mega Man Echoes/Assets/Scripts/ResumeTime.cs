using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumeTime : MonoBehaviour
{
	// Explanation for screen transitions in Assets/Documentation/Screen Transitions.txt
	
	public ScreenTransition screenTransition;
	
    public void resumeTime()
	{
		screenTransition.resumeTime();
	}
}
