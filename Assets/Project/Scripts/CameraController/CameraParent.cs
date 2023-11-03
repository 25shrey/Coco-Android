using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCoreFramework;

public class CameraParent : MonoBehaviour
{
    private CameraController cam;
    public Camera _mainCam;
    public float time = 6f;

    private void Awake()
    {
        _mainCam.transform.SetParent(transform);
        _mainCam.gameObject.transform.position = transform.GetChild(0).transform.position;
        _mainCam.gameObject.transform.rotation = transform.GetChild(0).transform.rotation;

        cam = _mainCam.GetComponent<CameraController>();
    }

    private void Start()
    {
        StartCoroutine(FOV());
    }

    public void RemoveChild()
    {
        CameraController.Instance.enabled = true;
        cam.gameObject.transform.SetParent(null);
    }
    IEnumerator FOV()
    {
        while (time > 0)
        {
            _mainCam.fieldOfView = Mathf.Lerp(_mainCam.fieldOfView, 90, 2f * Time.deltaTime);

            time -= Time.deltaTime;

            yield return null;
        }

        _mainCam.fieldOfView = 90;
    }
}
