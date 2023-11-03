using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp : MonoBehaviour
{
    public Rigidbody ballPrefab;  // Reference to the ball prefab
    public float throwForce = 10f;  // Force to apply when throwing the ball

    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // Check for left mouse button click
        {
            Throw();  // Call the throw method
        }
    }

    void Throw()
    {
        // Instantiate a new ball at the current position and rotation
        Rigidbody ballInstance = Instantiate(ballPrefab, transform.position, transform.rotation);

        // Get the forward direction of the thrower object
        Vector3 throwDirection = transform.forward;

        // Apply force to the ball in the throw direction
        ballInstance.AddForce(throwDirection * throwForce, ForceMode.Impulse);
    }
}
