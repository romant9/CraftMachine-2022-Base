using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class CheckPlayerModelCommand : ModelCommand
	{
		public int RandomCallCount { get; private set; }

		public int RandomState { get; private set; }

		public int RandomInitialSeed { get; private set; }

		public long ClientHashCode { get; private set; }

		public long ClientModelCount { get; private set; }

		public long CombatOccupancyHash { get; private set; }

		public string PreviousCommand { get; private set; }

		public CheckPlayerModelCommand()
		{
		}

		public CheckPlayerModelCommand(TWDModelManager modelManager, ModelCommand previousCommand)
		{
			if (modelManager.Mode == ModelManagerMode.Client)
			{
				ClientHashCode = modelManager.GetDebugModelsHashCode();
				ClientModelCount = modelManager.GetDebugModelsCount();
				PreviousCommand = previousCommand.ToString() + " " + modelManager.GetMessageSerializer().SerializeObject(previousCommand);
				if (modelManager.Player.Combat != null)
				{
					CombatOccupancyHash = CalculateCombatOccupancyHash(modelManager.Player.Combat);
				}
				RandomState = modelManager.Player.PlayerRandom.State;
				RandomCallCount = modelManager.Player.PlayerRandom.CallCount;
				RandomInitialSeed = modelManager.Player.PlayerRandom.InitialSeed;
			}
		}

		private long CalculateCombatOccupancyHash(CombatModel combatModel)
		{
			long num = 0L;
			if (combatModel.Occupiers != null)
			{
				GridField<ActorModel> occupiers = combatModel.Occupiers;
				for (int i = 0; i < occupiers.Length; i++)
				{
					if (occupiers[i] != null)
					{
						num = num * 314159 + i;
					}
				}
			}
			return num;
		}

		private string GetModelList(TWDModelManager manager)
		{
			string text = "";
			List<string> modelListReport = manager.GetModelListReport();
			for (int i = 0; i < modelListReport.Count; i++)
			{
				text = text + modelListReport[i] + " : \n";
			}
			return text;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (tWDModelManager.Mode == ModelManagerMode.Server)
			{
				if (RandomState != tWDModelManager.Player.PlayerRandom.State || RandomCallCount != tWDModelManager.Player.PlayerRandom.CallCount || RandomInitialSeed != tWDModelManager.Player.PlayerRandom.InitialSeed)
				{
					tWDModelManager.Debug.LogError("Desync error: Client / Server random out of sync! Last command:" + PreviousCommand + " Hotfixed: " + tWDModelManager.Player.ModelHotfixWasApplied + " State: " + RandomState + "/" + tWDModelManager.Player.PlayerRandom.State + " Call count: " + RandomCallCount + " / " + tWDModelManager.Player.PlayerRandom.CallCount + " Initial seed: " + RandomInitialSeed + "/" + tWDModelManager.Player.PlayerRandom.InitialSeed + " Last visit:" + tWDModelManager.Player.LastVisitDebugInfo);
					return new NGModelCommandRespond(this, TWDModelResult.PlayerRandomMismatch);
				}
				if (ClientHashCode != tWDModelManager.GetDebugModelsHashCode())
				{
					tWDModelManager.Debug.LogError("Desync error: Client / Server model list out of sync! Last command:" + PreviousCommand + " Hotfixed: " + tWDModelManager.Player.ModelHotfixWasApplied + " Model count: " + ClientModelCount + "/" + tWDModelManager.GetDebugModelsCount() + " Model hash:" + ClientHashCode + "/" + tWDModelManager.GetDebugModelsHashCode() + " Last visit:" + tWDModelManager.Player.LastVisitDebugInfo);
					tWDModelManager.Debug.Log("Server model list: " + GetModelList(tWDModelManager));
					return new NGModelCommandRespond(this, TWDModelResult.ModelListMismatch);
				}
				if (tWDModelManager.Player.Combat != null && CombatOccupancyHash != CalculateCombatOccupancyHash(tWDModelManager.Player.Combat))
				{
					tWDModelManager.Debug.LogError("Desync error: Client / Server combat occupancy map out of sync! Last command:" + PreviousCommand + " Hotfixed: " + tWDModelManager.Player.ModelHotfixWasApplied + " Occupancy hash: " + CombatOccupancyHash + "/" + CalculateCombatOccupancyHash(tWDModelManager.Player.Combat) + " Last visit:" + tWDModelManager.Player.LastVisitDebugInfo);
					return new NGModelCommandRespond(this, TWDModelResult.CombatOccupancyMismatch);
				}
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
