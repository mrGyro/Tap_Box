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

        public void Initialize()
        {
            Init();
        }

        private async void Init()
        {
            await Task.Delay(1000);
            InitializePurchasing();
            await Task.Delay(1000);

            if (PlayerPrefs.HasKey("firstStart") == false)
            {
                PlayerPrefs.SetInt("firstStart", 1);
                RestoreMyProduct();
            }

            RestoreVariable();
            Debug.LogError("---init");
            _btnNoAds.onClick.AddListener(BuyNoAds);
        }

        public void BuyNoAds()
        {
            Debug.LogError("---BuyNoAds");

            BuyProduct(NoAds);
        }


        public void CheckNoEdsButton()
        {
            _btnNoAds.gameObject.SetActive(PlayerPrefs.GetInt("ads", 0) == 0);
        }

        private async void InitializePurchasing()
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

        void RestoreVariable()
        {
            if (PlayerPrefs.HasKey("ads"))
            {
                _btnNoAds.gameObject.SetActive(false);
            }
        }

        public bool HasNoAds()
        {
            return PlayerPrefs.HasKey("ads");
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
            Debug.Log($"Product_NoAds");

            PlayerPrefs.SetInt("ads", 1);
            _btnNoAds.gameObject.SetActive(false);
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
            CheckNoEdsButton();
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.LogError("OnPurchaseFailed " + product.metadata.localizedTitle + " " + failureDescription.productId + " " + failureDescription.message + " " + failureDescription.reason);
        }

        public async void RestoreMyProduct()
        {
            await UniTask.WaitWhile(() =>
            {
                return m_StoreController == null;
            });
            
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

            var withID = m_StoreController.products.WithID(NoAds);

            if (withID == null)
            {
                Debug.LogError("withID == null");
                return;
            }

            if (withID.metadata == null)
            {
                Debug.LogError("withID.metadata == null");
                return;
            }

            if (withID.hasReceipt)
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