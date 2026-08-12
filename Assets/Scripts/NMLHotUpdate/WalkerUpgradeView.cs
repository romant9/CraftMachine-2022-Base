using TWDModel;
using UnityEngine;

public class WalkerUpgradeView : MonoBehaviour
{
	[Header("Labels")]
	[Header("Left")]
	[SerializeField]
	private UILabel oldLevel;

	[SerializeField]
	private UILabel damageTitle;

	[SerializeField]
	private UILabel healthTitle;

	[Header("Right")]
	[SerializeField]
	private UILabel newLevel;

	[SerializeField]
	private UILabel damageDifference;

	[SerializeField]
	private UILabel healthDifference;

	[Header("Traits Panel")]
	[SerializeField]
	private UnlockedTraitPanel traitsPanel;

	[Header("Traits Description")]
	[SerializeField]
	private GameObject traitDescriptionParent;

	[SerializeField]
	private UILabel traitDescriptionLabel;

	private Animator animator;

	private void Start()
	{
		animator = GetComponent<Animator>();
	}

	[ContextMenu("Show")]
	public void Show()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/survivor_upgrade_stats");
		base.gameObject.SetActive(value: true);
		setParam(value: true);
	}

	[ContextMenu("Hide")]
	public void Hide()
	{
		setParam(value: false);
	}

	public void SetInfo(OutpostWalkerModel outpostWalkerModel)
	{
		if (outpostWalkerModel != null)
		{
			if (oldLevel != null && newLevel != null)
			{
				oldLevel.text = LocalizationManager.GetText("Popup.SurvivorUpgradeView.Stats.Level{Parameter}", outpostWalkerModel.Level - 1);
				newLevel.text = LocalizationManager.GetText("Popup.SurvivorUpgradeView.Stats.Level{Parameter}", outpostWalkerModel.Level);
			}
			if (damageTitle != null && healthTitle != null && damageDifference != null && healthDifference != null)
			{
				int damageForLevel = outpostWalkerModel.GetDamageForLevel(outpostWalkerModel.Level);
				int damageForLevel2 = outpostWalkerModel.GetDamageForLevel(outpostWalkerModel.Level - 1);
				int hitpointsForLevel = outpostWalkerModel.GetHitpointsForLevel(outpostWalkerModel.Level);
				int hitpointsForLevel2 = outpostWalkerModel.GetHitpointsForLevel(outpostWalkerModel.Level - 1);
				damageTitle.text = LocalizationManager.GetText("Popup.SurvivorUpgradeView.Stats.Damage{Parameter}", damageForLevel2);
				healthTitle.text = LocalizationManager.GetText("Popup.SurvivorUpgradeView.Stats.Health{Parameter}", hitpointsForLevel2);
				damageDifference.text = damageForLevel + " (+" + (damageForLevel - damageForLevel2) + ")";
				healthDifference.text = hitpointsForLevel + " (+" + (hitpointsForLevel - hitpointsForLevel2) + ")";
			}
			_ = traitsPanel != null;
		}
	}

	public void AnimationShowEnded()
	{
	}

	public void SurvivorUpgradeAnimTutorial()
	{
	}

	public void AnimationHideEnded()
	{
		base.gameObject.SetActive(value: false);
	}

	public void setParam(bool value)
	{
		if (animator != null)
		{
			animator.SetBool("Show", value);
		}
	}
}
