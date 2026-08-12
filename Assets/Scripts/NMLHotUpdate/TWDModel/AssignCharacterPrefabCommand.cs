using BaseModel;

namespace TWDModel
{
	public class AssignCharacterPrefabCommand : ModelCommand
	{
		public string PrefabName { get; set; }

		public string OutfitDefinitionID { get; set; }

		public AssignCharacterPrefabCommand()
		{
		}

		public AssignCharacterPrefabCommand(ActorModel actor, string prefabName, string outfitDefinitionID)
			: base(actor)
		{
			PrefabName = prefabName;
			OutfitDefinitionID = outfitDefinitionID;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			ActorModel model = manager.GetModel<ActorModel>(base.ModelId);
			if (model == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (PrefabName == null && OutfitDefinitionID == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (PrefabName != null && PrefabName.Length > 0)
			{
				model.CharacterPrefab = PrefabName;
			}
			if (OutfitDefinitionID != null && tWDModelManager.Player.gameEconomyData.ConfigData.BetaFlag_Outfits && tWDModelManager.Player.gameEconomyData.GetOutfitDefinition(OutfitDefinitionID) != null && tWDModelManager.Player.SurvivorContainer.HasOutfit(OutfitDefinitionID))
			{
				model.OutfitDefinitionID = OutfitDefinitionID;
			}
			else if (OutfitDefinitionID == null)
			{
				model.OutfitDefinitionID = null;
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
