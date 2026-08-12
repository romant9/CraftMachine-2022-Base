using BaseModel;

namespace TWDModel
{
	public class ChangeGuildNameCommand : TWDSocialModelCommand
	{
		public string Name { get; set; }

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return new ChangeGuildNameGroupCommand
			{
				Name = Name
			};
		}

		public override IModelCommandRespond Execute(ModelManager modelManager)
		{
			TWDModelResult tWDModelResult = (modelManager as TWDModelManager).Player.PayForChangeGuildName();
			if (tWDModelResult == TWDModelResult.OK)
			{
				return base.Execute(modelManager) as NGModelCommandRespond;
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
