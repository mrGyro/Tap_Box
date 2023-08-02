using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;
using Product = UnityEngine.Purchasing.Product;
using Unity.Services.Core;
using Unity.Services.Core.Environments;

namespace Managers
{
    public class IAPManager : MonoBehaviour, IDetailedStoreListener, IInitializable
    {
        [SerializeField] private Button _btnNoAds;

        private IStoreController m_StoreController;

        private const string NoAds = "com.gyrogame.tapbox.noads";
        public string environment = "production";
        private Product _noAds;

        public async void Initialize()
        {

        }

        public async UniTask AwaitInitialization()
        {
            await InitializePurchasing();

            CheckAndRestoreNoAds();

            _btnNoAds.onClick.AddListener(BuyNoAds);

            await UniTask.WaitWhile(() => _noAds == null);
            Debug.LogError("end of initialization");
        }

        private void BuyNoAds()
        {
            Debug.LogError("---BuyNoAds");

            BuyProduct(NoAds);
        }


        public void CheckNoEdsButton()
        {
            _btnNoAds.gameObject.SetActive(_noAds.availableToPurchase);
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
                builder.AddProduct(NoAds, ProductType.NonConsumable);

                UnityPurchasing.Initialize(this, builder);

                Debug.Log($"InitializePurchasing");
            }
            catch (Exception exception)
            {
                Debug.LogError($"Error initialize");
            }
        }

        public bool HasNoAds()
        {
            return !_noAds.availableToPurchase;
        }

        public void BuyProduct(string productName)
        {
            //_analyticsManager.OnButtonClick(Constants.Buttons.BuyProductClick + productName);
            Debug.LogError("---BuyProduct " + productName);

            m_StoreController.InitiatePurchase(productName);
        }

        public async void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.LogError("initialize failed " + error + " " + message);
            await Task.Delay(2000);
            InitializePurchasing();
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            var product = args.purchasedProduct;

            Debug.LogError(product.definition.id);
            if (product.definition.id == NoAds)
            {
                Product_NoAds();
            }

            Debug.Log($"Purchase Complete - Product: {product.definition.id}");

            return PurchaseProcessingResult.Complete;
        }

        private void Product_NoAds()
        {
            CheckNoEdsButton();
        }

        public async void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log($"In-App Purchasing initialize failed: {error}");
            await Task.Delay(2000);
            InitializePurchasing();
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("In-App Purchasing successfully initialized");
            m_StoreController = controller;
            //CheckNoEdsButton();
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.LogError("OnPurchaseFailed " + product.metadata.localizedTitle + " " + failureDescription.productId + " " + failureDescription.message + " " + failureDescription.reason);
        }

        public async void CheckAndRestoreNoAds()
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

            _noAds = m_StoreController.products.WithID(NoAds);

            if (_noAds == null)
            {
                Debug.LogError("withID == null");
                return;
            }

            if (_noAds.metadata == null)
            {
                Debug.LogError("withID.metadata == null");
                return;
            }

            Debug.LogError(_noAds.hasReceipt + " " + _noAds.availableToPurchase);
            if (_noAds.availableToPurchase)
            {
                Debug.LogError("---avalible");
                CheckNoEdsButton();
            }
            else
            {
                Product_NoAds();
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