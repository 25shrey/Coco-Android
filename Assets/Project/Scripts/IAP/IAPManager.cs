using System;
using System;
using System.Collections;
using System.Collections.Generic;
using Game.BaseFramework;
using JetBrains.Annotations;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using Product = UnityEngine.Purchasing.Product;

public class IAPManager : Singleton<IAPManager>, IDetailedStoreListener
{
    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;

    [SerializeField] private List<IAPData> IAPList;

    private string currentProductID;

    public Action purchaseCallback;


    private async void Start()
    {
        var options = new InitializationOptions().SetEnvironmentName("Release");
        
        await UnityServices.InitializeAsync(options);
        
        if (m_StoreController == null)
        {
            InitializePurchasing();
        }

        SetProductCosts();
    }

    public void InitializePurchasing()
    {
        if (IsInitialized())
        {
            SetProductCosts();
            return;
        }
        
        //Development
        var module = StandardPurchasingModule.Instance();
        module.useFakeStoreAlways = true;
        module.useFakeStoreUIMode = FakeStoreUIMode.DeveloperUser;
        var builder = ConfigurationBuilder.Instance(module);

        //var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance()); // Production
        
        // Add all list of Products to Builder
        for (int index = 0; index < IAPList.Count; index++)
        {
            builder.AddProduct(IAPList[index].productID, ProductType.Consumable);
        }
        UnityPurchasing.Initialize(this, builder);
        
    }

    private bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }
    
    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log("Store Initialization Failed! Error : " + error  + " Message : " + message);
    }

    public void SetProductCosts()
    {
        Debug.Log("Application Internet : " + Application.internetReachability);
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            if (IsInitialized())
            {
                Debug.Log("Inside - Initiazed!");
                // IAP Content
                for (int index = 0; index < IAPList.Count; index++)
                {
                    IAPList[index].productCost = m_StoreController.products.WithID(IAPList[index].productID).metadata.localizedPriceString;
                }
            }
            else
            {
                Debug.Log("IAP Not Initialized!");
                for (int index = 0; index < IAPList.Count; index++)
                {
                    IAPList[index].SetOfflineData();
                }
            }
        }
        else
        {
            Debug.Log("Internet - Offline (IAPManager)");
            for (int index = 0; index < IAPList.Count; index++)
            {
                IAPList[index].SetOfflineData();
            }
        }
    }
    

    public void BuyProductID(string productId, Action Callback)
    {
        Debug.Log("Inside - Buy Product ID");
        if (IsInitialized())
        {
            // IAP
            Debug.Log("Inside - Buy Product ID - Initialized");

            Product product = m_StoreController.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                Debug.Log("Inside - Buy Product ID - Product Found");
                currentProductID = productId;
                purchaseCallback = Callback;
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                m_StoreController.InitiatePurchase(product);
                
            }
            else
            {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            Debug.Log("BuyProductID Failed. Not initialized.");
        }
    }


    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("Inside - OnInitialize");
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializationFailed InitializationFailureReason: " + error);
    }
    

    /*public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        /*if (String.Equals(args.purchasedProduct.definition.id, currentProductID, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // Add coins
            Debug.Log("Inside - Success");
            purchaseCallback?.Invoke();
        }
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        }#1#
        
        Debug.Log("Inside - ProcessPurchase");
        
        purchaseCallback?.Invoke();
        return PurchaseProcessingResult.Complete;
    }*/

   
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        Debug.Log("Inside - ProcessPurchase");
        
        purchaseCallback?.Invoke();
        return PurchaseProcessingResult.Complete;
        
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}",
            product.definition.storeSpecificId, failureReason));
        UIController.Instance.ShowPopup(PopupType.CommonPopup);
        CommonPopup popup = UIController.Instance.GetPopup<CommonPopup>(PopupType.CommonPopup);
        if (popup != null)
        {
            string Error = $"In App Purchased Failed Due to {Enum.GetName(failureReason.GetType(), failureReason)}. Please try again later.!";
            popup.SetData(CommonPopupType.Error, Error); /*false, null, null, 5f);*/
        };
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}",
            product.definition.storeSpecificId, failureDescription.message));
        UIController.Instance.ShowPopup(PopupType.CommonPopup);
        CommonPopup popup = UIController.Instance.GetPopup<CommonPopup>(PopupType.CommonPopup);
        if (popup != null)
        {
            string Error = $"In App Purchased Failed Due to {failureDescription.message}. Please try again later.!";
            popup.SetData(CommonPopupType.Error, Error); /*false, null, null, 5f);*/
        };
    }
}


