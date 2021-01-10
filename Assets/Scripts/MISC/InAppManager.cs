using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;


namespace IAP
{
	public class InAppManager : MonoBehaviour, IStoreListener
	{

		private static IStoreController m_StoreController;
		private static IExtensionProvider m_StoreExtensionProvider;


		//public const string pMoney80 = "money_80";
		//public const string pNoAds = "no_ads";

		//public const string pMoney80AppStore = "app_money_80";
		//public const string pNoAdsAppStore = "app_no_ads";


		//public const string pMoney80GooglePlay = "gp_money_80";
		//public const string pNoAdsGooglePlay = "gp_no_ads";

		public const string pOpenAllThemes = "open_all_themes";
		public const string pOpenAllThemesAppStore = "app_open_all_themes";
		public const string pOpenAllThemesGooglePlay = "gp_open_all_themes";

		//public const string pStars4096Gems256 = "stars_4096_gems_256";
		//public const string pStars4096Gems256AppStore = "app_stars_4096_gems_256";
		//public const string pStars4096Gems256GooglePlay = "gp_stars_4096_gems_256";

		public const string pNoAds = "no_ads";
		public const string pNoAdsAppStore = "app_no_ads";
		public const string pNoAdsGooglePlay = "gp_no_ads";

		void Start()
		{
			if (m_StoreController == null)
			{
				InitializePurchasing();
			}
		}


		public void InitializePurchasing()
		{
			if (IsInitialized())
			{
				return;
			}


			var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
			//builder.AddProduct(pMoney80, ProductType.Consumable, new IDs() { { pMoney80AppStore, AppleAppStore.Name }, { pMoney80GooglePlay, GooglePlay.Name } });
			//builder.AddProduct(pNoAds, ProductType.NonConsumable, new IDs() { { pNoAdsAppStore, AppleAppStore.Name }, { pNoAdsGooglePlay, GooglePlay.Name } });

			builder.AddProduct(pOpenAllThemes, ProductType.NonConsumable, new IDs() { { pOpenAllThemesAppStore, AppleAppStore.Name }, { pOpenAllThemesGooglePlay, GooglePlay.Name } });
			//builder.AddProduct(pStars4096Gems256, ProductType.Consumable, new IDs() { { pStars4096Gems256AppStore, AppleAppStore.Name }, { pStars4096Gems256, GooglePlay.Name } });

			//ads
			builder.AddProduct(pNoAds, ProductType.NonConsumable, new IDs() { {pNoAdsAppStore, AppleAppStore.Name }, { pNoAdsGooglePlay, GooglePlay.Name } });

			// темы
			StoreItemSettings[] themeItems = Store.GetThemeItems();
			for (int i = 0; i < themeItems.Length; i++) 
			{
				if (!themeItems[i].special)
					continue;

				string id = themeItems[i].itemID;
				string app_id = themeItems[i].appStoreID;
				string gp_id = themeItems[i].gPlayID;

				builder.AddProduct(id, ProductType.NonConsumable, new IDs(){ { app_id, AppleAppStore.Name }, { gp_id, GooglePlay.Name } });
			}

			//валюта
			PointsItemSettings[] pointsItems = Store.GetPointsItens();
			for (int i = 0; i < pointsItems.Length; i++) 
			{
				string id = pointsItems[i].itemID;
				string app_id = pointsItems[i].appStoreID;
				string gp_id = pointsItems[i].gPlayID;

				builder.AddProduct(id, ProductType.Consumable, new IDs() { { app_id, AppleAppStore.Name }, { gp_id, GooglePlay.Name } });
			}

			UnityPurchasing.Initialize(this, builder);
		}


		private bool IsInitialized()
		{
			return m_StoreController != null && m_StoreExtensionProvider != null;
		}


		public void BuyProductID(string productId)
		{
			try
			{
				if (IsInitialized())
				{
					Product product = m_StoreController.products.WithID(productId);


					if (product != null && product.availableToPurchase)
					{
						Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
						m_StoreController.InitiatePurchase(product);
					}
					else
					{
						Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
						Store.OnThemePurchaseFail();
					}
				}
				else
				{
					Debug.Log("BuyProductID FAIL. Not initialized.");
					Store.OnThemePurchaseFail();
				}
			}
			catch (Exception e)
			{
				Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
				Store.OnThemePurchaseFail();
			}
		}


		public void RestorePurchases()
		{
			if (!IsInitialized())
			{
				Debug.Log("RestorePurchases FAIL. Not initialized.");
				return;
			}


			if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
			{
				Debug.Log("RestorePurchases started ...");


				var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
				apple.RestoreTransactions((result) =>
				{
					Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");

				});
			}
			else
			{
				Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
			}
		}


		public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
		{
			Debug.Log("OnInitialized: Completed!");


			m_StoreController = controller;
			m_StoreExtensionProvider = extensions;
		}


		public void OnInitializeFailed(InitializationFailureReason error)
		{
			Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
		}


		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
		{
			Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));

			if (String.Equals(args.purchasedProduct.definition.id, pOpenAllThemes, StringComparison.Ordinal))
			{
				Store.OnOpenAllThemesPurchased(); 
			}
			else if (String.Equals(args.purchasedProduct.definition.id, pNoAds, StringComparison.Ordinal))
			{
				Store.OnNoAdsPurchased();
			}
			else if (args.purchasedProduct.definition.id.StartsWith("points_"))
			{
				Store.OnPointsPurchased();
			}
			else if (args.purchasedProduct.definition.id.StartsWith("theme_"))  
			{
				//Action for theme
				string themeItemID = args.purchasedProduct.definition.id;
				Store.OnThemePurchased(themeItemID);
			}

			return PurchaseProcessingResult.Complete;
		}


		public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
		{
			Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
		}
	}
}
