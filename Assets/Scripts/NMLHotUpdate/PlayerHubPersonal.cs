using TWDModel;
using UnityEngine;

public class PlayerHubPersonal : MonoBehaviour
{
	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private GameObject guildNameContainer;

	[SerializeField]
	private UILabel guildNameLabel;

	[SerializeField]
	private UILabel leveLabel;

	[SerializeField]
	private UILabel influenceLabel;

	[SerializeField]
	private UIButton changeNameButton;

	[SerializeField]
	private UILabel buildingPointProgressionLabel;

	[SerializeField]
	private PlayerEmblemEditor playerEmblemEditor;

	private void OnEnable()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		nameLabel.gameObject.SetActive(!string.IsNullOrEmpty(playerModel.Name));
		nameLabel.text = playerModel.Name;
		guildNameContainer.SetActive(playerModel.IsGuildMember);
		if (playerModel.GuildModel != null)
		{
			guildNameLabel.text = playerModel.GuildModel.Name;
		}
		leveLabel.text = LocalizationManager.GetText("Generic.Level{Level}", playerModel.Level);
		influenceLabel.text = playerModel.RankingScore.ToString();
		if (changeNameButton != null)
		{
			changeNameButton.gameObject.SetActive(playerModel.CanChangePlayerName);
		}
		PlayerLevelData currentPlayerLevelData = playerModel.GetCurrentPlayerLevelData();
		if (currentPlayerLevelData != null)
		{
			HelpersUI.SetContentToLabel(buildingPointProgressionLabel, $"{playerModel.Xp}/{currentPlayerLevelData.NextLevelXp}");
		}
	}

	public void OnShowMyClasses()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.PlayerHubPopup);
		UnlockClassPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.UnlockClassPopup) as UnlockClassPopup;
		obj.ForceOpenSurvivorClass = SurvivorClass.None;
		obj.StoryTellerModel = null;
		obj.Open();
	}

	public void OnChangeName()
	{
		EnterNamePopup enterNamePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SocialEnterName) as EnterNamePopup;
		if (enterNamePopup != null)
		{
			enterNamePopup.OnSubmitCallback = OnPlayerNameChangedSuccessfully;
			enterNamePopup.Open();
		}
	}

	public void OnPlayerNameChangedSuccessfully(UIType popupType)
	{
		OnEnable();
	}

	public void OnClickEditEmblem()
	{
		playerEmblemEditor.Activate();
	}

	public void OnClickEditCopy()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel != null && playerModel.HashedId != null)
		{
			GUIUtility.systemCopyBuffer = playerModel.HashedId;
		}
	}
}
