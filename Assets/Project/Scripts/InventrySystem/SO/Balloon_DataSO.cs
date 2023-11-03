using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Balloon_DataSO", order = 1)]
public class Balloon_DataSO : GenericInventoryDataSO<BalloonData>
{
    
}

[System.Serializable]
public class BalloonData : BaseInventoryData
{
    public int inventoryItemIndex;
    public InventoryItemType inventoryItemType;
    public List<Texture> balloonTextureList;
    public InventoryItemPurchaseState purchaseState;
}