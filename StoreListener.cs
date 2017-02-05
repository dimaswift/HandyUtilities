namespace HandyUtilities
{
    using UnityEngine;
#if UNITY_PURCHASING
    using UnityEngine.Purchasing;
#endif

    public class IAPManager
    {
        public event System.Action<string> onPurchaseSucceed;
        public event System.Action<string> onPurchaseFailed;
        public event System.Action<bool> onPurchasesRestored;
#if UNITY_PURCHASING
        StoreListener m_storeListener;
#endif
        public void Init(IIAPProduct[] products)
        {
#if UNITY_PURCHASING
            m_storeListener = Object.FindObjectOfType<StoreListener>();
            m_storeListener.Init(products);
            m_storeListener.onProductPurchasedSucceed += onPurchaseSucceed;
            m_storeListener.onProductPurchasedFailed += onPurchaseFailed;
            m_storeListener.onPurchasesRestored += onPurchasesRestored;
#else
            onPurchaseSucceed += (s) => { Debug.Log("UNITY_PURCHASING is not enabled."); };
            onPurchaseFailed += (s) => { Debug.Log("UNITY_PURCHASING is not enabled."); };
            onPurchasesRestored += (s) => { Debug.Log("UNITY_PURCHASING is not enabled."); };
#endif
        }

        public void BuyProduct(IIAPProduct product)
        {
#if UNITY_PURCHASING
            m_storeListener.BuyProductID(product);
#endif
        }

        public void RestorePurchases()
        {
#if UNITY_PURCHASING
            m_storeListener.RestorePurchases();
#endif
        }
    }

    public interface IIAPProduct
    {
        int index { get; }
        string productID { get; }
        bool consumable { get; }
    }

#if UNITY_PURCHASING
    public class StoreListener : MonoBehaviour, IStoreListener
    {
        public IIAPProduct[] products;
        public IStoreController m_StoreController;
        private IExtensionProvider m_StoreExtensionProvider;
        private IIAPProduct pendingProduct;

        public event System.Action<string> onProductPurchasedSucceed;
        public event System.Action<string> onProductPurchasedFailed;
        public event System.Action<bool> onPurchasesRestored;
        public void Init(IIAPProduct[] products)
        {
            this.products = products;
            if (m_StoreController == null)
            {
                InitializePurchasing();
            }
            onPurchasesRestored += (success) => Debug.Log(string.Format("Puchases restore success: {0}", success));
        }

        public void InitializePurchasing()
        {
            if (IsInitialized())
            {
                return;
            }

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            for (int i = 0; i < products.Length; i++)
            {
                var product = products[i];
                builder.AddProduct(product.productID, product.consumable ? ProductType.Consumable : ProductType.NonConsumable);
            }

            UnityPurchasing.Initialize(this, builder);
        }

        public string GetPriceString(string id)
        {
            if (m_StoreController != null && m_StoreController.products != null)
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
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }

        public void BuyProductID(IIAPProduct gameProduct)
        {
            Debug.Log(string.Format("{0}", gameProduct.productID));
            try
            {
                pendingProduct = gameProduct;

                if (IsInitialized())
                {
                    Product product = m_StoreController.products.WithID(gameProduct.productID);

                    if (product != null && product.availableToPurchase)
                    {
                        Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                        m_StoreController.InitiatePurchase(product);

                    }
                    else
                    {
                        if (onProductPurchasedFailed != null)
                            onProductPurchasedFailed("not available");
                        Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                    }
                }
                else
                {
                    Debug.Log("BuyProductID FAIL. Not initialized.");
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
            }
        }


        public void RestorePurchases()
        {
            if (!IsInitialized())
            {
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                Debug.Log("RestorePurchases started ...");
                var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
                apple.RestoreTransactions(onPurchasesRestored);
            }
            else
            {
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            m_StoreController = controller;
            m_StoreExtensionProvider = extensions;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            if (onProductPurchasedSucceed != null)
                onProductPurchasedSucceed(args.purchasedProduct.definition.id);
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
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
}
