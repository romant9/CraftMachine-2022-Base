using System;
using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class SurvivorNode : NodeBase
	{
		[JsonIgnore]
		[GraphItExportData("Survivors", "")]
		public List<ActorModel> Survivors => base.manager.Player.Combat?.GetFactionActors(Faction.Survivor);

		[JsonIgnore]
		[GraphItExportData("Last Instigator", "")]
		public ActorModel LastInstigator { get; set; }

		[JsonIgnore]
		[GraphItExportData("Alive Count", "")]
		public int AliveCount
		{
			get
			{
				int num = 0;
				for (int i = 0; i < Survivors.Count; i++)
				{
					if (Survivors[i] != null && !Survivors[i].IsDead && Survivors[i].Faction == Faction.Survivor)
					{
						num++;
					}
				}
				return num;
			}
		}

		[JsonIgnore]
		[GraphItExportData("MinSurvivorLevel", "Lowest survivor level")]
		public int MinSurvivorLevel
		{
			get
			{
				int num = int.MaxValue;
				for (int i = 0; i < Survivors.Count; i++)
				{
					num = Math.Min(Survivors[i].Level, num);
				}
				if (num < int.MaxValue)
				{
					return num;
				}
				return 1;
			}
		}

		[JsonIgnore]
		[GraphItExportData("MaxSurvivorLevel", "Highest survivor level")]
		public int MaxSurvivorLevel
		{
			get
			{
				int num = int.MinValue;
				for (int i = 0; i < Survivors.Count; i++)
				{
					num = Math.Max(Survivors[i].Level, num);
				}
				if (num > int.MinValue)
				{
					return num;
				}
				return 1;
			}
		}

		public SurvivorNode()
		{
		}

		public SurvivorNode(SurvivorNode node)
			: base(node)
		{
			LastInstigator = node.LastInstigator;
		}

		public override NodeBase RecordValue()
		{
			return new SurvivorNode(this);
		}

		public override void Start()
		{
			base.Start();
			List<ActorModel> survivors = Survivors;
			for (int i = 0; i < survivors.Count; i++)
			{
				base.manager.RegisterDelayedEventListener(survivors[i], OnActorModelChanged);
			}
			base.manager.RegisterDelayedEventListener(base.manager.Player.Combat, OnCombatModelChanged);
		}

		public override void ClearListener()
		{
			base.ClearListener();
			List<ActorModel> survivors = Survivors;
			for (int i = 0; i < survivors.Count; i++)
			{
				base.manager.UnregisterDelayedEventListener(survivors[i], OnActorModelChanged);
			}
			base.manager.UnregisterDelayedEventListener(base.manager.Player.Combat, OnCombatModelChanged);
		}

		private void OnActorModelChanged(ModelObject m, string changed, object args)
		{
			ActorModel actorModel = m as ActorModel;
			if (actorModel.Faction == Faction.Survivor)
			{
				LastInstigator = actorModel;
				if (LastInstigator != null && changed == "actorAbilityCompleted")
				{
					AbilityCompleted();
				}
			}
		}

		private void OnCombatModelChanged(ModelObject m, string changed, object args)
		{
			if (changed == "actorCreated")
			{
				ActorModel actorModel = args as ActorModel;
				if (actorModel.Faction == Faction.Survivor)
				{
					base.manager.RegisterDelayedEventListener(actorModel, OnActorModelChanged);
				}
			}
		}

		[GraphItOutput("Ability Completed", "")]
		public void AbilityCompleted()
		{
			Fire("AbilityCompleted");
		}
	}
}
