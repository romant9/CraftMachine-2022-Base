using System;
using System.Collections.Generic;
using System.Text;
using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class PlayerHubSocialCard : UIListCard<string>
{
	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UISprite iconSprite;

	[SerializeField]
	private GameObject shareDiscordContainer;

	[SerializeField]
	private UITexture shareDiscordRewardTexture;

	[SerializeField]
	private UILabel shareDiscordRewardNum;

	public override void UpdateUI()
	{
		base.UpdateUI();
		titleLabel.text = LocalizationManager.GetText("Popup.PlayerHub.Social." + base.Item);
		iconSprite.spriteName = HelpersGfx.GetSocialIconName(base.Item);
		if (base.Item == "Discord")
		{
			UpdateDiscord();
		}
	}

	public void UpdateDiscord()
	{
		Dictionary<ShareType, ShareModel> obtainedRewards = GameManager.Instance.playerModel.ShareManagerModel.ObtainedRewards;
		bool active = false;
		if (obtainedRewards.Count == 0)
		{
			string contentPath = "Image/ydlBanana2023";
			string text = "0";
			if (GameManager.Instance.gameEconomyData.ConfigData.ShareToDiscordRewardImage != null)
			{
				contentPath = GameManager.Instance.gameEconomyData.ConfigData.ShareToDiscordRewardImage;
			}
			if (GameManager.Instance.gameEconomyData.ConfigData.ShareToDiscordReward != null)
			{
				text = GameManager.Instance.gameEconomyData.ConfigData.ShareToDiscordReward;
				string[] array = text.Split('(')[1].Split(')');
				text = "x" + array[0];
			}
			LoadImageFromCdn.LoadImageToTarget(shareDiscordRewardTexture, contentPath);
			shareDiscordRewardNum.text = text;
			active = true;
		}
		shareDiscordContainer.SetActive(active);
	}

	public void OnButton()
	{
		Helpers.ExecuteCommand(new PlayerHubCommand
		{
			EventName = "player_hub_click_social",
			ItemId = base.Item
		});
		switch (base.Item)
		{
		case "Fb":
			if (GameManager.CanOpenURLScheme("fb://"))
			{
				Application.OpenURL(GameManager.Instance.gameEconomyData.ConfigData.PlayerHubSocialFacebookApp);
			}
			else
			{
				Application.OpenURL(GameManager.Instance.gameEconomyData.ConfigData.PlayerHubSocialFacebookWeb);
			}
			break;
		case "Twitter":
			if (GameManager.CanOpenURLScheme("twitter://"))
			{
				Application.OpenURL(GameManager.Instance.gameEconomyData.ConfigData.PlayerHubSocialTwitterApp);
			}
			else
			{
				Application.OpenURL(GameManager.Instance.gameEconomyData.ConfigData.PlayerHubSocialTwitterWeb);
			}
			break;
		case "Instagram":
			if (GameManager.CanOpenURLScheme("instagram://"))
			{
				Application.OpenURL(GameManager.Instance.gameEconomyData.ConfigData.PlayerHubSocialInsApp);
			}
			else
			{
				Application.OpenURL(GameManager.Instance.gameEconomyData.ConfigData.PlayerHubSocialInsWeb);
			}
			break;
		case "Forums":
			Application.OpenURL(GameManager.Instance.gameEconomyData.ConfigData.PlayerHubSocialDiscussWeb);
			break;
		case "Discord":
			Application.OpenURL(GameManager.Instance.gameEconomyData.ConfigData.PlayerHubSocialDiscordWeb);
			if (GameManager.Instance.playerModel.ShareManagerModel.ObtainedRewards.Count == 0)
			{
				if (Helpers.ExecuteCommand(new ShareRewardCommand(GameManager.Instance.playerModel.ShareManagerModel, ShareType.Discord)) == TWDModelResult.OK)
				{
					Debug.LogError("fasonglea");
					PlayerModel playerModel = GameManager.Instance.playerModel;
					if (playerModel == null || playerModel.BundleManager == null || playerModel.BundleManager.ShareRewardEntrys.Count <= 0 || !(SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.OpenLootInUi) == null))
					{
						break;
					}
					OpenLootInUi openLootInUi = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi) as OpenLootInUi;
					if (openLootInUi != null)
					{
						openLootInUi.OpenForModel(GameManager.Instance.playerModel.BundleManager);
						UpdateDiscord();
						PlayerHubPopup playerHubPopup2 = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.PlayerHubPopup) as PlayerHubPopup;
						if (playerHubPopup2 != null)
						{
							playerHubPopup2.UpdateDiscord();
						}
						CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud) as CampHUD;
						if (campHUD != null)
						{
							campHUD.UpdateDiscord();
						}
					}
				}
				else
				{
					Debug.LogError("meifasong");
				}
			}
			else
			{
				Debug.LogError("meiyoule");
			}
			break;
		case "Banana":
		{
			if (GameManager.Instance.gameEconomyData?.ConfigData == null)
			{
				break;
			}
			if (Helpers.GetClickInternal())
			{
				if (GameManager.Instance.IsConnectedToServer)
				{
					SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Open();
					SignalRClient.Instance.RequestCommand("GetBananaLoginCode", OnGetTransferCode, waitForResponse: true);
				}
				break;
			}
			ShopPopupHelper.OpenWithIndex(2);
			PlayerHubPopup playerHubPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.PlayerHubPopup) as PlayerHubPopup;
			if (playerHubPopup != null)
			{
				playerHubPopup.Close();
			}
			break;
		}
		}
	}

	private void OnGetTransferCode(string message)
	{
		if (CheckError(message))
		{
			return;
		}
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
		TransferCode transferCode = GameManager.Instance.modelManager.GetMessageSerializer().DeserializeObject<TransferCode>(message);
		if (transferCode != null && !string.IsNullOrEmpty(transferCode.Code))
		{
			PlayerModel playerModel = GameManager.Instance.playerModel;
			string bananaURL = Helpers.GetBananaURL();
			if (playerModel != null && playerModel.HashedId != null)
			{
				string text = Convert.ToBase64String(Encoding.UTF8.GetBytes("ydldeca" + playerModel.HashedId + "twd"));
				string deviceId = GameManager.Instance.LoginRequest.Device.DeviceId;
				bananaURL = bananaURL + "?id=" + text + "&code=" + transferCode.Code + "&DeviceId=" + deviceId + "&OS=" + Helpers.GetPlatformName(Application.platform);
				Application.OpenURL(bananaURL);
			}
		}
		else
		{
			CheckError("");
		}
	}

	private bool CheckError(string message)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
		if (string.IsNullOrEmpty(message) || message == "null")
		{
			AlertPopup.ShowPopupGetText("Error.Error", "Error.ErrorGeneric", "Button.Ok", null);
			return true;
		}
		return false;
	}
}
