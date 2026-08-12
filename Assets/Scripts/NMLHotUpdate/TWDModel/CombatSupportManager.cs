using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class CombatSupportManager : TWDModelObject
	{
		public ModelList<CombatSupportModel> Supports { get; set; }

		public int LastUsedTurn { get; set; } = -1;

		private int CurrentTurn => base.manager.CombatModel.TurnManager.TurnCount;

		public void InitializeEquippedSupports()
		{
			Supports = new ModelList<CombatSupportModel>();
			MapMissionModel attackTargetMissionModel = base.manager.Player.MapContainerModel.AttackTargetMissionModel;
			CombatModel combatModel = base.manager.CombatModel;
			PlayerModel player = base.manager.Player;
			bool flag = SupportHelpers.AreSupportsFixed(attackTargetMissionModel);
			for (int i = 0; i < 3; i++)
			{
				SupportModel missionSupport = SupportHelpers.GetMissionSupport(attackTargetMissionModel, player, i);
				if (missionSupport != null && combatModel.Survivors.Count > i && (!combatModel.ExtraSurvivors.Contains(combatModel.Survivors[i]) || flag))
				{
					CombatSupportModel combatSupportModel = new CombatSupportModel(missionSupport.SupportId, i, ((SurvivorModel)combatModel.Survivors[i]).IdForAnalytics);
					combatSupportModel.SetManager(base.manager);
					if (!flag)
					{
						combatSupportModel.SupportModel.MissionsPlayedCount++;
					}
					Supports.Add(combatSupportModel);
				}
			}
		}

		public override bool IsValid()
		{
			return true;
		}

		public bool TryGetSupport(int slotIndex, out CombatSupportModel combatSupportModel)
		{
			foreach (CombatSupportModel support in Supports)
			{
				if (support.SlotIndex == slotIndex)
				{
					combatSupportModel = support;
					return true;
				}
			}
			combatSupportModel = null;
			return false;
		}

		public bool TryGetSupport(ActorModel actor, out CombatSupportModel combatSupportModel)
		{
			foreach (CombatSupportModel support in Supports)
			{
				if (support.AttachedSurvivor == actor)
				{
					combatSupportModel = support;
					return true;
				}
			}
			combatSupportModel = null;
			return false;
		}

		public bool Execute(int equippedSupportIndex, GridCoordinate target, out ICollection<ActorModel> affectedTargets)
		{
			if (TryGetSupport(equippedSupportIndex, out var combatSupportModel))
			{
				LastUsedTurn = CurrentTurn;
				combatSupportModel.Execute(target, out affectedTargets);
				return true;
			}
			affectedTargets = null;
			return false;
		}

		public CombatSupportAvailability GetAvailability(int equippedSupportIndex)
		{
			if (TryGetSupport(equippedSupportIndex, out var combatSupportModel))
			{
				return GetAvailability(combatSupportModel);
			}
			return CombatSupportAvailability.SupportNotEquipped;
		}

		public CombatSupportAvailability GetAvailability(CombatSupportModel support)
		{
			if (support == null)
			{
				return CombatSupportAvailability.AlreadyUsed;
			}
			if (support.SupportModel?.definition == null)
			{
				return CombatSupportAvailability.SupportNotEquipped;
			}
			MapMissionModel attackTargetMissionModel = base.manager.Player.MapContainerModel.AttackTargetMissionModel;
			if (attackTargetMissionModel != null && attackTargetMissionModel.IsSupportCoolDown(support.SupportModel.definition))
			{
				return CombatSupportAvailability.AlreadyUsed;
			}
			if (support.RealCooldown <= 0 && support.usedCount > 0)
			{
				return CombatSupportAvailability.AlreadyUsed;
			}
			SurvivorModel attachedSurvivor = support.AttachedSurvivor;
			if (attachedSurvivor == null || attachedSurvivor.IsDead)
			{
				return CombatSupportAvailability.SurvivorIsDead;
			}
			SurvivorModel attachedSurvivor2 = support.AttachedSurvivor;
			if (attachedSurvivor2 == null || attachedSurvivor2.TurnComplete)
			{
				return CombatSupportAvailability.SurvivorIsUnavailable;
			}
			if (LastUsedTurn == CurrentTurn)
			{
				return CombatSupportAvailability.AnotherSupportUsedThisTurn;
			}
			if (support.RemainingCooldown > 0)
			{
				return CombatSupportAvailability.OnCooldown;
			}
			if (support.RemainingInnerCooldown > 0)
			{
				return CombatSupportAvailability.OnCooldown;
			}
			return CombatSupportAvailability.Executable;
		}
	}
}
