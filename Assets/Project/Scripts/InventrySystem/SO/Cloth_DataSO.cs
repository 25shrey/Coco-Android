using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/FullOutfit_And_Accessories_DataSO", order = 1)]
public class Cloth_DataSO : GenericInventoryDataSO<ClothData>
{
    
}

[System.Serializable]
public class ClothData : BaseInventoryData
{
    public int inventoryItemIndex;
    public InventoryItemType inventoryItemType;
    public InventoryItemPurchaseState purchaseState;
    public bool isSkinnedMeshNeeded;
    public ClothItemPrefab clothObject;
}