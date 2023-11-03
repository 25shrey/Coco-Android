using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPowerUpUiItem : BaseInventoryUiItem
{
    public PowerUpType uiPowerUpType;
    public int uiPowerUpQuantity;
    public TMP_Text uiPowerUpQuantityText;

    public override void SetUiItemData(BaseInventoryData data)
    {
        PowerUpsData powerUpsData = new PowerUpsData();
        powerUpsData =(PowerUpsData) data;
        base.SetUiItemData(powerUpsData);
        uiPowerUpType = powerUpsData.powerUpType;
        uiPowerUpQuantity = powerUpsData.powerUpQuantity;
        uiPowerUpQuantityText.text = powerUpsData.powerUpQuantity.ToString();
    }
    public void BuyItem()
    {
        InventoryManager.Instance.BuyPowerUp(this);
    }
    
}
