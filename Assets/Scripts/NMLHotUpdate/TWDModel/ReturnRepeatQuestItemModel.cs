using Newtonsoft.Json;

namespace TWDModel
{
	public class ReturnRepeatQuestItemModel : TWDModelObject
	{
		public int DefinitionId { get; private set; }

		public int CurrentProgress { get; set; }

		[JsonIgnore]
		public ReturnRepeatQuestDefinition Definition => base.gameEconomyData?.GetReturnRepeatQuestDefinition(DefinitionId);

		public override bool IsValid()
		{
			return true;
		}

		public ReturnRepeatQuestItemModel(int definitionId)
		{
			DefinitionId = definitionId;
		}
	}
}
