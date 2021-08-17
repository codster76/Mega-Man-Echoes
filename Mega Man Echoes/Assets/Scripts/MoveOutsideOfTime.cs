using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOutsideOfTime : MonoBehaviour
{
	private Animator animator;
	
	void Start()
	{
		animator = gameObject.GetComponent<Animator>();
	}
	
    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Test"))
		{
			animator.SetTrigger("MoveCamera");
			Time.timeScale = 0;
		}
    }
	
	public void resumeTime()
	{
		Time.timeScale = 1;
	}
}
