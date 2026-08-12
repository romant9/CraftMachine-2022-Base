using BaseModel;

namespace TWDModel
{
	public class SetMemberGvGInfoCommand : TWDSocialModelCommand
	{
		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return SetMemberGvGInfo(manager);
		}

		public override IModelCommandRespond Execute(ModelManager modelManager)
		{
			return base.Execute(modelManager) as NGModelCommandRespond;
		}

		public static GroupCommandBase SetMemberGvGInfo(TWDModelManager manager)
		{
			string hashedId = manager.Player.HashedId;
			int totalVPPoints = manager.Player.CalculateLifeTimeGvGVpAccumulated();
			return new SetMemberGvGInfoGroupCommand(hashedId, totalVPPoints);
		}
	}
}
