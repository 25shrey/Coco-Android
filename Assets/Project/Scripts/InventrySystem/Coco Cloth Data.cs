using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CocoClothData : MonoBehaviour
{
    public Transform boneStructure;
    public Transform newSkinMeshOutfitParent;
    public Transform newMeshOutfitParent;
    public List<CurrentCloths> currentClothsList;
}

[System.Serializable]
public class CurrentCloths
{
    public InventoryItemType inventoryItemType;
    public ClothItemPrefab currentWearingClothObject;
}
