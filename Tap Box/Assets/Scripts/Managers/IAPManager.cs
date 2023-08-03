using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.MessengerStatic;
using Cysharp.Threading.Tasks;
using IAP;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using Product = UnityEngine.Purchasing.Product;
using Unity.Services.Core;
using Unity.Services.Core.Environments;

namespace Managers
{
    public class IAPManager : MonoBehaviour, IDetailedStoreListener, IInitializable
    {
        private IStoreController m_StoreController;

        public string environment = "production";
        private List<Product> _products;
        private List<IapProduct> _productsIds;

        public async void Initialize()
        {
        }

        public async UniTask AwaitInitialization(List<IapProduct> products)
        {
            _productsIds = products;
            await InitializePurchasing();
            await CheckAndRestoreNoAds();

            Debug.Log("end of initialization");
        }


        private async UniTask InitializePurchasing()
        {
            try
            {
                if (UnityServices.State != ServicesInitializationState.Initialized)
                {
                    var options = new InitializationOptions()
                        .SetEnvironmentName(environment);

                    await UnityServices.InitializeAsync(options);
                }

                var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
                foreach (var variable in _productsIds)
                {
                    builder.AddProduct(variable.Id, variable.ProductType);
                }

                UnityPurchasing.Initialize(this, builder);

                Debug.Log($"InitializePurchasing");
            }
            catch (Exception exception)
            {
                Debug.LogError($"Error initialize " + exception.Message);
            }
        }

        public bool HasNonConsumableProduct(string id)
        {
            var product = _products.FirstOrDefault(x => x.definition.id == id);
            if (product == null)
            {
                return false;
            }

            return product.hasReceipt;
        }

        public void BuyProduct(string productName)
        {
            //_analyticsManager.OnButtonClick(Constants.Buttons.BuyProductClick + productName);
            Debug.Log("---BuyProduct " + productName);

            m_StoreController.InitiatePurchase(productName);
        }

        public async void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.Log("initialize failed " + error + " " + message);
            await Task.Delay(2000);
            InitializePurchasing();
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            var product = args.purchasedProduct;

            Messenger<string>.Broadcast(Constants.IAP.PurchaseSuccess, product.definition.id);
            Debug.Log($"Purchase Complete - Product: {product.definition.id}");

            return PurchaseProcessingResult.Complete;
        }

        public async void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError($"In-App Purchasing initialize failed: {error}");
            await Task.Delay(2000);
            InitializePurchasing();
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.LogError($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("In-App Purchasing successfully initialized");
            m_StoreController = controller;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.LogError("OnPurchaseFailed " + product.metadata.localizedTitle + " " + failureDescription.productId + " " + failureDescription.message + " " + failureDescription.reason);
        }

        public async UniTask CheckAndRestoreNoAds()
        {
            await UniTask.WaitWhile(() => { return m_StoreController == null; });
            if (m_StoreController == null)
            {
                Debug.LogError("m_StoreController == null");
                return;
            }

            if (m_StoreController.products == null)
            {
                Debug.LogError("m_StoreController.products == null");
                return;
            }

            _products = new List<Product>();
            foreach (var vaProductsId in _productsIds)
            {
                var prod = m_StoreController.products.WithID(vaProductsId.Id);

                if (prod == null)
                {
                    continue;
                }

                _products.Add(prod);
                Debug.Log("_noAds.Id = " + prod.metadata.localizedTitle);
                Debug.Log("_noAds.hasReceipt = " + prod.hasReceipt);
                await UniTask.Yield();
            }
        }

#if UNITY_EDITOR
        [MenuItem("Prefs/ClearPlayerPrefs")]
        public static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

        [MenuItem("Prefs/SetOpenAll")]
        public static void SetOpenAll()
        {
            PlayerPrefs.SetInt("ads", 1);
        }
#endif
    }
}