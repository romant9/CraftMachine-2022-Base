using TWDModel;

public class OutpostAlarmView : ModelView<OutpostAlarmModel>, IRunLocationItem
{
	public override bool AutoGenerateViewID => true;

	public TWDModelObject Apply(IRunLocationItemContainer runLocation, IRunLocationErrorContext errors)
	{
		OutpostAlarmModel outpostAlarmModel = new OutpostAlarmModel(ViewId);
		runLocation.AddModelObject(outpostAlarmModel);
		return outpostAlarmModel;
	}
}
