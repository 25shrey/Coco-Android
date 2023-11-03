using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableClass : MonoBehaviour, IClickable, IPointerDownHandler, IPointerUpHandler 
{
    [SerializeField]
    private bool _click;

    public Transform coco;

    public bool clickStatus 
    {
        get { return _click; }
        set { _click = value; }
    }

    public void OnClick()
    {
        coco = CocoMainMenuHandler.Instance.mainMenuCoco.transform;
       // coco.GetComponent<Animator>().enabled = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        clickStatus = true;
        OnClick();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        clickStatus = false;
        MenuInput.Instance.isAllowedToRotate = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (clickStatus && MenuInput.Instance.isAllowedToRotate)
        {
            coco.Rotate(0, MenuInput.Instance.RotationAmount() * Time.deltaTime * MenuInput.Instance.rotateSpeed, 0);
        }
    }
}
