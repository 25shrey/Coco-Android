using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InventoryVFXEffectUiItem : BaseInventoryUiItem
{
    public override void SetUiItemData(BaseInventoryData data)
    {
        VFXEffectData vfxEffectData = new VFXEffectData();
        vfxEffectData =(VFXEffectData) data;
        base.SetUiItemData(vfxEffectData);
        //InventoryManager.Instance.currentUiItemList.Add(this);
    }
    public void BuyItem()
    {
        InventoryManager.Instance.BuyVFXEffect(this);
    }
    
}
