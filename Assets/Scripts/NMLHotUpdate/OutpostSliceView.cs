using BaseModel;
using TWDModel;

public class OutpostSliceView : ModelView<OutpostSliceModel>
{
	public SlicePosition SlicePosition;

	public override bool AutoGenerateViewID => true;

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
	}
}
