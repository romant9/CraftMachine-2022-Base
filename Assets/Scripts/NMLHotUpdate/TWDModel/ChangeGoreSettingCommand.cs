using BaseModel;

namespace TWDModel
{
	public class ChangeGoreSettingCommand : ModelCommand
	{
		public bool EnableGore { get; private set; }

		public ChangeGoreSettingCommand()
		{
		}

		public ChangeGoreSettingCommand(bool enableGore)
		{
			EnableGore = enableGore;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (manager is TWDModelManager tWDModelManager)
			{
				if (EnableGore)
				{
					tWDModelManager.Player.Blackboard.ClearToggle("Toggle.GoreDisabled");
				}
				else
				{
					tWDModelManager.Player.Blackboard.SetToggle("Toggle.GoreDisabled");
				}
				tWDModelManager.Player.Blackboard.SetToggle("Toggle.GoreUsed");
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
