using System;
using System.Linq;
using TWDModel;
using UnityEngine;

public class ConsumablesCombatHud : HUDElement
{
	[Header("Order should be Grenade, Medkit, Flare, Blast, Gore")]
	[SerializeField]
	private UILabel[] consumableAmount;

	[SerializeField]
	private UILabel[] consumableAmountBigCard;

	[SerializeField]
	private UISprite[] consumableAmountBg;

	[SerializeField]
	private UISprite[] consumableAmountBgBigCard;

	[SerializeField]
	private GameObject[] bigCards;

	[SerializeField]
	private GameObject[] consumableSelectedHighlights;

	[SerializeField]
	private GameObject[] consumableCooldownGO;

	[SerializeField]
	private UILabel[] consumableCooldownLabel;

	[SerializeField]
	private UIButton[] consumableUseButton;

	[SerializeField]
	private UILabel[] consumableUseButtonLabel;

	[SerializeField]
	private UILabel[] consumableUseButtonLabelSecondary;

	[SerializeField]
	private UILabel[] cooldownAfterUseDescriptionConsumable;

	[SerializeField]
	private UILabel[] alreadyUsedToolThisTurn;

	[SerializeField]
	private GameObject howTo;

	[SerializeField]
	private UILabel grenadeDescription;

	[SerializeField]
	private UILabel medKitDescription;

	[SerializeField]
	private UILabel flareDescription;

	[SerializeField]
	private UILabel blastGrenadeDescription;

	[SerializeField]
	private UILabel goreDescription;

	[SerializeField]
	private UILabel grenadeThreatDescription;

	[SerializeField]
	private UILabel blastGrenadeThreatDescription;

	[Header("Amount Label Colors")]
	[SerializeField]
	private Color amountTextColorDefault;

	[SerializeField]
	private Color amountTextColorZero;

	[SerializeField]
	private Color amountTextBgColorDefault;

	[SerializeField]
	private Color amountTextBgColorZero;

	private const string ConsumablesHowTo = "ConsumablesHowTo";

	private const string LocalizationKeyOnCooldown = "Consumable.Menu.Button.OnCooldown";

	private const string LocalizationKeyTurnsCooldown = "Consumable.Menu.ButtonInfo.OnCooldown{Turns}";

	private const string LocalizationKeyUse = "Consumable.Menu.Button.Use";

	private const string LocalizationKeyTarget = "Consumable.Menu.ButtonInfo.Target{Survivor}";

	private const string LocalizationKeyTurns = "Consumable.Card.Cooldown{Turns}";

	private const string LocalizationKeyCooldownAfterUse = "Consumable.Menu.Stat.CooldownAfterUse{Turns}";

	private const string LocalizationKeyThreatGenerated = "Consumable.Menu.Stat.ThreatGenerated{Amount}";

	private const string LocalizationKeyGrenadeDescription = "Consumable.Grenade.Description{DamageFlat}{DamagePercentage}";

	private const string LocalizationKeyMedKitDescription = "Consumable.Medkit.Description{HealPercentage}";

	private const string LocalizationKeyFullHealthMedKit = "Consumable.Medkit.InvalidTarget{Survivor}";

	private const string LocalizationKeyBlastGrenadeDescription = "Consumable.BlastGrenade.Description{Parameters}";

	private const string LocalizationKeyFlareDescription = "Consumable.Flare.Description{Duration}";

	private const string LocalizationKeyGoreDescription = "Consumable.Gore.Description{Duration}";

	public override void Open()
	{
		base.Open();
		RefreshUI();
		EquipmentModel.ConsumableType consumableType = EquipmentModel.ConsumableType.Unknown;
		foreach (EquipmentModel.ConsumableType value in Enum.GetValues(typeof(EquipmentModel.ConsumableType)))
		{
			if (value != EquipmentModel.ConsumableType.Unknown)
			{
				int count = GameManager.Instance.playerModel.Equipment.GetConsumablesOfType(value).Count;
				int cooldown = GameManager.Instance.playerModel.Combat.GetCooldown(value);
				if (cooldown == 0 && count > 0)
				{
					consumableType = value;
					break;
				}
				if (cooldown == 0 && consumableType == EquipmentModel.ConsumableType.Unknown)
				{
					consumableType = value;
				}
			}
		}
		switch (consumableType)
		{
		case EquipmentModel.ConsumableType.Grenade:
			OnGrenadeClick();
			break;
		case EquipmentModel.ConsumableType.MedKit:
			OnMedkitClick();
			break;
		case EquipmentModel.ConsumableType.Flare:
			OnFlareClick();
			break;
		case EquipmentModel.ConsumableType.BlastGrenade:
			OnBlastGrenadeClick();
			break;
		case EquipmentModel.ConsumableType.Gore:
			OnGoreClick();
			break;
		default:
			OnMedkitClick();
			break;
		}
	}

