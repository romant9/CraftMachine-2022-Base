using System;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

[Obsolete]
public class CombatAbilityButton : MonoBehaviour
{
	private UIButton buttonComponent;

	[Tooltip("Ability usage indicator.")]
	[SerializeField]
	private UILabel usageIndicator;

	[Tooltip("Ability usage background.")]
	[SerializeField]
	private GameObject usageIndicatorBackground;

	[Tooltip("Ability sprite icon.")]
	[SerializeField]
	private UISprite icon;

	[Tooltip("Special ability indicator.")]
	[SerializeField]
	public CombatSpecialAbilityIndicator SpecialAbilityIndicator;

	[Tooltip("Positive Trait Prefab.")]
	[SerializeField]
	private GameObject positiveTraitPrefab;

	[Tooltip("Negative Trait Prefab.")]
	[SerializeField]
	private GameObject negativeTraitPrefab;

	[Tooltip("Trait Icons Container.")]
	[SerializeField]
	private GameObject traitIconsContainer;

	private AbilityModel ability;

	public AbilityModel Ability
	{
		get
		{
			if (SpecialAbility != null && SpecialAbilityIndicator.Selected)
			{
				return SpecialAbility;
			}
			return ability;
		}
		private set
		{
			ability = value;
		}
	}

	public AbilityModel SpecialAbility { get; private set; }

	private void Start()
	{
	}

	public void Setup(AbilityModel ability, AbilityModel specialAbility = null, List<TraitDefinition> traits = null)
	{
		Ability = ability;
		if (usageIndicator != null && usageIndicatorBackground != null)
		{
			usageIndicator.text = (Ability.MaxUses - Ability.TotalUses).ToString();
			usageIndicator.gameObject.SetActive(Ability.MaxUses > -1);
			usageIndicatorBackground.gameObject.SetActive(Ability.MaxUses > -1);
		}
		buttonComponent.isEnabled = Ability.MaxUses == -1 || Ability.TotalUses < Ability.MaxUses;
		AbilityResourceEntry resources = GameManager.Instance.GetResources<AbilityResourceEntry>(ability.Definition.Identifier);
		if (resources == null)
		{
			Debug.LogError("Could not load ability resources prefab " + ability.Definition.Identifier + "!");
			return;
		}
		if (icon != null)
		{
			Vector3 localScale = icon.gameObject.transform.localScale;
			icon.spriteName = resources.IconSprite;
			icon.MakePixelPerfect();
			icon.transform.localScale = localScale;
		}
		SpecialAbility = specialAbility;
		SpecialAbilityIndicator.gameObject.SetActive(SpecialAbility != null);
		SpecialAbilityIndicator.Ability = SpecialAbility;
	}

	public void ToggleSpecialAbility()
	{
		if (SpecialAbility != null && SpecialAbilityIndicator != null)
		{
			SpecialAbilityIndicator.Selected = !SpecialAbilityIndicator.Selected;
		}
	}

	private void Update()
	{
	}

	private void OnEnable()
	{
		buttonComponent = GetComponent<UIButton>();
		string normalSprite = buttonComponent.normalSprite;
		buttonComponent.normalSprite = normalSprite;
		buttonComponent.hoverSprite = normalSprite;
	}
}
