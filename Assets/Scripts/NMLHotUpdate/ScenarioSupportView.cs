using TWDModel;
using UnityEngine;

public class ScenarioSupportView : ModelView<ScenarioSupportModel>, IRunLocationItem
{
	[SerializeField]
	private string supportId;

	[SerializeField]
	private int supportLevel;

	[SerializeField]
	private int equippedIndex;

	public override bool AutoGenerateViewID => true;

	public TWDModelObject Apply(IRunLocationItemContainer runLocation, IRunLocationErrorContext errors)
	{
		ScenarioSupportModel scenarioSupportModel = new ScenarioSupportModel(supportId, supportLevel, equippedIndex);
		runLocation.AddModelObject(scenarioSupportModel);
		return scenarioSupportModel;
	}
}
