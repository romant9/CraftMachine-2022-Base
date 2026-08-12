using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class CombatSupportSurvivorSelectedList : MonoBehaviour
{
	[SerializeField]
	private UIGrid grid;

	[SerializeField]
	private GameObject SupportContainer;

	[SerializeField]
	private UISprite SupportIcon;

	[SerializeField]
	private GameObject SupportCooldownContainer;

	[SerializeField]
	private GameObject SupportOperateContainer;

	[SerializeField]
	private GameObject SupportDarkBG;

	[SerializeField]
	private GameObject SurvivorContainer;

	[SerializeField]
	private UISprite SurvivorIcon;

	[SerializeField]
	private GameObject SurvivorCooldownContainer;

	[SerializeField]
	private GameObject SurvivorChargeContainer;

	[SerializeField]
	private GameObject SurvivorOperateContainer;

	private int ActorSlotIndex = -1;

	private CombatModel combatModel;

	private ActorModel actorModel;

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

	public void UpdateUI(int slotIndex)
	{
		ActorSlotIndex = slotIndex;
		combatModel = GameManager.Instance.modelManager.CombatModel;
		if (combatModel == null)
		{
			return;
		}
		if (!IsValidData())
		{
			Helpers.GameObjectSetActive(SupportContainer, value: false);
			Helpers.GameObjectSetActive(SurvivorContainer, value: false);
			return;
		}
		List<ActorModel> factionActors = combatModel.GetFactionActors(Faction.Survivor);
		actorModel = factionActors[ActorSlotIndex];
		ActorModel activeActor = combatModel.ActiveActor;
		if (actorModel != activeActor)
		{
			Helpers.GameObjectSetActive(SupportContainer, value: false);
			Helpers.GameObjectSetActive(SurvivorContainer, value: false);
		}
		else
		{
			UpdateSupport();
			UpdateSurvivor();
			grid.repositionNow = true;
		}
	}

	private void UpdateSupport()
	{
		if (combatModel.SupportManager.Supports == null || combatModel.SupportManager.Supports.Count <= 0)
		{
			Helpers.GameObjectSetActive(SupportContainer, value: false);
			return;
		}
		if (!combatModel.SupportManager.TryGetSupport(actorModel, out var combatSupportModel))
		{
			Helpers.GameObjectSetActive(SupportContainer, value: false);
			return;
		}
		if (combatSupportModel == null || PlayerInputManager.Instance == null)
		{
			Helpers.GameObjectSetActive(SupportContainer, value: false);
			return;
		}
		Helpers.GameObjectSetActive(SupportContainer, value: true);
		SupportIcon.spriteName = HelpersGfx.GetSupportSkillIconName(combatSupportModel.SupportId);
		SupportIcon.color = Color.white;
		CombatSupportManager supportManager = combatSupportModel.manager.CombatModel.SupportManager;
		if (supportManager == null)
		{
			return;
		}
		CombatSupportAvailability availability = supportManager.GetAvailability(combatSupportModel);
		Helpers.GameObjectSetActive(SupportCooldownContainer, value: false);
		if (availability != CombatSupportAvailability.AlreadyUsed && combatSupportModel.RemainingCooldown > 0)
		{
			Helpers.GameObjectSetActive(SupportCooldownContainer, value: true);
			SupportIcon.color = Color.gray;
			SupportCooldownContainer.GetComponentInChildren<UILabel>().text = combatSupportModel.RemainingCooldown.ToString();
		}
		Helpers.GameObjectSetActive(SupportDarkBG, value: false);
		if (availability != CombatSupportAvailability.Executable)
		{
			Helpers.GameObjectSetActive(SupportDarkBG, value: true);
		}
		GameObject obj = SupportOperateContainer.transform.Find("InfoContainer").gameObject;
		GameObject obj2 = SupportOperateContainer.transform.Find("CloseContainer").gameObject;
		Helpers.GameObjectSetActive(obj, value: false);
		Helpers.GameObjectSetActive(obj2, value: false);
		SupportInteractionManager supportInteractionManager = PlayerInputManager.Instance.GetHandler<SupportInputHandler>()?.SupportInteractionManager;
		if (supportInteractionManager != null)
		{
			if (supportInteractionManager.IsUsingSurpportInteraction())
			{
				Helpers.GameObjectSetActive(obj2, value: true);
			}
			else
			{
				Helpers.GameObjectSetActive(obj, value: true);
			}
		}
	}

	private void UpdateSurvivor()
	{
		BaseCommandSkill actorCommandSkill = actorModel.CommandSkillModelManager.ActorCommandSkill;
		if (actorCommandSkill == null)
		{
			Helpers.GameObjectSetActive(SurvivorContainer, value: false);
			return;
		}
		Helpers.GameObjectSetActive(SurvivorContainer, value: true);
		SurvivorIcon.spriteName = actorCommandSkill.Definition.Icon;
		SurvivorIcon.color = Color.white;
		Helpers.GameObjectSetActive(SurvivorCooldownContainer, value: false);
		Helpers.GameObjectSetActive(SurvivorChargeContainer, value: false);
		if (actorModel.SurvivalGameLeftCD > 0)
		{
			SurvivorIcon.color = Color.gray;
			Helpers.GameObjectSetActive(SurvivorCooldownContainer, value: true);
			SurvivorCooldownContainer.GetComponentInChildren<UILabel>().text = actorModel.SurvivalGameLeftCD.ToString();
		}
		ShadowedGuardSkill shadowedGuardSkill = actorModel.CommandSkillModelManager?.GetActorCommandSkill<ShadowedGuardSkill>(CommandSkillType.CommandSkillShadowedGuard);
		if (shadowedGuardSkill != null && shadowedGuardSkill.LeftCooldownTurns > 0)
		{
			SurvivorIcon.color = Color.gray;
			Helpers.GameObjectSetActive(SurvivorCooldownContainer, value: true);
			SurvivorCooldownContainer.GetComponentInChildren<UILabel>().text = shadowedGuardSkill.LeftCooldownTurns.ToString();
		}
		if (!SurvivorCooldownContainer.activeSelf && actorCommandSkill.Definition.SkillFunc == CommandSkillFuncType.Charge)
		{
			FixedPoint value = 0L;
			if (actorCommandSkill.Type == CommandSkillType.CommandSkillShadowedGuard)
			{
				combatModel.manager.Player.AbilityManager.VisitParameter("LeaderBuffShadowedGuard_Charge_MaxNum", ref value, actorModel);
			}
			SurvivorChargeContainer.GetComponentInChildren<UILabel>().text = actorModel.ChargeNum.ToString() + "/" + (int)value;
			bool flag = actorModel.ChargeNum < value;
			SurvivorIcon.color = (flag ? Color.gray : Color.white);
			Helpers.GameObjectSetActive(SurvivorChargeContainer, flag);
		}
		Helpers.GameObjectSetActive(SurvivorOperateContainer, value: true);
		Helpers.GameObjectSetActive(SurvivorOperateContainer.transform.Find("InfoContainer").gameObject, value: true);
		Helpers.GameObjectSetActive(SurvivorOperateContainer.transform.Find("CloseContainer").gameObject, value: false);
	}
}
