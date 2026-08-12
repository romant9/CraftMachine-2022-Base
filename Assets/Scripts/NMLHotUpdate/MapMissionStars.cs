using System;
using System.Linq;
using Newtonsoft.Json;
using TWDModel;

public class MapMissionStars : TWDModelObject
{
	public bool[] Stars { get; set; }

	public int TotalStars { get; set; }

	public int TotalBonusStars { get; set; }

	public bool FeaturedHeroExtraChallengeStar { get; set; }

	[JsonIgnore]
	public int NumberStars
	{
		get
		{
			int num = 0;
			for (int i = 0; i < 3; i++)
			{
				if (Stars[i])
				{
					num++;
				}
			}
			if (FeaturedHeroExtraChallengeStar)
			{
				num++;
			}
			return num;
		}
	}

	public override void Start()
	{
		base.Start();
	}

	public override void Initialize()
	{
		base.Initialize();
		Stars = new bool[3];
		FeaturedHeroExtraChallengeStar = false;
		TotalStars = 0;
	}

	public int GiveStars(MissionStarCondition[] missionStarConditions, bool inWeeklyChallenge = false)
	{
		CombatModel combat = base.manager.Player.Combat;
		bool[] array = new bool[3];
		for (int i = 0; i < 3; i++)
		{
			MissionStarCondition missionStarCondition = missionStarConditions[i];
			if (missionStarCondition.Type == MissionStarsType.CompleteMission)
			{
				if (combat.MissionStatistics.LastCombatResult == ECombatResult.Successful)
				{
					array[i] = true;
				}
			}
			else if (missionStarCondition.Type == MissionStarsType.NoStruggle)
			{
				if (combat.MissionStatistics.StruggleCount == 0)
				{
					array[i] = true;
				}
			}
			else if (missionStarCondition.Type == MissionStarsType.MaxTurns)
			{
				if (combat.TurnManager.TurnCount <= int.Parse(missionStarCondition.Parameter))
				{
					array[i] = true;
				}
			}
			else if (missionStarCondition.Type == MissionStarsType.KillXWalkers)
			{
				if (combat.MissionStatistics.WalkersKilled >= int.Parse(missionStarCondition.Parameter))
				{
					array[i] = true;
				}
			}
			else
			{
				if (missionStarCondition.Type != MissionStarsType.NoHitTaken)
				{
					continue;
				}
				bool flag = false;
				foreach (SurvivorModel item in combat.MissionRoster)
				{
					if (item.InjuryType != InjuryType.None || item.IsDead)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					array[i] = true;
				}
			}
		}
		int num = 0;
		for (int j = 0; j < 3; j++)
		{
			if (array[j])
			{
				num++;
			}
		}
		int num2 = 0;
		if (inWeeklyChallenge)
		{
			if (FeaturedHeroExtraChallengeStar = num == 3 && combat.MissionRoster.Any((SurvivorModel x) => x.IsFeaturedHero && x.IsLeader))
			{
				num++;
			}
			num2 = Math.Max(0, num - Math.Min(TotalStars, 4));
			TotalStars += num2;
		}
		else
		{
			TotalStars += num;
		}
		if (inWeeklyChallenge || num > NumberStars)
		{
			for (int num3 = 0; num3 < 3; num3++)
			{
				Stars[num3] = array[num3];
			}
		}
		if (!inWeeklyChallenge)
		{
			return num;
		}
		return num2;
	}

	public void ResetStarsForNewChallengeCycle()
	{
		Stars = new bool[3];
		FeaturedHeroExtraChallengeStar = false;
		TotalStars = 0;
	}

	public override bool IsValid()
	{
		return true;
	}
}
