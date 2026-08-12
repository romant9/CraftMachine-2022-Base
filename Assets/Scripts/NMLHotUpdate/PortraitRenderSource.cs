using TWDModel;

public class PortraitRenderSource : IPortraitRenderSource
{
	public ActorGender Gender { get; protected set; }

	public string Prefab { get; set; }

	public string ActorDefinitionId { get; protected set; }

	public string OutfitDefinitionId { get; protected set; }

	public string UniqueId { get; protected set; }

	private PortraitRenderSource()
	{
	}

	public static PortraitRenderSource fromActorModel(ActorModel actorModel)
	{
		return new PortraitRenderSource
		{
			Gender = actorModel.Gender,
			Prefab = actorModel.CharacterPrefab,
			ActorDefinitionId = actorModel.ActorDefinitionID,
			OutfitDefinitionId = actorModel.OutfitDefinitionID,
			UniqueId = actorModel.ModelId.ToString()
		};
	}

	public static PortraitRenderSource fromActorDefinition(ActorDefinition actorDefinition)
	{
		return new PortraitRenderSource
		{
			Gender = actorDefinition.Gender,
			Prefab = actorDefinition.VisualAsset,
			ActorDefinitionId = actorDefinition.ID,
			OutfitDefinitionId = actorDefinition.OutfitDefinitionID
		};
	}

	public override bool Equals(object value)
	{
		if (!(value is PortraitRenderSource portraitRenderSource))
		{
			return false;
		}
		if (Gender == portraitRenderSource.Gender && Prefab == portraitRenderSource.Prefab && ActorDefinitionId == portraitRenderSource.ActorDefinitionId && OutfitDefinitionId == portraitRenderSource.OutfitDefinitionId)
		{
			return UniqueId == portraitRenderSource.UniqueId;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((((13 * 7 + Gender.GetHashCode()) * 7 + ((Prefab != null) ? Prefab.GetHashCode() : 0)) * 7 + ((ActorDefinitionId != null) ? ActorDefinitionId.GetHashCode() : 0)) * 7 + ((OutfitDefinitionId != null) ? OutfitDefinitionId.GetHashCode() : 0)) * 7 + ((UniqueId != null) ? UniqueId.GetHashCode() : 0);
	}

	public override string ToString()
	{
		return "PortraitRenderSource: " + Gender.ToString() + " " + Prefab + " " + ActorDefinitionId + " " + OutfitDefinitionId + " " + UniqueId;
	}



	#region myparams
	public bool IsRebuild { get; set; }
	#endregion
}
