using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
	public GameObject objectToSpawn;
	public int numberOfObjects;
	private Stack pool;
	
    // Start is called before the first frame update
    void Start()
    {
        pool = new Stack();
		for(int i = 0;i<numberOfObjects;i++)
		{
			GameObject toAdd = Instantiate(objectToSpawn);
			toAdd.SetActive(false);
			pool.Push(toAdd);
		}
    }
	
	public GameObject deploy()
	{
		return (GameObject)pool.Pop();
	}
	
	public void returnToPool(GameObject toReturn)
	{
		pool.Push(toReturn);
	}
}
