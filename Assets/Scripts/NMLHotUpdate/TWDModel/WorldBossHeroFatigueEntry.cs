namespace TWDModel
{
	public class WorldBossHeroFatigueEntry
	{
		public int Charges { get; set; }

		public long BaseUtcMs { get; set; }

		public WorldBossHeroFatigueEntry()
		{
		}

		public WorldBossHeroFatigueEntry(int charges, long baseUtcMs)
		{
			Charges = charges;
			BaseUtcMs = baseUtcMs;
		}

		public int GetCurrentCharges(int limit, long recoverMs, long nowMs)
		{
			if (Charges >= limit)
			{
				return limit;
			}
			if (recoverMs <= 0)
			{
				return Charges;
			}
			long num = nowMs - BaseUtcMs;
			if (num <= 0)
			{
				return Charges;
			}
			long num2 = num / recoverMs;
			long num3 = Charges + num2;
			if (num3 < limit)
			{
				return (int)num3;
			}
			return limit;
		}

		public long GetNextRecoverRemainingMs(int limit, long recoverMs, long nowMs)
		{
			if (recoverMs <= 0)
			{
				return 0L;
			}
			if (GetCurrentCharges(limit, recoverMs, nowMs) >= limit)
			{
				return 0L;
			}
			long num = nowMs - BaseUtcMs;
			if (num < 0)
			{
				num = 0L;
			}
			return recoverMs - num % recoverMs;
		}

		public void Settle(int limit, long recoverMs, long nowMs)
		{
			if (Charges >= limit)
			{
				Charges = limit;
			}
			else
			{
				if (recoverMs <= 0)
				{
					return;
				}
				long num = nowMs - BaseUtcMs;
				if (num <= 0)
				{
					return;
				}
				long num2 = num / recoverMs;
				if (num2 > 0)
				{
					long num3 = Charges + num2;
					if (num3 >= limit)
					{
						Charges = limit;
						return;
					}
					Charges = (int)num3;
					BaseUtcMs += num2 * recoverMs;
				}
			}
		}

		public bool TryConsume(int limit, long recoverMs, long nowMs)
		{
			Settle(limit, recoverMs, nowMs);
			if (Charges <= 0)
			{
				return false;
			}
			bool num = Charges >= limit;
			Charges--;
			if (num)
			{
				BaseUtcMs = nowMs;
			}
			return true;
		}
	}
}
