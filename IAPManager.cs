using UnityEngine;
using System.Threading.Tasks;
using System;
using UnityEngine.UIElements;
#if !IAP
namespace DVAH
{

    public class IAPManager : Singleton<IAPManager>
    {
        public void BuyProductID(string productId, Action<bool> onBuyDone = null) {
            Debug.LogError(CONSTANT.Prefix + "==>Mark using IAP on checklist menu or add IAP on define symbols!<==");
        }

        public async Task<bool> TryAddRestoreEvent(string productID, Action eventRestore = null, bool isTimeOut = false)
        {
            Debug.LogError(CONSTANT.Prefix + "==>Mark using IAP on checklist menu or add IAP on define symbols!<==");
            await Task.Delay(1000);

            return false;
        }    

    }
}
#else 
using System.Collections.Generic;

using UnityEngine.Purchasing;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Purchasing.Security;
using System.Collections;


namespace DVAH
{

    public class IAPManager : Singleton<IAPManager>, IStoreListener
    {

        private static IStoreController m_StoreController;          // The Unity Purchasing system.
        private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

        List<string> _restoreItemCheck = new List<string>();

        private Action<bool, Product> _onBuyDone = null;

        public bool IsInitDone => IsInitialized();
        private bool _isBuying = false;

        Action _onInitDone;
        public int PurchaseInfo = 0;
        protected override void Awake()
        {
            DontDestroyOnLoad(this);
            Debug.Log(CONSTANT.Prefix + $"==========><color=#00FF00>IAP start Init!</color><==========");
           
            if (!ProductCatalog.LoadDefaultCatalog().enableCodelessAutoInitialization)
                InitializePurchasing();
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



            var catalog = ProductCatalog.LoadDefaultCatalog();

            foreach (var product in catalog.allValidProducts)
            {
                if (product.allStoreIDs.Count > 0)
                {
                    var ids = new IDs();
                    foreach (var storeID in product.allStoreIDs)
                    {
                        ids.Add(storeID.id, storeID.store);
                    }
                    builder.AddProduct(product.id, product.type, ids);
                }
                else
                {
                    builder.AddProduct(product.id, product.type);
                }


            }



            // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
            // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
            UnityPurchasing.Initialize(this, builder);

        }


        private bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }

        /// <summary>
        /// Restore product which user already purchase!
        /// </summary>
        /// <param name="productID">ID of product (in catalog)</param>
        /// <param name="eventRestore"> action when product restore success</param>
        /// <returns> await table Task, true if restore action done and false if wrong ID </returns>
        public async Task<bool> TryAddRestoreEvent(string productID, Action eventRestore = null, bool isTimeOut = false)
        {
            var catalog = ProductCatalog.LoadDefaultCatalog();
            double countTime = 0;
            while (!_restoreItemCheck.Contains(productID) || !IsInitialized())
            {
                countTime += 500;
                if (isTimeOut)
                    if (countTime >= 360000f)
                    {
                        Debug.LogError(string.Format(CONSTANT.Prefix + "==>Restored product {0} fail, becuz time out! Check your network please!<==", productID));
                        return false;
                    }
                await Task.Delay(500);
            }
            if (_restoreItemCheck.Contains(productID))
            {
                UnityMainThread.wkr.AddJob(eventRestore);
                return true;
            }
            Debug.LogError(string.Format(CONSTANT.Prefix + "==>Restored product {0} fail, Check product ID!<==", productID));
            return false;
        }

        /// <summary>
        /// Check if product is restored or not (maybe for hide button buy ...)
        /// </summary>
        /// <param name="productId">ID of product (in catalog)</param>
        /// <returns>true if product already buy or restored</returns>
        public bool CheckRestoredProduct(string productId)
        {
            return this._restoreItemCheck.Contains(productId);
        }


        /// <summary>
        /// Call if player click btn buy product
        /// </summary>
        /// <param name="productId">ID of product (in catalog)</param>
        /// <param name="onBuyDone">do sth if buy done with a bool to describe success or not (example: add coin ...)</param>
        public void BuyProductID(string productId, Action<bool> onBuyDone = null)
        {
            //onBuyDone?.Invoke(true);
            //return;
            BuyProductID(productId, (isSuccess, product) => {
                onBuyDone?.Invoke(isSuccess);
            });
        }
        int level = 0;
        string screenName = "";
        public void BuyProductID(string productId, Action<bool, Product> onBuyDone = null)
        {
            if (_isBuying) return;

            _onBuyDone = onBuyDone;

            if (!string.IsNullOrEmpty(productId))
                Debug.Log(CONSTANT.Prefix + $"==> buy productId : " + productId + " <==");
            // If Purchasing has been initialized ...
            if (IsInitialized())
            {
                 
                _isBuying = true;
                // ... look up the Product reference with the general product identifier and the Purchasing 
                // system's products collection.
                Product product = m_StoreController.products.WithID(productId);

                if (product != null && product.availableToPurchase)
                {
                    //Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                    // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                    // asynchronously.
                    m_StoreController.InitiatePurchase(product, productId);
                    return;
                }

                _isBuying = false;
                // ... report the product look-up failure situation  
                Debug.LogError(CONSTANT.Prefix + $"==> BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase <==");

                return;
            }

            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            _isBuying = false;
            Debug.LogError(CONSTANT.Prefix + $"==> BuyProductID FAIL. Not initialized <==");
            //NoticeManager.Instance.LogNotice("BuyProductID FAIL. Not initialized.");

        }


        // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
        // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.

        public void RestorePurchases()
        {
            // If Purchasing has not yet been set up ...
            if (!IsInitialized())
            {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                Debug.LogError(CONSTANT.Prefix + $"==> RestorePurchases FAIL. Not initialized <==");
                //NoticeManager.Instance.LogNotice("RestorePurchases FAIL. Not initialized.");
                return;
            }

            // If we are running on an Apple device ... 
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                // ... begin restoring purchases
                Debug.Log(CONSTANT.Prefix + $"==> RestorePurchases started ...<==");

                // Fetch the Apple store-specific subsystem.
                var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
                // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
                // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
                apple.RestoreTransactions((result) =>
                {
                    // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                    // no purchases are available to be restored.
                    Debug.Log(CONSTANT.Prefix + $"==> RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore <==");
                });
            }
            // Otherwise ...
            else
            {
                // We are not running on an Apple device. No work is necessary to restore purchases.
                Debug.LogError(CONSTANT.Prefix + $"==> RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform + " <==");
            }
        }


        //  
        // --- IStoreListener
        //

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            // Purchasing has succeeded initializing. Collect our Purchasing references.
            // Overall Purchasing system, configured with products for this application.
            m_StoreController = controller;
            // Store specific subsystem, for accessing device-specific store features.
            m_StoreExtensionProvider = extensions;

            _onInitDone?.Invoke();


        }
        public string GetPrice(string productId)
        {
            if (string.IsNullOrEmpty(productId)) return "";
            if (m_StoreController != null)
            {
                string localizedPrice = m_StoreController.products.WithID(productId).metadata.localizedPriceString.ToString();
                string price = "";
                foreach (char a in localizedPrice)
                {
                    if (Char.IsDigit(a))
                    {
                        price += a;
                    }
                    else if (a.ToString() == "." || a.ToString() == ",")
                    {
                        price += a;
                    }
                }

                return price;
            }
            else
            {
                return "";
            }

        }
        public string GetCurrency(string productId)
        {
            if (string.IsNullOrEmpty(productId)) return "";
            if (m_StoreController != null)
            {
                return m_StoreController.products.WithID(productId).metadata.isoCurrencyCode;
            }
            else
            {
                return "";
            }

        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            Debug.LogError(CONSTANT.Prefix + $"==> OnInitializeFailed InitializationFailureReason:" + error + " <==");
            _onInitDone?.Invoke();
        }


        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            
            var catalog = ProductCatalog.LoadDefaultCatalog();
            foreach (var product in catalog.allValidProducts)
            {

                if (String.Equals(args.purchasedProduct.definition.id, product.id, StringComparison.Ordinal))
                {
                    Debug.Log(string.Format(CONSTANT.Prefix + "==> ProcessPurchase: PASS. Product: '{0}' <==", args.purchasedProduct.definition.id));
                    //NoticeManager.Instance.LogNotice(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                    //#if  UNITY_EDITOR

                    if (!_restoreItemCheck.Contains(product.id))
                    {
                        lock (_restoreItemCheck)
                        {
                            _restoreItemCheck.Add(product.id);
                        }
                    }

                    try
                    {
                        _onBuyDone?.Invoke(true, args.purchasedProduct);
                        _onBuyDone = null;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(CONSTANT.Prefix + "==> Buy production success but fail on invoke callback, error: " + e.Message);
                    }

                     
                     
                    _isBuying = false;
                    return PurchaseProcessingResult.Complete;

                }
            }

            _isBuying = false;
            // Return a flag indicating whether this product has completely been received, or if the application needs 
            // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
            // saving purchased products to the cloud, and when that save is delayed. 
            return PurchaseProcessingResult.Pending;
        }


        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
            // this reason with the user to guide their troubleshooting actions.
            _isBuying = false;
            Debug.LogError(string.Format(CONSTANT.Prefix + "==> OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1} <==", product.definition.storeSpecificId, failureReason));

            try
            {
                _onBuyDone?.Invoke(false, null);
                _onBuyDone = null;
            }
            catch (Exception e)
            {
                Debug.LogError(CONSTANT.Prefix + "==> Buy production fail then fail on invoke callback, error: " + e.Message);
            }


        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.LogError(CONSTANT.Prefix + "==> OnInitializeFailed: " + message);
        }
    }

}

#endif
