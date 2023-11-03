using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteAlways]
public class CameraRotationLock : MonoBehaviour
{
    [ExecuteAlways]
    private void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
}
