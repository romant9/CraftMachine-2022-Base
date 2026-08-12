using TWDModel;

public class OutpostObjectiveView : ModelView<OutpostObjectiveModel>, IRunLocationItem
{
	public OutpostObjectiveType OutpostObjectiveType;

	public override bool AutoGenerateViewID => true;

	public TWDModelObject Apply(IRunLocationItemContainer runLocation, IRunLocationErrorContext errors)
	{
		OutpostObjectiveModel outpostObjectiveModel = new OutpostObjectiveModel(ViewId);
		outpostObjectiveModel.OutpostObjectiveType = OutpostObjectiveType;
		runLocation.AddModelObject(outpostObjectiveModel);
		return outpostObjectiveModel;
	}
}
