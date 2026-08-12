using UnityEngine;

public class UnlockPopup : HUDElement
{
	[SerializeField]
	[Tooltip("Container that contains all the game objects related to unlocking something.")]
	private GameObject unlockContainer;

	[SerializeField]
	private UILabel thingUnlockedLabel;

	[SerializeField]
	private UILabel typeUnlockedLabel;

	[SerializeField]
	private UILabel descriptionLabel;

	[SerializeField]
	private UITexture texture;

	public IReward Reward { get; set; }

	public override void Open()
	{
		base.Open();
		UpdateUI();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		unlockContainer.SetActive(Reward != null);
		if (Reward != null && Reward is RewardSurvivorClass)
		{
			RewardSurvivorClass rewardSurvivorClass = Reward as RewardSurvivorClass;
			HelpersGfx.SetSurvivorClassMaterial(texture, rewardSurvivorClass.SurvivorClass);
			thingUnlockedLabel.text = HelpersLocalization.GetSurvivorClassName(rewardSurvivorClass.SurvivorClass);
			typeUnlockedLabel.text = LocalizationManager.GetText("Generic.SurvivorClass");
			descriptionLabel.text = HelpersLocalization.GetSurvivorClassDescription(rewardSurvivorClass.SurvivorClass);
		}
	}
}
