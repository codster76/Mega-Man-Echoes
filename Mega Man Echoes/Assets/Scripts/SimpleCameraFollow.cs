using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    public Rigidbody2D cameraRigidBody;

    // Update is called once per frame
    void FixedUpdate()
    {
        cameraRigidBody.MovePosition(new Vector3(transform.position.x, cameraRigidBody.transform.position.y));
    }
}
