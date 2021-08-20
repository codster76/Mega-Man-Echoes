using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillChild : MonoBehaviour
{
    public GameObject child;
	
	public void killChild()
	{
		Destroy(child);
	}
}
