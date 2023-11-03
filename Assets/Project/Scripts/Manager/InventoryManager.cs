using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Game.BaseFramework;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

#region Enums

public enum InventoryItemType
{
    FullOutfit,
    Cap,
    Shoe,
    Bag,
    Glove,
    BalloonSkin,

    VFXEffect
    //CoinGemPack,
    //PowerUp
}

public enum CurrencyType
{
    Coin,
    Gems,
    IAP
}

public enum InventoryItemPurchaseState
{
    Locked,
    Select,
    Selected
}

#endregion

public class InventoryManager : Game.BaseFramework.Singleton<InventoryManager>
{
    public Transform newArmature;
    public Transform newSkinMeshParent;
    public Transform newMeshParent;
    public CocoClothData cocoClothReference;
    public Material balloonMaterial;
    public Sprite coinCurrencyImage;
    public Sprite gemCurrencyImage;
    public List<InventoryClothUiItem> currentUiItemList;
    public List<InventoryBalloonUiItem> currentBalloonUiItemList;

    [Header("---> Scriptable Objects <---")]
    public List<Cloth_DataSO> clothDataSO;

    public Balloon_DataSO balloonDataSO;
    public VFXEffect_DataSO VFXEffectDataSO;

    [Header("---> Coco's Current Inventory Data <---")]
    public List<FinalInventoryItem> finalInventoryItemData;

    public FinalInventoryItem finalBalloonInventoryItem;
    
    public string AbbreviateNumber(float val)
    {
        string cash="0";
        
        if(val==0)
        {
            cash = "0";
        }
        else
        {
            if(val>=1000 && val < 1000000)
            {
                cash = Math.Round(val / 1000f, 2) + "K";
            }
            else if(val>= 1000000 && val < 1000000000)
            {
                cash = Math.Round(val / 1000000f, 2) + "M";
            }
            else if (val >= 1000000000)
            {
                cash = Math.Round(val / 1000000000f, 2) + "B";
            }
            else
            {
                cash = FormatFloat(val);
            }
        }

        return cash;
    }
    
    public string FormatFloat(float val)
    {
        if (val % 1f < float.Epsilon)
        {
            return val.ToString("N0", CultureInfo.CurrentCulture);
        }
        else
        {
            return val.ToString("n2");
            // return val.ToString("N2", CultureInfo.CurrentCulture);
        }
    }

    #region ClothManagement

    public void TransferSkinnedMeshes(ClothItemPrefab clothItemPrefab)
    {
        //Debug.Log("--- TransferSkinnedMeshes ");
        foreach (var t in clothItemPrefab.clothMeshRendererObject)
        {
            string cachedRootBoneName = t.rootBone.name;
            var newBones = new Transform[t.bones.Length];
            for (var x = 0; x < t.bones.Length; x++)
                foreach (var newBone in newArmature.GetComponentsInChildren<Transform>())
                    if (newBone.name == t.bones[x].name)
                    {
                        newBones[x] = newBone;
                    }

            Transform matchingRootBone = GetRootBoneByName(newArmature, cachedRootBoneName);
            t.rootBone = matchingRootBone != null ? matchingRootBone : newArmature.transform;
            t.bones = newBones;
        }
    }

    static Transform GetRootBoneByName(Transform parentTransform, string name)
    {
        return parentTransform.GetComponentsInChildren<Transform>()
            .FirstOrDefault(transformChild => transformChild.name == name);
    }

    public void SaveClothData(InventoryClothUiItem item)
    {
        Cloth_DataSO dataSO = clothDataSO.Find(x => x.inventoryItemType == item.inventoryItemType);
        FinalInventoryItem finalInventoryItem =
            finalInventoryItemData.Find(x => x.inventoryItemType == item.inventoryItemType);
        currentUiItemList[finalInventoryItem.itemIndex].SetState(InventoryItemPurchaseState.Select);
        dataSO.inventoryData[finalInventoryItem.itemIndex].purchaseState = InventoryItemPurchaseState.Select;
        dataSO.inventoryData[item.itemDisplayIndex].purchaseState = InventoryItemPurchaseState.Selected;
        finalInventoryItem.itemIndex = item.itemDisplayIndex;
        SavedDataHandler.Instance.SaveInventoryData();
        TryClothItem(item);
    }

