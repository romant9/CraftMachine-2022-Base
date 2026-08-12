using BaseModel;
using TWDModel;

public class ChargeMeterView : ModelView<ChargeMeterModel>
{
	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
		model.Changed += OnModelChanged;
	}

	private void OnDestroy()
	{
		base.Model.Changed -= OnModelChanged;
	}

	public void OnModelChanged(ModelObject model, string changed, object args)
	{
		if (!(changed == "chargeMeterValueChanged"))
		{
			return;
		}
		int oldValue = (int)args;
		ActorView damageActor = GameManager.Instance.GetViewForModel(base.Model.Actor) as ActorView;
		if (base.Model.Actor.Faction != Faction.Survivor)
		{
			return;
		}
		if (oldValue < base.Model.ChargeLevel)
		{
			string textId = ((base.Model.Actor.ChargeMeter.ChargeLevel < base.Model.Actor.ChargeMeter.MaxLevel) ? "ActorNotification.ChargePointReceived" : "ActorNotification.FullyCharged");
			bool stackMultiple = base.Model.Actor.ChargeMeter.ChargeLevel < base.Model.Actor.ChargeMeter.MaxLevel;
			bool wipeAllPreviousOfSameType = base.Model.Actor.ChargeMeter.ChargeLevel >= base.Model.Actor.ChargeMeter.MaxLevel;
			damageActor?.AddNotification(new ActorNotificationMessage(LocalizationManager.GetText(textId), ActorNotificationType.ChargePoint), dueLuck: false, base.Model.Actor, delegate
			{
				damageActor.HealthIndicator?.UpdateChargeMeterIcons(base.Model);
				GainChargePoints(oldValue, base.Model.ChargeLevel);
			}, TimedEffectType.None, stackMultiple, wipeAllPreviousOfSameType);
		}
		else if (oldValue > base.Model.ChargeLevel)
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model.Actor, delegate
			{
				damageActor?.HealthIndicator?.UpdateChargeMeterIcons(base.Model);
				ReduceChargePoints(oldValue, base.Model.ChargeLevel);
			}));
		}
	}

	private void GainChargePoints(int oldPoints, int newPoints)
	{
		CombatView.Instance.CombatHUD.FreshChargePointActivated(base.Model.Actor);
		for (int i = oldPoints; i < newPoints; i++)
		{
			if (i == base.Model.MaxLevel - 1)
			{
				CombatView.Instance.CombatHUD.SetChargeButtonEnabled(base.Model.Actor, enabled: true);
			}
		}
	}

	private void ReduceChargePoints(int oldPoints, int newPoints)
	{
		CombatView.Instance.CombatHUD.SetChargeButtonEnabled(base.Model.Actor, base.Model.ChargeAvailable);
		CombatView.Instance.CombatHUD.FreshChargePointActivated(base.Model.Actor);
		CombatView.Instance.CombatHUD.ResetChargePoint(base.Model.Actor);
	}
}
