using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BaseScroll : MonoBehaviour,IEndDragHandler
{
    public int maxPage;
    public int currentPage;
    public Vector3 targetPos;
    public Vector3 initialPos;
    public Vector3 pageStep;
    public RectTransform content;
    public float scrollSpeed;
    public float dragThreshold;
    public List<ScrollIndecator> dotList;
    public Sprite enableDotColor;
    public Sprite disableDotColor;

    private void OnEnable()
    {
        MenuInput._scrollRight += Next;
        MenuInput._scrollLeft += Previous;
    }

    private void OnDisable()
    {
        MenuInput._scrollRight -= Next;
        MenuInput._scrollLeft -= Previous;
    }

    public virtual void Start()
    {
        currentPage = 1;
        targetPos = content.localPosition;
        initialPos = content.localPosition;
        dragThreshold = Screen.width / 15;
    }

    public void ResetScroll()
    {
        currentPage = 1;
        targetPos = initialPos;
        //UpdateDot();
        MovePage();
    }

    public void Next()
    {
        if (currentPage<maxPage)
        {
            currentPage++;
            targetPos += pageStep;
            MovePage();
        }
    }
    
    public void Previous()
    {
        if (currentPage>1)
        {
            currentPage--;
            targetPos -= pageStep;
            MovePage();
        } 
    }
    
    public void MovePage()
    {
        content.DOLocalMoveX(targetPos.x,scrollSpeed).SetEase(Ease.Linear);
        UpdateDot();
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(eventData.position.x-eventData.pressPosition.x)>dragThreshold)
        {
            if (eventData.position.x > eventData.pressPosition.x)
            {
                Previous();
            }
            else
            {
                Next();
            }
        }
        else
        {
            MovePage();
        }
    }

    public virtual void UpdateDot()
    {
        foreach (var item in dotList)
        {
            item.dotImage.sprite = disableDotColor;
        }
        dotList[currentPage - 1].dotImage.sprite = enableDotColor;
    }
}
