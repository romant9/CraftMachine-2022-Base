using TWDModel;
using UnityEngine;

public class WeeklyClassEventPanel : MonoBehaviour
{
	[SerializeField]
	private GameObject panelContainer;

	[SerializeField]
	private UITexture classTexture;

	[SerializeField]
	private UISprite icon;

	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel messageLabel;

	public void Init(MapMissionModel mapMissionModel, SurvivorContainerModel.SurvivorType survivorType)
	{
		bool active = false;
		if (!TutorialView.Instance.Running && GameManager.Instance.gameEconomyData.WeeklyClassEvents != null && GameManager.Instance.gameEconomyData.WeeklyClassEvents.Length != 0)
		{
			for (int i = 0; i < GameManager.Instance.gameEconomyData.WeeklyClassEvents.Length; i++)
			{
				WeeklyClassEvent weeklyClassEvent = GameManager.Instance.gameEconomyData.WeeklyClassEvents[i];
				if (mapMissionModel == null || mapMissionModel.MissionSpawnPointGroup.Category != weeklyClassEvent.MissionCategory)
				{
					continue;
				}
				active = true;
				if (weeklyClassEvent.SurvivorClass == SurvivorClass.None)
				{
					if (weeklyClassEvent.Affects == WeeklyClassEvent.AffectType.Xp)
					{
						icon.gameObject.SetActive(value: true);
						classTexture.gameObject.SetActive(value: false);
						titleLabel.text = LocalizationManager.GetText("Popup.TeamSelection.WeeklyClassEvent.TitleXp");
						messageLabel.text = LocalizationManager.GetText("Popup.TeamSelection.WeeklyClassEvent.Action." + weeklyClassEvent.Affects.ToString() + "{Multipiler}", (weeklyClassEvent.Multiplier - 1L) * 100L);
					}
					else
					{
						active = false;
					}
					continue;
				}
				icon.gameObject.SetActive(value: false);
				classTexture.gameObject.SetActive(value: true);
				HelpersGfx.SetSurvivorClassMaterial(classTexture, weeklyClassEvent.SurvivorClass);
				titleLabel.text = LocalizationManager.GetText("Popup.TeamSelection.WeeklyClassEvent.Title{ClassName}", HelpersLocalization.GetSurvivorClassName(weeklyClassEvent.SurvivorClass));
				int num = 0;
				if (weeklyClassEvent.Affects == WeeklyClassEvent.AffectType.Damage)
				{
					num = (int)((weeklyClassEvent.Multiplier - 1.0) * 100.0);
				}
				else if (weeklyClassEvent.Affects == WeeklyClassEvent.AffectType.Defense)
				{
					num = (int)((1.0 - weeklyClassEvent.Multiplier) * 100.0);
				}
				messageLabel.text = LocalizationManager.GetText("Popup.TeamSelection.WeeklyClassEvent.Action." + weeklyClassEvent.Affects.ToString() + "{Multipiler}", num);
			}
		}
		panelContainer.SetActive(active);
	}
}