	private void UpdateCooldowns()
	{
		foreach (EquipmentModel.ConsumableType value in Enum.GetValues(typeof(EquipmentModel.ConsumableType)))
		{
			if (value != EquipmentModel.ConsumableType.Unknown && GameManager.Instance.playerModel.Combat.ActiveActor != null)
			{
				int num = (int)(value - 1);
				int cooldown = GameManager.Instance.playerModel.Combat.GetCooldown(value);
				bool flag = cooldown > 0;
				consumableCooldownGO[num].SetActive(flag);
				consumableCooldownLabel[num].text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Consumable.Card.Cooldown{Turns}", cooldown);
				consumableUseButtonLabel[num].text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(flag ? "Consumable.Menu.Button.OnCooldown" : "Consumable.Menu.Button.Use");
				if (value == EquipmentModel.ConsumableType.MedKit)
				{
					bool flag2 = GameManager.Instance.playerModel.Combat.ActiveActor.Hitpoints == GameManager.Instance.playerModel.Combat.ActiveActor.MaxHitPoints;
					string text = GameManager.Instance.playerModel.Combat.ActiveActor.Name;
					consumableUseButtonLabelSecondary[num].text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(flag2 ? "Consumable.Medkit.InvalidTarget{Survivor}" : "Consumable.Menu.ButtonInfo.Target{Survivor}", text);
					HelpersUI.SetButtonState(consumableUseButton[num], (flag2 || flag) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal);
				}
				else
				{
					consumableUseButtonLabelSecondary[num].text = (flag ? SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Consumable.Menu.ButtonInfo.OnCooldown{Turns}", cooldown) : SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Consumable.Menu.ButtonInfo.Target{Survivor}", GameManager.Instance.playerModel.Combat.ActiveActor.Name));
					HelpersUI.SetButtonState(consumableUseButton[num], flag ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal);
				}
			}
		}
	}

	private void OnGrenadeClick()
	{
		SetBigCardAndHighlightVisibility(EquipmentModel.ConsumableType.Grenade);
		SetCooldownAfterUse(EquipmentModel.ConsumableType.Grenade);
		grenadeDescription.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Consumable.Grenade.Description{DamageFlat}{DamagePercentage}", (int)ConsumableUtils.GetFlatDamage(GameManager.Instance.modelManager, EquipmentModel.ConsumableType.Grenade), ConsumableUtils.GetPercentageDamageDefinition(GameManager.Instance.modelManager, EquipmentModel.ConsumableType.Grenade));
		grenadeThreatDescription.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Consumable.Menu.Stat.ThreatGenerated{Amount}", ConsumableUtils.GetThreatDefinition(GameManager.Instance.modelManager, EquipmentModel.ConsumableType.Grenade));
		UpdateWhiteHealthBar();
	}

	private void OnMedkitClick()
	{
		SetBigCardAndHighlightVisibility(EquipmentModel.ConsumableType.MedKit);
		SetCooldownAfterUse(EquipmentModel.ConsumableType.MedKit);
		medKitDescription.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Consumable.Medkit.Description{HealPercentage}", ConsumableUtils.GetMedKitRecoveredHealthDefinition(GameManager.Instance.modelManager));
		UpdateWhiteHealthBar();
	}

	public void OnFlareClick()
	{
		SetBigCardAndHighlightVisibility(EquipmentModel.ConsumableType.Flare);
		SetCooldownAfterUse(EquipmentModel.ConsumableType.Flare);
		flareDescription.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Consumable.Flare.Description{Duration}", ConsumableUtils.GetFlareDuration(GameManager.Instance.modelManager));
		UpdateWhiteHealthBar();
	}

	public void OnBlastGrenadeClick()
	{
		SetBigCardAndHighlightVisibility(EquipmentModel.ConsumableType.BlastGrenade);
		SetCooldownAfterUse(EquipmentModel.ConsumableType.BlastGrenade);
		blastGrenadeDescription.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Consumable.BlastGrenade.Description{Parameters}", ConsumableUtils.GetBlastGrenadePushDistance(GameManager.Instance.modelManager), (int)ConsumableUtils.GetFlatDamage(GameManager.Instance.modelManager, EquipmentModel.ConsumableType.BlastGrenade), ConsumableUtils.GetPercentageDamageDefinition(GameManager.Instance.modelManager, EquipmentModel.ConsumableType.BlastGrenade));
		blastGrenadeThreatDescription.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Consumable.Menu.Stat.ThreatGenerated{Amount}", ConsumableUtils.GetThreatDefinition(GameManager.Instance.modelManager, EquipmentModel.ConsumableType.BlastGrenade));
		UpdateWhiteHealthBar();
	}

	public void OnGoreClick()
	{
		SetBigCardAndHighlightVisibility(EquipmentModel.ConsumableType.Gore);
		SetCooldownAfterUse(EquipmentModel.ConsumableType.Gore);
		goreDescription.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Consumable.Gore.Description{Duration}", ConsumableUtils.GetGoreDuration(GameManager.Instance.modelManager));
		UpdateWhiteHealthBar();
	}

	public void OnUseGrenadeClick()
	{
		ActorModel activeActor = GameManager.Instance.playerModel.Combat.ActiveActor;
		EquipmentItemModel equipmentItemModel = GameManager.Instance.playerModel.Equipment.GetConsumablesOfType(EquipmentModel.ConsumableType.Grenade).FirstOrDefault();
		if (equipmentItemModel != null && !GameManager.Instance.playerModel.Combat.IsConsumableInCooldown(EquipmentModel.ConsumableType.Grenade))
		{
			CombatView.Instance.CombatHUD.UnequipChargeEquipment(activeActor);
			if (Helpers.ExecuteCommand(new EquipConsumableCommand(activeActor, equipmentItemModel)) == TWDModelResult.OK)
			{
				OnClickClose();
				CombatView.Instance.CombatHUD.ConsumableSelected();
			}
		}
	}

	public void OnUseMedkitClick()
	{
		ActorModel activeActor = GameManager.Instance.playerModel.Combat.ActiveActor;
		if (activeActor.Hitpoints != activeActor.MaxHitPoints && !activeActor.IsDead && !activeActor.IsRaider)
		{
			EquipmentItemModel equipmentItemModel = GameManager.Instance.playerModel.Equipment.GetConsumablesOfType(EquipmentModel.ConsumableType.MedKit).FirstOrDefault();
			if (equipmentItemModel != null && !GameManager.Instance.playerModel.Combat.IsConsumableInCooldown(EquipmentModel.ConsumableType.MedKit) && Helpers.ExecuteCommand(new EquipConsumableCommand(activeActor, equipmentItemModel)) == TWDModelResult.OK)
			{
				OnClickClose();
			}
		}
	}

	public void OnUseFlareClick()
	{
		ActorModel activeActor = GameManager.Instance.playerModel.Combat.ActiveActor;
		EquipmentItemModel equipmentItemModel = GameManager.Instance.playerModel.Equipment.GetConsumablesOfType(EquipmentModel.ConsumableType.Flare).FirstOrDefault();
		if (equipmentItemModel != null && !GameManager.Instance.playerModel.Combat.IsConsumableInCooldown(EquipmentModel.ConsumableType.Flare))
		{
			CombatView.Instance.CombatHUD.UnequipChargeEquipment(activeActor);
			if (Helpers.ExecuteCommand(new EquipConsumableCommand(activeActor, equipmentItemModel)) == TWDModelResult.OK)
			{
				OnClickClose();
				CombatView.Instance.CombatHUD.ConsumableSelected();
			}
		}
	}

	public void OnUseBlastGrenadeClick()
	{
		ActorModel activeActor = GameManager.Instance.playerModel.Combat.ActiveActor;
		EquipmentItemModel equipmentItemModel = GameManager.Instance.playerModel.Equipment.GetConsumablesOfType(EquipmentModel.ConsumableType.BlastGrenade).FirstOrDefault();
		if (equipmentItemModel != null && !GameManager.Instance.playerModel.Combat.IsConsumableInCooldown(EquipmentModel.ConsumableType.BlastGrenade))
		{
			CombatView.Instance.CombatHUD.UnequipChargeEquipment(activeActor);
			if (Helpers.ExecuteCommand(new EquipConsumableCommand(activeActor, equipmentItemModel)) == TWDModelResult.OK)
			{
				OnClickClose();
				CombatView.Instance.CombatHUD.ConsumableSelected();
			}
		}
	}

	public void OnUseGoreClick()
	{
		ActorModel activeActor = GameManager.Instance.playerModel.Combat.ActiveActor;
		EquipmentItemModel equipmentItemModel = GameManager.Instance.playerModel.Equipment.GetConsumablesOfType(EquipmentModel.ConsumableType.Gore).FirstOrDefault();
		if (equipmentItemModel != null && !GameManager.Instance.playerModel.Combat.IsConsumableInCooldown(EquipmentModel.ConsumableType.Gore))
		{
			CombatView.Instance.CombatHUD.UnequipChargeEquipment(activeActor);
			if (Helpers.ExecuteCommand(new EquipConsumableCommand(activeActor, equipmentItemModel)) == TWDModelResult.OK)
			{
				OnClickClose();
			}
		}
	}

	private void OnEnable()
	{
		GameManager.Instance.playerModel.Combat.TurnManager.ActorChanged += OnActorChanged;
	}

	private void OnDisable()
	{
		GameManager.Instance.playerModel.Combat.TurnManager.ActorChanged -= OnActorChanged;
		DisableWhiteHealthBars();
	}

	private void OnActorChanged(ActorModel actor)
	{
		RefreshUI();
		UpdateWhiteHealthBar();
	}

	private void SetBigCardAndHighlightVisibility(EquipmentModel.ConsumableType consumableType)
	{
		for (int i = 0; i < bigCards.Length; i++)
		{
			if (i == (int)(consumableType - 1))
			{
				bigCards[i].SetActive(value: true);
				consumableSelectedHighlights[i].SetActive(value: true);
			}
			else
			{
				bigCards[i].SetActive(value: false);
				consumableSelectedHighlights[i].SetActive(value: false);
			}
		}
	}

	private void SetCooldownAfterUse(EquipmentModel.ConsumableType consumableType)
	{
		cooldownAfterUseDescriptionConsumable[(int)(consumableType - 1)].text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Consumable.Menu.Stat.CooldownAfterUse{Turns}", ConsumableUtils.GetCooldownDefinition(GameManager.Instance.modelManager, consumableType));
	}

	private void UpdateWhiteHealthBar()
	{
		if (bigCards[1].activeSelf)
		{
			ActorModel activeActor = GameManager.Instance.playerModel.Combat.TurnManager.ActiveActor;
			ActorView actorView = null;
			if (activeActor != null)
			{
				actorView = GameManager.Instance.GetViewForModel(activeActor) as ActorView;
			}
			PortraitHealthBar[] array = UnityEngine.Object.FindObjectsOfType<PortraitHealthBar>();
			float medKitRecoveredHealthDefinition = ConsumableUtils.GetMedKitRecoveredHealthDefinition(GameManager.Instance.modelManager);
			PortraitHealthBar[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].SetHealthBarRecovered((actorView == null) ? null : actorView.HealthIndicator, medKitRecoveredHealthDefinition);
			}
		}
		else
		{
			DisableWhiteHealthBars();
		}
	}

	public void ToggleHowTo()
	{
		howTo.SetActive(!howTo.activeSelf);
	}

	public void HowToGotIt()
	{
		PlayerPrefs.SetInt("ConsumablesHowTo", 1);
		ToggleHowTo();
	}

	private void RefreshUI()
	{
		if (base.IsClosing)
		{
			return;
		}
		if (PlayerPrefs.GetInt("ConsumablesHowTo", 0) == 0)
		{
			howTo.SetActive(value: true);
		}
		if (PlayerPrefs.GetInt("ConsumablesHowTo", 0) == 1 && howTo.activeSelf)
		{
			howTo.SetActive(value: false);
		}
		foreach (EquipmentModel.ConsumableType value in Enum.GetValues(typeof(EquipmentModel.ConsumableType)))
		{
			if (value != EquipmentModel.ConsumableType.Unknown)
			{
				int num = (int)(value - 1);
				int count = GameManager.Instance.playerModel.Equipment.GetConsumablesOfType(value).Count;
				consumableAmount[num].text = "x" + count;
				consumableAmountBigCard[num].text = "x" + count;
				bool flag = count == 0;
				consumableAmount[num].color = (flag ? amountTextColorZero : amountTextColorDefault);
				consumableAmountBg[num].color = (flag ? amountTextBgColorZero : amountTextBgColorDefault);
				consumableAmountBigCard[num].color = (flag ? amountTextColorZero : amountTextColorDefault);
				consumableAmountBgBigCard[num].color = (flag ? amountTextBgColorZero : amountTextBgColorDefault);
				ActorModel activeActor = GameManager.Instance.playerModel.Combat.ActiveActor;
				if (activeActor.UsedToolThisTurn || (count == 0 && !GameManager.Instance.playerModel.Combat.IsConsumableInCooldown(value)))
				{
					consumableUseButton[num].gameObject.SetActive(value: false);
				}
				else
				{
					consumableUseButton[num].gameObject.SetActive(value: true);
				}
				if (activeActor.UsedToolThisTurn)
				{
					alreadyUsedToolThisTurn[num].text = LocalizationManager.GetText("Consumable.Combat.AlreadyUsedThisTurn{HeroName}", activeActor.Name);
				}
				alreadyUsedToolThisTurn[num].gameObject.SetActive(activeActor.UsedToolThisTurn);
			}
		}
		UpdateCooldowns();
	}

	private static void DisableWhiteHealthBars()
	{
		PortraitHealthBar[] array = UnityEngine.Object.FindObjectsOfType<PortraitHealthBar>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetHealthBarRecovered(null, 0f);
		}
	}
}
