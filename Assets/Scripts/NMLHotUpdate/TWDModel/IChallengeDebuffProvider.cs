using System.Collections.Generic;

namespace TWDModel
{
	public interface IChallengeDebuffProvider
	{
		bool IsInApocalyptiWeeklyChallenge { get; }

		List<DifficultyIncrementalDebuff> GetChallengeDebuffs();
	}
}
