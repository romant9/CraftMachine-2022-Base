using BaseModel;
using TWDModel;
using UnityEngine;

public class PlayerLevelHUD : MonoBehaviour
{
	[SerializeField]
	private UILabel playerLevel;

	[SerializeField]
	private UIProgressBar xpProgressBar;

	private void Start()
	{
		GameManager instance = GameManager.Instance;
		if (instance != null)
		{
			instance.playerModel.Changed += OnPlayerChange;
			UpdateUI();
		}
	}

	protected void UpdateUI()
	{
		if (xpProgressBar != null && playerLevel != null)
		{
			PlayerModel playerModel = GameManager.Instance.playerModel;
			playerLevel.text = playerModel.Level.ToString();
			PlayerLevelData currentPlayerLevelData = playerModel.GetCurrentPlayerLevelData();
			if (currentPlayerLevelData != null)
			{
				xpProgressBar.value = (float)playerModel.Xp / (float)currentPlayerLevelData.NextLevelXp;
			}
		}
	}

	private void OnPlayerChange(ModelObject m, string changed, object args)
	{
		if (changed == "xp" || changed == "level")
		{
			UpdateUI();
		}
	}

	private void OnClick()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		PlayerModel playerModel = GameManager.Instance.playerModel;
		PlayerLevelData currentPlayerLevelData = playerModel.GetCurrentPlayerLevelData();
		string text = "";
		if (!string.IsNullOrEmpty(playerModel.Name))
		{
			text = playerModel.Name + "\n\n";
		}
		NGTooltip.Show("[b]" + text + LocalizationManager.GetText("Tooltip.PlayerLevel.PlayerLevel") + " " + playerModel.Level + "\n\n" + LocalizationManager.GetText("Tooltip.PlayerLevel.ExperiencePointToNextLevel") + ":\n" + playerModel.Xp + " / " + currentPlayerLevelData.NextLevelXp + "[/b]\n\n" + LocalizationManager.GetText("Tooltip.PlayerLevel.How to get xp"));
	}

	private void OnDestroy()
	{
		GameManager instance = GameManager.Instance;
		if (instance != null)
		{
			instance.playerModel.Changed -= OnPlayerChange;
		}
	}
}
