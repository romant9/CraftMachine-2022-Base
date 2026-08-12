using BaseModel;
using TWDModel;

public class LeaderBuffBetterTogetherStateIndicator : LeaderBuffStateIndicator
{
	private int currentStack;

	private bool isLeaderTrait;

	public override void OnActorModelChanged(ModelObject model, string changed, object args)
	{
		if (changed == "BetterTogetherCountChanged")
		{
			UpdateState();
		}
	}

	public override void UpdateState()
	{
		if (this == null || base.gameObject == null || actor == null)
		{
			return;
		}
		bool flag = false;
		CombatModel combatModel = GameManager.Instance.modelManager.CombatModel;
		if (combatModel == null)
		{
			return;
		}
		if (combatModel.Survivors[0].HasAnyLevelTrait("LeaderBuffBetterTogether"))
		{
			isLeaderTrait = true;
		}
		foreach (ActorModel survivor in combatModel.Survivors)
		{
			if (survivor.HasAnyLevelTrait("BaseBetterTogether") || survivor.HasAnyLevelTrait("LeaderBuffBetterTogether"))
			{
				flag = true;
			}
		}
		base.gameObject.SetActive(flag);
		if (flag)
		{
			int num = ((combatModel.Survivors.Count <= 2) ? 1 : 2);
			if (isLeaderTrait)
			{
				num *= 2;
			}
			else if (!actor.HasAnyLevelTrait("LeaderBuffBetterTogether"))
			{
				num = 1;
			}
			currentStack = actor.BetterTogetherMultiplier;
			chargeState.fillAmount = ((currentStack == 0) ? 0f : ((float)currentStack / (float)num));
			buffCountText.text = currentStack + "/" + num;
			fullStateEffect.SetActive(num == currentStack && num != 0 && currentStack != 0);
		}
	}
}
