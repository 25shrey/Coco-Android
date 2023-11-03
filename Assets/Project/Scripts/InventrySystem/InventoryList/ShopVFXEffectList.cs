using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopVFXEffectList : MonoBehaviour
{
    public RectTransform rootTransform;
    public InventoryVFXEffectUiItem itemPrefab;

    public void PopulateList(VFXEffect_DataSO data)
    {
        foreach (Transform t in rootTransform)
        {
            Destroy(t.gameObject);
        }

        for (int i = 0; i < data.inventoryData.Capacity; i++)
        {
            InventoryVFXEffectUiItem vfxEffectItem = Instantiate(itemPrefab, rootTransform);
            vfxEffectItem.SetUiItemData(data.inventoryData[i]);
        }
    }
}