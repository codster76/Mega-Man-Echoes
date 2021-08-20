using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public float projectileSpeed;
	public string direction;
	private Rigidbody2D rb2d;
	
    // Start is called before the first frame update
    void Start()
    {
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
}
