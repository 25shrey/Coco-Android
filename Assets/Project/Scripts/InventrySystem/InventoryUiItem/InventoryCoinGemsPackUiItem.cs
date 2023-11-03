using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InventoryCoinGemsPackUiItem : BaseInventoryUiItem
{
    public CurrencyType uiRewardCurrencyType;
    public int uiItemRewardAmount;
    public TMP_Text uiItemRewardAmountText;

    public override void SetUiItemData(BaseInventoryData data)
    {
        CoinGemsPackData coinGemsPackData = new CoinGemsPackData();
        coinGemsPackData =(CoinGemsPackData) data;
        base.SetUiItemData(coinGemsPackData);
        uiRewardCurrencyType = coinGemsPackData.rewardCurrencyType;
        uiItemRewardAmount = coinGemsPackData.rewardAmount;
        uiItemRewardAmountText.text = coinGemsPackData.rewardAmount.ToString();
    }
    public void BuyItem()
    {
        InventoryManager.Instance.BuyCoinGemsPacks(this);
    }
}
