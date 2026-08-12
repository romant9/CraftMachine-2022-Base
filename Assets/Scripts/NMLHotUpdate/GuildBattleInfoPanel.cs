using TWDModel;
using UnityEngine;

public class GuildBattleInfoPanel : MonoBehaviourExtended
{
	[SerializeField]
	private GameObject TitlesAndTextParent;

	[SerializeField]
	private UIPlayerNameLabel playerName;

	[SerializeField]
	private UIProgressBar vpProgressBar;

	[Header("Title Parts")]
	[SerializeField]
	private GameObject titleParent;

	private void Awake()
	{
		DebugIdString = "GuildBattleInfoPanel";
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	public virtual void UpdateUI()
	{
		GuildBattleModel currentBattle = GuildWarHelper.GetCurrentBattle();
		if (currentBattle != null)
		{
			if (playerName != null)
			{
				playerName.UpdateUI();
			}
			vpProgressBar.value = (float)currentBattle.CalculateTotalVictoryPoints() / 100f;
		}
		UpdateTitleParts();
	}

	public void UpdateTitleParts()
	{
		Helpers.GameObjectSetActive(titleParent, value: true);
	}

	private void OnUIEvent(string type, object parameter = null)
	{
		if (type == "OnPopUpClose" && SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.DetailMapPopUp).IsOpen && !SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.DetailMapPopUp).IsClosing)
		{
			UpdateUI();
		}
		else if (type == "SocialGuildPlayerChanged" || type == "SocialMembershipAccepted")
		{
			UpdateUI();
		}
	}
}
