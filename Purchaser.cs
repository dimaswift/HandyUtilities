using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_PURCHASING
using UnityEngine.Purchasing;
#endif
using UnityEngine.Events;


public interface IProductCell 
{
    void OnPurchaseSuccess();
    void SetUp(string id, System.Action onClick);
}
#if UNITY_PURCHASING
// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
public class Purchaser : MonoBehaviour, IStoreListener
{
    public UnityEvent onInitialized;
    public List<IGameProduct> products = new List<IGameProduct>();
    public IStoreController m_StoreController;                                                                  // Reference to the Purchasing system.
    private IExtensionProvider m_StoreExtensionProvider;                                                         // Reference to store-specific Purchasing subsystems.
    public event Action<string> onProductPurchasedSucceed; 
    public event Action<string> onProductPurchasedFailed;
    public event Action onPurchasedSuccessfullyRestored;
    // Product identifiers for all products capable of being purchased: "convenience" general identifiers for use with Purchasing, and their store-specific identifier counterparts 
    // for use with and outside of Unity Purchasing. Define store-specific identifiers also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

    // General handle for the subscription product.

    public interface IGameProduct
    {
        string ProductID { get; }
        ProductType Type { get; }
        IProductCell cell { get; }
    }

    public void Init()
    {
        if (m_StoreController == null)
        {
            InitializePurchasing();
        }
    }

    public string GetPrice(string id)
    {
        var p = Array.Find(m_StoreController.products.all, _p => id == _p.definition.storeSpecificId);
        if(p != null)
        {
            return p.metadata.localizedPriceString;
        }
        return null;
    }

    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
  
        // Add a product to sell / restore by way of its identifier, associating the general identifier with its store-specific identifiers.

        for (int i = 0; i < products.Count; i++)
        {
            var product = products[i];
            builder.AddProduct(product.ProductID, ProductType.NonConsumable);// Continue adding the non-consumable product.
        }
        UnityPurchasing.Initialize(this, builder);
        onInitialized.Invoke();
        foreach (var p in products)
        {
            if(p.cell != null)
            {
                var prod = p;
                p.cell.SetUp(GetPriceString(p.ProductID), () => BuyProductID(prod));
            }
        }
    }

    public string GetPriceString(string id)
    {
        if(m_StoreController != null && m_StoreController.products != null)
        {
            Product p = m_StoreController.products.WithID(id);
            if (p != null)
            {
                return p.metadata.localizedPriceString;
            }
        } 
        return string.Empty;
    }
    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void BuyProductID(IGameProduct gemProduct)
    {
        try
        {
            // If Purchasing has been initialized ...
            if (IsInitialized())
            {
                // ... look up the Product reference with the general product identifier and the Purchasing system's products collection.
                Product product = m_StoreController.products.WithID(gemProduct.ProductID);
                
                // If the look up found a product for this device's store and that product is ready to be sold ... 
                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
                    m_StoreController.InitiatePurchase(product);

                }
                // Otherwise ...
                else
                {
                    if(onProductPurchasedFailed != null)
                        onProductPurchasedFailed("not available");
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            // Otherwise ...
            else
            {
                // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or retrying initiailization.
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }
        // Complete the unexpected exception handling ...
        catch (Exception e)
        {
            // ... by reporting any unexpected exception for later diagnosis.
            Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
        }
    }


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases. Apple currently requires explicit purchase restoration for IAP.
    public void RestorePurchases()
    {
        
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) => {
				if( result ) {
                    if (onPurchasedSuccessfullyRestored != null)
                        onPurchasedSuccessfullyRestored();
                }
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }


    //  
    // --- IStoreListener
    //

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
       // Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        foreach (var p in products)
        {
            if(p.cell != null && p.ProductID == args.purchasedProduct.definition.id)
            {
                p.cell.OnPurchaseSuccess();
            }
        }

        if(onProductPurchasedSucceed != null)
            onProductPurchasedSucceed(args.purchasedProduct.definition.id);
        Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));//If the consumable item has been successfully purchased, add 100 coins to the player's in-game score.
        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        if (onProductPurchasedFailed != null)
            onProductPurchasedFailed("PURCHASE FAILED: " + failureReason.ToString());
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
    
}
#endif