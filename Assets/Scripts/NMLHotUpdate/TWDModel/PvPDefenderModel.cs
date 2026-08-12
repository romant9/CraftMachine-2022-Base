using System.Collections.Generic;

namespace TWDModel
{
	public class PvPDefenderModel : TWDSpatialModelObject
	{
		public PvPDefenderSpawnState State;

		public AIMode DefensiveMode;

		public int DefenderIndex { get; set; }

		private SurvivorModel SpawnedDefenderModel { get; set; }

		private List<EquipmentItemModel> SpawnedItemModels { get; set; }

		public PvPDefenderModel()
		{
		}

		public PvPDefenderModel(string viewId)
		{
			base.ViewId = viewId;
		}

		public override void Start()
		{
			base.Start();
		}

		public void Spawn()
		{
			if (State != PvPDefenderSpawnState.Enabled)
			{
				return;
			}
			OutpostCombat outpostCombat = ((base.manager.Player.Combat == null) ? null : base.manager.Player.Combat.OutpostCombat);
			if (outpostCombat == null)
			{
				base.manager.Debug.LogError("OutpostCombat instance is NULL, cannot spawn defenders!");
				return;
			}
			SurvivorModel survivorModel = ((DefenderIndex >= 0 && DefenderIndex < outpostCombat.DefendingSurvivors.Count) ? outpostCombat.DefendingSurvivors[DefenderIndex] : null);
			if (survivorModel == null)
			{
				base.manager.Debug.LogError("Could not get defending survivor for DefenderIndex = " + DefenderIndex + "!");
				return;
			}
			if (survivorModel != null)
			{
				if (survivorModel.TimedActionModel != null && survivorModel.TimedActionModel.IsActionUnderway())
				{
					survivorModel.TimedActionModel.Paused = true;
				}
				survivorModel.PvPDefenderIndex = DefenderIndex;
				survivorModel.Faction = Faction.Raider;
				survivorModel.GridCoordinate = base.Location.Coordinate;
				CombatModel combatModel = base.manager.CombatModel;
				survivorModel.SetManager(base.manager);
				survivorModel.Start();
				survivorModel.SetupForCombat(combatModel);
				survivorModel.AIDataModel.Mode = DefensiveMode;
				if (DefensiveMode == AIMode.Aggressive)
				{
					survivorModel.AIDataModel.Alertness = AIAlertness.Homing;
				}
				else
				{
					survivorModel.AIDataModel.Alertness = AIAlertness.Idle;
				}
				combatModel.RegisterActor(survivorModel);
			}
			State = PvPDefenderSpawnState.Disabled;
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
