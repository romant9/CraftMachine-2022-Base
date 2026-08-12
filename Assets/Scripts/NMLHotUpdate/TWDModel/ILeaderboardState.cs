using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public interface ILeaderboardState
	{
		bool LeaderboardUpdated { get; set; }

		[JsonIgnore]
		string LeaderboardName { get; }

		bool SaveLeaderboard(IServerService serverService, LeaderboardEntry entry);
	}
}
