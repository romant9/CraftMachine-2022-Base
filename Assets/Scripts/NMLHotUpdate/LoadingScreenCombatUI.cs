using TWDModel;
using UnityEngine;

public class LoadingScreenCombatUI : MonoBehaviour
{
	[SerializeField]
	private UIProgressBar loadingBar;

	[SerializeField]
	private UILabel touchToCountinueLabel;

	[SerializeField]
	private GameObject deadlyMissionContainer;

	[Header("Guild Battle PVP")]
	[SerializeField]
	private GuildBattlePlayerInfo playerInfo;

	[SerializeField]
	private GuildBattlePlayerInfo enemyInfo;

	[SerializeField]
	private GameObject vsLabel;

	[SerializeField]
	private GameObject guildWarsLabel;

	private void Start()
	{
		touchToCountinueLabel.gameObject.SetActive(value: false);
		deadlyMissionContainer.SetActive(value: false);
	}

	public void UpdateAutoContinueTimer(int secondsLeft)
	{
		if (touchToCountinueLabel != null)
		{
			touchToCountinueLabel.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("LoadingScreen.TouchToContinue.Counter{seconds}", secondsLeft);
		}
	}

	public void SetIsDeadly(bool deadly)
	{
		deadlyMissionContainer.SetActive(deadly);
	}

	public void UpdateProgress(float value)
	{
		loadingBar.value = value;
	}

	public void ShowGuildWarsLabel(bool show)
	{
		Helpers.GameObjectSetActive(guildWarsLabel, show);
	}

	public void LoadingOver()
	{
		if (touchToCountinueLabel != null && touchToCountinueLabel.gameObject != null)
		{
			touchToCountinueLabel.gameObject.SetActive(value: true);
		}
	}

	public void Cleanup()
	{
		if (touchToCountinueLabel != null && touchToCountinueLabel.gameObject != null)
		{
			touchToCountinueLabel.gameObject.SetActive(value: false);
		}
	}

	public void UpdatePlayersInfo()
	{
		GuildBattleMapMissionModel model = GameManager.Instance.playerModel.GetAttackTargetMissionModel() as GuildBattleMapMissionModel;
		playerInfo.Model = model;
		enemyInfo.Model = model;
		Helpers.GameObjectSetActive(playerInfo, value: true);
		Helpers.GameObjectSetActive(enemyInfo, value: true);
		Helpers.GameObjectSetActive(vsLabel, value: true);
		playerInfo.UpdateUI();
		enemyInfo.UpdateUI();
		Helpers.GameObjectSetActive(playerInfo, value: true);
		Helpers.GameObjectSetActive(enemyInfo, value: true);
	}
}
