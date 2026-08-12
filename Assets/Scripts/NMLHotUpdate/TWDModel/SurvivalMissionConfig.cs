using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class SurvivalMissionConfig
	{
		public enum Type
		{
			Invalid = 0,
			Survival = 1,
			GuildBattle = 2
		}

		public enum SurvivalObjectiveType
		{
			Invalid = 0,
			KillAllWalkers = 1,
			KillAllRaiders = 2,
			GoToExit = 3,
			KillAmountAndExit = 4,
			KillBossAndExit = 5,
			FindLoot = 6,
			SurviveTurnAmountAndExit = 7
		}

		public static readonly int[] CountedTags = new int[3] { 1342177280, 1342177281, 1342177282 };

		public static readonly WalkerType[] SupportedWalkerTypes = new WalkerType[12]
		{
			WalkerType.WalkerNormal,
			WalkerType.WalkerTank,
			WalkerType.WalkerArmored,
			WalkerType.WalkerExplosive,
			WalkerType.WalkerSpiked,
			WalkerType.WalkerGoo,
			WalkerType.WalkerMetalhead,
			WalkerType.WalkerFast,
			WalkerType.WalkerWhisperer,
			WalkerType.WalkerWhispererMelee,
			WalkerType.ExplosiveBarrel,
			WalkerType.WalkerCommonWealth
		};

		public static readonly SurvivorClass[] SupportedRaiderTypes = new SurvivorClass[6]
		{
			SurvivorClass.Scout,
			SurvivorClass.Bruiser,
			SurvivorClass.Hunter,
			SurvivorClass.Warrior,
			SurvivorClass.Assault,
			SurvivorClass.Shooter
		};

		public static readonly string SurvivorPlayerConst = "SurvivorPlayer";

		[JsonIgnore]
		public Type MissionType;

		public string ConfigName;

		public int MissionOrderInSection;

		public string TitleDisplayLocale;

		public string BriefingDisplayLocale;

		public SurvivalObjectiveType ObjectiveType;

		public int KillsRequired;

		public int ThreatStart;

		public int ThreatFrequency;

		public int SpawnerCount;

		public int WalkersNormal;

		public int WalkersTank;

		public int WalkersArmored;

		public int WalkersExplosive;

		public int WalkersSpiked;

		public int WalkersGoo;

		public int WalkersMetalhead;

		public int WalkersFast;

		public int WalkersWhisperer;

		public int WalkersCommonWealth;

		public int ExplosiveBarrels;

		public string CustomVariables;

		public string Raiders;

		public string BossTypes;

		private int bossTypesMask;

		public string BurningTypes;

		private int burningWalkerTypesMask;

		private int burningRaiderTypesMask;

		public int InteractiveDuration = -1;

		public int SurviveDuration = -1;

		private int[] RaidersByType = new int[6];

		private int SurvivorPlayerAmount;

		public static bool IsCountedActorTag(int actorTag)
		{
			for (int i = 0; i < CountedTags.Length; i++)
			{
				if (actorTag == CountedTags[i])
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsSupportedWalkerType(WalkerType wtype)
		{
			for (int i = 0; i < SupportedWalkerTypes.Length; i++)
			{
				if (wtype == SupportedWalkerTypes[i])
				{
					return true;
				}
			}
			return false;
		}

		private bool TryParseTypeCount(string str, string typeName, ref int countVar)
		{
			if (str == typeName)
			{
				countVar++;
				return true;
			}
			if (str.Length < typeName.Length + 1)
			{
				return false;
			}
			if (str.StartsWith(typeName) && str[typeName.Length] == '(')
			{
				if (!str.EndsWith(")"))
				{
					return false;
				}
				int num = typeName.Length + 1;
				string s = str.Substring(num, str.Length - num - 1);
				int result = 0;
				bool result2 = int.TryParse(s, out result);
				countVar += result;
				return result2;
			}
			return false;
		}

		public void UpdateRaiderTypesCounts()
		{
			if (string.IsNullOrEmpty(Raiders))
			{
				for (int i = 0; i < 6; i++)
				{
					RaidersByType[i] = 0;
				}
				return;
			}
			string[] array = Raiders.Split(',');
			for (int j = 0; j < array.Length; j++)
			{
				string str = array[j].Trim();
				for (int k = 0; k < 6; k++)
				{
					SurvivorClass survivorClass = (SurvivorClass)k;
					TryParseTypeCount(str, survivorClass.ToString(), ref RaidersByType[k]);
				}
				if (MissionType == Type.GuildBattle)
				{
					TryParseTypeCount(str, SurvivorPlayerConst, ref SurvivorPlayerAmount);
				}
			}
		}

		private bool TryParseToWalkerTypeMask(string str, string walkerTypeString, WalkerType walkerType, ref int typeMaskToAddTo)
		{
			if (str == walkerTypeString)
			{
				typeMaskToAddTo |= 1 << (int)walkerType;
				return true;
			}
			return false;
		}

		private int ParseWalkerTypesMask(string valueString)
		{
			int typeMaskToAddTo = 0;
			string[] array = valueString.Split(',');
			for (int i = 0; i < array.Length; i++)
			{
				string str = array[i].Trim();
				for (int j = 0; j < SupportedWalkerTypes.Length; j++)
				{
					WalkerType walkerType = SupportedWalkerTypes[j];
					string text = walkerType.ToString();
					if (text.StartsWith("Walker"))
					{
						text = text.Substring("Walker".Length);
					}
					TryParseToWalkerTypeMask(str, text, walkerType, ref typeMaskToAddTo);
				}
			}
			return typeMaskToAddTo;
		}

		private bool TryParseToRaiderTypeMask(string str, string raiderTypeString, SurvivorClass raiderType, ref int typeMaskToAddTo)
		{
			if (str == raiderTypeString)
			{
				typeMaskToAddTo |= 1 << (int)raiderType;
				return true;
			}
			return false;
		}

		private int ParseRaiderTypesMask(string valueString)
		{
			int typeMaskToAddTo = 0;
			string[] array = valueString.Split(',');
			for (int i = 0; i < array.Length; i++)
			{
				string str = array[i].Trim();
				for (int j = 0; j < 6; j++)
				{
					SurvivorClass raiderType = (SurvivorClass)j;
					TryParseToRaiderTypeMask(str, raiderType.ToString(), raiderType, ref typeMaskToAddTo);
				}
			}
			return typeMaskToAddTo;
		}

		public void UpdateBossTypesMask()
		{
			if (string.IsNullOrEmpty(BossTypes))
			{
				bossTypesMask = 0;
			}
			else
			{
				bossTypesMask = ParseWalkerTypesMask(BossTypes);
			}
		}

		public void UpdateBurningTypesMask()
		{
			if (string.IsNullOrEmpty(BurningTypes))
			{
				burningWalkerTypesMask = 0;
				burningRaiderTypesMask = 0;
			}
			else
			{
				burningWalkerTypesMask = ParseWalkerTypesMask(BurningTypes);
				burningRaiderTypesMask = ParseRaiderTypesMask(BurningTypes);
			}
		}

		public int GetNumOpponentsOfType(WalkerType walkerType)
		{
			return GetNumWalkersOfType(walkerType);
		}

		public int GetNumOpponentsOfType(SurvivorClass raiderType)
		{
			return GetNumRaidersByType(raiderType);
		}

		public int GetNumWalkersOfType(WalkerType type)
		{
			switch (type)
			{
			case WalkerType.WalkerNormal:
				return WalkersNormal;
			case WalkerType.WalkerTank:
				return WalkersTank;
			case WalkerType.WalkerArmored:
				return WalkersArmored;
			case WalkerType.WalkerExplosive:
				return WalkersExplosive;
			case WalkerType.WalkerSpiked:
				return WalkersSpiked;
			case WalkerType.WalkerGoo:
				return WalkersGoo;
			case WalkerType.WalkerMetalhead:
				return WalkersMetalhead;
			case WalkerType.WalkerFast:
				return WalkersFast;
			case WalkerType.WalkerWhisperer:
			case WalkerType.WalkerWhispererMelee:
				return WalkersWhisperer;
			case WalkerType.ExplosiveBarrel:
				return ExplosiveBarrels;
			case WalkerType.WalkerCommonWealth:
				return WalkersCommonWealth;
			default:
				return 0;
			}
		}

		public bool HasWalker(WalkerType type)
		{
			return GetNumWalkersOfType(type) > 0;
		}

		public bool HasAnyWalker()
		{
			if (GetNumWalkersOfType(WalkerType.WalkerNormal) <= 0 && GetNumWalkersOfType(WalkerType.WalkerTank) <= 0 && GetNumWalkersOfType(WalkerType.WalkerArmored) <= 0 && GetNumWalkersOfType(WalkerType.WalkerExplosive) <= 0 && GetNumWalkersOfType(WalkerType.WalkerSpiked) <= 0 && GetNumWalkersOfType(WalkerType.WalkerGoo) <= 0 && GetNumWalkersOfType(WalkerType.WalkerMetalhead) <= 0 && GetNumWalkersOfType(WalkerType.WalkerFast) <= 0 && GetNumWalkersOfType(WalkerType.WalkerWhisperer) <= 0 && GetNumWalkersOfType(WalkerType.WalkerWhispererMelee) <= 0 && GetNumWalkersOfType(WalkerType.ExplosiveBarrel) <= 0)
			{
				return GetNumWalkersOfType(WalkerType.WalkerCommonWealth) > 0;
			}
			return true;
		}

		public bool IsWalkerTypeBoss(WalkerType type)
		{
			return (bossTypesMask & (1 << (int)type)) != 0;
		}

		public bool IsWalkerTypeBurning(WalkerType type)
		{
			return (burningWalkerTypesMask & (1 << (int)type)) != 0;
		}

		public bool IsRaiderTypeBurning(SurvivorClass type)
		{
			return (burningRaiderTypesMask & (1 << (int)type)) != 0;
		}

		public bool HasAnyRaiderType()
		{
			for (int i = 0; i < 6; i++)
			{
				if (RaidersByType[i] > 0)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasAnySurvivorPlayer()
		{
			if (SurvivorPlayerAmount > 0)
			{
				return true;
			}
			return false;
		}

		public bool HasBurningTypes()
		{
			return !string.IsNullOrEmpty(BurningTypes);
		}

		public int GetNumRaidersByType(SurvivorClass cls)
		{
			return RaidersByType[(int)cls];
		}

		public int GetNumSurvivorPlayers()
		{
			return SurvivorPlayerAmount;
		}

		public bool HasRaider(SurvivorClass cls)
		{
			return GetNumRaidersByType(cls) > 0;
		}
	}
}
