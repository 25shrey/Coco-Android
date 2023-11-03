using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BaseInventoryClasses : MonoBehaviour
{
}

public class GenericInventoryDataSO<T> : ScriptableObject where T : BaseInventoryData
{
    public InventoryItemType inventoryItemType;
    public List<T> inventoryData;
}

public class BaseInventoryData
{
    public string InventoryItemName;
    public string InventoryItemShortDescription;
    public Sprite InventoryItemImage;
    public CurrencyType InventoryItemPurchaseCurrencyType;
    public int InventoryItemPrice;
    public IAPData IAPData;
}

public class BaseInventoryUiItem : MonoBehaviour
{
    public TMP_Text uiInventoryItemName;
    public string uiInventoryItemShortDescription;
    public Image uiInventoryItemImage;
    public CurrencyType uiInventoryItemPurchaseCurrencyType;
    public int uiInventoryItemPrice;
    public TMP_Text uiInventoryItemPriceText;
    public Image uiInventoryItemCurrencyImage;
    public IAPData IAPData;
    public Button selectButton;
    public int pageIndex;
    public TMP_Text uiInventoryItemIAPPriceText;

    public virtual void SetUiItemData(BaseInventoryData data)
    {
        uiInventoryItemName.text = data.InventoryItemName;
        uiInventoryItemShortDescription = data.InventoryItemShortDescription;
        uiInventoryItemImage.sprite = data.InventoryItemImage;
        uiInventoryItemPurchaseCurrencyType = data.InventoryItemPurchaseCurrencyType;
        
        uiInventoryItemPrice = data.InventoryItemPrice;
        
        if (data.IAPData != null)
        {
            uiInventoryItemIAPPriceText.text = data.IAPData.productCost;
        }
        else
        {
            uiInventoryItemIAPPriceText.text = "$0";
        }

        uiInventoryItemPriceText.text = InventoryManager.Instance.AbbreviateNumber(data.InventoryItemPrice); //data.InventoryItemPrice.ToString();
        
        if (data.InventoryItemPurchaseCurrencyType == CurrencyType.Coin)
        {
            uiInventoryItemCurrencyImage.sprite = InventoryManager.Instance.coinCurrencyImage;
        }
        else if (data.InventoryItemPurchaseCurrencyType == CurrencyType.Gems)
        {
            uiInventoryItemCurrencyImage.sprite = InventoryManager.Instance.gemCurrencyImage;
        }

        if (data.IAPData != null)
        {
            IAPData = data.IAPData;
        }
    }
}