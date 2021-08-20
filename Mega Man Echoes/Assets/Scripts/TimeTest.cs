using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTest : MonoBehaviour
{
	public Animator animator;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		// just to get my thoughts down somewhere, my idea is that when you press the shoot button, the script checks the normalized time, scales it based on 1 cycle (so if the animation is 3 seconds long and it's currently at 16 seconds, you'd calculate the normalised time as if you're 1 second in) and plays the shooting animation at that point.
		
        //Debug.Log(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
		
		if(Input.GetButtonDown("Test"))
		{
			animator.Play("MegaManWalk", -1, animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
		}
    }
}
