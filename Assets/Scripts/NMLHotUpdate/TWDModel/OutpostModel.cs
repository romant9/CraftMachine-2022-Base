using System;
using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class OutpostModel : TWDModelObject
	{
		public ModelList<OutpostAttackNotificationModel> OutpostAttackNotificationModels { get; set; }

		public ModelList<OutpostWalkerModel> WalkerModels { get; set; }

		public List<string> PurchasedBackgroundIds { get; set; }

		public RunLocationModel OutpostRunLocation { get; set; }

		public int PublishedLevelDataVersion { get; set; }

		public OutpostLevelModel StoredLevelModel { get; set; }

		public bool MatchMakingPaid { get; set; }

		[JsonIgnore]
		public OutpostLevelModel EditLevelModel { get; set; }

		[JsonIgnore]
		public int DeployedWalkerCount
		{
			get
			{
				if (StoredLevelModel != null)
				{
					int num = 0;
					if (CageEnabledWalkerModels == null)
					{
						UpdateCageEnabledWalkers();
					}
					for (int i = 0; i < CageEnabledWalkerModels.Count; i++)
					{
						OutpostWalkerModel outpostWalkerModel = CageEnabledWalkerModels[i];
						WalkerType walkerType = (WalkerType)Enum.Parse(typeof(WalkerType), outpostWalkerModel.ActorDefinition.ID);
						if (walkerType != WalkerType.WalkerNormal)
						{
							num += StoredLevelModel.GetTotalWalkersAssigned(walkerType);
						}
					}
					return num;
				}
				return 0;
			}
		}

		[JsonIgnore]
		public List<OutpostWalkerModel> CageEnabledWalkerModels { get; private set; }

		public void Reset()
		{
			OutpostRunLocation = null;
			StoredLevelModel = null;
			EditLevelModel = null;
		}

		public override void Start()
		{
			base.Start();
			bool flag = false;
			WalkerType[] array = (WalkerType[])Enum.GetValues(typeof(WalkerType));
			for (int i = 0; i < array.Length; i++)
			{
				WalkerType walkerType = array[i];
				string walkerId = walkerType.ToString();
				if (GetWalkerModel(walkerId) == null)
				{
					flag = true;
					AddWalkerModel(walkerId);
				}
			}
			if (flag)
			{
				UpdateModelObjects();
			}
			UpdateCageEnabledWalkers();
		}

		public void UpdateCageEnabledWalkers()
		{
			if (CageEnabledWalkerModels == null)
			{
				CageEnabledWalkerModels = new List<OutpostWalkerModel>();
			}
			else
			{
				CageEnabledWalkerModels.Clear();
			}
			for (int i = 0; i < WalkerModels.Count; i++)
			{
				if (IsCageEnabled(WalkerModels[i].Id))
				{
					CageEnabledWalkerModels.Add(WalkerModels[i]);
				}
			}
		}

		public void MigrationAddMissingWalkers(bool playerCreatedBeforeGEDChanges)
		{
			UpdateCageEnabledWalkers();
			for (int i = 0; i < ((CageEnabledWalkerModels != null) ? CageEnabledWalkerModels.Count : 0); i++)
			{
				CageEnabledWalkerModels[i].ApplyUpgradeTrackingMigration(playerCreatedBeforeGEDChanges);
			}
		}

		public bool IsBackgroundUnlocked(string outpostTemplateDefinitionId)
		{
			OutpostTemplateDefinition outpostTemplateDefinition = base.manager.Player.gameEconomyData.GetOutpostTemplateDefinition(outpostTemplateDefinitionId);
			if (outpostTemplateDefinition != null)
			{
				if (base.manager.Player.OutpostLevel >= outpostTemplateDefinition.OutpostLevelRequirement)
				{
					return true;
				}
				if (PurchasedBackgroundIds != null && PurchasedBackgroundIds.Contains(outpostTemplateDefinition.Id))
				{
					return true;
				}
			}
			return false;
		}

		public bool AddPurchasedBackground(string outpostTemplateDefinitionId)
		{
			if (base.manager.Player.gameEconomyData.GetOutpostTemplateDefinition(outpostTemplateDefinitionId) != null)
			{
				if (PurchasedBackgroundIds == null)
				{
					PurchasedBackgroundIds = new List<string>();
				}
				if (!PurchasedBackgroundIds.Contains(outpostTemplateDefinitionId))
				{
					PurchasedBackgroundIds.Add(outpostTemplateDefinitionId);
					return true;
				}
			}
			return false;
		}

		public override void Initialize()
		{
			WalkerModels = new ModelList<OutpostWalkerModel>();
			WalkerModels.SetManager(base.manager);
			WalkerModels.Initialize();
			OutpostAttackNotificationModels = new ModelList<OutpostAttackNotificationModel>();
			OutpostAttackNotificationModels.SetManager(base.manager);
			OutpostAttackNotificationModels.Initialize();
			OutpostAttackNotificationModel outpostAttackNotificationModel = new OutpostAttackNotificationModel();
			outpostAttackNotificationModel.PlayerName = "Rik";
			outpostAttackNotificationModel.Level = 99;
			outpostAttackNotificationModel.SetManager(base.manager);
			outpostAttackNotificationModel.Initialize();
			OutpostAttackNotificationModels.Add(outpostAttackNotificationModel);
			outpostAttackNotificationModel = new OutpostAttackNotificationModel();
			outpostAttackNotificationModel.PlayerName = "Carl";
			outpostAttackNotificationModel.Level = 98;
			outpostAttackNotificationModel.SetManager(base.manager);
			outpostAttackNotificationModel.Initialize();
			OutpostAttackNotificationModels.Add(outpostAttackNotificationModel);
		}

		public void AddWalkerModel(string walkerId)
		{
			OutpostWalkerModel outpostWalkerModel = new OutpostWalkerModel();
			outpostWalkerModel.SetManager(base.manager);
			outpostWalkerModel.Initialize();
			outpostWalkerModel.InitWalkerId(walkerId);
			outpostWalkerModel.Start();
			WalkerModels.Add(outpostWalkerModel);
		}

		public OutpostWalkerModel GetWalkerModel(string walkerId)
		{
			for (int i = 0; i < WalkerModels.Count; i++)
			{
				if (WalkerModels[i].Id == walkerId)
				{
					return WalkerModels[i];
				}
			}
			return null;
		}

		public int GetWalkerPower()
		{
			int num = 0;
			WalkerType[] array = (WalkerType[])Enum.GetValues(typeof(WalkerType));
			for (int i = 0; i < array.Length; i++)
			{
				OutpostWalkerModel walkerModel = GetWalkerModel(array[i].ToString());
				if (walkerModel != null)
				{
					num = ((array[i] != WalkerType.WalkerNormal) ? (num + walkerModel.Level * walkerModel.Amount) : (num + walkerModel.Level));
				}
			}
			return num;
		}

		public bool IsCageEnabled(string walkerId)
		{
			return base.manager.GameEconomyData.GetCageDefinition(walkerId, 0)?.Enabled ?? false;
		}

		public bool IsPlaceableEnabled(string walkerId)
		{
			return base.manager.GameEconomyData.GetCageDefinition(walkerId, 0)?.Placeable ?? false;
		}

		public override bool IsValid()
		{
			return true;
		}

		public bool InitializeEditModel()
		{
			IMessageSerializer messageSerializer = base.manager.GetMessageSerializer();
			string text = messageSerializer.SerializeObject(StoredLevelModel);
			if (text != null)
			{
				OutpostLevelModel outpostLevelModel = messageSerializer.DeserializeObject<OutpostLevelModel>(text);
				if (outpostLevelModel != null)
				{
					EditLevelModel = outpostLevelModel;
					EditLevelModel.SetManager(base.manager);
					return true;
				}
			}
			return false;
		}

		public void SaveEditModel()
		{
			StoredLevelModel = EditLevelModel;
			EditLevelModel = null;
		}

		public void DiscardEditModel()
		{
			EditLevelModel = null;
		}

		public Cashier GetNextMatchCashier()
		{
			return Cashier.CreateOneItemCashier(base.manager, PurchaseType.Attack, CurrencyType.Outpost, base.gameEconomyData.ConfigData.OutpostMatchMakingNextCost);
		}

		public Cashier GetRaidCashier()
		{
			int cost = base.gameEconomyData.ConfigData.OutpostRaidGasCost;
			if (base.manager.Player.IsTimedBonusActive(TimedBonusType.UnlimitedGas))
			{
				cost = 0;
			}
			return Cashier.CreateOneItemCashier(base.manager, PurchaseType.RechargeCurrency, CurrencyType.ReplayToken, cost);
		}

		public void SetupAnalytics(ref Dictionary<string, string> outDictionary)
		{
			if (outDictionary == null)
			{
				return;
			}
			if (StoredLevelModel != null && !outDictionary.ContainsKey("outpost_base_run_location") && !string.IsNullOrEmpty(StoredLevelModel.BaseRunLocationID))
			{
				outDictionary.Add("outpost_base_run_location", StoredLevelModel.BaseRunLocationID.ToString());
			}
			if (WalkerModels != null && !outDictionary.ContainsKey("outpost_total_walkers"))
			{
				outDictionary.Add("outpost_total_walkers", WalkerModels.Count.ToString());
			}
			if (!outDictionary.ContainsKey("outpost_deployed_walkers_count"))
			{
				outDictionary.Add("outpost_deployed_walkers_count", DeployedWalkerCount.ToString());
			}
			if (OutpostRunLocation != null && !outDictionary.ContainsKey("outpost_run_location_display_name") && !string.IsNullOrEmpty(OutpostRunLocation.DisplayName))
			{
				outDictionary.Add("outpost_run_location_display_name", OutpostRunLocation.DisplayName.ToString());
			}
			if (base.manager.Player != null && base.manager.Player.CurrentOutpostTier != null)
			{
				if (!outDictionary.ContainsKey("player_tier_id") && !string.IsNullOrEmpty(base.manager.Player.CurrentOutpostTier.Id))
				{
					outDictionary.Add("player_tier_id", base.manager.Player.CurrentOutpostTier.Id);
				}
				if (!outDictionary.ContainsKey("player_tier_set_id"))
				{
					outDictionary.Add("player_tier_set_id", base.manager.Player.CurrentOutpostTier.TierSetId.ToString());
				}
			}
		}
	}
}
