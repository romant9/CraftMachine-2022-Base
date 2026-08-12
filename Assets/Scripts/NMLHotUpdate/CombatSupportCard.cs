using System;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class CombatSupportCard : MonoBehaviour
{
	public enum ActivationState
	{
		Inactive = 0,
		Regular = 1,
		Targeted = 2
	}

	[SerializeField]
	private UITexture iconTexture;

	[SerializeField]
	private UILabel[] cooldownTexts;

	[SerializeField]
	private UISprite[] skillIconSprites;

	[SerializeField]
	private GameObject[] cooldownObjects;

	[SerializeField]
	private GameObject[] unavailabilityObjects;

	[SerializeField]
	private GameObject[] activatedObjects;

	[SerializeField]
	private GameObject skillInfoTooltipPivot;

	[SerializeField]
	private UISprite rarityBorder;

	[SerializeField]
	private UISprite minimizedSprite;

	[SerializeField]
	private int minimizeTween;

	[SerializeField]
	private int maximizeTween;

	[SerializeField]
	private CombatSupportSurvivorSelectedList selectedList;

	[SerializeField]
	private CombatSupportSurvivorUnselectList unselectList;

	private int ActorSlotIndex = -1;

	private CombatModel combatModel;

	public event Action SupportCancelClicked;

	private bool IsValidData()
	{
		if (ActorSlotIndex < 0 || combatModel == null)
		{
			return false;
		}
		List<ActorModel> factionActors = combatModel.GetFactionActors(Faction.Survivor);
		if (factionActors == null || factionActors.Count <= 0 || ActorSlotIndex >= factionActors.Count)
		{
			return false;
		}
		if (factionActors[ActorSlotIndex].IsDead)
		{
			return false;
		}
		return true;
	}

	private CombatSupportModel GetCurCombatSupportModel()
	{
		ActorModel actor = combatModel.GetFactionActors(Faction.Survivor)[ActorSlotIndex];
		if (!combatModel.SupportManager.TryGetSupport(actor, out var combatSupportModel))
		{
			return null;
		}
		return combatSupportModel;
	}

	public void Initialize(int slotIndex)
	{
		ActorSlotIndex = slotIndex;
		combatModel = GameManager.Instance.modelManager.CombatModel;
	}

	public void SurvivorClick()
	{
		CombatHUD combatHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatHUD) as CombatHUD;
		if (combatHUD != null)
		{
			combatHUD.OnClickSurvivorSkillUse();
		}
	}

	public void SupportClick()
	{
		CombatHUD combatHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatHUD) as CombatHUD;
		if (combatHUD != null)
		{
			combatHUD.OnSupportClicked(ActorSlotIndex);
		}
	}

	public void SupportInfoClick()
	{
		if (IsValidData())
		{
			TooltipManager.OpenForCombatSupport(skillInfoTooltipPivot, GetCurCombatSupportModel()?.SupportModel);
		}
	}

	public void SurvivorInfoClick()
	{
		if (IsValidData() && combatModel.GetFactionActors(Faction.Survivor)[ActorSlotIndex].CommandSkillModelManager.ActorCommandSkill != null)
		{
			TooltipManager.OpenForCombatSupportSurvivor(skillInfoTooltipPivot);
		}
	}

	public void CancelClick()
	{
		this.SupportCancelClicked?.Invoke();
		UpdateUI();
	}

	public void UpdateUI()
	{
		if (!IsValidData())
		{
			Helpers.GameObjectSetActive(selectedList, value: false);
			Helpers.GameObjectSetActive(unselectList, value: false);
			return;
		}
		Helpers.GameObjectSetActive(selectedList, value: true);
		Helpers.GameObjectSetActive(unselectList, value: true);
		selectedList.UpdateUI(ActorSlotIndex);
		unselectList.UpdateUI(ActorSlotIndex);
	}
}