    public void TryClothItem(InventoryClothUiItem item)
    {
        CurrentCloths currentCloth =
            cocoClothReference.currentClothsList.Find(x => x.inventoryItemType == item.inventoryItemType);
        if (currentCloth.currentWearingClothObject != null)
        {
            Destroy(currentCloth.currentWearingClothObject.gameObject);
        }

        currentCloth.currentWearingClothObject = GeneratingClothObject(item.uiClothObject, item.uiIsSkinnedMeshNeeded);
    }

    public ClothItemPrefab GeneratingClothObject(ClothItemPrefab clothItemPrefab, bool isSkinMeshNeeded)
    {
        if (isSkinMeshNeeded)
        {
            //Debug.Log("--- skinnedMesh ");
            ClothItemPrefab clothItem = Instantiate(clothItemPrefab, newSkinMeshParent);
            TransferSkinnedMeshes(clothItem);
            return clothItem;
        }
        else
        {
            //Debug.Log("--- simple mesh ");
            ClothItemPrefab clothItem = Instantiate(clothItemPrefab, newMeshParent);
            return clothItem;
        }
    }

    #endregion

    #region BalloonSkinManagement

    /*public void BuyBalloonSkin(InventoryBalloonUiItem item)
    {
        if (IsEnoughCurrencyForPurchase(item.uiInventoryItemCurrencyType,item.uiInventoryItemPrice))
        {
            balloonMaterial.mainTexture = item.balloonTextureList[0];
            balloonMaterial.SetTexture("_MetallicGlossMap", item.balloonTextureList[1]);
            balloonMaterial.SetTexture("_BumpMap", item.balloonTextureList[2]);
        }
    }*/

    public void SaveBalloonData(InventoryBalloonUiItem item)
    {
        /*Balloon_DataSO dataSO =
            balloonDataSO.Find(x => x.inventoryItemType == item.inventoryItemType);*/
        //FinalInventoryItem finalInventoryItem = finalInventoryItemData.Find(x => x.inventoryItemType == item.inventoryItemType);
        currentBalloonUiItemList[finalBalloonInventoryItem.itemIndex].SetState(InventoryItemPurchaseState.Select);
        balloonDataSO.inventoryData[finalBalloonInventoryItem.itemIndex].purchaseState =
            InventoryItemPurchaseState.Select;
        balloonDataSO.inventoryData[item.itemDisplayIndex].purchaseState = InventoryItemPurchaseState.Selected;
        finalBalloonInventoryItem.itemIndex = item.itemDisplayIndex;
        SavedDataHandler.Instance.SaveBalloonInventoryData();
        balloonMaterial.mainTexture = item.balloonTextureList[0];
        balloonMaterial.SetTexture("_MetallicGlossMap", item.balloonTextureList[1]);
        balloonMaterial.SetTexture("_BumpMap", item.balloonTextureList[2]);
    }

    public void SetBalloonItemMaterial(int index)
    {
        List<Texture> balloonTexture = balloonDataSO.inventoryData[index].balloonTextureList;
        balloonMaterial.mainTexture = balloonTexture[0];
        balloonMaterial.SetTexture("_MetallicGlossMap", balloonTexture[1]);
        balloonMaterial.SetTexture("_BumpMap", balloonTexture[2]);
    }

    #endregion

    #region PowerUpManagement

