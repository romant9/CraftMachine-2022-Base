using Newtonsoft.Json;

namespace TWDModel
{
	public class QuestModel : TWDModelObject
	{
		public string DefinitionID { get; set; }

		[JsonIgnore]
		public QuestDefinition QuestDefinition
		{
			get
			{
				if (base.manager == null)
				{
					return null;
				}
				return base.manager.GameEconomyData.GetQuestDefinition(DefinitionID);
			}
		}

		[JsonIgnore]
		public Rewards Rewards { get; set; }

		[JsonIgnore]
		public virtual int Steps => 1;

		[JsonIgnore]
		public virtual int CompletedSteps => 0;

		[JsonIgnore]
		public bool HasCompleted => CompletedSteps >= Steps;

		public override void Start()
		{
			base.Start();
			if (QuestDefinition != null)
			{
				Rewards = QuestDefinition.GetRewards();
			}
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
