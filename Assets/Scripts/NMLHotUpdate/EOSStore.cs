using System.Collections.Generic;
using Epic.OnlineServices;
using Epic.OnlineServices.Ecom;
using PlayEveryWare.EpicOnlineServices;

public class EOSStore
{
	public delegate void OnEpicPruchaseCallback(string offerId, string transactionId);

	public delegate void OnEpicQueryOffersCallback(bool success);

	private static OnEpicQueryOffersCallback _EpicQueryOffersCallback = null;

	private static OnEpicPruchaseCallback _EpicPruchaseCallback = null;

	private static Dictionary<string, CatalogOffer> _CatalogOffers = new Dictionary<string, CatalogOffer>();

	private static Dictionary<string, string> _CatalogOfferIdToCatalogItemIdDic = new Dictionary<string, string>();

	private static string _CheckoutOfferId;

	private static List<Entitlement> _Entitlements = new List<Entitlement>();

	public static Dictionary<string, CatalogOffer> GetCatalogOffers()
	{
		return _CatalogOffers;
	}

	public static bool GetCatalogByOfferId(string offerId, out CatalogOffer? outOffer)
	{
		if (offerId == null)
		{
			outOffer = null;
			return false;
		}
		if (_CatalogOffers.ContainsKey(offerId))
		{
			outOffer = _CatalogOffers[offerId];
			return true;
		}
		outOffer = null;
		return false;
	}

	public static string GetCatalogItemIdByOfferId(string offerId)
	{
		if (offerId == null)
		{
			return null;
		}
		if (_CatalogOfferIdToCatalogItemIdDic.ContainsKey(offerId))
		{
			return _CatalogOfferIdToCatalogItemIdDic[offerId];
		}
		return null;
	}

	public static void QueryOffers(OnEpicQueryOffersCallback epicQueryOffersCallback)
	{
		if (_CatalogOffers.Count > 0)
		{
			epicQueryOffersCallback?.Invoke(success: true);
			return;
		}
		_EpicQueryOffersCallback = epicQueryOffersCallback;
		QueryOffersOptions options = new QueryOffersOptions
		{
			LocalUserId = EOSManager.Instance.GetLocalUserId(),
			OverrideCatalogNamespace = null
		};
		EOSManager.Instance.GetEOSPlatformInterface().GetEcomInterface().QueryOffers(ref options, null, OnQueryOffers);
	}

	private static void OnQueryOffers(ref QueryOffersCallbackInfo queryOffersCallbackInfo)
	{
		_CatalogOffers.Clear();
		_CatalogOfferIdToCatalogItemIdDic.Clear();
		if (queryOffersCallbackInfo.ResultCode == Result.Success)
		{
			GetOfferCountOptions options = new GetOfferCountOptions
			{
				LocalUserId = EOSManager.Instance.GetLocalUserId()
			};
			uint offerCount = EOSManager.Instance.GetEOSPlatformInterface().GetEcomInterface().GetOfferCount(ref options);
			for (int i = 0; i < offerCount; i++)
			{
				CopyOfferByIndexOptions options2 = new CopyOfferByIndexOptions
				{
					LocalUserId = EOSManager.Instance.GetLocalUserId(),
					OfferIndex = (uint)i
				};
				CatalogOffer? outOffer;
				Result result = EOSManager.Instance.GetEOSPlatformInterface().GetEcomInterface().CopyOfferByIndex(ref options2, out outOffer);
				if (result == Result.Success)
				{
					_CatalogOffers.Add(outOffer?.Id, outOffer.Value);
					GetOfferItemCountOptions options3 = new GetOfferItemCountOptions
					{
						LocalUserId = EOSManager.Instance.GetLocalUserId(),
						OfferId = outOffer?.Id
					};
					EOSManager.Instance.GetEOSPlatformInterface().GetEcomInterface().GetOfferItemCount(ref options3);
					CopyOfferItemByIndexOptions options4 = new CopyOfferItemByIndexOptions
					{
						LocalUserId = EOSManager.Instance.GetLocalUserId(),
						OfferId = outOffer?.Id,
						ItemIndex = 0u
					};
					if (EOSManager.Instance.GetEOSPlatformInterface().GetEcomInterface().CopyOfferItemByIndex(ref options4, out var outItem) == Result.Success)
					{
						_CatalogOfferIdToCatalogItemIdDic.Add(outOffer?.Id, outItem?.Id);
					}
				}
				else
				{
					Debug.LogError($"Offer {i} invalid: {result}");
				}
			}
			if (_EpicQueryOffersCallback != null)
			{
				_EpicQueryOffersCallback(success: true);
			}
		}
		else
		{
			Debug.LogError("Error calling QueryOffers: " + queryOffersCallbackInfo.ResultCode);
			if (_EpicQueryOffersCallback != null)
			{
				_EpicQueryOffersCallback(success: false);
			}
		}
	}

