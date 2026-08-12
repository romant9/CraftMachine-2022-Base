using BaseModel;

namespace TWDModel
{
	public class ModifyGuildCommand : TWDSocialModelCommand
	{
		public string Description { get; set; }

		public GuildJoinType JoinType { get; set; }

		public string Purpose { get; set; }

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return new ModifyGuildGroupCommand
			{
				Description = Description,
				JoinType = JoinType,
				Purpose = Purpose
			};
		}
	}
}
