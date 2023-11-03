using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    #region PUBLIC_VARS

    public Rigidbody rb;
    
    #endregion

    #region PRIVATE_VARS

    #endregion

    #region UNITY_CALLBACKS

    #endregion

    #region PUBLIC_FUNCTIONS

    public void Role(Vector3 TargetPos)
    {
        
    }
    
    public void Launch(Vector3 targetPosition, float launchAngle)
    {
        // Ref : https://vilbeyli.github.io/Projectile-Motion-Tutorial-for-Arrows-and-Missiles-in-Unity3D/

        // think of it as top-down view of vectors: 
        //   we don't care about the y-component(height) of the initial and target position.
        Vector3 projectileXZPos = new Vector3(transform.position.x, 0.0f, transform.position.z);
        Vector3 targetXZPos = new Vector3(targetPosition.x, 0.0f, targetPosition.z);

        // rotate the object to face the target
        transform.LookAt(targetPosition);
        // shorthands for the formula
        float R = Vector3.Distance(projectileXZPos, targetXZPos);
        float G = Physics.gravity.y;
        float tanAlpha = Mathf.Tan(launchAngle * Mathf.Deg2Rad);
        float H = targetPosition.y - transform.position.y;

        // calculate the local space components of the velocity 
        // required to land the projectile on the target object 
        float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)));
        float Vy = tanAlpha * Vz;

        // create the velocity vector in local space and get it in global space
        Vector3 localVelocity = new Vector3(0f, Vy, Vz);
        Vector3 globalVelocity = transform.TransformDirection(localVelocity);

        // launch the object by setting its initial velocity and flipping its state
        rb.velocity = globalVelocity;
    }
    
    #endregion

    #region PRIVATE_FUNCTIONS

    #endregion

    #region CO-ROUTINES

    #endregion

    #region EVENT_HANDLERS

    #endregion

    #region UI_CALLBACKS

    #endregion
}
