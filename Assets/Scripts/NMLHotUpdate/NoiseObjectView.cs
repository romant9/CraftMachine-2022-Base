using BaseModel;
using TWDModel;
using UnityEngine;

public class NoiseObjectView : ModelView<NoiseObjectModel>, IRunLocationItem
{
	[SerializeField]
	[Tooltip("How far does the noise carry.")]
	private int NoiseRange = 5;

	[SerializeField]
	[Tooltip("How much threat does the interaction with the object generate.")]
	private int ThreatValue = 5;

	public override bool AutoGenerateViewID => true;

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
		model.Changed += OnModelChanged;
	}

	public TWDModelObject Apply(IRunLocationItemContainer runLocation, IRunLocationErrorContext errors)
	{
		GridCoordinate configuredCoordinate = GridView.ActiveInstance.GetConfiguredCoordinate(base.transform.position);
		NoiseObjectModel noiseObjectModel = new NoiseObjectModel(ViewId, configuredCoordinate, NoiseRange, ThreatValue);
		runLocation.AddModelObject(noiseObjectModel);
		return noiseObjectModel;
	}

	public void Start()
	{
	}

	private void Update()
	{
	}

	public void OnModelChanged(ModelObject model, string changed, object args)
	{
	}
}
