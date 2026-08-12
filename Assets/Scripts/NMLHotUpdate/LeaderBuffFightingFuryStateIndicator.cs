using BaseModel;

public class LeaderBuffFightingFuryStateIndicator : LeaderBuffStateIndicator
{
	private int attacksGiven;

	private int attacksLeft;

	private bool isActive;

	private bool shouldUpdate = true;

	public override void OnActorModelChanged(ModelObject model, string changed, object args)
	{
		if (changed == "actorNewTurn")
		{
			shouldUpdate = true;
		}
		switch (changed)
		{
		case "actorAdditionalAttackChecked":
			UpdateState();
			shouldUpdate = true;
			break;
		case "actorSecondMoveCompleted":
			shouldUpdate = false;
			break;
		case "actorUsedFreeAttack":
			shouldUpdate = true;
			break;
		case "actorStunnedEvent":
		case "ActorReloadingStarted":
			shouldUpdate = false;
			break;
		case "AbilityVisited":
			if (args is object[] array && array[0] is string text && text == "LeaderBuffLooter" && !actor.HasGainedExtraAP)
			{
				ResetIndicator();
				shouldUpdate = false;
			}
			break;
		}
	}

	private void ResetIndicator()
	{
		if (!(this == null) && !(base.gameObject == null) && actor != null)
		{
			chargeState.fillAmount = 0f;
			buffCountText.text = "0/0";
			fullStateEffect.SetActive(value: false);
		}
	}

	public override void UpdateState()
	{
		if (this == null || base.gameObject == null || actor == null)
		{
			return;
		}
		isActive = actor.HasAnyLevelTrait("BaseFightingFury") || actor.HasAnyLevelTrait("FightingFury");
		base.gameObject.SetActive(isActive);
		if (isActive)
		{
			attacksGiven = ((actor.GivenAdditionalAttacks == 0) ? actor.GivenAdditionalAttacks : (actor.GivenAdditionalAttacks - 1));
			attacksLeft = ((actor.AdditionalAttackCount >= attacksGiven) ? attacksGiven : actor.AdditionalAttackCount);
			if (attacksGiven == 0 || attacksLeft == 0 || !shouldUpdate)
			{
				ResetIndicator();
				return;
			}
			chargeState.fillAmount = ((attacksGiven == 0) ? 0f : ((float)attacksLeft / (float)attacksGiven));
			buffCountText.text = attacksLeft + "/" + attacksGiven;
			fullStateEffect.SetActive(attacksGiven == attacksLeft && attacksGiven != 0 && attacksLeft != 0);
		}
	}
}
