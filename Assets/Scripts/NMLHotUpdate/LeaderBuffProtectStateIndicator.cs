using BaseModel;

public class LeaderBuffProtectStateIndicator : LeaderBuffStateIndicator
{
	public override void OnActorModelChanged(ModelObject model, string changed, object args)
	{
		if (changed == "ShieldChanged")
		{
			UpdateState();
		}
	}

	public override void UpdateState()
	{
		if (!(this == null) && actor != null && !(base.gameObject == null))
		{
			if (actor != null && actor.IsProtectorDarylShielded)
			{
				int num = actor.ShieldTimedEffect.Duration - actor.ShieldTimedEffect.Counter;
				base.gameObject.SetActive(value: true);
				buffCountText.text = num.ToString() ?? "";
			}
			else
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}
}
