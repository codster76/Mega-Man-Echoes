using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransitionInfo : MonoBehaviour
{
	// Explanation for screen transitions in Assets/Documentation/Screen Transitions.txt
	
    public Transform cameraPosition; // The spot to put the camera in the next screen
	public string direction; // up, down, left or right
}
