using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.VFX;

public class MushroomSpring : MonoBehaviour
{
    //Game objects involved in the morion
    GameObject mushroom;
    private Coroutine routine;
    [SerializeField]
    private bool isJumping;
    public float height, distance;

    [Header("Visual Effect")]
    [SerializeField] private VisualEffect mushroomSpringVFX;

    #region Unity Methods
    private void Start()
    {
        var par = transform.parent.gameObject;
        mushroom = par.transform.GetChild(0).gameObject;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            if (!isJumping)
            {
                routine = StartCoroutine(SpringMotion(collision));
                isJumping = true;
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            if (routine != null)
            {
                if (isJumping)
                {
                    StopCoroutine(routine);
                    isJumping = false;
                    //GameManager.instance.input.enabled = true;
                    GameManager.instance.input.EnableInput();
                }
            }
        }
    }

    IEnumerator SpringMotion(Collision collision)
    {
        //  GameManager.instance.Player._sounds.SoundToBeUsed(7, SoundManager.Soundtype.other, 0.6f);
        mushroom.transform.DOScale(new Vector3(0.8f, 0.7f, 0.8f), 0.05f).SetEase(Ease.Linear);
        yield return null;
        //GameManager.instance.input.enabled = false;
        GameManager.instance.input.DisableInput();
        mushroomSpringVFX.Play();
        yield return new WaitForSeconds(0.05f);
        mushroom.transform.DOScale(Vector3.one, 0.05f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.05f);
        SoundManager._soundManager._otherSounds.SoundToBeUsed(7, SoundManager.Soundtype.other, false, true);
        GameManager.instance.Player.CustumJump(height, distance);
        //GameManager.instance.input.enabled = true;
        GameManager.instance.input.EnableInput();
    }

    #endregion
}
