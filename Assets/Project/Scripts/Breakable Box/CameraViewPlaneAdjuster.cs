using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraViewPlaneAdjuster : MonoBehaviour
{
    public float nearPlane;
    public LayerMask layerMask; 

    private void OnTriggerEnter(Collider other)
    {
        if(((1 << other.gameObject.layer) & layerMask) != 0)
        {
            GameManager.instance.CameraController.gameCamera.nearClipPlane = 0.1f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & layerMask) != 0)
        {
            GameManager.instance.CameraController.gameCamera.nearClipPlane = GameManager.instance.CameraController.nearClippingPlane;
        }
    }
}
