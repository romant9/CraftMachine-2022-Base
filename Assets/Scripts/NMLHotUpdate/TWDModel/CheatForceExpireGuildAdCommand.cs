using BaseModel;

namespace TWDModel
{
	public class CheatForceExpireGuildAdCommand : TWDSocialModelCommand
	{
		public long TimeLeftMs { get; set; }

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return new CheatForceExpireGuildAdGroupCommand
			{
				TimeLeftMs = TimeLeftMs
			};
		}
	}
}
