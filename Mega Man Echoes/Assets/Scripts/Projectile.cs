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
	public ObjectPoolClass shotPool; // every projectile needs an object pool
	public int damage = 1;
	private SpriteRenderer sprite;
	
	void Start()
	{
		shotPool = transform.parent.GetComponent<ObjectPoolClass>();
		rb2d = gameObject.GetComponent<Rigidbody2D>();
		sprite = gameObject.GetComponent<SpriteRenderer>();
	}

	// Update is called once per frame
	void Update()
	{
		if(direction == "Left")
		{
			sprite.flipX = true;
		}
		else
		{
			sprite.flipX = false;
		}
		
		rb2d.velocity = new Vector2(projectileSpeed, 0);
	}
	
	void OnCollisionEnter2D(Collision2D c)
	{
		damage -= 1;
		if(damage <= 0)
		{
			returnToPool();
		}
	}
	
	private void returnToPool()
	{
		shotPool.returnToPool(gameObject);
	}
}
}