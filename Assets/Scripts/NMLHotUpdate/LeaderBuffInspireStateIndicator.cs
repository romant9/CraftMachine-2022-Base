using BaseModel;
using TWDModel;

public class LeaderBuffInspireStateIndicator : LeaderBuffStateIndicator
{
	public override void OnActorModelChanged(ModelObject model, string changed, object args)
	{
		if (changed == "actorTraitGained")
		{
			TraitDefinition traitDefinition = (TraitDefinition)args;
			if (traitDefinition != null && traitDefinition.Identifier.ToLower() == "InspirePerKillIncreaseExtraChargePointChanceModifierTrait".ToLower())
			{
				UpdateState();
			}
		}
	}

	public override void UpdateState()
	{
		if (this == null || actor == null || base.gameObject == null)
		{
			return;
		}
		if (abilityManager == null)
		{
			abilityManager = GameManager.Instance.playerModel.AbilityManager;
		}
		FixedPoint value = 0.0;
		FixedPoint value2 = 0.0;
		FixedPoint value3 = 0.0;
		if (abilityManager.VisitParameter("LeaderBuffInspireIncreaseExtraChargePointChance", ref value, actor))
		{
			abilityManager.VisitParameter("LeaderBuffInspireMaxExtraChargePointChance", ref value2, actor);
			abilityManager.VisitParameter("AbilityModifierLeaderBuffInspireExtraChargePointChance", ref value3, actor);
			int num = (int)FixedPoint.Ceiling(value * 1000.0);
			int num2 = (int)FixedPoint.Ceiling(value2 * 1000.0) / num;
			int num3 = (int)FixedPoint.Ceiling(value3 * 1000.0) / num;
			chargeState.fillAmount = (float)num3 / (float)num2;
			buffCountText.text = num3 + "/" + num2;
			if (num3 == num2)
			{
				fullStateEffect.SetActive(value: true);
			}
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
