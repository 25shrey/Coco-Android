using GameCoreFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BalloonBase : MonoBehaviour
{
    public float speed;
    public Transform endPosition;
    public Animator anim;
    public InputController Input;
    public CameraController cam;

    public abstract void MoveCamera();

    public IEnumerator Move(Vector3 targetPoint)
    {

        yield return new WaitForSeconds(0.1f);

        GameManager.instance.Player.GetComponent<Animator>().enabled = false;

        yield return new WaitForSeconds(0.1f);

        Vector3 currentPos = transform.position;
        float distance = Vector3.Distance(currentPos, targetPoint);
        float time = distance / speed;
        float totalTime = time;

        while (time > 0)
        {
            time -= Time.deltaTime;
            transform.position =
                Vector3.Lerp(currentPos, targetPoint, (totalTime - time) / totalTime);

            yield return null;
        }

        transform.position = targetPoint;

        if (transform.position == endPosition.position)
        {
            anim.SetBool("doorOpen", true);

            yield return new WaitForSeconds(0.2f);

            GameManager.instance.Player.GetComponent<Animator>().enabled = true;

            //Input.GetComponent<InputController>().enabled = true;
            GameManager.instance.input.EnableInput();

            GameManager.instance.Player.transform.SetParent(null);

            GameManager.instance.Player.CustomJumpAnimation();
        }
    }

    public void SwitchState(Transform obj)
    {
        StartCoroutine(SwitchCoroutine(obj));
    }

    IEnumerator SwitchCoroutine(Transform obj)
    {
        yield return new WaitForSeconds(3f);

        obj.gameObject.SetActive(false);
    }

}
