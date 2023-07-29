using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;

namespace Managers
{
    public class IAPManager : MonoBehaviour, IDetailedStoreListener
    {
        [SerializeField] private Button _btnNoAds;

        private IStoreController m_StoreController;

        private const string NoAds = "com.gyrogame.tapbox.noads";

        void Start()
        {
            Init();
        }

        private async void Init()
        {
            await Task.Delay(2);
            InitializePurchasing();

            if (PlayerPrefs.HasKey("firstStart") == false)
            {
                PlayerPrefs.SetInt("firstStart", 1);
                RestoreMyProduct();
            }

            RestoreVariable();
            CheckNoEdsButton();
            
            _btnNoAds.onClick.AddListener(BuyNoAds);
        }

        public void BuyNoAds()
        {
            BuyProduct(NoAds);
        }
        

        public void CheckNoEdsButton()
        {
            _btnNoAds.gameObject.SetActive(PlayerPrefs.GetInt("ads", 0) == 0);
        }

        void InitializePurchasing()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            builder.AddProduct(NoAds, ProductType.NonConsumable);

            UnityPurchasing.Initialize(this, builder);
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

            m_StoreController.InitiatePurchase(productName);
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.LogError("initialize failed " + error + " " + message);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            var product = args.purchasedProduct;

            if (product.definition.id == NoAds)
            {
                Product_NoAds();
            }

            Debug.Log($"Purchase Complete - Product: {product.definition.id}");

            return PurchaseProcessingResult.Complete;
        }

        private void Product_NoAds()
        {
            PlayerPrefs.SetInt("ads", 1);
            _btnNoAds.gameObject.SetActive(false);
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log($"In-App Purchasing initialize failed: {error}");
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
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

        public void RestoreMyProduct()
        {
            // _analyticsManager.OnButtonClick(Constants.Buttons.Restore);

            if (CodelessIAPStoreListener.Instance.StoreController.products.WithID(NoAds).hasReceipt)
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