    public void BuyPowerUp(InventoryPowerUpUiItem item)
    {
        if (IsEnoughCurrencyForPurchase(item))
        {
            switch (item.uiPowerUpType)
            {
                case PowerUpType.Magnet:
                    SavedDataHandler.Instance.PowerUpMagnetCount += item.uiPowerUpQuantity;
                    break;
                case PowerUpType.Shield:
                    SavedDataHandler.Instance.PowerUpShieldCount += item.uiPowerUpQuantity;
                    break;
                case PowerUpType.FireBall:
                    SavedDataHandler.Instance.PowerUpFireBallCount += item.uiPowerUpQuantity;
                    break;
            }
        }
        else
        {
            Debug.Log("Product Id: " + item.IAPData.productID);
            IAPManager.Instance.BuyProductID(item.IAPData.productID, () =>
            {
                Debug.Log("IAP PowerUp Purchase Successful");
                //AddInGameCurrencies(item);
                switch (item.uiPowerUpType)
                {
                    case PowerUpType.Magnet:
                        SavedDataHandler.Instance.PowerUpMagnetCount += item.uiPowerUpQuantity;
                        break;
                    case PowerUpType.Shield:
                        SavedDataHandler.Instance.PowerUpShieldCount += item.uiPowerUpQuantity;
                        break;
                    case PowerUpType.FireBall:
                        SavedDataHandler.Instance.PowerUpFireBallCount += item.uiPowerUpQuantity;
                        break;
                }
            });
        }
        //if (item.uiInventoryItemPurchaseCurrencyType != CurrencyType.IAP)
        //{
        /*if (IsEnoughCurrencyForPurchase(item))
        {
            switch (item.uiPowerUpType)
            {
                case PowerUpType.Magnet:
                    SavedDataHandler.Instance.PowerUpMagnetCount += item.uiPowerUpQuantity;
                    break;
                case PowerUpType.Shield:
                    SavedDataHandler.Instance.PowerUpShieldCount += item.uiPowerUpQuantity;
                    break;
                case PowerUpType.FireBall:
                    SavedDataHandler.Instance.PowerUpFireBallCount += item.uiPowerUpQuantity;
                    break;
            }
        }
        else
        {*/
            /*UIController.Instance.ShowPopup(PopupType.CommonPopup);
            CommonPopup popup = UIController.Instance.GetPopup<CommonPopup>(PopupType.CommonPopup);
            if (popup != null)
            {
                string notEnoughtCurrencyText = $"Not Enough {Enum.GetName(item.uiInventoryItemPurchaseCurrencyType.GetType(), item.uiInventoryItemPurchaseCurrencyType)}!";
                popup.SetData(CommonPopupType.Error, notEnoughtCurrencyText); /*false, null, null, 5f);#1#
            }*/
        
        //}
        //}
    }

    #endregion

    #region CoinGemsPackManagement

    public void BuyCoinGemsPacks(InventoryCoinGemsPackUiItem item)
    {
        Debug.Log("Product Id: " + item.IAPData.productID);
        IAPManager.Instance.BuyProductID(item.IAPData.productID, () =>
        {
            Debug.Log("Add IAP Money");
            AddInGameCurrencies(item);
        });
        /*if (item.uiInventoryItemPurchaseCurrencyType != CurrencyType.IAP)
        {
            
        }
        else if (IsEnoughCurrencyForPurchase(item))
        {
            AddInGameCurrencies(item);
        }*/
    }

    #endregion

    #region VFXEffectManagement

    public void BuyVFXEffect(InventoryVFXEffectUiItem item)
    {
    }

    #endregion

    #region LoadData

    public void LoadInventoryData()
    {
        for (int i = 0; i < finalInventoryItemData.Count; i++)
        {
            //Debug.Log("--- Load Data ");
            Cloth_DataSO data =
                clothDataSO.Find(x => x.inventoryItemType == finalInventoryItemData[i].inventoryItemType);
            CurrentCloths currentCloth = cocoClothReference.currentClothsList.Find(x =>
                x.inventoryItemType == finalInventoryItemData[i].inventoryItemType);
            currentCloth.currentWearingClothObject = GeneratingClothObject(
                data.inventoryData[finalInventoryItemData[i].itemIndex].clothObject,
                data.inventoryData[finalInventoryItemData[i].itemIndex].isSkinnedMeshNeeded);
        }
    }

    #endregion

    #region ValidationFunction

    public bool IsEnoughCurrencyForPurchase(BaseInventoryUiItem item)
    {
        bool isPurchasable = false;
        switch (item.uiInventoryItemPurchaseCurrencyType)
        {
            case CurrencyType.Coin:
                if (SavedDataHandler.Instance.Coin >= item.uiInventoryItemPrice)
                {
                    SavedDataHandler.Instance.Coin -= item.uiInventoryItemPrice;
                    isPurchasable = true;
                }

                break;
            case CurrencyType.Gems:
                if (SavedDataHandler.Instance.Gems >= item.uiInventoryItemPrice)
                {
                    SavedDataHandler.Instance.Gems -= item.uiInventoryItemPrice;
                    isPurchasable = true;
                }

                break;
        }

        return isPurchasable;
    }

    private void AddInGameCurrencies(InventoryCoinGemsPackUiItem item)
    {
        switch (item.uiRewardCurrencyType)
        {
            case CurrencyType.Coin:
                SavedDataHandler.Instance.Coin += item.uiItemRewardAmount;
                break;
            case CurrencyType.Gems:
                SavedDataHandler.Instance.Gems += item.uiItemRewardAmount;
                break;
        }
    }

    #endregion
}

[System.Serializable]
public class FinalInventoryItem
{
    public InventoryItemType inventoryItemType;
    public int itemIndex;
}