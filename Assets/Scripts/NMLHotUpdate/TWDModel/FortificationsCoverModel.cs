using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class FortificationsCoverModel : TWDModelObjectWithViewId
	{
		[IgnoreModelProperty]
		public ActorModel Owner { get; private set; }

		public int OwnerModelId { get; private set; }

		public GridCoordinate GridCoordinate { get; private set; }

		public FacingDirection Facing { get; private set; }

		public FortificationsCoverModel()
		{
		}

		public FortificationsCoverModel(FortificationsCoverModel other)
		{
			Owner = other.Owner;
			OwnerModelId = other.OwnerModelId;
			GridCoordinate = other.GridCoordinate;
			Facing = other.Facing;
			base.ViewId = other.ViewId;
		}

		public FortificationsCoverModel(ActorModel owner, GridCoordinate gridCoordinate, FacingDirection facing)
		{
			Owner = owner;
			OwnerModelId = owner?.ModelId ?? 0;
			GridCoordinate = gridCoordinate;
			Facing = facing;
			base.ViewId = ((owner != null) ? $"FortificationsCover_{owner.ModelId}" : "FortificationsCover");
		}

		public static List<FortificationsCoverModel> FindByOwner(CombatModel combat, ActorModel owner)
		{
			List<FortificationsCoverModel> list = new List<FortificationsCoverModel>();
			if (combat == null || owner == null)
			{
				return list;
			}
			List<TWDModelObject> models = combat.GetModels<FortificationsCoverModel>();
			for (int i = 0; i < models.Count; i++)
			{
				if (models[i] is FortificationsCoverModel fortificationsCoverModel && (fortificationsCoverModel.Owner == owner || (fortificationsCoverModel.OwnerModelId != 0 && fortificationsCoverModel.OwnerModelId == owner.ModelId)))
				{
					list.Add(fortificationsCoverModel);
				}
			}
			return list;
		}

		public static void RemoveByOwner(CombatModel combat, ActorModel owner)
		{
			List<FortificationsCoverModel> list = FindByOwner(combat, owner);
			for (int i = 0; i < list.Count; i++)
			{
				combat.RemoveModel(list[i]);
			}
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
