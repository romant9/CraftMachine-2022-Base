using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Newtonsoft.Json;
using TWDModel.ContentTypes;

namespace TWDModel
{
	public class EndlessModeCombatModel : TWDModelObject
	{
		[JsonIgnore]
		public List<EndlessModeSpawnDefinition> EndlessModeSpawnDefinitions;

		[JsonIgnore]
		public PlayerModel PlayerModel { get; set; }

		[JsonIgnore]
		public CombatModel CombatModel { get; set; }

		public int CurrentTurnCount { get; set; }

		public int CurrentWaveIndex { get; set; }

		public int CurrentRoundIndex { get; set; }

		public int CurrentDifficultyLevel { get; set; }

		public int CurrentWaveSurviveRewardPoints { get; set; }

		public int PreviousWaveSurviveRewardPoints { get; set; }

		public int CurrentWaveDuration { get; set; }

		public List<int> NextSpawnPointIndices { get; set; }

		public int CurrentScoreIncrementIndex { get; set; }

		public FixedPoint CurrentKillScoreMultiplier { get; set; }

		public bool KilledEnemyInTurn { get; set; }

		public List<WalkerModel> KilledWalkersInSurvivorTurn { get; set; }

		public FixedPoint MaxMultiplierReached { get; set; }

		public List<FixedPoint> SpawnPointChanceWeights { get; set; }

		public long CurrentScore { get; set; }

		public bool CombatResolved { get; set; }

		public bool DefeatedByOverrun { get; set; }

		[JsonIgnore]
		public EndlessModeManagerModel EndlessModeManager => base.manager.Player.EndlessModeManager;

		[JsonIgnore]
		public Dictionary<int, List<WalkerType>> GetCurrentSpawnWalkerTypes => base.manager.GameEconomyData.GetCurrentSpawnCompositions(CurrentSpawnDefinition(), CurrentRoundIndex);

		private int GetWalkerIncrementLevel => CurrentSpawnDefinition().LevelOffSet;

		[JsonIgnore]
		public int TurnsToWave => CurrentWaveDuration - CurrentTurnCount;

		[JsonIgnore]
		public int GetNextWaveSpawnCount => PlayerModel.gameEconomyData.GetCurrentSpawnCompositions(CurrentSpawnDefinition(), CurrentRoundIndex).Values.SelectMany((List<WalkerType> x) => x).ToList().Count;

		[JsonIgnore]
		public int GetCurrentOverAllWaveIndex => CurrentRoundIndex * EndlessModeSpawnDefinitions.Count + CurrentWaveIndex;

		private int DifferenceBetweenLowestHightestWalker
		{
			get
			{
				List<int> walkerLevels = CombatModel.WalkerLevels;
				if (walkerLevels == null || walkerLevels.Count <= 0)
				{
					return 0;
				}
				return CombatModel.WalkerLevels.Max() - CombatModel.WalkerLevels.Min();
			}
		}

