using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class GuildBanModel
	{
		[Serializable]
		public class BanEntry
		{
			public string PlayerId;

			public long Until;
		}

		public List<BanEntry> Bans { get; set; } = new List<BanEntry>();

		private void ClearOldBans(long currentTime)
		{
			for (int num = Bans.Count - 1; num >= 0; num--)
			{
				if (Bans[num].Until <= currentTime)
				{
					Bans.RemoveAt(num);
				}
			}
		}

		public bool IsBanned(string playerId, long currentTime)
		{
			foreach (BanEntry ban in Bans)
			{
				if (ban.PlayerId == playerId && ban.Until > currentTime)
				{
					return true;
				}
			}
			return false;
		}

		public void Ban(string playerId, long timeUntil, long currentTime)
		{
			ClearOldBans(currentTime);
			for (int i = 0; i < Bans.Count; i++)
			{
				if (Bans[i].PlayerId == playerId)
				{
					Bans.RemoveAt(i);
					break;
				}
			}
			Bans.Add(new BanEntry
			{
				PlayerId = playerId,
				Until = timeUntil
			});
		}
	}
}
