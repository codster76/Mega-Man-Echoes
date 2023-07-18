using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformEffectorCustom : MonoBehaviour
{
    private BoxCollider2D platformCollider;
    private BoxCollider2D playerCollider;

    void Start() {
        platformCollider = GetComponent<BoxCollider2D>();
        playerCollider = FindObjectOfType<Player>().GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerCollider.transform.position.y - playerCollider.size.y/2 > transform.position.y + transform.localScale.y/2) {
            platformCollider.enabled = true;
        } else {
            platformCollider.enabled = false;
        }

        Debug.DrawLine(new Vector2(transform.position.x, transform.position.y + platformCollider.size.y/2), new Vector2(transform.position.x, transform.position.y - platformCollider.size.y/2), Color.red);
    }
}
