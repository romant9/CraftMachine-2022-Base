using Newtonsoft.Json;

namespace TWDModel
{
	public class DailyQuest : Achievement
	{
		[JsonIgnore]
		public virtual bool CanComplete => true;

		public bool CanGiveChallengeBonus { get; set; }

		[JsonIgnore]
		public virtual bool IsValidForBonusStars => false;

		public void Start(PlayerModel player)
		{
			Player = player;
			base.Valid = true;
		}
	}
}
