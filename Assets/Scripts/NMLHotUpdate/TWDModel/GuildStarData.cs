using System;

namespace TWDModel
{
	[Serializable]
	public class GuildStarData
	{
		public string GuildId;

		public int StarCount;

		public GuildStarData()
		{
		}

		public GuildStarData(string guildId, int starCount)
		{
			GuildId = guildId;
			StarCount = starCount;
		}
	}
}
