using System;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class ActorSpawnPointNode : NodeBase
	{
		[IgnoreModelProperty]
		public ActorSpawnPointModel ActorSpawnPoint { get; set; }

		[JsonIgnore]
		[GraphItExportData("Last Spawn Count", "")]
		public int LastSpawnCount { get; set; }

		[JsonIgnore]
		[GraphItImportData("Spawn Per Activation", "")]
		public int SpawnPerActivation
		{
			get
			{
				object obj = Import("Spawn Per Activation");
				if (obj != null)
				{
					return (int)obj;
				}
				return 0;
			}
		}

		public ActorSpawnPointNode()
		{
		}

		public ActorSpawnPointNode(ActorSpawnPointNode node)
			: base(node)
		{
			LastSpawnCount = node.LastSpawnCount;
			ActorSpawnPoint = node.ActorSpawnPoint;
		}

		public override NodeBase RecordValue()
		{
			return new ActorSpawnPointNode(this);
		}

		public override void Start()
		{
			base.Start();
			base.manager.RegisterDelayedEventListener(ActorSpawnPoint, OnSpawnPointChanged);
		}

		public override void ClearListener()
		{
			base.ClearListener();
			base.manager.UnregisterDelayedEventListener(ActorSpawnPoint, OnSpawnPointChanged);
		}

		private void OnSpawnPointChanged(ModelObject m, string changed, object args)
		{
			if (changed == "Spawned")
			{
				LastSpawnCount = (int)args;
				Spawned();
			}
		}

		[GraphItInput("Start Spawn", "")]
		public void StartSpawn()
		{
			if (ActorSpawnPoint != null)
			{
				if (!ActorSpawnPoint.CanActivate)
				{
					ActorSpawnPoint.Reset();
				}
				ActorSpawnPoint.OnTriggered(null);
			}
			Out();
		}

		[GraphItInput("Close Spawn", "")]
		public void CloseSpawn()
		{
			if (ActorSpawnPoint != null)
			{
				ActorSpawnPoint.StopAndClose();
			}
			Out();
		}

		[GraphItInput("Change Count", "")]
		public void ChangeCount()
		{
			int num = SpawnPerActivation;
			if (num < 0)
			{
				num = 0;
			}
			if (ActorSpawnPoint != null && num != ActorSpawnPoint.SpawnCountPerAction)
			{
				ActorSpawnPoint.SpawnCountPerAction = num;
			}
		}

		[GraphItOutput("Spawned", "")]
		public void Spawned()
		{
			Fire("Spawned");
		}

		[GraphItOutput("Out", "")]
		public void Out()
		{
			Fire("Out");
		}
	}
}
