using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class SurvivalCharacterContainerModel : TWDModelObject
	{
		[IgnoreModelProperty]
		public ModelList<SurvivorModel> SurvivalModeSurvivors { get; private set; }

		public List<SurvivalCharacterStateModel> SurvivalModeStates { get; set; }

		public List<SurvivalCharacterShieldStateModel> SurvivalModelShieldStates { get; set; }

		[JsonIgnore]
		public bool HasOutOfActionSurvivorInCombatTeam
		{
			get
			{
				for (int i = 0; i < base.manager.Player.SurvivorContainer.CombatSurvivors.Count; i++)
				{
					SurvivorModel survivor = base.manager.Player.SurvivorContainer.CombatSurvivors[i];
					SurvivalCharacterStateModel survivorStateInSurvivalMode = GetSurvivorStateInSurvivalMode(survivor);
					if (survivorStateInSurvivalMode == null || survivorStateInSurvivalMode.OutOfAction)
					{
						return true;
					}
				}
				return false;
			}
		}

		public void RemoveAll()
		{
			SurvivalModeSurvivors.Clear();
			SurvivalModeStates.Clear();
			SurvivalModelShieldStates.Clear();
			ValidateListIntegrity();
		}

		public override void Start()
		{
			base.Start();
			ValidateListIntegrity();
		}

		public override void Initialize()
		{
			base.Initialize();
			SurvivalModeSurvivors = new ModelList<SurvivorModel>();
			SurvivalModeSurvivors.SetManager(base.Manager);
			SurvivalModeStates = new List<SurvivalCharacterStateModel>();
			SurvivalModelShieldStates = new List<SurvivalCharacterShieldStateModel>();
			ValidateListIntegrity();
		}

		public int GetNumSurvivorsAvailableForAction()
		{
			int num = 0;
			for (int i = 0; i < SurvivalModeStates.Count; i++)
			{
				if (!SurvivalModeStates[i].OutOfAction)
				{
					num++;
				}
			}
			return num;
		}

		private void ValidateListIntegrity()
		{
			if (SurvivalModeSurvivors.Count != SurvivalModeStates.Count)
			{
				base.Debug.LogError("SurvivalCharacterContainerModel.ValidateListIntegrity detected list size mismatch, data integrity lost.");
			}
		}

		public void OnNewSurvivorReceived(SurvivorModel survivor)
		{
			if (GetSurvivorIndexInSurvivalMode(survivor) != -1)
			{
				base.Debug.LogError("SurvivalCharacterContainerModel.OnNewSurvivorReceived called for a survivor that was already in the survival list (this should not be possible for a new survivor).");
				return;
			}
			ValidateListIntegrity();
			SurvivalModeSurvivors.Add(survivor);
			SurvivalCharacterStateModel survivalCharacterStateModel = new SurvivalCharacterStateModel();
			survivalCharacterStateModel.ResetToInitial();
			SurvivalModeStates.Add(survivalCharacterStateModel);
			SurvivalCharacterShieldStateModel survivalCharacterShieldStateModel = new SurvivalCharacterShieldStateModel();
			survivalCharacterShieldStateModel.ResetToInitial();
			SurvivalModelShieldStates.Add(survivalCharacterShieldStateModel);
		}

		public void OnSurvivorRemoved(SurvivorModel survivor)
		{
			int survivorIndexInSurvivalMode = GetSurvivorIndexInSurvivalMode(survivor);
			if (survivorIndexInSurvivalMode != -1)
			{
				SurvivalModeSurvivors.RemoveAt(survivorIndexInSurvivalMode);
				SurvivalModeStates.RemoveAt(survivorIndexInSurvivalMode);
				SurvivalModelShieldStates.RemoveAt(survivorIndexInSurvivalMode);
				ValidateListIntegrity();
			}
		}

		private int GetSurvivorIndexInSurvivalMode(SurvivorModel survivor)
		{
			ValidateListIntegrity();
			for (int i = 0; i < SurvivalModeSurvivors.Count; i++)
			{
				if (SurvivalModeSurvivors[i] == survivor)
				{
					return i;
				}
			}
			return -1;
		}

		public bool IsSurvivorInSurvivalMode(SurvivorModel survivor)
		{
			return GetSurvivorIndexInSurvivalMode(survivor) != -1;
		}

		public SurvivalCharacterStateModel GetSurvivorStateInSurvivalMode(SurvivorModel survivor)
		{
			int survivorIndexInSurvivalMode = GetSurvivorIndexInSurvivalMode(survivor);
			if (survivorIndexInSurvivalMode == -1)
			{
				base.Debug.LogError("SurvivalCharacterContainerModel.GetSurvivorStateInSurvivalMode called for a survivor that is not in survival list.");
				return null;
			}
			return SurvivalModeStates[survivorIndexInSurvivalMode];
		}

		public SurvivalCharacterShieldStateModel GetSurvivorShieldStateInSurvivalMode(SurvivorModel survivor)
		{
			int survivorIndexInSurvivalMode = GetSurvivorIndexInSurvivalMode(survivor);
			if (survivorIndexInSurvivalMode == -1)
			{
				base.Debug.LogError("SurvivalCharacterContainerModel.GetSurvivorShieldStateInSurvivalMode called for a survivor that is not in survival list.");
				return null;
			}
			return SurvivalModelShieldStates[survivorIndexInSurvivalMode];
		}

		public void ClearSavedState()
		{
			for (int i = 0; i < SurvivalModeSurvivors.Count; i++)
			{
				SurvivalModeStates[i].ResetToInitial();
				SurvivalModelShieldStates[i].ResetToInitial();
			}
		}

		public override bool IsValid()
		{
			return true;
		}

		public bool CanSurvivorRest(SurvivorModel survivor)
		{
			SurvivalCharacterStateModel survivorStateInSurvivalMode = GetSurvivorStateInSurvivalMode(survivor);
			if (survivorStateInSurvivalMode != null && !survivorStateInSurvivalMode.OutOfAction && (survivorStateInSurvivalMode.HealthPercentage < 100L || survivorStateInSurvivalMode.StrugglesLeft == 0))
			{
				return true;
			}
			return false;
		}

		public bool CanAnySurvivorRest()
		{
			for (int i = 0; i < SurvivalModeSurvivors.Count; i++)
			{
				if (CanSurvivorRest(SurvivalModeSurvivors[i]))
				{
					return true;
				}
			}
			return false;
		}

		public List<SurvivorModel> GetSurvivorsForRest()
		{
			List<SurvivorModel> list = new List<SurvivorModel>();
			for (int i = 0; i < SurvivalModeSurvivors.Count; i++)
			{
				if (CanSurvivorRest(SurvivalModeSurvivors[i]))
				{
					list.Add(SurvivalModeSurvivors[i]);
				}
			}
			return list;
		}

		public int ComputeSurvivorsForRestingCount()
		{
			int num = 0;
			for (int i = 0; i < SurvivalModeSurvivors.Count; i++)
			{
				if (CanSurvivorRest(SurvivalModeSurvivors[i]))
				{
					num++;
				}
			}
			return num;
		}

		private void ApplyRestToSurvivors()
		{
			List<SurvivorModel> survivorsForRest = GetSurvivorsForRest();
			for (int i = 0; i < survivorsForRest.Count; i++)
			{
				SurvivalCharacterStateModel survivorStateInSurvivalMode = GetSurvivorStateInSurvivalMode(survivorsForRest[i]);
				int survivalRestEffectPercentage = base.manager.GameEconomyData.ConfigData.SurvivalRestEffectPercentage;
				FixedPoint fixedPoint;
				if (survivorStateInSurvivalMode.StrugglesLeft == 0 && survivorStateInSurvivalMode.HealthPercentage + survivalRestEffectPercentage >= 100L)
				{
					survivorStateInSurvivalMode.StrugglesLeft = 1;
					survivorStateInSurvivalMode.StrugglesLeftBeforeCombat = 1;
					fixedPoint = survivorStateInSurvivalMode.HealthPercentage + survivalRestEffectPercentage - 100L;
					if (fixedPoint < 5L)
					{
						fixedPoint = 5L;
					}
				}
				else
				{
					fixedPoint = survivorStateInSurvivalMode.HealthPercentage + survivalRestEffectPercentage;
				}
				if (fixedPoint > 100L)
				{
					fixedPoint = 100L;
				}
				survivorStateInSurvivalMode.HealthPercentage = fixedPoint;
			}
		}

		public TWDModelResult BuyRest()
		{
			if (CanAnySurvivorRest())
			{
				TWDModelResult num = GetPurchaseRestCashier().Pay(this);
				if (num == TWDModelResult.OK)
				{
					ApplyRestToSurvivors();
				}
				return num;
			}
			return TWDModelResult.AlreadyMaxAmount;
		}

		public Cashier GetPurchaseRestCashier()
		{
			if (CanAnySurvivorRest())
			{
				Cashier cashier = new Cashier(base.manager);
				CashierItem cashierItem = new CashierItem(PurchaseType.SurvivalRest);
				cashierItem.SetCost(CurrencyType.Diamonds, base.manager.GameEconomyData.ConfigData.SurvivalRestCost);
				cashier.AddItem(cashierItem);
				return cashier;
			}
			return null;
		}
	}
}
