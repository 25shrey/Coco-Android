using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PowerUps_DataSO", order = 1)]
public class PowerUps_DataSO : GenericInventoryDataSO<PowerUpsData>
{
    
}

[System.Serializable]
public class PowerUpsData : BaseInventoryData
{
    public PowerUpType powerUpType;
    public int powerUpQuantity;
}