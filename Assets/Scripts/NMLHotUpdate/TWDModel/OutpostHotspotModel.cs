using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class OutpostHotspotModel : TWDModelObjectWithViewId
	{
		public HotspotType Type { get; set; }

		public HotspotState State { get; set; }

		[IgnoreModelProperty]
		public PvPDefenderModel DefenderModel { get; set; }

		[IgnoreModelProperty]
		public ActorSpawnPointModel SpawnPointModel { get; set; }

		[IgnoreModelProperty]
		public List<TWDModelObject> FlagModels { get; set; }

		[IgnoreModelProperty]
		public List<TWDModelObject> ResourceContainerModels { get; set; }

		[JsonIgnore]
		public bool CanAssignDefender
		{
			get
			{
				if (Type != HotspotType.SingleActor)
				{
					return Type == HotspotType.Defender;
				}
				return true;
			}
		}

		[JsonIgnore]
		public bool CanAssignWalker
		{
			get
			{
				if (Type != HotspotType.SingleActor)
				{
					return Type == HotspotType.Walker;
				}
				return true;
			}
		}

		[JsonIgnore]
		public bool IsActorSpawn
		{
			get
			{
				if (Type != HotspotType.SingleActor && Type != HotspotType.Walker)
				{
					return Type == HotspotType.Defender;
				}
				return true;
			}
		}

		[JsonIgnore]
		public WalkerType WalkerType
		{
			get
			{
				if (SpawnPointModel is WalkerSpawnPointModel walkerSpawnPointModel)
				{
					return walkerSpawnPointModel.OverrideWalkerType;
				}
				return WalkerType.WalkerNormal;
			}
		}

		[JsonIgnore]
		public int SpawnCount
		{
			get
			{
				if (SpawnPointModel is WalkerSpawnPointModel walkerSpawnPointModel)
				{
					return walkerSpawnPointModel.TotalSpawnCount;
				}
				return 1;
			}
		}

		[JsonIgnore]
		public InteractiveObjectModel FlagInteractiveObjectModel
		{
			get
			{
				if (FlagModels != null)
				{
					for (int i = 0; i < FlagModels.Count; i++)
					{
						if (FlagModels[i] is InteractiveObjectModel result)
						{
							return result;
						}
					}
				}
				return null;
			}
		}

		[JsonIgnore]
		public InteractiveObjectModel ResourceContainerInteractiveObjectModel
		{
			get
			{
				if (FlagModels != null)
				{
					for (int i = 0; i < ResourceContainerModels.Count; i++)
					{
						if (ResourceContainerModels[i] is InteractiveObjectModel result)
						{
							return result;
						}
					}
				}
				return null;
			}
		}

		[JsonIgnore]
		public GridCoordinate Position
		{
			get
			{
				GridCoordinate invalid = GridCoordinate.Invalid;
				if (IsActorSpawn)
				{
					if (SpawnPointModel == null)
					{
						return GridCoordinate.Invalid;
					}
					return SpawnPointModel.Location.Coordinate;
				}
				if (Type == HotspotType.Goal)
				{
					InteractiveObjectModel interactiveObjectModel = FlagInteractiveObjectModel;
					if (interactiveObjectModel == null)
					{
						interactiveObjectModel = ResourceContainerInteractiveObjectModel;
					}
					if (interactiveObjectModel != null)
					{
						return interactiveObjectModel.Location.Coordinate;
					}
				}
				return invalid;
			}
		}

		public OutpostHotspotModel()
		{
			FlagModels = new List<TWDModelObject>();
			ResourceContainerModels = new List<TWDModelObject>();
		}

		public OutpostHotspotModel(string viewId)
			: this()
		{
			base.ViewId = viewId;
		}

		public void GetDimensions(out int width, out int height)
		{
			width = 1;
			height = 1;
			if (Type == HotspotType.MultiActor && SpawnPointModel != null)
			{
				width = SpawnPointModel.Location.Width;
				height = SpawnPointModel.Location.Height;
			}
		}

		public override void Initialize()
		{
			base.Initialize();
			if (DefenderModel != null)
			{
				DefenderModel.State = PvPDefenderSpawnState.Disabled;
			}
			if (SpawnPointModel != null)
			{
				SpawnPointModel.StopAndClose();
			}
			switch (State)
			{
			case HotspotState.Walker:
				SpawnPointModel.Reset();
				break;
			case HotspotState.DefenderSpawn_0:
			case HotspotState.DefenderSpawn_1:
			case HotspotState.DefenderSpawn_2:
				DefenderModel.State = PvPDefenderSpawnState.Enabled;
				break;
			case HotspotState.Flag:
			case HotspotState.ResourceContainer:
				break;
			}
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
