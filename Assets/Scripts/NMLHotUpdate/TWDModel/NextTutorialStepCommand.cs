using BaseModel;

namespace TWDModel
{
	public class NextTutorialStepCommand : ModelCommand
	{
		public bool ShowSuppliesHud { get; set; }

		public bool ShowGasHud { get; set; }

		public bool ShowDiamondsHud { get; set; }

		public bool ShowDailyQuestHud { get; set; }

		public NextTutorialStepCommand()
		{
		}

		public NextTutorialStepCommand(TutorialModel tutorial)
			: base(tutorial)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TutorialModel model = manager.GetModel<TutorialModel>(base.ModelId);
			model.RecordTdEvent();
			model.NextStep();
			if (ShowSuppliesHud)
			{
				model.ShowSuppliesHud = true;
			}
			if (ShowGasHud)
			{
				model.ShowGasHud = true;
			}
			if (ShowDiamondsHud)
			{
				model.ShowDiamondsHud = true;
			}
			if (ShowDailyQuestHud)
			{
				model.ShowDailyQuestHud = true;
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
