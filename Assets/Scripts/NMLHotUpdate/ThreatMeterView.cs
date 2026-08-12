using BaseModel;
using TWDModel;

public class ThreatMeterView : ModelView<ThreatMeterModel>
{
	private ThreatMeterOverlay threatOverlay;

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
		model.Changed += OnModelChanged;
		if (model is ThreatMeterModel threatMeterModel)
		{
			threatMeterModel.ThreatValueChanged += OnThreatValueChanged;
		}
	}

	public void Start()
	{
		threatOverlay = CombatView.Instance.CombatHUD.ThreatMeterOverlay;
		threatOverlay.ThreatLevel = 0f;
		_ = CombatView.Instance != null;
	}

	private void OnDestroy()
	{
		base.Model.Changed -= OnModelChanged;
		base.Model.ThreatValueChanged -= OnThreatValueChanged;
	}

	public void OnModelChanged(ModelObject model, string changed, object args)
	{
		_ = changed == "threatMeterValueChanged";
	}

	private void OnStateChange()
	{
	}

	private void OnThreatValueChanged(int oldValue, ThreatInstigator instigator)
	{
		DelayedNotificationVisualizationTask task = new DelayedNotificationVisualizationTask(null, delegate
		{
			int valueChange = base.Model.ThreatLevel - oldValue;
			UpdateThreatMeter(valueChange, instigator);
		});
		VisualizationQueue.Instance.Add(task);
	}

	private void UpdateThreatMeter(int valueChange, ThreatInstigator instigator)
	{
	}
}
