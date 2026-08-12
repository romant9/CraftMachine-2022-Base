using BaseModel;

namespace TWDModel
{
	public class ClearBlackboardToggleCommand : ConsumeCurrencyCommand
	{
		public string BlackboardToggle { get; set; }

		public ClearBlackboardToggleCommand()
		{
		}

		public ClearBlackboardToggleCommand(string toggle)
		{
			BlackboardToggle = toggle;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			(manager as TWDModelManager).Blackboard.ClearToggle(BlackboardToggle);
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
