using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

public class SpawnShieldRipples : MonoBehaviour
{
    [SerializeField] private float dissolveTime;
    [SerializeField] private float displacementMagnitude;
    [SerializeField] AnimationCurve _DisplacementCurve;
    [SerializeField] private MeshRenderer bubbleShieldMeshRenderer;
    //public bool isShieldEnabled;
    private Coroutine _shieldCoroutine;

    private void OnCollisionStay(Collision co)
    {
        bubbleShieldMeshRenderer.material.SetVector("_HitPos", co.contacts[0].point);
        StartCoroutine(HitDisplacement());
    }

    public void EnableOrDisableShield(bool isShieldOn, Action Callback = null)
    {
        float target;
        if (!isShieldOn)
        {
            if (_shieldCoroutine!=null)
            {
                StopCoroutine(_shieldCoroutine);
            }
            target = 1;
            _shieldCoroutine=StartCoroutine(DissolveShield(target, Callback));
        }
        else
        {
            if (_shieldCoroutine!=null)
            {
                StopCoroutine(_shieldCoroutine);
            }
            target = 0;
            _shieldCoroutine=StartCoroutine(DissolveShield(target, Callback));
        }
    }

    private IEnumerator DissolveShield(float target, Action Callback = null)
    {
        //isShieldEnabled = true;
        float start = bubbleShieldMeshRenderer.material.GetFloat("_Disolve");
        float lerpTime = 0;
        while (true)
        {
            if (lerpTime < dissolveTime)
            {
                bubbleShieldMeshRenderer.material.SetFloat("_Disolve", Mathf.Lerp(start, target, lerpTime));
                lerpTime += Time.deltaTime;
                yield return null;
            }
            else
            {
                bubbleShieldMeshRenderer.material.SetFloat("_Disolve", target);
                //isShieldEnabled = false;
                //Debug.Log("*Dissolve isShieldEnabled"+isShieldEnabled);
                Callback?.Invoke();
                break;
            }
        }
    }

    private IEnumerator HitDisplacement()
    {
        float lerpTime = 0;
        while (lerpTime < dissolveTime)
        {
            bubbleShieldMeshRenderer.material.SetFloat("_DisplacementStrength", _DisplacementCurve.Evaluate(lerpTime) * displacementMagnitude);
            lerpTime += Time.deltaTime;
            yield return null;
        }
    }
}
