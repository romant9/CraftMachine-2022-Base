using System;
using Newtonsoft.Json;

namespace TWDModel
{
	public class StoryTellerModel : SurvivorModel
	{
		public const string QuestAccepted = "QuestAccepted";

		public const string QuestCompleted = "QuestCompleted";

		public const string CanAcceptQuestEvent = "CanAcceptQuest";

		public const string StoryTellerChangedEvent = "StoryTellerChanged";

		public int StoryTellerId { get; private set; }

		public QuestModel CurrentQuest { get; private set; }

		public int QuestIndex { get; private set; }

		public long LastQuestCompleted { get; private set; }

		[JsonIgnore]
		public QuestDefinition CurrentQuestDefinition => base.manager.GameEconomyData.GetQuestDefinition(QuestIndex, StoryTellerId);

		[JsonIgnore]
		public bool FirstQuestAccepted
		{
			get
			{
				if (CurrentQuest == null)
				{
					return QuestIndex > 0;
				}
				return true;
			}
		}

		[JsonIgnore]
		public bool CanCompleteQuest
		{
			get
			{
				if (CurrentQuest != null)
				{
					return CurrentQuest.HasCompleted;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool CanAcceptQuest
		{
			get
			{
				if (CurrentQuestDefinition == null || CurrentQuest != null)
				{
					return false;
				}
				return true;
			}
		}

		public StoryTellerModel()
		{
		}

		public StoryTellerModel(int id, int startingLevel, int rarityLevel)
			: base(startingLevel, rarityLevel)
		{
			StoryTellerId = id;
		}

		public override void Initialize()
		{
			base.Initialize();
			LastQuestCompleted = 0L;
			QuestIndex = -1;
			CurrentQuest = null;
			FindNewQuest();
		}

		public override void Start()
		{
			base.Start();
		}

		private void FindNewQuest()
		{
			do
			{
				QuestIndex++;
			}
			while (CurrentQuestDefinition != null && CurrentQuestDefinition.Giver != StoryTellerId);
		}

		public QuestDefinition GetCurrentUncompletedQuestDefinition()
		{
			if (CurrentQuestDefinition != null && !CanCompleteQuest)
			{
				return CurrentQuestDefinition;
			}
			QuestDefinition[] questDefinitions = base.manager.GameEconomyData.QuestDefinitions;
			QuestDefinition result = null;
			if (questDefinitions != null)
			{
				for (int i = 0; i < questDefinitions.Length; i++)
				{
					if (questDefinitions[i] != null && questDefinitions[i].Giver == StoryTellerId)
					{
						result = questDefinitions[i];
						if (questDefinitions[i].Order > QuestIndex)
						{
							return questDefinitions[i];
						}
					}
				}
			}
			return result;
		}

		public bool ClaimQuestCompleted()
		{
			if (CanCompleteQuest)
			{
				bool flag = false;
				if (base.ActorDefinitionID == "StoryTeller_1")
				{
					flag = true;
					base.ActorDefinitionID = "StoryTeller_Angie";
				}
				foreach (IReward rewards in CurrentQuest.Rewards.RewardsList)
				{
					object obj = null;
					obj = ((rewards.Type != RewardType.Equipment && rewards.Type != RewardType.RandomEquipment) ? rewards.Give(base.manager) : rewards.Give(base.manager, new object[1] { base.manager.Player.PlayerRandom }));
					if (obj is EquipmentItemModel)
					{
						base.manager.Metrics.AddFind().AddEquipment(obj as EquipmentItemModel, "Equipment", (rewards as RewardEquipment)?.Amount ?? 1).AddEpisode(CurrentQuest.QuestDefinition.GetUnlockedEpisode(base.manager))
							.Send();
					}
					else if (rewards.Type == RewardType.Currency && rewards is RewardCurrency)
					{
						RewardCurrency rewardCurrency = (RewardCurrency)rewards;
						base.manager.Metrics.AddFind().AddResources(rewardCurrency.CurrencyType, rewardCurrency.Amount, rewardCurrency.AmountActuallyAdded).AddEpisode(CurrentQuest.QuestDefinition.GetUnlockedEpisode(base.manager))
							.Send();
					}
				}
				LastQuestCompleted = base.manager.Time;
				NotifyChange("QuestCompleted", CurrentQuest);
				CurrentQuest = null;
				FindNewQuest();
				if (flag)
				{
					NotifyChange("StoryTellerChanged");
				}
				else
				{
					NotifyChange("CanAcceptQuest");
				}
				return true;
			}
			return false;
		}

		public bool AcceptQuest()
		{
			Type type = ReflectionUtils.FindDerivedTypeStartingWith(typeof(QuestModel), CurrentQuestDefinition.ClassName);
			if (type != null)
			{
				CurrentQuest = ReflectionUtils.Instantiate(type, CurrentQuestDefinition.ConstructionParameters) as QuestModel;
				CurrentQuest.SetManager(base.manager);
				CurrentQuest.DefinitionID = CurrentQuestDefinition.Identifier;
				CurrentQuest.Initialize();
				CurrentQuest.Start();
			}
			else
			{
				base.Debug.LogError("Cannot instantiate quest " + CurrentQuestDefinition.ClassName);
			}
			if (CurrentQuest != null)
			{
				NotifyChange("QuestAccepted");
			}
			return CurrentQuest != null;
		}
	}
}
