using BaseModel;

namespace TWDModel
{
	public class StartChallengeGroupCommand : TWDGroupCommand
	{
		public string ChallengeId;

		public StartChallengeGroupCommand()
		{
		}

		public StartChallengeGroupCommand(string challengeId)
		{
			ChallengeId = challengeId;
		}

		public override GroupCommandBase Execute(ModelManager manager)
		{
			GuildModel guildModel = (GuildModel)manager.GetGroupModel(GroupId);
			if (guildModel.CurrentChallengeId != ChallengeId)
			{
				guildModel.StartChallenge(ChallengeId);
				SaveGroupModel(manager);
			}
			return this;
		}
	}
}