		[JsonIgnore]
		public bool IsOverRunByWalkerLevelDifference
		{
			get
			{
				if (DifferenceBetweenLowestHightestWalker >= EndlessModeManager.EndlessModeConfig.MaxLevelDifferenceBetweenWalkers)
				{
					return EndlessModeManager.EndlessModeConfig.IsUsingMaxLevelDifference;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool CanReduceMultiplier
		{
			get
			{
				if (CombatModel.Walkers.Count > 0)
				{
					return !KilledEnemyInTurn;
				}
				return false;
			}
		}

		[JsonIgnore]
		public FixedPoint KillScoreMultiplierMin => EndlessModeManager.EndlessModeConfig.StartingScoreMultiplier;

		[JsonIgnore]
		public FixedPoint KillScoreMultiplierMax => EndlessModeManager.EndlessModeConfig.MaximumBaseScoreMultiplier;

		public override void Initialize()
		{
			base.Initialize();
			CurrentTurnCount = 0;
			CurrentWaveIndex = 0;
			CurrentDifficultyLevel = GetStartingDifficulty();
			CurrentWaveDuration = EndlessModeManager.EndlessModeConfig.StartingWaveTurnCount;
			CurrentKillScoreMultiplier = EndlessModeManager.EndlessModeConfig.StartingScoreMultiplier;
		}

		public override void Start()
		{
			base.Start();
			CombatModel = base.manager.CombatModel;
			PlayerModel = base.manager.Player;
			EndlessModeSpawnDefinitions = GetCurrentCycleSpawnDefinitions();
			if (KilledWalkersInSurvivorTurn == null)
			{
				KilledWalkersInSurvivorTurn = new List<WalkerModel>();
			}
			if (NextSpawnPointIndices == null)
			{
				SetupInitialSpawnIndices();
			}
		}

		public bool CanSpawnWave()
		{
			if (CurrentTurnCount >= CurrentWaveDuration)
			{
				if (CurrentWaveSurviveRewardPoints > 0)
				{
					CurrentScore += CurrentWaveSurviveRewardPoints;
				}
				SpawnWave();
				if (CurrentWaveIndex > EndlessModeSpawnDefinitions.Count - 1)
				{
					CurrentScoreIncrementIndex++;
					CurrentRoundIndex++;
					CurrentWaveIndex = 0;
				}
				CurrentTurnCount = 0;
				NextSpawnPointIndices = GenerateRandomSequence().Take(GetCurrentSpawnWalkerTypes.Count).ToList();
				return true;
			}
			return false;
		}

		private void SpawnWave()
		{
			Dictionary<int, List<WalkerType>> getCurrentSpawnWalkerTypes = GetCurrentSpawnWalkerTypes;
			int count = getCurrentSpawnWalkerTypes.Count;
			for (int i = 0; i < count; i++)
			{
				int index = NextSpawnPointIndices[i];
				WalkerSpawnPointModel randomSpawnPoint = GetRandomSpawnPoint(index);
				if (randomSpawnPoint != null)
				{
					List<WalkerType> list = getCurrentSpawnWalkerTypes[i];
					CurrentDifficultyLevel += GetWalkerIncrementLevel;
					randomSpawnPoint.OverrideWalkerTypes = list;
					randomSpawnPoint.SpawnCountPerAction = list.Count;
					randomSpawnPoint.OverrideWalkerLevel = CurrentDifficultyLevel;
					randomSpawnPoint.Activate(instant: true);
					randomSpawnPoint.State = SpawnPointState.Deactive;
				}
			}
			PreviousWaveSurviveRewardPoints = CurrentWaveSurviveRewardPoints;
			CurrentWaveSurviveRewardPoints = CurrentSpawnDefinition().WaveSurviveRewardPoints + GetWaveIncrementScore();
			CurrentWaveDuration = CurrentSpawnDefinition().WaveDuration;
			CurrentWaveIndex++;
		}

		private WalkerSpawnPointModel GetRandomSpawnPoint(int index)
		{
			return base.manager.CombatModel.OrderedSpawnPoints[index] as WalkerSpawnPointModel;
		}

		private int GetStartingDifficulty()
		{
			int result = 0;
			if (EndlessModeManager.EndlessModeGameModeType == EndlessModeGameModeType.Normal)
			{
				_ = EndlessModeManager.EndlessModeConfig.MaxLevelOffset;
				result = EndlessModeManager.EndlessModeConfig.NormalModeStartingLevel;
			}
			if (EndlessModeManager.EndlessModeGameModeType == EndlessModeGameModeType.Expert)
			{
				_ = EndlessModeManager.EndlessModeConfig.ExpertModeStartingOffset;
				result = EndlessModeManager.EndlessModeConfig.ExpertModeStartingLevel;
			}
			return result;
		}

		public void HandleKillScoreIncrease()
		{
			if (CombatModel.MissionCompleted || DefeatedByOverrun || CombatResolved)
			{
				return;
			}
			FixedPoint overallKillScore = 0L;
			FixedPoint fixedPoint = 0L;
			foreach (WalkerModel item in KilledWalkersInSurvivorTurn)
			{
				string iD = item.Definition.ID;
				fixedPoint += GetCurrentKillScoreMultiplier(iD);
				overallKillScore += CalculateWalkerKillScore(iD, item.Level);
			}
			CurrentKillScoreMultiplier = FixedPoint.Clamp(CurrentKillScoreMultiplier + fixedPoint, KillScoreMultiplierMin, KillScoreMultiplierMax);
			if (CurrentKillScoreMultiplier >= MaxMultiplierReached)
			{
				MaxMultiplierReached = CurrentKillScoreMultiplier;
			}
			CurrentScore += CalculateOverAllKillScore(overallKillScore);
			KilledWalkersInSurvivorTurn.Clear();
		}

		private FixedPoint CalculateWalkerKillScore(string walkerType, int level)
		{
			EndlessModeScoringDefinition endlessModeScoringDefinitionByWalkerType = base.manager.GameEconomyData.GetEndlessModeScoringDefinitionByWalkerType(walkerType);
			if (endlessModeScoringDefinitionByWalkerType != null)
			{
				return endlessModeScoringDefinitionByWalkerType.Score + level / endlessModeScoringDefinitionByWalkerType.EnemyConstant;
			}
			return 0L;
		}

		private int CalculateOverAllKillScore(FixedPoint overallKillScore)
		{
			return (int)FixedPoint.Ceiling(overallKillScore * CurrentKillScoreMultiplier);
		}

		public void KillLowLevelWalkers()
		{
			int num = CombatModel.WalkerLevels.Max();
			int maxLevelDifferenceBetweenWalkers = EndlessModeManager.EndlessModeConfig.MaxLevelDifferenceBetweenWalkers;
			for (int num2 = CombatModel.Walkers.Count - 1; num2 >= 0; num2--)
			{
				ActorModel actorModel = CombatModel.Walkers[num2];
				if (num - actorModel.Level >= maxLevelDifferenceBetweenWalkers)
				{
					actorModel.KilledByLevelDifference = true;
					actorModel.DealDamage(int.MaxValue, null, DamageType.Base);
				}
			}
		}

		private EndlessModeSpawnDefinition CurrentSpawnDefinition()
		{
			if (CurrentWaveIndex >= EndlessModeSpawnDefinitions.Count - 1)
			{
				return EndlessModeSpawnDefinitions.LastOrDefault();
			}
			return EndlessModeSpawnDefinitions[CurrentWaveIndex];
		}

		private List<EndlessModeSpawnDefinition> GetCurrentCycleSpawnDefinitions()
		{
			string text = ((EndlessModeManager.EndlessModeGameModeType != EndlessModeGameModeType.Expert) ? EndlessModeManager.CurrentEndlessModeSpawnName : EndlessModeManager.CurrentExpertEndlessModeSpawnName);
			List<EndlessModeSpawnDefinition> list = new List<EndlessModeSpawnDefinition>();
			EndlessModeSpawnDefinition[] endlessModelSpawnDefinitions = base.manager.GameEconomyData.EndlessModelSpawnDefinitions;
			foreach (EndlessModeSpawnDefinition endlessModeSpawnDefinition in endlessModelSpawnDefinitions)
			{
				if (endlessModeSpawnDefinition.SpawnSetupID == text)
				{
					list.Add(endlessModeSpawnDefinition);
				}
			}
			return list;
		}

		public FixedPoint GetCurrentKillScoreMultiplier(string walkerType)
		{
			return base.manager.GameEconomyData.GetEndlessModeScoringDefinitionByWalkerType(walkerType)?.MultiplierIncrease ?? ((FixedPoint)0L);
		}

		public void HandleReducingKillScoreMultiplier()
		{
			FixedPoint multiplierDecreaseRate = GetMultiplierDecreaseRate();
			CurrentKillScoreMultiplier = FixedPoint.Clamp(CurrentKillScoreMultiplier - multiplierDecreaseRate, KillScoreMultiplierMin, KillScoreMultiplierMax);
		}

		private FixedPoint GetMultiplierDecreaseRate()
		{
			int getCurrentOverAllWaveIndex = GetCurrentOverAllWaveIndex;
			return PlayerModel.gameEconomyData.TryGetEndlessModeMultiplierDecreaseRate(getCurrentOverAllWaveIndex);
		}

		private int GetWaveIncrementScore()
		{
			if (CurrentScoreIncrementIndex >= CurrentSpawnDefinition().GetWaveIncreamentCosts.Length - 1)
			{
				return CurrentSpawnDefinition().GetWaveIncreamentCosts.Last();
			}
			return CurrentSpawnDefinition().GetWaveIncreamentCosts[CurrentScoreIncrementIndex];
		}

		public void SetSurvivorsSurvivedWaveCount()
		{
			for (int i = 0; i < CombatModel.MissionRoster.Count; i++)
			{
				SurvivorModel survivorModel = CombatModel.MissionRoster[i];
				if (survivorModel != null && !survivorModel.IsDead)
				{
					survivorModel.SurvivedUntilWave = GetCurrentOverAllWaveIndex;
				}
			}
		}

		private void SetupInitialSpawnIndices()
		{
			NextSpawnPointIndices = new List<int>();
			int count = GetCurrentSpawnWalkerTypes.Count;
			NextSpawnPointIndices = GenerateRandomSequence().Take(count).ToList();
		}

		private List<int> GenerateRandomSequence()
		{
			List<int> list = Enumerable.Range(0, CombatModel.OrderedSpawnPoints.Count).ToList();
			List<int> list2 = new List<int>();
			for (int i = 0; i < CombatModel.OrderedSpawnPoints.Count; i++)
			{
				int randomElement = PlayerModel.PlayerRandom.GetRandomElement(list, remove: true);
				list2.Add(randomElement);
			}
			if (NextSpawnPointIndices.Count > 0 && list2.First() == NextSpawnPointIndices.First())
			{
				list2.Reverse();
			}
			return list2;
		}

		public override bool IsValid()
		{
			return true;
		}

		public override void SetManager(ModelManager manager)
		{
			base.SetManager(manager);
			if (KilledWalkersInSurvivorTurn == null)
			{
				return;
			}
			foreach (WalkerModel item in KilledWalkersInSurvivorTurn)
			{
				item.SetManager(manager);
			}
		}
	}
}
