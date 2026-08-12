using System;
using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	[Serializable]
	public class SuggestActionNode : NodeBase
	{
		[GraphItVariable("Action to suggest")]
		public SuggestAction SuggestAction;

		[GraphItVariable("Forced suggestion")]
		public bool ForcedSuggestion;

		[GraphItVariable("Target Walkers")]
		public bool TargetWalkers = true;

		[GraphItVariable("Target Raiders")]
		public bool TargetRaiders = true;

		public bool Active;

		[IgnoreModelProperty]
		public RegionModel Region { get; set; }

		public SuggestActionNode()
		{
		}

		public SuggestActionNode(SuggestActionNode node)
			: base(node)
		{
			Region = node.Region;
			SuggestAction = node.SuggestAction;
			ForcedSuggestion = node.ForcedSuggestion;
			TargetWalkers = node.TargetWalkers;
			TargetRaiders = node.TargetRaiders;
			Active = node.Active;
		}

		public override NodeBase RecordValue()
		{
			return new SuggestActionNode(this);
		}

		public override void Initialize()
		{
			base.Initialize();
			Active = false;
		}

		public override void Start()
		{
			base.Start();
			base.manager.OnModelStarted += OnModelStarted;
		}

		private void OnModelStarted()
		{
			if (Active)
			{
				ShowSuggestion();
			}
			base.manager.OnModelStarted -= OnModelStarted;
		}

		[GraphItInput("Suggest", "")]
		public void Suggest()
		{
			ShowSuggestion();
		}

		private List<ActorModel> GetPotentialAttackTargets()
		{
			CombatModel combatModel = base.manager.CombatModel;
			List<ActorModel> factionActors = combatModel.GetFactionActors(Faction.Walker);
			List<ActorModel> factionActors2 = combatModel.GetFactionActors(Faction.Raider);
			List<ActorModel> list = new List<ActorModel>();
			if (Region != null)
			{
				if (TargetWalkers && factionActors != null)
				{
					for (int i = 0; i < factionActors.Count; i++)
					{
						if (Region.Location.Coordinates.Contains(factionActors[i].GridCoordinate))
						{
							list.Add(factionActors[i]);
						}
					}
				}
				if (TargetRaiders && factionActors2 != null)
				{
					for (int j = 0; j < factionActors2.Count; j++)
					{
						if (Region.Location.Coordinates.Contains(factionActors2[j].GridCoordinate))
						{
							list.Add(factionActors2[j]);
						}
					}
				}
			}
			return list;
		}

		private void ShowSuggestion()
		{
			if (Active)
			{
				HideSuggestion();
			}
			CombatModel combatModel = base.manager.CombatModel;
			ActorModel activeActor = base.manager.CombatModel.TurnManager.ActiveActor;
			if (activeActor != null && activeActor.Faction == Faction.Survivor)
			{
				GridCoordinate gridCoordinate = GridCoordinate.Invalid;
				GridField<CellValidity> validTargets = new GridField<CellValidity>(combatModel.Grid.Width, combatModel.Grid.Height, new CellValidity(CellStatus.Invalid, null, null));
				GridField<FixedPoint> threatField = null;
				bool enabled = combatModel.gameEconomyData.GetFeature("PathfindingImprovements").Enabled;
				List<AfterMoveAbilityTarget> afterMoveAbilityTargets = null;
				if (enabled)
				{
					threatField = new GridField<FixedPoint>(combatModel.manager.GridModel.Width, combatModel.manager.GridModel.Height, 0L);
				}
				CombatHelpers.GetValidTargets(combatModel, activeActor, activeActor.GridCoordinate, CombatHelpers.GetMoveRange(activeActor), ref validTargets, ref afterMoveAbilityTargets, ref threatField);
				if (SuggestAction == SuggestAction.Move)
				{
					if (Region != null)
					{
						FixedPoint fixedPoint = FixedPoint.MaxValue;
						for (int i = 0; i < validTargets.Length; i++)
						{
							if (validTargets[i].InteractiveObject == null && validTargets[i].Target == null && validTargets[i].Valid)
							{
								GridCoordinate coordinate = combatModel.Grid.GetCoordinate(i);
								FixedPoint fixedPoint2 = activeActor.GridCoordinate.SquaredDistanceTo(coordinate);
								if (Region.Location.Coordinates.Contains(coordinate) && (gridCoordinate == GridCoordinate.Invalid || fixedPoint2 < fixedPoint))
								{
									gridCoordinate = coordinate;
									fixedPoint = fixedPoint2;
								}
							}
						}
					}
				}
				else if (SuggestAction == SuggestAction.Attack)
				{
					ActorModel actorModel = null;
					EquipmentItemModel weaponEquipment = activeActor.GetWeaponEquipment();
					if (((weaponEquipment != null) ? weaponEquipment.Ability : activeActor.SelectedAbility) != null)
					{
						List<ActorModel> potentialAttackTargets = GetPotentialAttackTargets();
						if (potentialAttackTargets != null && potentialAttackTargets.Count > 0)
						{
							actorModel = null;
							FixedPoint fixedPoint3 = FixedPoint.MaxValue;
							for (int j = 0; j < potentialAttackTargets.Count; j++)
							{
								ActorModel actorModel2 = potentialAttackTargets[j];
								if (validTargets[actorModel2.GridCoordinate].Status != CellStatus.Invalid)
								{
									FixedPoint fixedPoint4 = activeActor.GridCoordinate.SquaredDistanceTo(actorModel2.GridCoordinate);
									if (actorModel == null || fixedPoint4 < fixedPoint3)
									{
										actorModel = actorModel2;
										fixedPoint3 = fixedPoint4;
									}
								}
							}
						}
					}
					if (actorModel != null)
					{
						gridCoordinate = actorModel.GridCoordinate;
					}
				}
				else if (SuggestAction == SuggestAction.Interact)
				{
					FixedPoint fixedPoint5 = FixedPoint.MaxValue;
					for (int k = 0; k < validTargets.Length; k++)
					{
						if (validTargets[k].InteractiveObject != null && validTargets[k].Valid)
						{
							GridCoordinate coordinate2 = combatModel.Grid.GetCoordinate(k);
							FixedPoint fixedPoint6 = activeActor.GridCoordinate.SquaredDistanceTo(coordinate2);
							if (gridCoordinate == GridCoordinate.Invalid || fixedPoint6 < fixedPoint5)
							{
								gridCoordinate = coordinate2;
								fixedPoint5 = fixedPoint6;
							}
						}
					}
				}
				if (gridCoordinate != GridCoordinate.Invalid)
				{
					base.manager.CombatModel.SetSuggestedInteractionTarget(activeActor, gridCoordinate, ForcedSuggestion);
					activeActor.Changed += OnActorModelChanged;
					base.manager.CombatModel.TurnManager.FactionPostChanged += OnFactionPostChanged;
					base.manager.CombatModel.TurnManager.ActorChanged += OnTurnManagerActorChanged;
					Active = true;
					Success();
					return;
				}
			}
			Fail();
		}

		private void HideSuggestion()
		{
			if (Active)
			{
				Active = false;
				if (base.manager.CombatModel.SuggestedInteractionActor != null)
				{
					base.manager.CombatModel.SuggestedInteractionActor.Changed -= OnActorModelChanged;
				}
				base.manager.CombatModel.TurnManager.FactionPostChanged -= OnFactionPostChanged;
				base.manager.CombatModel.TurnManager.ActorChanged -= OnTurnManagerActorChanged;
				base.manager.CombatModel.ClearSuggestedInteractionTarget();
			}
		}

		private void OnFactionPostChanged(Faction oldFaction, Faction newFaction)
		{
			if (newFaction != Faction.Survivor)
			{
				HideSuggestion();
			}
		}

		private void OnActorModelChanged(ModelObject m, string changed, object args)
		{
			ActorModel actorModel = m as ActorModel;
			if (changed == "actorMoveCompleted")
			{
				if (actorModel == base.manager.CombatModel.SuggestedInteractionActor)
				{
					HideSuggestion();
				}
				ActionPerformed();
			}
			if (changed == "actorAbilityCompleted")
			{
				if (actorModel == base.manager.CombatModel.SuggestedInteractionActor)
				{
					HideSuggestion();
				}
				ActionPerformed();
			}
		}

		private void OnTurnManagerActorChanged(ActorModel actor)
		{
			if (actor != base.manager.CombatModel.SuggestedInteractionActor)
			{
				HideSuggestion();
			}
		}

		[GraphItOutput("Fail", "")]
		public void Fail()
		{
			Fire("Fail");
		}

		[GraphItOutput("Success", "")]
		public void Success()
		{
			Fire("Success");
		}

		[GraphItOutput("Action Performed", "")]
		public void ActionPerformed()
		{
			Fire("Action Performed");
		}
	}
}
