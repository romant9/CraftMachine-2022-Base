using Newtonsoft.Json;

namespace TWDModel
{
	public class ReturnDailyQuestItemModel : TWDModelObject
	{
		public int DefinitionId { get; private set; }

		public int CurrentProgress { get; set; }

		public bool Claimed { get; set; }

		[JsonIgnore]
		public ReturnDailyQuestDefinition Definition => base.gameEconomyData?.GetReturnDailyQuestDefinition(DefinitionId);

		public override bool IsValid()
		{
			return true;
		}

		public ReturnDailyQuestItemModel(int definitionId)
		{
			DefinitionId = definitionId;
		}
	}
}
