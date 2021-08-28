using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransitionInfo : MonoBehaviour
{
	// Explanation for screen transitions in Assets/Documentation/Screen Transitions.txt
	
    public Transform endPosition; // The spot to put the camera in the next screen
	public Transform startPosition;
	public string animation; // the name of the animation state to play. Figure out how to make a dropdown for this.
	public ScreenTransitionInfo nextScreenTransition; // Temporary. I'll need to load all screen transitions for the next room at once, rather than just one.
}