	public static void CheckOutOverlayByOfferId(string offerId, OnEpicPruchaseCallback epicPruchaseCallback = null)
	{
		_CheckoutOfferId = offerId;
		_EpicPruchaseCallback = epicPruchaseCallback;
		CheckoutEntry checkoutEntry = new CheckoutEntry
		{
			OfferId = offerId
		};
		CheckoutOptions options = new CheckoutOptions
		{
			LocalUserId = EOSManager.Instance.GetLocalUserId(),
			Entries = new CheckoutEntry[1] { checkoutEntry }
		};
		EOSManager.Instance.GetEOSPlatformInterface().GetEcomInterface().Checkout(ref options, null, OnCheckout);
	}

	private static void OnCheckout(ref CheckoutCallbackInfo checkoutCallbackInfo)
	{
		if (!(checkoutCallbackInfo.TransactionId != null))
		{
			Debug.LogError("Error Checkout offerId " + _CheckoutOfferId + " : " + checkoutCallbackInfo.ResultCode);
		}
		if (_EpicPruchaseCallback != null)
		{
			_EpicPruchaseCallback(_CheckoutOfferId, checkoutCallbackInfo.TransactionId);
		}
	}

	public static void QueryEntitlements()
	{
		QueryEntitlementsOptions options = new QueryEntitlementsOptions
		{
			LocalUserId = EOSManager.Instance.GetLocalUserId(),
			EntitlementNames = null,
			IncludeRedeemed = false
		};
		EOSManager.Instance.GetEOSPlatformInterface().GetEcomInterface().QueryEntitlements(ref options, null, OnQueryEntitlements);
	}

	private static void OnQueryEntitlements(ref QueryEntitlementsCallbackInfo queryEntitlementsCallbackInfo)
	{
		_Entitlements.Clear();
		if (queryEntitlementsCallbackInfo.ResultCode == Result.Success)
		{
			GetEntitlementsCountOptions options = new GetEntitlementsCountOptions
			{
				LocalUserId = EOSManager.Instance.GetLocalUserId()
			};
			uint entitlementsCount = EOSManager.Instance.GetEOSPlatformInterface().GetEcomInterface().GetEntitlementsCount(ref options);
			for (int i = 0; i < entitlementsCount; i++)
			{
				CopyEntitlementByIndexOptions options2 = new CopyEntitlementByIndexOptions
				{
					LocalUserId = EOSManager.Instance.GetLocalUserId(),
					EntitlementIndex = (uint)i
				};
				Entitlement? outEntitlement;
				Result result = EOSManager.Instance.GetEOSPlatformInterface().GetEcomInterface().CopyEntitlementByIndex(ref options2, out outEntitlement);
				if (result == Result.Success)
				{
					_Entitlements.Add(outEntitlement.Value);
				}
				else
				{
					Debug.LogError($"Entitlement {i} invalid: {result}");
				}
			}
		}
		else
		{
			Debug.LogError("Error calling QueryEntitlements: " + queryEntitlementsCallbackInfo.ResultCode);
		}
	}

	private static void RedeemEntitlements()
	{
		RedeemEntitlementsOptions options = new RedeemEntitlementsOptions
		{
			LocalUserId = EOSManager.Instance.GetLocalUserId(),
			EntitlementIds = new Utf8String[1]
		};
		options.EntitlementIds[0] = _Entitlements[0].EntitlementId;
		EOSManager.Instance.GetEOSPlatformInterface().GetEcomInterface().RedeemEntitlements(ref options, null, OnRedeemEntitlements);
	}

	private static void OnRedeemEntitlements(ref RedeemEntitlementsCallbackInfo redeemEntitlementsCallbackInfo)
	{
		if (redeemEntitlementsCallbackInfo.ResultCode != Result.Success)
		{
			Debug.LogError("Error calling RedeemEntitlements: " + redeemEntitlementsCallbackInfo.ResultCode);
		}
	}
}
