using BaseModel;

namespace TWDModel
{
	public class SetBlackboardToggleCommand : ConsumeCurrencyCommand
	{
		public string BlackboardToggle { get; set; }

		public SetBlackboardToggleCommand()
		{
		}

		public SetBlackboardToggleCommand(string toggle)
		{
			BlackboardToggle = toggle;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			(manager as TWDModelManager).Blackboard.SetToggle(BlackboardToggle);
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
