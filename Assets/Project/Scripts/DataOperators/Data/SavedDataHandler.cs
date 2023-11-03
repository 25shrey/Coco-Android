using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Game.BaseFramework
{
    public class SavedDataHandler : Singleton<SavedDataHandler>
    {
        [Header("Data Credentials")] public string password;

        [Header("Current Save Data")] public SaveData _saveData;

        [Header("Default Data")] public SaveData _DefaultSaveData;

        [Header("References")] [SerializeField]
        private PlayerInput playerInput;

        #region Getter/Setter

        private int _coin;

        public int Coin
        {
            get { return _coin; }
            set
            {
                _coin = value;
                _saveData.coinData = value;
                for (int i = 0; i < UIController.Instance.coinTexts.Count; i++)
                {
                    Debug.Log("===Coin save data");
                    UIController.Instance.coinTexts[i].text = value.ToString();
                }
            }
        }

        private int _gems;

        public int Gems
        {
            get { return _gems; }
            set
            {
                _gems = value;
                _saveData.gemsData = value;
                for (int i = 0; i < UIController.Instance.gemsTexts.Count; i++)
                {
                    Debug.Log("===Gems save data");
                    UIController.Instance.gemsTexts[i].text = value.ToString();
                }
            }
        }

        private int _powerUpFireBallCount;

        public int PowerUpFireBallCount
        {
            get { return _powerUpFireBallCount; }
            set
            {
                _powerUpFireBallCount = value;
                _saveData.powerUpFireBallData = value;
                UIController.Instance.powerUpFireBallText.text = value.ToString();
                UIController.Instance.shopPowerUpFireBallText.text = value.ToString();
            }
        }

        private int _powerUpMagnetCount;

        public int PowerUpMagnetCount
        {
            get { return _powerUpMagnetCount; }
            set
            {
                _powerUpMagnetCount = value;
                _saveData.powerUpMagnetData = value;
                UIController.Instance.powerUpMagnetText.text = value.ToString();
                UIController.Instance.shopPowerUpMagnetText.text = value.ToString();
            }
        }

        private int _powerUpShieldCount;

        public int PowerUpShieldCount
        {
            get { return _powerUpShieldCount; }
            set
            {
                _powerUpShieldCount = value;
                _saveData.powerUpShieldData = value;
                UIController.Instance.powerUpShieldText.text = value.ToString();
                UIController.Instance.shopPowerUpShieldText.text = value.ToString();
            }
        }

        #endregion

        public override void Awake()
        {
            base.Awake();
            print(Application.persistentDataPath);
            if (!File.Exists(SaveGameData.filePath))
            {
                print("does not exist");
                SetFirstLaunch();
            }
            else
            {
                _saveData = SaveGameData.Load(_DefaultSaveData, password);
            }
            //_saveData = SaveGameData.Load(_DefaultSaveData, password);
        }

        private void Start()
        {
            LoadCurrency_PowerUp_Data();
            LoadPurchaseItemData();
            LoadPurchasedBalloonItemData();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                //SaveGameData.Save(_saveData, password);
            }
            else
            {
                //_saveData = SaveGameData.Load(_DefaultSaveData, password);
            }
        }

        public bool IsFirstTimeLaunched()
        {
            return _saveData.isFirstLaunch;
        }

        [ContextMenu("Clear Saved Data")]
        public void ResetToDefault()
        {
            _saveData = SaveGameData.Clear(_DefaultSaveData, password);
        }

        public void SetFirstLaunch()
        {
            if (!_saveData.isFirstLaunch)
            {
                _saveData.isFirstLaunch = true;
                _saveData.additionalHealth = 0;
                _saveData.totalCoinsCollected = 0;
                _saveData.lastLevelCompleted = 0;
                _saveData.score = 0;
                _saveData.controllerBindingData = null;
            }

            var obj = transform.GetChild(1).gameObject;
            obj.transform.GetChild(2).GetComponent<LevelMapUI>().InitialeLevelHider();
            SaveGameData.Save(_saveData, password);
        }

        #region Svae & Load functions

        public void SaveControllerRebingData()
        {
            _saveData.controllerBindingData = playerInput.currentActionMap.SaveBindingOverridesAsJson();
            Debug.Log(_saveData.controllerBindingData);
        }

        public void LoadControllerRebingData()
        {
            if (!string.IsNullOrEmpty(_saveData.controllerBindingData))
            {
                string rebingData = _saveData.controllerBindingData;
                Debug.Log(rebingData);
                playerInput.currentActionMap.LoadBindingOverridesFromJson(rebingData);
            }
        }

        public void SaveInputType(InputType input)
        {
            Debug.Log(" Save Input Type InputType : " + input); 
            _saveData.inputTypeData = input;
        }

        public InputType LoadInputType()
        {
            InputType input = _saveData.inputTypeData;
            return input;
        }

        public void SavePurchaseItemData(int index, InventoryItemType itemType)
        {
            PurchasedInventoryItemData
                itemData = _saveData.purchasedItemData.Find(x => x.inventoryItemType == itemType);
            itemData.purchasedItemIndexList.Add(index);
        }

        public void LoadPurchaseItemData()
        {
            if (_saveData.purchasedItemData.Count == 0 && _saveData.purchasedItemData == null)
            {
                Debug.Log("purchasedItemData is null or empty");
                _saveData.purchasedItemData = _DefaultSaveData.purchasedItemData;
            }

            for (int i = 0; i < _saveData.purchasedItemData.Count; i++)
            {
                PurchasedInventoryItemData data = _saveData.purchasedItemData[i];
                Cloth_DataSO dataSO =
                    InventoryManager.Instance.clothDataSO.Find(x => x.inventoryItemType == data.inventoryItemType);
                for (int k = 0; k < dataSO.inventoryData.Count; k++)
                {
                    dataSO.inventoryData[k].purchaseState = InventoryItemPurchaseState.Locked;
                }
                for (int j = 0; j < data.purchasedItemIndexList.Count; j++)
                {
                    dataSO.inventoryData[data.purchasedItemIndexList[j]].purchaseState = InventoryItemPurchaseState.Select;
                }
                FinalInventoryItem selectedItem =
                    _saveData.saveInventoryItemData.Find(x => x.inventoryItemType == data.inventoryItemType);
                dataSO.inventoryData[selectedItem.itemIndex].purchaseState = InventoryItemPurchaseState.Selected;
            }
        }

        public void SavePurchasedBalloonItemData(int index)
        {
            _saveData.balloonPurchasedInventoryItemData.purchasedItemIndexList.Add(index);
        }
        
        public void LoadPurchasedBalloonItemData()
        {
            if (_saveData.balloonPurchasedInventoryItemData.purchasedItemIndexList.Count == 0)
            {
                Debug.Log("purchasedItemData is null or empty");
                _saveData.balloonPurchasedInventoryItemData = _DefaultSaveData.balloonPurchasedInventoryItemData;
            }

            for (int i = 0; i < InventoryManager.Instance.balloonDataSO.inventoryData.Count; i++)
            {
                InventoryManager.Instance.balloonDataSO.inventoryData[i].purchaseState =
                    InventoryItemPurchaseState.Locked;
            }
            for (int j = 0; j < _saveData.balloonPurchasedInventoryItemData.purchasedItemIndexList.Count; j++)
            {
                InventoryManager.Instance.balloonDataSO.inventoryData[_saveData.balloonPurchasedInventoryItemData.purchasedItemIndexList[j]].purchaseState = InventoryItemPurchaseState.Select;
            }
            InventoryManager.Instance.balloonDataSO.inventoryData[_saveData.balloonSaveInventoryItemData.itemIndex].purchaseState = InventoryItemPurchaseState.Selected;
            InventoryManager.Instance.SetBalloonItemMaterial(_saveData.balloonSaveInventoryItemData.itemIndex);
        }

        public void SaveInventoryData()
        {
            _saveData.saveInventoryItemData = InventoryManager.Instance.finalInventoryItemData;
        }

        public void LoadInventoryData()
        {
            if (_saveData.saveInventoryItemData != null && _saveData.saveInventoryItemData.Count != 0)
            {
                Debug.Log("if LoadClothData");
                InventoryManager.Instance.finalInventoryItemData = _saveData.saveInventoryItemData;
            }
            else
            {
                InventoryManager.Instance.finalInventoryItemData = _DefaultSaveData.saveInventoryItemData;
            }
        }

        public void SaveBalloonInventoryData()
        {
            _saveData.balloonSaveInventoryItemData = InventoryManager.Instance.finalBalloonInventoryItem;
        }
        
        public void LoadBalloonInventoryData()
        {
            InventoryManager.Instance.finalBalloonInventoryItem = _saveData.balloonSaveInventoryItemData;
        }

        public void LoadCurrency_PowerUp_Data()
        {
            Coin = _saveData.coinData;
            Gems = _saveData.gemsData;
            PowerUpMagnetCount = _saveData.powerUpMagnetData;
            PowerUpShieldCount = _saveData.powerUpShieldData;
            PowerUpFireBallCount = _saveData.powerUpFireBallData;
        }

        public void SaveSettingsData(SettingData settingData)
        {
            _saveData.savedSettingData = settingData;
        }

        public SettingData LoadSettingsData()
        {
            return _saveData.savedSettingData;
        }

        #endregion

        public void ResetData()
        {
            _saveData.isFirstLaunch = false;
            SetFirstLaunch();
        }

        private void OnDisable()
        {
            SaveGameData.Save(_saveData, password);
        }
    }

    [Serializable]
    public class SaveData
    {
        public bool isFirstLaunch;
        public int totalCoinsCollected;
        public int additionalHealth;
        public int lastLevelCompleted;
        public int score;
        public string controllerBindingData;
        public List<FinalInventoryItem> saveInventoryItemData;
        public FinalInventoryItem balloonSaveInventoryItemData;
        public List<PurchasedInventoryItemData> purchasedItemData;
        public PurchasedInventoryItemData balloonPurchasedInventoryItemData;
        public int coinData;
        public int gemsData;
        public int powerUpFireBallData;
        public int powerUpShieldData;
        public int powerUpMagnetData;
        public InputType inputTypeData;
        public SettingData savedSettingData;
    }

    [Serializable]
    public class PurchasedInventoryItemData
    {
        public InventoryItemType inventoryItemType;
        public List<int> purchasedItemIndexList;
    }
}