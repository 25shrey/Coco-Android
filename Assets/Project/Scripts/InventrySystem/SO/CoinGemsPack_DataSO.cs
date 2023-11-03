using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CoinGemsPack_DataSO", order = 1)]
public class CoinGemsPack_DataSO : GenericInventoryDataSO<CoinGemsPackData>
{
    
}

[System.Serializable]
public class CoinGemsPackData : BaseInventoryData
{
    public CurrencyType rewardCurrencyType;
    public int rewardAmount;
}