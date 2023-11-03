using System.Collections;
using System.Collections.Generic;
using Game.BaseFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryBalloonUiItem : BaseInventoryUiItem
{
    public InventoryItemType inventoryItemType;
    public int itemDisplayIndex;
    public List<Texture> balloonTextureList;
    public InventoryItemPurchaseState purchaseState;
    public TMP_Text uiInventoryItemStatusText;

    public override void SetUiItemData(BaseInventoryData data)
    {
        BalloonData balloonData = new BalloonData();
        balloonData =(BalloonData) data;
        base.SetUiItemData(balloonData);
        inventoryItemType = balloonData.inventoryItemType;
        itemDisplayIndex = balloonData.inventoryItemIndex;
        balloonTextureList = balloonData.balloonTextureList;
        SetState(balloonData.purchaseState);
        InventoryManager.Instance.currentBalloonUiItemList.Add(this);
    }
    /*public void BuyItem()
    {
        InventoryManager.Instance.BuyBalloonSkin(this);
    }*/
    
    public void OnButtonClick()
    {
        switch (purchaseState)
        {
            case InventoryItemPurchaseState.Locked:
                if (InventoryManager.Instance.IsEnoughCurrencyForPurchase(this))
                {
                    SetState(InventoryItemPurchaseState.Selected);
                    InventoryManager.Instance.SaveBalloonData(this);
                    SavedDataHandler.Instance.SavePurchasedBalloonItemData(itemDisplayIndex);
                }
                else
                {
                    IAPManager.Instance.BuyProductID(IAPData.productID, () =>
                    {
                        Debug.Log("Purchase BalloonItem Successful");
                        SetState(InventoryItemPurchaseState.Selected);
                        InventoryManager.Instance.SaveBalloonData(this);
                        SavedDataHandler.Instance.SavePurchasedBalloonItemData(itemDisplayIndex);
                    });
                }
                break;

            case InventoryItemPurchaseState.Select:
                SetState(InventoryItemPurchaseState.Selected);
                InventoryManager.Instance.SaveBalloonData(this);
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
            /*uiInventoryItemPriceText.text = purchaseState.ToString();
            uiInventoryItemCurrencyImage.enabled = false;*/
            uiInventoryItemStatusText.text = purchaseState.ToString();
        }
        else
        {
            uiInventoryItemStatusText.text = "BUY";
        }
    }
    
}
