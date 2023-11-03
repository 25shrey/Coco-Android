using System;
using System.Collections;
using System.Collections.Generic;
using Game.BaseFramework;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InventoryClothUiItem : BaseInventoryUiItem
{
    public InventoryItemType inventoryItemType;
    public int itemDisplayIndex;
    public InventoryItemPurchaseState purchaseState;
    public bool uiIsSkinnedMeshNeeded;
    public ClothItemPrefab uiClothObject;
    public TMP_Text uiInventoryItemStatusText;
    private IAPData currentIAPData;
    public BaseScroll scrollReference;

    private void Start()
    {
        
    }

    public override void SetUiItemData(BaseInventoryData data)
    {
        ClothData clothData = new ClothData();
        clothData =(ClothData) data;
        base.SetUiItemData(clothData);
        inventoryItemType = clothData.inventoryItemType;
        itemDisplayIndex = clothData.inventoryItemIndex;
        SetState(clothData.purchaseState);
        uiIsSkinnedMeshNeeded = clothData.isSkinnedMeshNeeded;
        uiClothObject = clothData.clothObject;
        InventoryManager.Instance.currentUiItemList.Add(this);
    }

    public void OnButtonClick()
    {
        switch (purchaseState)
        {
            case InventoryItemPurchaseState.Locked:
                if (InventoryManager.Instance.IsEnoughCurrencyForPurchase(this))
                {
                    SetState(InventoryItemPurchaseState.Selected);
                    InventoryManager.Instance.SaveClothData(this);
                    SavedDataHandler.Instance.SavePurchaseItemData(itemDisplayIndex, inventoryItemType);
                }
                else
                {
                    IAPManager.Instance.BuyProductID(IAPData.productID, () =>
                    {
                        Debug.Log("Purchase Cloth Successful");
                        SetState(InventoryItemPurchaseState.Selected);
                        InventoryManager.Instance.SaveClothData(this);
                        SavedDataHandler.Instance.SavePurchaseItemData(itemDisplayIndex, inventoryItemType);
                    });
                }
                break;

            case InventoryItemPurchaseState.Select:
                SetState(InventoryItemPurchaseState.Selected);
                InventoryManager.Instance.SaveClothData(this);
                break;
            case InventoryItemPurchaseState.Selected:
                break;
        }
    }

    public void SetState(InventoryItemPurchaseState state)
    {
        purchaseState = state;
        if (state != InventoryItemPurchaseState.Locked)
        {
            uiInventoryItemStatusText.text = purchaseState.ToString();
            //uiInventoryItemCurrencyImage.enabled = false;
        }
        else
        {
            uiInventoryItemStatusText.text = "BUY";
        }
    }

    public void OnButtonSelected()
    {
        if(pageIndex>scrollReference.currentPage)
        {
            scrollReference.Next();
        }
        else if(pageIndex < scrollReference.currentPage)
        {
            scrollReference.Previous();
        }
    }
}