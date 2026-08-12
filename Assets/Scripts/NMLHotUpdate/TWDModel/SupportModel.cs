using System;
using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class SupportModel : TWDModelObject, IAttributeActorSystem, IAttributeBase
	{
		public const string Shiva = "Shiva";

		public const string Dog = "Dog";

		public const string WhisperersMask = "WhisperersMask";

		public const string CommonwealthArmor = "CommonwealthArmor";

		public const string RainbowCat = "RainbowCat";

		public const string Hwacha = "Hwacha";

		public const string CarolsCookies = "CarolsCookies";

		public const string WalkerMike = "WalkerMike";

		public const string Badge = "Badge";

		public const string Pasta = "Pasta";

		public const string Notebook = "Notebook";

		public const string Cap = "Cap";

		public const string SupportUpgraded = "SupportUpgraded";

		[JsonIgnore]
		public SupportDefinition definition;

		public string SupportId { get; set; }

		public int Level { get; set; }

		public int MissionsPlayedCount { get; set; }

		public bool InitializedTalent { get; set; }

		public ModelList<SupportTalentTreeModel> SupportTalentTreeModels { get; set; }

		public Dictionary<int, int> SlotAssembledTalentIds { get; set; }

		[JsonIgnore]
		public bool Unlocked => Level > 0;

		[JsonIgnore]
		public int Cooldown => definition.GetCooldown(Level);

		[JsonIgnore]
		public int ChallengeCooldown => definition.GetChallengeCooldown(Level);

		[JsonIgnore]
		public int DistanceCooldown => definition.GetDistanceCooldown(Level);

		[JsonIgnore]
		public int GVGCooldown => definition.GetGVGCooldown(Level);

		[JsonIgnore]
		public int InnerCooldown => definition.GetInnerCooldown(Level);

		[JsonIgnore]
		public int TokensToUpgrade => definition.GetTokensToUnlock(Level);

		[JsonIgnore]
		public bool CanUpgrade
		{
			get
			{
				if (Level >= MaxLevel)
				{
					return false;
				}
				if (base.manager == null)
				{
					return false;
				}
				if (definition == null)
				{
					return false;
				}
				Dictionary<CurrencyType, int> upgradCostInfo = definition.GetUpgradCostInfo(Level);
				if (upgradCostInfo == null || upgradCostInfo.Count == 0)
				{
					return false;
				}
				foreach (KeyValuePair<CurrencyType, int> item in upgradCostInfo)
				{
					if (base.manager.Player.GetCurrency(item.Key).Value < item.Value)
					{
						return false;
					}
				}
				return true;
			}
		}

		[JsonIgnore]
		public int ParameterCount => definition.ParameterCount;

		[JsonIgnore]
		public int[] SupportTalentTreeIds => definition.GetSupportTalentTreesByLevel(Level);

		[JsonIgnore]
		public int SupportTalentSlot => definition.GetSupportTalentSlotByLevel(Level);

		[JsonIgnore]
		public CurrencyType Currency => definition.Currency;

		[JsonIgnore]
		public int MaxLevel => definition.MaxLevel;

		public SupportTalentTreeModel GetSupportTalentTreeModelByID(int treeID)
		{
			if (SupportTalentTreeModels == null)
			{
				return null;
			}
			foreach (SupportTalentTreeModel supportTalentTreeModel in SupportTalentTreeModels)
			{
				if (supportTalentTreeModel.TreeId == treeID)
				{
					return supportTalentTreeModel;
				}
			}
			return null;
		}

		public SupportModel()
		{
		}

		public SupportModel(string id)
		{
			SupportId = id;
			Level = 0;
			MissionsPlayedCount = 0;
			SupportTalentTreeModels = new ModelList<SupportTalentTreeModel>();
			SlotAssembledTalentIds = new Dictionary<int, int>();
		}

		public override bool IsValid()
		{
			return true;
		}

		public override void SetManager(ModelManager manager)
		{
			base.SetManager(manager);
			definition = GameManager.Instance.gameEconomyData.GetSupportDefinition(SupportId);
			if (SupportTalentTreeModels != null && SupportTalentTreeModels.Manager == null)
			{
				SupportTalentTreeModels.SetManager(manager);
			}
		}

		public override void Start()
		{
			base.Start();
			if (SlotAssembledTalentIds == null)
			{
				SlotAssembledTalentIds = new Dictionary<int, int>();
			}
		}

		public FixedPoint GetParameter(int index)
		{
			return definition.GetParameter(Level, index);
		}

		public FixedPoint GetParameterNextLevel(int index)
		{
			int level = Math.Min(Level + 1, definition.MaxLevel);
			return definition.GetParameter(level, index);
		}

		public bool CheckCanUse(MapCategory category)
		{
			if (category == MapCategory.ApocalypticChallenge)
			{
				foreach (string item in base.manager.Player.ApocalypseWeeklyChallenge.CurrentCircleDefinition.Debuff)
				{
					if (item.Contains("Supportcooldown") && char.IsDigit(item[item.Length - 1]) && int.Parse(item[item.Length - 1].ToString()) == definition.Index)
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool Upgrade()
		{
			if (!CanUpgrade)
			{
				return false;
			}
			Dictionary<CurrencyType, int> upgradCostInfo = definition.GetUpgradCostInfo(Level);
			if (upgradCostInfo == null)
			{
				return false;
			}
			foreach (KeyValuePair<CurrencyType, int> item in upgradCostInfo)
			{
				if (base.manager != null)
				{
					base.manager.Player.GetCurrency(item.Key).Subtract(item.Value);
					SendUpgradeAnalyticsEvent(item.Value, item.Key);
				}
			}
			Level++;
			NotifyChange("SupportUpgraded", this);
			return true;
		}

		private void SendUpgradeAnalyticsEvent(int currencySpent, CurrencyType currency)
		{
			Metrics metrics = base.manager.Metrics.Reset().AddSpend().AddResources(currency, -currencySpent, -currencySpent);
			if (Level <= 1)
			{
				metrics.AddUnlock().AddSupportUnit(this);
			}
			else
			{
				metrics.AddUpgrade().AddSupportUnit(this).AddRarity();
			}
			metrics.Send();
		}

		public void InitializeTalentTrees()
		{
			SupportTalentTreeModels = new ModelList<SupportTalentTreeModel>();
			SupportTalentTreeModels.SetManager(base.manager);
			int[] supportTalentTreeIds = SupportTalentTreeIds;
			for (int i = 0; i < supportTalentTreeIds.Length; i++)
			{
				SupportTalentTreeModel supportTalentTreeModel = new SupportTalentTreeModel(supportTalentTreeIds[i], definition.Currency);
				supportTalentTreeModel.SetManager(base.manager);
				supportTalentTreeModel.Initialize();
				supportTalentTreeModel.Start();
				SupportTalentTreeModels.Add(supportTalentTreeModel);
			}
			InitializedTalent = true;
			UpdateModelObjects();
		}

		public FixedPoint GetHP()
		{
			FixedPoint result = 0.0;
			if (SupportTalentTreeModels == null)
			{
				return result;
			}
			foreach (SupportTalentTreeModel supportTalentTreeModel in SupportTalentTreeModels)
			{
				if (supportTalentTreeModel.Definition.UnlockLevel > Level)
				{
					continue;
				}
				if (supportTalentTreeModel.TrunkNodes == null || supportTalentTreeModel.TrunkNodes.Count == 0)
				{
					break;
				}
				foreach (SupportTalentNodeTrunkModel trunkNode in supportTalentTreeModel.TrunkNodes)
				{
					if (trunkNode.Level == 0)
					{
						continue;
					}
					SupportTalentDefinition currentTalentNodeDefinition = trunkNode.GetCurrentTalentNodeDefinition();
					if (currentTalentNodeDefinition.Type == SupportTalentType.Attribute && currentTalentNodeDefinition.TalentAttributeType == AttributeType.Hp)
					{
						result += (FixedPoint)currentTalentNodeDefinition.TalentAttributeValue;
					}
					if (trunkNode.SupportTalentNodeBranchModel != null && trunkNode.SupportTalentNodeBranchModel.Level > 0)
					{
						SupportTalentDefinition currentTalentNodeDefinition2 = trunkNode.SupportTalentNodeBranchModel.GetCurrentTalentNodeDefinition();
						if (currentTalentNodeDefinition2.Type == SupportTalentType.Attribute && currentTalentNodeDefinition2.TalentAttributeType == AttributeType.Hp)
						{
							result += (FixedPoint)currentTalentNodeDefinition2.TalentAttributeValue;
						}
					}
				}
			}
			return result;
		}

		public FixedPoint GetAttack()
		{
			FixedPoint result = 0.0;
			if (SupportTalentTreeModels == null)
			{
				return result;
			}
			foreach (SupportTalentTreeModel supportTalentTreeModel in SupportTalentTreeModels)
			{
				if (supportTalentTreeModel.Definition.UnlockLevel > Level)
				{
					continue;
				}
				if (supportTalentTreeModel.TrunkNodes == null || supportTalentTreeModel.TrunkNodes.Count == 0)
				{
					break;
				}
				foreach (SupportTalentNodeTrunkModel trunkNode in supportTalentTreeModel.TrunkNodes)
				{
					if (trunkNode.Level == 0)
					{
						continue;
					}
					SupportTalentDefinition currentTalentNodeDefinition = trunkNode.GetCurrentTalentNodeDefinition();
					if (currentTalentNodeDefinition.Type == SupportTalentType.Attribute && currentTalentNodeDefinition.TalentAttributeType == AttributeType.Attack)
					{
						result += (FixedPoint)currentTalentNodeDefinition.TalentAttributeValue;
					}
					if (trunkNode.SupportTalentNodeBranchModel != null && trunkNode.SupportTalentNodeBranchModel.Level > 0)
					{
						SupportTalentDefinition currentTalentNodeDefinition2 = trunkNode.SupportTalentNodeBranchModel.GetCurrentTalentNodeDefinition();
						if (currentTalentNodeDefinition2.Type == SupportTalentType.Attribute && currentTalentNodeDefinition2.TalentAttributeType == AttributeType.Attack)
						{
							result += (FixedPoint)currentTalentNodeDefinition2.TalentAttributeValue;
						}
					}
				}
			}
			return result;
		}

		public List<int> GetAvailableTraitsTalentIds()
		{
			List<int> list = new List<int>();
			if (SupportTalentTreeModels == null)
			{
				return list;
			}
			foreach (SupportTalentTreeModel supportTalentTreeModel in SupportTalentTreeModels)
			{
				if (supportTalentTreeModel.Definition.UnlockLevel > Level)
				{
					continue;
				}
				if (supportTalentTreeModel.TrunkNodes == null || supportTalentTreeModel.TrunkNodes.Count == 0)
				{
					break;
				}
				foreach (SupportTalentNodeTrunkModel trunkNode in supportTalentTreeModel.TrunkNodes)
				{
					if (trunkNode.Level == 0)
					{
						continue;
					}
					SupportTalentDefinition currentTalentNodeDefinition = trunkNode.GetCurrentTalentNodeDefinition();
					if (currentTalentNodeDefinition.Type == SupportTalentType.Trait)
					{
						list.Add(currentTalentNodeDefinition.Id);
					}
					if (trunkNode.SupportTalentNodeBranchModel != null && trunkNode.SupportTalentNodeBranchModel.Level > 0)
					{
						SupportTalentDefinition currentTalentNodeDefinition2 = trunkNode.SupportTalentNodeBranchModel.GetCurrentTalentNodeDefinition();
						if (currentTalentNodeDefinition2.Type == SupportTalentType.Trait)
						{
							list.Add(currentTalentNodeDefinition2.Id);
						}
					}
				}
			}
			return list;
		}

		[JsonIgnore]
		public int TokensSpent
		{
			get
			{
				int num = 0;
				for (int i = 0; i < Level; i++)
				{
					num += definition.GetTokensToUnlock(i);
				}
				return num;
			}
		}
	}
}
