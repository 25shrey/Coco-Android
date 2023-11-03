using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class FloatingPlatform : FloatingObject
{
    public enum Direction
    {
        X, Y, Z
    }
    public Direction direction;


    public override void Start()
    {
        base.Start();
    }


    public override void FixedUpdate()
    {
        float time = Time.fixedTime - delay;

        switch (direction)
        {
            case Direction.X:
                {
                    float newX = startPos.x + amplitude * Mathf.Sin(speed * time);
                    transform.position = new Vector3(newX, transform.position.y, transform.position.z);
                    break;
                }
            case Direction.Y:
                {
                    float newY = startPos.y + amplitude * Mathf.Sin(speed * time);
                    transform.position = new Vector3(transform.position.x, newY, transform.position.z);
                    break;
                }
            case Direction.Z:
                {
                    float newZ = startPos.z + amplitude * Mathf.Sin(speed * time);
                    transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
                    break;
                }
        }
    }
}
