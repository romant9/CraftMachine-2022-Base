using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class EndlessModeCombatModelBackup : TWDModelObject
	{
		[IgnoreModelProperty]
		public EndlessModeCombatModel Model { get; set; }

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

		public override bool IsValid()
		{
			return true;
		}

		public override void Start()
		{
			base.Start();
		}

		public void RecordStatus(EndlessModeCombatModel model)
		{
			Model = model;
			CurrentTurnCount = model.CurrentTurnCount;
			CurrentWaveIndex = model.CurrentWaveIndex;
			CurrentRoundIndex = model.CurrentRoundIndex;
			CurrentDifficultyLevel = model.CurrentDifficultyLevel;
			CurrentWaveSurviveRewardPoints = model.CurrentWaveSurviveRewardPoints;
			PreviousWaveSurviveRewardPoints = model.PreviousWaveSurviveRewardPoints;
			CurrentWaveDuration = model.CurrentWaveDuration;
			NextSpawnPointIndices = ((model.NextSpawnPointIndices == null) ? null : new List<int>(model.NextSpawnPointIndices));
			CurrentScoreIncrementIndex = model.CurrentScoreIncrementIndex;
			CurrentKillScoreMultiplier = model.CurrentKillScoreMultiplier;
			KilledEnemyInTurn = model.KilledEnemyInTurn;
			MaxMultiplierReached = model.MaxMultiplierReached;
			SpawnPointChanceWeights = ((model.SpawnPointChanceWeights == null) ? null : new List<FixedPoint>(model.SpawnPointChanceWeights));
			CurrentScore = model.CurrentScore;
			CombatResolved = model.CombatResolved;
			DefeatedByOverrun = model.DefeatedByOverrun;
			KilledWalkersInSurvivorTurn = ((model.KilledWalkersInSurvivorTurn == null) ? null : new List<WalkerModel>(model.KilledWalkersInSurvivorTurn));
		}

		public void BackUp()
		{
			Model.CurrentTurnCount = CurrentTurnCount;
			Model.CurrentWaveIndex = CurrentWaveIndex;
			Model.CurrentRoundIndex = CurrentRoundIndex;
			Model.CurrentDifficultyLevel = CurrentDifficultyLevel;
			Model.CurrentWaveSurviveRewardPoints = CurrentWaveSurviveRewardPoints;
			Model.PreviousWaveSurviveRewardPoints = PreviousWaveSurviveRewardPoints;
			Model.CurrentWaveDuration = CurrentWaveDuration;
			Model.NextSpawnPointIndices = ((NextSpawnPointIndices == null) ? null : new List<int>(NextSpawnPointIndices));
			Model.CurrentScoreIncrementIndex = CurrentScoreIncrementIndex;
			Model.CurrentKillScoreMultiplier = CurrentKillScoreMultiplier;
			Model.KilledEnemyInTurn = KilledEnemyInTurn;
			Model.MaxMultiplierReached = MaxMultiplierReached;
			Model.SpawnPointChanceWeights = ((SpawnPointChanceWeights == null) ? null : new List<FixedPoint>(SpawnPointChanceWeights));
			Model.CurrentScore = CurrentScore;
			Model.CombatResolved = CombatResolved;
			Model.DefeatedByOverrun = DefeatedByOverrun;
			Model.KilledWalkersInSurvivorTurn = ((KilledWalkersInSurvivorTurn == null) ? null : new List<WalkerModel>(KilledWalkersInSurvivorTurn));
		}
	}
}
