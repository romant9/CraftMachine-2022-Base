using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class CreateGuildPopup : HUDElement
{
	[SerializeField]
	private UIInput nameInput;

	[SerializeField]
	private UIInput descriptionInput;

	[SerializeField]
	private UIPopupList purposeTypeDropDown;

	[SerializeField]
	private UILabel purposeTypeDropDownLabel;

	private GuildModel guildInfo;

	private string currentPurposeSelection;

	public override void Open()
	{
		base.Open();
		GameManager.Instance.playerModel.Changed += OnPlayerChanged;
		if (GameManager.Instance.guildModel != null)
		{
			GameManager.Instance.guildModel.Changed += OnGuildChanged;
		}
		defaultPopup.ShowPayButtons();
		defaultPopup.HideInstantPayButton();
		defaultPopup.SetInstantPayPanel(active: false);
		defaultPopup.SetPayButton(LocalizationManager.GetText("Popup.CreateGuild.Button.Create"), GameManager.Instance.GuildManager.GetCreateGuildCashier());
		defaultPopup.SetPayButtonClickCallback(OnCreate);
		nameInput.characterLimit = 15;
		descriptionInput.characterLimit = 200;
		if (purposeTypeDropDown != null)
		{
			List<string> guildPurposeTypes = GameManager.Instance.gameEconomyData.ConfigData.GuildPurposeTypes;
			if (guildPurposeTypes != null && guildPurposeTypes.Count > 0)
			{
				purposeTypeDropDown.items = new List<string>();
				for (int i = 0; i < guildPurposeTypes.Count; i++)
				{
					string purpose = guildPurposeTypes[i];
					purposeTypeDropDown.items.Add(HelpersLocalization.GetGuildPurpose(purpose));
				}
			}
		}
		string text = (currentPurposeSelection = GuildModel.GetDefaultPurpose(GameManager.Instance.gameEconomyData.ConfigData.GuildPurposeTypes));
		if (purposeTypeDropDownLabel != null && text != null)
		{
			purposeTypeDropDownLabel.text = HelpersLocalization.GetGuildPurpose(text);
		}
	}

	public override void Close()
	{
		base.Close();
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
		GameManager.Instance.playerModel.Changed -= OnPlayerChanged;
		if (GameManager.Instance.guildModel != null)
		{
			GameManager.Instance.guildModel.Changed -= OnGuildChanged;
		}
	}

	private void OnCreate()
	{
		guildInfo = new GuildModel();
		string text = nameInput.value.Trim();
		string description = descriptionInput.value.Trim();
		string purpose = null;
		int num = purposeTypeDropDown.items.IndexOf(currentPurposeSelection);
		if (num > -1 && GameManager.Instance.gameEconomyData.ConfigData.GuildPurposeTypes != null && GameManager.Instance.gameEconomyData.ConfigData.GuildPurposeTypes.Count > num)
		{
			purpose = GameManager.Instance.gameEconomyData.ConfigData.GuildPurposeTypes[num];
		}
		if (!guildInfo.IsValidNameLength(text))
		{
			HUDNotification.Error(LocalizationManager.GetText("Popup.EnterName.InvalidLength{Min}{Max}", 3, 15));
		}
		else if (!guildInfo.IsValidCharacters(text))
		{
			HUDNotification.Error(LocalizationManager.GetText("Popup.EnterName.InvalidName"));
		}
		else if (!guildInfo.IsValidDescriptionLength(description))
		{
			HUDNotification.Error(LocalizationManager.GetText("Popup.CreateGuild.InvalidDescriptonLength{Min}{Max}", 0, 200));
		}
		else
		{
			guildInfo.Name = text;
			guildInfo.Description = description;
			guildInfo.JoinType = GuildJoinType.Open;
			guildInfo.Purpose = purpose;
			ConsumeCurrencyCommandUtils.ExecuteForSocialCommands(GameManager.Instance.GuildManager.GetCreateGuildCashier(), CreateGuildCall);
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	public void OnPurposeTypeChanged()
	{
		if (purposeTypeDropDown != null)
		{
			currentPurposeSelection = purposeTypeDropDown.value;
			if (purposeTypeDropDownLabel != null)
			{
				purposeTypeDropDownLabel.text = currentPurposeSelection;
			}
		}
	}

	private void CreateGuildCall(TWDModelResult result)
	{
		if (result == TWDModelResult.OK)
		{
			if (!GameManager.Instance.GuildManager.CreateGuild(guildInfo))
			{
				defaultPopup.ShowCommandError(TWDModelResult.Error);
			}
			else
			{
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Open();
			}
		}
	}

	private void OnPlayerChanged(ModelObject modelObject, string changed, object args)
	{
		if (changed == "guildChanged" && GameManager.Instance.guildModel != null)
		{
			GameManager.Instance.guildModel.Changed += OnGuildChanged;
		}
	}

	private void OnGuildChanged(GroupModelBase groupModelBase, string changed, object args)
	{
		if (changed == "GuildCreated")
		{
			Close();
			UIEvent.Send("SocialGuildPlayerChanged");
		}
	}
}
