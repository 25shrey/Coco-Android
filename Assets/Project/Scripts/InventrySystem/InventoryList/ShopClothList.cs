using System;
using System.Collections;
using System.Collections.Generic;
using TS.PageSlider;
using UnityEngine;
using UnityEngine.UI;

public class ShopClothList : MonoBehaviour
{
    public RectTransform rootTransform;
    public RectTransform dotRootTransform;
    public InventoryClothUiItem itemPrefab;
    public ScrollPage pageContainerItemPrefab;
    public ScrollPage pageContainerItemReference;
    public BaseScroll scrollSnapReference;
    public List<InventoryClothUiItem> clothUiItemsButtonData;

    public ScrollIndecator dotPrefab;

    [SerializeField] private CustomiseUI ui;

    public void PopulateList(Cloth_DataSO data)
    {
        foreach (Transform t in rootTransform)
        {
            Destroy(t.gameObject);
        }
        
        foreach (Transform t in dotRootTransform)
        {
            Destroy(t.gameObject);
        }

        InventoryManager.Instance.currentUiItemList.Clear();
        clothUiItemsButtonData.Clear();
        scrollSnapReference.dotList.Clear();

        scrollSnapReference.maxPage = 0;

        int counter = 0;
        int page = 1;
        int index = 1;
        for (int i = 0; i < data.inventoryData.Capacity; i++)
        {
            if (i==0 || counter!>5)
            {
                pageContainerItemReference=Instantiate(pageContainerItemPrefab, rootTransform);
                ScrollIndecator dotImage = Instantiate(dotPrefab, dotRootTransform);
                scrollSnapReference.dotList.Add(dotImage);
                scrollSnapReference.maxPage++;
                counter = 0;
            }
            InventoryClothUiItem clothItem = Instantiate(itemPrefab, pageContainerItemReference.content);
            clothUiItemsButtonData.Add(clothItem);
            clothItem.SetUiItemData(data.inventoryData[i]);
            counter++;
            clothItem.pageIndex = index;
            clothItem.scrollReference = scrollSnapReference;
            page++;
            if(page > 6)
            {
                page = 1;
                index++;
            }
        }
        
        scrollSnapReference.ResetScroll();
        
        if (scrollSnapReference.maxPage==1)
        {
            dotRootTransform.gameObject.SetActive(false);
        }
        else
        {
            dotRootTransform.gameObject.SetActive(true);
        }
        SetNavigation();
    }

    public void SetNavigation()
    {
        for (int index = 0; index < clothUiItemsButtonData.Count; index++)
        {
            Navigation navigation = new Navigation();
            navigation.mode = Navigation.Mode.Explicit;
            if(index == 0)
            {
                navigation.selectOnRight = clothUiItemsButtonData[index + 1].selectButton;
                navigation.selectOnLeft = ui.AllButtons[0];
            }
            else if (index == clothUiItemsButtonData.Count-1)
            {
                navigation.selectOnLeft = clothUiItemsButtonData[index - 1].selectButton;
            }
            else
            {
                navigation.selectOnRight = clothUiItemsButtonData[index + 1].selectButton;
                navigation.selectOnLeft = clothUiItemsButtonData[index - 1].selectButton;
            }
            clothUiItemsButtonData[index].selectButton.navigation = navigation;
        }

        for (int i = 0; i <= ui.AllButtons.Count - 1; i++)
        {
            if (i == 0)
            {
                Navigation panelNav = new Navigation();
                panelNav.mode = Navigation.Mode.Explicit;
                panelNav.selectOnDown = ui.AllButtons[i + 1];
                panelNav.selectOnRight = clothUiItemsButtonData[0].selectButton;
                ui.AllButtons[i].navigation = panelNav;
            }
            else if (i == ui.AllButtons.Count - 2)
            {
                Navigation panelNav = new Navigation();
                panelNav.mode = Navigation.Mode.Explicit;
                panelNav.selectOnUp = ui.AllButtons[i - 1];
                panelNav.selectOnDown = ui.AllButtons[i + 1];
                ui.AllButtons[i].navigation = panelNav;
            }
            else if(i != ui.AllButtons.Count - 1)
            {
                Navigation panelNav = new Navigation();
                panelNav.mode = Navigation.Mode.Explicit;
                panelNav.selectOnDown = ui.AllButtons[i + 1];
                panelNav.selectOnUp = ui.AllButtons[i - 1];
                ui.AllButtons[i].navigation = panelNav;
            }
        }
    }
}