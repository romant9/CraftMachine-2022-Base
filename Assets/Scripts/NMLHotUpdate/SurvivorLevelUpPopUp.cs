using System.Collections;
using TWDModel;
using UnityEngine;

public class SurvivorLevelUpPopUp : HUDElement
{
	[SerializeField]
	private SurvivorCard survivorCard;

	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel survivorDamageLabel;

	[SerializeField]
	private UILabel survivorHealthLabel;

	[SerializeField]
	private UILabel survivorMoveLabel;

	[SerializeField]
	private UILabel survivorEquipmentLevelLabel;

	private SurvivorModel survivor;

	private float animationTime;

	private const float FADE_EFFECT_DURATION = 0.5f;

	private const float STATS_ANIMATION_DURATION = 0.5f;

	private const float EQUIPMENT_LEVEL_UPGRADE_ANIMATION_DURATION = 0.5f;

	private int previousLevelDamage;

	private int previousLevelHealth;

	private int previousLevelMove;

	private int currentLevelDamage;

	private int currentLevelHealth;

	private int currentLevelMove;

	public override void Open()
	{
		base.Open();
		UIPanel component = defaultPopup.GetComponent<UIPanel>();
		if (component != null)
		{
			component.depth = 7;
		}
		survivor = GetModel<SurvivorModel>();
		if (survivorCard != null)
		{
			survivorCard.Item = survivor;
			survivorCard.UpdateUI();
		}
		animationTime = 0f;
		previousLevelDamage = survivor.GetDamageForPreferredWeaponForLevel(survivor.Level - 1);
		currentLevelDamage = survivor.GetDamageForPreferredWeaponForLevel(survivor.Level);
		previousLevelHealth = survivor.GetHitpointsForLevel(survivor.Level - 1);
		currentLevelHealth = survivor.Hitpoints;
		previousLevelMove = survivor.GetMoveRangeForLevel(survivor.Level - 1);
		currentLevelMove = survivor.MoveRange;
		titleLabel.text = "Level " + survivor.Level + "!";
		titleLabel.alpha = 0f;
		survivorDamageLabel.text = previousLevelDamage.ToString();
		survivorHealthLabel.text = previousLevelHealth.ToString();
		survivorMoveLabel.text = previousLevelMove.ToString();
		survivorEquipmentLevelLabel.alpha = 0f;
		UpdateUI();
		animationTime = 0f;
		StartCoroutine("AnimationSequence");
	}

	public override void Close()
	{
		base.Close();
		if (defaultPopup != null)
		{
			UIPanel component = defaultPopup.GetComponent<UIPanel>();
			if (component != null)
			{
				component.depth = 1;
			}
		}
	}

	private IEnumerator AnimateStats(UILabel label)
	{
		int previousValue = 0;
		int nextValue = 0;
		if (label == survivorDamageLabel)
		{
			previousValue = previousLevelDamage;
			nextValue = currentLevelDamage;
		}
		else if (label == survivorHealthLabel)
		{
			previousValue = previousLevelHealth;
			nextValue = currentLevelHealth;
		}
		else if (label == survivorMoveLabel)
		{
			previousValue = previousLevelMove;
			nextValue = currentLevelMove;
		}
		while (animationTime <= 0.5f)
		{
			animationTime += Time.deltaTime;
			float num = animationTime / 0.5f;
			label.text = ((int)((float)previousValue + (float)(nextValue - previousValue) * num)).ToString();
			yield return null;
		}
		animationTime = 0f;
		label.text = nextValue.ToString();
		yield return new WaitForSeconds(0.1f);
	}

	private IEnumerator FadeInEffect(UILabel label)
	{
		while (animationTime <= 0.5f)
		{
			animationTime += Time.deltaTime;
			label.alpha = animationTime / 0.5f;
			yield return null;
		}
		animationTime = 0f;
		label.alpha = 1f;
		yield return new WaitForSeconds(0.1f);
	}

	public override void UpdateUI()
	{
		survivor = GetModel<SurvivorModel>();
		if (survivorEquipmentLevelLabel != null)
		{
			survivorEquipmentLevelLabel.text = "Survivor can equip level " + survivor.Level + " equipments.";
		}
	}

	public IEnumerator AnimationSequence()
	{
		while (animationTime > -1f)
		{
			yield return StartCoroutine(FadeInEffect(titleLabel));
			if (currentLevelDamage > previousLevelDamage)
			{
				yield return StartCoroutine(AnimateStats(survivorDamageLabel));
			}
			if (currentLevelHealth > previousLevelHealth)
			{
				yield return StartCoroutine(AnimateStats(survivorHealthLabel));
			}
			if (currentLevelMove > previousLevelMove)
			{
				yield return StartCoroutine(AnimateStats(survivorMoveLabel));
			}
			yield return StartCoroutine(FadeInEffect(survivorEquipmentLevelLabel));
			animationTime = -1f;
			yield return new WaitForSeconds(1f);
		}
		yield return null;
	}
}
