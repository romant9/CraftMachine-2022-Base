using TWDModel;
using UnityEngine;

public class LoadingScreenSurvivorInfo : MonoBehaviour
{
	[SerializeField]
	private UILabel levelLabel;

	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UISprite classSprite;

	[SerializeField]
	private SurvivorRarityAndClassPanel starsPanel;

	[SerializeField]
	private float deadSurvivorDimmingValue = 0.3f;

	public void UpdateUI(SurvivorModel combatSurvivor, int survivorIndex)
	{
		bool flag = false;
		if (combatSurvivor != null && combatSurvivor.manager != null && combatSurvivor.manager.Player != null && combatSurvivor.manager.Player.SurvivorContainer != null && combatSurvivor.manager.Player.SurvivorContainer.Survivors != null)
		{
			flag = GameManager.Instance.playerModel.SurvivorContainer.Survivors.Contains(combatSurvivor);
		}
		if (starsPanel != null)
		{
			starsPanel.UpdateWithSurvivor(combatSurvivor);
		}
		nameLabel.text = (flag ? combatSurvivor.Name : GameManager.Instance.GetFilteredText(combatSurvivor.Name));
		levelLabel.text = combatSurvivor.Level.ToString();
		classSprite.spriteName = HelpersGfx.GetSurvivorClassIconName(combatSurvivor);
		if (!flag && GameManager.Instance.playerModel.GetAttackTargetMissionModel() is GuildBattleMapMissionModel guildBattleMapMissionModel && guildBattleMapMissionModel.SavedData.Contains(survivorIndex - 3))
		{
			GetComponent<UIWidget>().alpha = deadSurvivorDimmingValue;
		}
	}
}
