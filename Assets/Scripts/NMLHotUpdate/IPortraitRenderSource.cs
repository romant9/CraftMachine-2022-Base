using TWDModel;

public interface IPortraitRenderSource
{
	ActorGender Gender { get; }

	public string Prefab { get; set; }

	string ActorDefinitionId { get; }

	string OutfitDefinitionId { get; }

	string UniqueId { get; }


	#region mycode
	public void SetPrefab(string prefab)
	{
		Prefab = prefab;
	}

	public bool IsRebuild { get; set; }
	#endregion
}
