using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollItemClickHandler : MonoBehaviour, IPointerClickHandler
{
    private event Action _clickListener;
    private void OnDestroy() => RemoveAllListeners();

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData) => _clickListener?.Invoke();

    public void AddClickListener(Action clickListener) => _clickListener += clickListener;

    public void RemoveClickListener(Action clickListener) => _clickListener -= clickListener;

    public void RemoveAllListeners() => _clickListener = null;
}
