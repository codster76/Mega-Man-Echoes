using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaMan
{
public class Projectile : MonoBehaviour
{
	public float projectileSpeed;
	public string direction;
	private Rigidbody2D rb2d;
	public ObjectPoolClass shotPool;
	
	// Start is called before the first frame update
	void Start()
	{
		shotPool = transform.parent.GetComponent<ObjectPoolClass>();
		rb2d = gameObject.GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void Update()
	{
		if(direction == "Left")
		{
			rb2d.velocity = new Vector2(-projectileSpeed, 0);
		}
		else
		{
			rb2d.velocity = new Vector2(projectileSpeed, 0);
		}
	}
	
	void OnCollisionEnter2D(Collision2D c)
	{
		returnToPool();
	}
	
	private void returnToPool()
	{
		shotPool.returnToPool(gameObject);
	}
}
}