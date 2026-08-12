using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class SurvivalSavedMissionModel : TWDModelObject
	{
		public bool DoesSavedMissionDataExist { get; set; }

		public List<SurvivalEnemyStateModel> SavedCurrentMissionEnemyStates { get; set; }

		public List<PersistentMissionVariable> PersistentVariables { get; set; }

		public void ClearSavedState()
		{
			DoesSavedMissionDataExist = false;
			SavedCurrentMissionEnemyStates.Clear();
			PersistentVariables.Clear();
		}

		public override void Initialize()
		{
			base.Initialize();
			SavedCurrentMissionEnemyStates = new List<SurvivalEnemyStateModel>();
			PersistentVariables = new List<PersistentMissionVariable>();
			ClearSavedState();
		}

		public void SetPersistentIntVariableValue(string variableName, int value, bool readonlyDuringCombat)
		{
			if (!DoesSavedMissionDataExist)
			{
				base.Debug.LogWarning("SetPersistentIntVariableValue called when DoesSavedMissionDataExist is not set.");
			}
			for (int i = 0; i < PersistentVariables.Count; i++)
			{
				if (PersistentVariables[i].Name == variableName)
				{
					PersistentVariables[i].ValueInt = value;
					if (PersistentVariables[i].ReadOnly != readonlyDuringCombat)
					{
						base.Debug.LogWarning("Persistent value read-only flag inconsistency between different lists.");
					}
				}
			}
			PersistentMissionVariable item = new PersistentMissionVariable(variableName, value, readonlyDuringCombat);
			PersistentVariables.Add(item);
		}

		public bool DoesPersistentIntVariableValueExist(string variableName)
		{
			if (!DoesSavedMissionDataExist)
			{
				base.Debug.LogError("DoesPersistentIntVariableValueExist called when no saved persistent variable data exists.");
				return false;
			}
			for (int i = 0; i < PersistentVariables.Count; i++)
			{
				if (PersistentVariables[i].Name == variableName)
				{
					return true;
				}
			}
			return false;
		}

		public int GetPersistentIntVariableValue(string variableName)
		{
			if (!DoesSavedMissionDataExist)
			{
				base.Debug.LogError("GetPersistentIntVariableValue called when no saved persistent variable data exists.");
				return 0;
			}
			for (int i = 0; i < PersistentVariables.Count; i++)
			{
				if (PersistentVariables[i].Name == variableName)
				{
					return PersistentVariables[i].ValueInt;
				}
			}
			base.Debug.LogError("GetPersistentIntVariableValue failed as persistent variable '" + variableName + "' does not exist.");
			return 0;
		}

		public string GetPersistentStringVariableValue(string variableName)
		{
			if (!DoesSavedMissionDataExist)
			{
				base.Debug.LogError("GetPersistentStringVariableValue called when no saved persistent variable data exists.");
				return "";
			}
			for (int i = 0; i < PersistentVariables.Count; i++)
			{
				if (PersistentVariables[i].Name == variableName)
				{
					return PersistentVariables[i].ValueString;
				}
			}
			base.Debug.LogError("GetPersistentStringVariableValue failed as persistent variable '" + variableName + "' does not exist.");
			return "";
		}

		private int GetMatchingSavedEnemyStateIndexForRaider(SurvivorClass requiredSurvivorClass, int requiredActorTag)
		{
			for (int i = 0; i < SavedCurrentMissionEnemyStates.Count; i++)
			{
				if (SavedCurrentMissionEnemyStates[i].MatchesRaiderSpawnRequirement(requiredSurvivorClass, requiredActorTag))
				{
					return i;
				}
			}
			return -1;
		}

		private int GetMatchingSavedEnemyStateIndexForWalker(WalkerType requiredWalkerType, int requiredActorTag)
		{
			for (int i = 0; i < SavedCurrentMissionEnemyStates.Count; i++)
			{
				if (SavedCurrentMissionEnemyStates[i].MatchesWalkerSpawnRequirement(requiredWalkerType, requiredActorTag))
				{
					return i;
				}
			}
			return -1;
		}

		public int ClampSpawnCountForRaider(int requestedSpawns, SurvivorClass survivorClass, int actorTag)
		{
			int matchingSavedEnemyStateIndexForRaider = GetMatchingSavedEnemyStateIndexForRaider(survivorClass, actorTag);
			if (matchingSavedEnemyStateIndexForRaider != -1)
			{
				return Math.Min(SavedCurrentMissionEnemyStates[matchingSavedEnemyStateIndexForRaider].Count, requestedSpawns);
			}
			return requestedSpawns;
		}

		public bool CanSpawnEnemyRaider(SurvivorClass survivorClass, int actorTag)
		{
			int matchingSavedEnemyStateIndexForRaider = GetMatchingSavedEnemyStateIndexForRaider(survivorClass, actorTag);
			if (matchingSavedEnemyStateIndexForRaider != -1)
			{
				return SavedCurrentMissionEnemyStates[matchingSavedEnemyStateIndexForRaider].Count > 0;
			}
			return true;
		}

		public void SaveCountForRaider(int raidersAlive, SurvivorClass survivorClass, int actorTag)
		{
			int matchingSavedEnemyStateIndexForRaider = GetMatchingSavedEnemyStateIndexForRaider(survivorClass, actorTag);
			if (matchingSavedEnemyStateIndexForRaider != -1)
			{
				SavedCurrentMissionEnemyStates[matchingSavedEnemyStateIndexForRaider].Count = raidersAlive;
				return;
			}
			SurvivalEnemyStateModel item = new SurvivalEnemyStateModel(survivorClass, actorTag, raidersAlive);
			SavedCurrentMissionEnemyStates.Add(item);
		}

		public bool IsCountSavedForWalker(WalkerType walkerType, int actorTag)
		{
			return GetMatchingSavedEnemyStateIndexForWalker(walkerType, actorTag) != -1;
		}

		public int ClampSpawnCountForWalker(int requestedSpawns, WalkerType walkerType, int actorTag)
		{
			int matchingSavedEnemyStateIndexForWalker = GetMatchingSavedEnemyStateIndexForWalker(walkerType, actorTag);
			if (matchingSavedEnemyStateIndexForWalker != -1)
			{
				return Math.Min(SavedCurrentMissionEnemyStates[matchingSavedEnemyStateIndexForWalker].Count, requestedSpawns);
			}
			return requestedSpawns;
		}

		public bool CanSpawnWalker(WalkerType walkerType, int actorTag)
		{
			int matchingSavedEnemyStateIndexForWalker = GetMatchingSavedEnemyStateIndexForWalker(walkerType, actorTag);
			if (matchingSavedEnemyStateIndexForWalker != -1)
			{
				return SavedCurrentMissionEnemyStates[matchingSavedEnemyStateIndexForWalker].Count > 0;
			}
			return true;
		}

		public void SaveCountForWalker(int walkersAlive, WalkerType walkerType, int actorTag)
		{
			int matchingSavedEnemyStateIndexForWalker = GetMatchingSavedEnemyStateIndexForWalker(walkerType, actorTag);
			if (matchingSavedEnemyStateIndexForWalker != -1)
			{
				SavedCurrentMissionEnemyStates[matchingSavedEnemyStateIndexForWalker].Count = walkersAlive;
				return;
			}
			SurvivalEnemyStateModel item = new SurvivalEnemyStateModel(walkerType, actorTag, walkersAlive);
			SavedCurrentMissionEnemyStates.Add(item);
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
