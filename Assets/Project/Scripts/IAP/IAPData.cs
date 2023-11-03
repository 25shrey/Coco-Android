using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Game.BaseFramework;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "SO/IAP/IAPData")]
public class IAPData : ScriptableObject
{
    public string productID;
    public string productCost_Offline;
    //[HideInInspector]
    public string productCost;


    public void SetOfflineData()
    {
        productCost = productCost_Offline;
    }
}
