using System;
using System.Collections;
using System.Collections.Generic;
using Game.BaseFramework;
using UnityEngine;
using UnityEngine.InputSystem;

public class CocoMainMenu : MonoBehaviour
{
    public Animator anim;
    private RaycastHit hit;
    public LayerMask _layer;
    public Rigidbody rb;
    public float force;
    public int waitTime;
    //public SkinnedMeshRenderer skin;
    /*public Transform boneStructure;
    public Transform newSkinMeshOutfitParent;
    public Transform newMeshOutfitParent;*/
    public CocoClothData clothDataReference;
    public bool isCocoIdeal;

    private int _pointAnimation;
    private int _showWatchAnimation;
    private int _resetCocoAnimator;
    private int _dancingAnimation;

    private void Awake()
    {
        SetInventoryData();
    }
    
    public void Start()
    {
        _pointAnimation = Animator.StringToHash("PointAnimation");
        _showWatchAnimation = Animator.StringToHash("Show_Watch");
        _resetCocoAnimator = Animator.StringToHash("Reset");
        _dancingAnimation = Animator.StringToHash("Dancing");
        if (!isCocoIdeal)
        {
            StartCoroutine(WaitingTimer());
        }
    }
    
    public void SetInventoryData()
    {
        SavedDataHandler.Instance.LoadInventoryData();
        SavedDataHandler.Instance.LoadBalloonInventoryData();
        InventoryManager.Instance.newArmature = clothDataReference.boneStructure;
        InventoryManager.Instance.newSkinMeshParent = clothDataReference.newSkinMeshOutfitParent;
        InventoryManager.Instance.newMeshParent = clothDataReference.newMeshOutfitParent;
        InventoryManager.Instance.cocoClothReference = clothDataReference;
        //InventoryManager.Instance.cocoMainMenuReference = this;
        //Debug.Log("--- it is done");
        InventoryManager.Instance.LoadInventoryData();
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out hit))
            {
                if ((((1 << hit.transform.gameObject.layer) & _layer) != 0) && !anim.GetBool("PointAnimation"))
                {
                    //Debug.Log(anim.GetBool(_pointAnimation));
                    if (!isCocoIdeal)
                    {
                        anim.SetBool(_pointAnimation, true);
                        anim.SetBool(_dancingAnimation, false);
                        StartCoroutine(WaitingTimer());
                        Invoke("EndpointAnimation",1f);
                    }
                    StopAllCoroutines();
                    //anim.SetTrigger(excitedAnimation);
                }
                else if ((((1 << hit.transform.gameObject.layer) & _layer) != 0) && anim.GetBool("PointAnimation"))
                {
                   // anim.SetTrigger(excitedAnimation);
                }
                else if(hit.transform.gameObject.name == "Head")
                {
                   // anim.SetTrigger(angryAnimation);
                }
            }
        }
    }

    public IEnumerator WaitingTimer()
    {
        yield return new WaitForSeconds(waitTime);
        Debug.Log("WaitingTimer");
        anim.SetTrigger(_showWatchAnimation);
        if (!isCocoIdeal)
        {
            StartCoroutine(WaitingTimer());
        }
    }

    public void EndpointAnimation()
    {
        anim.SetBool(_pointAnimation, false);
        //Debug.Log(anim.GetBool(_pointAnimation));
        anim.SetBool(_dancingAnimation, true);
        //anim.SetTrigger(smileAnimation);
    }

    public void Jump()
    {
        rb.AddForce(Vector3.up * force, ForceMode.Impulse);
    }

    public void SetPlayerState(bool playerState)
    {
        isCocoIdeal = playerState;
        if (playerState)
        {
            anim.SetBool(_dancingAnimation, false);
            anim.SetTrigger(_resetCocoAnimator);
            StopAllCoroutines();
        }
        else
        {
            anim.SetBool(_dancingAnimation, true);
        }
    }
}


