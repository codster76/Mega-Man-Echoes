using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolClass : MonoBehaviour
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
			toAdd.transform.parent = gameObject.transform;
			toAdd.SetActive(false);
			pool.Push(toAdd);
		}
	}
	
	public GameObject deploy()
	{
		GameObject toDeploy = (GameObject)pool.Pop();
		toDeploy.SetActive(true);
		return toDeploy;
	}
	
	public void returnToPool(GameObject toReturn)
	{
		toReturn.SetActive(false);
		pool.Push(toReturn);
	}
	
	public int poolCount()
	{
		return pool.Count;
	}
}