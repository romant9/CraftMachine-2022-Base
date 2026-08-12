using System;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class EquipTokenItemModel : TWDModelObject, IComparable<EquipTokenItemModel>
	{
		public const int MaxEquipmentRarity = 6;

		private DateTime _lastUpdateTime;

		[JsonIgnore]
		private bool _isEquipmentValid;

		[JsonIgnore]
		private EquipmentDefinition _equipmentDefinition;

		[JsonIgnore]
		private EquipTokenDefinition _equipTokenDefinition;

		[JsonIgnore]
		private RewardEquipment _rewardEquipment;

		[JsonIgnore]
		private EquipmentItemModel _equipmentItem;

		public string EquipTokenId { get; private set; }

		public int OwnedTokensAmount { get; private set; }

		[JsonIgnore]
		public EquipmentDefinition EquipmentDefinition => _equipmentDefinition;

		[JsonIgnore]
		public EquipTokenDefinition Definition => _equipTokenDefinition;

		[JsonIgnore]
		public EquipmentItemModel EquipmentItemModel => _equipmentItem;

		[JsonIgnore]
		public RewardEquipment RewardEquipment => _rewardEquipment;

		public override bool IsValid()
		{
			return true;
		}

		public override void Start()
		{
			base.Start();
			init();
		}

		public override void Initialize()
		{
			base.Initialize();
			init();
		}

		private void init()
		{
			if (!_isEquipmentValid)
			{
				_equipTokenDefinition = base.manager.GameEconomyData.GetEquipTokenDefinition(EquipTokenId);
				_equipmentDefinition = base.manager.GameEconomyData.GetEquipmentDefinition(_equipTokenDefinition.RelateEquipId);
				int equipmentStartingLevel = base.manager.Player.LootManager.GetEquipmentStartingLevel(0, _equipmentDefinition.SurvivorClass);
				_equipmentItem = base.manager.Player.Equipment.GenerateAndInitializeEquipmentFromDefinition(_equipTokenDefinition.RelateEquipId, Definition.Star, equipmentStartingLevel, new ModelRandom(0), startModel: false);
				RewardEquipment rewardEquipment = new RewardEquipment();
				rewardEquipment.EquipmentId = _equipTokenDefinition.RelateEquipId;
				rewardEquipment.RarityLevel = Definition.Star;
				rewardEquipment.StartingLevel = 0;
				rewardEquipment.StartingLevelOffset = 0;
				rewardEquipment.EquipmentSource = EquipmentSource.EquipTokenUnlock;
				_rewardEquipment = rewardEquipment;
			}
			_isEquipmentValid = true;
		}

		public EquipTokenItemModel(string equipTokenId, int ownedTokensAmount)
		{
			EquipTokenId = equipTokenId;
			OwnedTokensAmount = ownedTokensAmount;
		}

		public bool CanUnlock()
		{
			if (OwnedTokensAmount >= Definition.TokensToUnlock)
			{
				return base.manager.Player.GetCurrency(CurrencyType.ApocalypticEquipToken).Value >= Definition.ApocalypticEquipToken;
			}
			return false;
		}

		public int CompareTo(EquipTokenItemModel otherEquipTokenItemModel)
		{
			if (CanUnlock() == otherEquipTokenItemModel.CanUnlock())
			{
				return Definition.Sort.CompareTo(otherEquipTokenItemModel.Definition.Sort);
			}
			if (CanUnlock())
			{
				return 1;
			}
			return -1;
		}

		public bool UnlockEquip()
		{
			EquipTokenDefinition definition = base.manager.GameEconomyData.GetEquipTokenDefinition(EquipTokenId);
			if (OwnedTokensAmount < definition.TokensToUnlock)
			{
				return false;
			}
			if (base.manager.Player.GetCurrency(CurrencyType.ApocalypticEquipToken).Value < definition.ApocalypticEquipToken)
			{
				return false;
			}
			if (definition.TokensToUnlock <= 0)
			{
				return false;
			}
			Func<TWDModelResult> nonCurrencyTokenSubtractFunc = delegate
			{
				if (OwnedTokensAmount < definition.TokensToUnlock)
				{
					return TWDModelResult.NotEnoughEquipToken;
				}
				OwnedTokensAmount -= definition.TokensToUnlock;
				return TWDModelResult.OK;
			};
			Cashier cashier = new Cashier(base.manager);
			CashierItem cashierItem = new CashierItem(PurchaseType.UnlockEquip);
			cashierItem.SetCost(CurrencyType.ApocalypticEquipToken, definition.ApocalypticEquipToken);
			cashier.AddItem(cashierItem);
			if (cashier.Pay(definition, null, null, nonCurrencyTokenSubtractFunc) != TWDModelResult.OK)
			{
				return false;
			}
			if (RewardEquipment.Give(base.manager, new object[1]
			{
				new ModelRandom(0)
			}) is EquipmentItemModel)
			{
				NotifyChange("EquipmentTokenTypeUnlockEvent", this);
				return true;
			}
			return false;
		}

		public void AddEquipToken(EquipTokenItemModel otherEquipTokenItem)
		{
			if (otherEquipTokenItem.OwnedTokensAmount > 0)
			{
				if (otherEquipTokenItem.OwnedTokensAmount > int.MaxValue - OwnedTokensAmount)
				{
					OwnedTokensAmount = int.MaxValue;
				}
				else
				{
					OwnedTokensAmount += otherEquipTokenItem.OwnedTokensAmount;
				}
			}
			else if (otherEquipTokenItem.OwnedTokensAmount < 0)
			{
				if (OwnedTokensAmount + otherEquipTokenItem.OwnedTokensAmount < 0)
				{
					OwnedTokensAmount = 0;
				}
				else
				{
					OwnedTokensAmount += otherEquipTokenItem.OwnedTokensAmount;
				}
			}
			_lastUpdateTime = DateTime.UtcNow;
		}

		public void AddEquipToken(int value)
		{
			if (value > 0)
			{
				if (value > int.MaxValue - OwnedTokensAmount)
				{
					OwnedTokensAmount = int.MaxValue;
				}
				else
				{
					OwnedTokensAmount += value;
				}
			}
			else if (value < 0)
			{
				if (OwnedTokensAmount + value < 0)
				{
					OwnedTokensAmount = 0;
				}
				else
				{
					OwnedTokensAmount += value;
				}
			}
			_lastUpdateTime = DateTime.UtcNow;
		}
	}
}
