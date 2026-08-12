using BaseModel;
using TWDModel;

public class CoverView : ModelView<CoverModel>
{
	public CoverType CoverType;

	public bool IsActiveAtStart = true;

	public override bool AutoGenerateViewID => true;

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
		model.Changed += OnModelChanged;
	}

	public void OnModelChanged(ModelObject model, string changed, object args)
	{
	}
}
