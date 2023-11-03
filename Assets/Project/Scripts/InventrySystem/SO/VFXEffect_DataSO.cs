using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/VFXEffect_DataSO", order = 1)]
public class VFXEffect_DataSO : GenericInventoryDataSO<VFXEffectData>
{
    
}

[System.Serializable]
public class VFXEffectData : BaseInventoryData
{
    
}