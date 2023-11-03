using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TempScript : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private VisualEffect vfx;

    [ContextMenu("CollideWithBox")]
    private void OnCollisionWithBox()
    {
        animator.SetTrigger("Destroy");
        vfx.enabled = true;
        StartCoroutine(OnBoxCollision());
    }

    public void Start()
    {
        StartCoroutine(OnBoxCollision());
    }

    private IEnumerator OnBoxCollision()
    {
        yield return new WaitForSeconds(2);
        if (vfx != null) vfx.SendEvent("OnBoxDestroyed");
    }
}
