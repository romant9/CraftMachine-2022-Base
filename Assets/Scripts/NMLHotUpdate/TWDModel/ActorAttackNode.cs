using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class ActorAttackNode : NodeBase
	{
		[GraphItVariable("How many percent of target health will the damage be. 100 means the attack will deal the same amount of damage that the target has, 150 means it would deal 1.5x the damage.")]
		public int DamagePercentage = -1;

		public GridCoordinate TargetCoordinate;

		[JsonIgnore]
		[GraphItImportData("Attacking Actor", "")]
		public List<ActorModel> AttackingActors => Import("Attacking Actor") as List<ActorModel>;

		[JsonIgnore]
		[GraphItImportData("Target Actors", "")]
		public List<ActorModel> TargetActors
		{
			get
			{
				List<object> list = ImportValues("Target Actors");
				if (list != null)
				{
					List<ActorModel> list2 = new List<ActorModel>();
					for (int i = 0; i < list.Count; i++)
					{
						object obj = list[i];
						if (obj != null)
						{
							if (obj is List<ActorModel> collection)
							{
								list2.AddRange(collection);
							}
							else if (obj is ActorModel item)
							{
								list2.Add(item);
							}
						}
					}
					return list2;
				}
				return null;
			}
		}

		public ActorAttackNode(ActorAttackNode node)
			: base(node)
		{
			DamagePercentage = node.DamagePercentage;
			TargetCoordinate = node.TargetCoordinate;
		}

		public ActorAttackNode()
		{
		}

		public override NodeBase RecordValue()
		{
			return new ActorAttackNode(this);
		}

		[GraphItInput("Attack", "")]
		public void Attack()
		{
			CombatModel combat = base.manager.Player.Combat;
			if (combat != null && AttackingActors != null && AttackingActors.Count == 1)
			{
				ActorModel actorModel = AttackingActors[0];
				EquipmentItemModel weaponEquipment = actorModel.GetWeaponEquipment();
				AbilityModel abilityModel = ((weaponEquipment != null) ? weaponEquipment.Ability : actorModel.SelectedAbility);
				if (abilityModel != null)
				{
					ActorModel actorModel2 = null;
					List<ActorModel> targetActors = TargetActors;
					if (targetActors != null && targetActors.Count > 0)
					{
						actorModel2 = null;
						FixedPoint fixedPoint = FixedPoint.MaxValue;
						for (int i = 0; i < targetActors.Count; i++)
						{
							ActorModel actorModel3 = targetActors[i];
							if (abilityModel.CanAbilityBePerformedOnGridCell(combat, actorModel, actorModel.GridCoordinate, actorModel3.GridCoordinate) == AbilityResult.Success)
							{
								FixedPoint fixedPoint2 = actorModel.GridCoordinate.SquaredDistanceTo(actorModel3.GridCoordinate);
								if (actorModel2 == null || fixedPoint2 < fixedPoint)
								{
									actorModel2 = actorModel3;
									fixedPoint = fixedPoint2;
								}
							}
						}
					}
					else if (combat.Grid.IsCoordinateValid(TargetCoordinate))
					{
						actorModel2 = combat.GetOccupier(TargetCoordinate);
					}
					if (actorModel2 != null)
					{
						if (DamagePercentage >= 0)
						{
							actorModel.AddTrait("TutorialSetDamage", DamagePercentage * actorModel2.Hitpoints / 100 + 1);
						}
						bool num = AbilityCommand.PerformActions(actorModel.manager, actorModel, abilityModel, actorModel2.GridCoordinate, ignoreAPRestrictions: true);
						if (DamagePercentage >= 0)
						{
							actorModel.RemoveTrait("TutorialSetDamage");
						}
						if (num)
						{
							Success();
							return;
						}
					}
				}
			}
			Fail();
		}

		[GraphItOutput("Success", "")]
		public void Success()
		{
			Fire("Success");
		}

		[GraphItOutput("Fail", "")]
		public void Fail()
		{
			Fire("Fail");
		}
	}
}
