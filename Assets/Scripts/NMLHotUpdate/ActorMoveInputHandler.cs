using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Client.Utils;
using TWDModel;
using UnityEngine;

public class ActorMoveInputHandler : PlayerInputHandler
{
	public DrawnPathChangedHandler DrawnPathChanged;

	public InteractiveObjectChangedHandler InteractiveObjectChanged;

	private GridField<CellValidity> validTargets;

	private GridField<FixedPoint> threatField;

	private GridField<CellStatus> validStatus;

	private List<WeaponRangeVisualization> explosionRangeVisualizers;

	private List<WeaponRangeVisualization> oneWithTheHerdTraitRangeHighlight;

	private List<WeaponRangeVisualization> riotShieldHerdTraitRangeHighlight = new List<WeaponRangeVisualization>();

	private WeaponRangeVisualization forestStalkerTraitRangeVisualization;

	private WeaponRangeVisualization pointBlankShotVisualiser;

	private WeaponRangeVisualization damageFalloffVisualiser;

	private WeaponRangeVisualization suppressTraitVisualiser;

	private WeaponRangeVisualization tridentLeftLineVisualizer;

	private WeaponRangeVisualization tridentRightLineVisualizer;

	private GridTargetHighlight forestStalkerTraitTargetHighlight;

	private List<GridTargetHighlight> oneWithTheHerdTraitTargetHighlight;

	private List<GridTargetHighlight> riotShieldHerdTraitTargetHighlight = new List<GridTargetHighlight>();

	private List<WeaponRangeVisualization> pointBlankShotTargetHighlight;

	private GridTargetHighlight gridAttackTargetHighlight;

	private List<CacheableObject> pushDirectionIndicators;

	private List<GridTargetHighlight> baitCandidatesTargetHighlight;

	private ActorView actorView;

	private GridCoordinate previousCoordinate;

	private GridField<FixedPoint> distanceFieldFromActor;

	private GridPath path;

	private InteractiveObjectModel currentInteractionTarget;

	private List<AfterMoveAbilityTarget> afterMoveAttackTargets = new List<AfterMoveAbilityTarget>();

	private GridCoordinate snapCoordinate;

	private GameObject pushDirectionIndicatorPrefab;

	public static float DragEndGestureDelay;

	private GridCoordinate lastValidCoordinate;

	private List<GridCoordinate> highlightCoordinates = new List<GridCoordinate>();

	private List<Color> highlightColors = new List<Color>();

	private List<int> highlightIndices = new List<int>();

	private WeaponRangeVisualization abilityRangeVisualizer
	{
		get
		{
			if (!(actorView != null))
			{
				return null;
			}
			return actorView.AbilityRangeVisualizer;
		}
	}

	private WeaponRangeVisualization activationRangeVisualizer
	{
		get
		{
			if (!(actorView != null))
			{
				return null;
			}
			return actorView.ActivationRangeVisualizer;
		}
	}

	public override bool TapOnly => false;

	private InteractiveObjectModel CurrentInteractionTarget
	{
		get
		{
			return currentInteractionTarget;
		}
		set
		{
			if (currentInteractionTarget != value)
			{
				currentInteractionTarget = value;
				NotifyInteractiveObjectChanged();
			}
		}
	}

	public override int Priority => 100;

	public override void Initialize()
	{
		base.Initialize();
		CombatHUD combatHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatHUD) as CombatHUD;
		if (combatHUD != null)
		{
			combatHUD.OnAbilitySelected -= OnAbilitySelected;
			combatHUD.OnAbilitySelected += OnAbilitySelected;
		}
		if (riotShieldHerdTraitTargetHighlight == null)
		{
			riotShieldHerdTraitTargetHighlight = new List<GridTargetHighlight>();
		}
		if (riotShieldHerdTraitRangeHighlight == null)
		{
			riotShieldHerdTraitRangeHighlight = new List<WeaponRangeVisualization>();
		}
	}

	private void RefreshValidTargets(ActorModel actor)
	{
		afterMoveAttackTargets.Clear();
		distanceFieldFromActor = DistanceField.CreateDistanceField(base.Combat, actor.GridCoordinate, new DistanceFieldOptions(1.5f, actor, actor));
		UpdateValidTargets(actor.GridCoordinate, CombatHelpers.GetMoveRange(actor));
	}

	public override void OnControlledActorChanged(ActorModel newControlledActor)
	{
		if (newControlledActor != null && newControlledActor.SelectedAbility != null)
		{
			if (!newControlledActor.AbilityCompleted || (newControlledActor.SelectedAbility.Definition.IsFreeAction && !newControlledActor.IsAIControlled))
			{
				actorView = GameManager.Instance.GetViewForModel(newControlledActor) as ActorView;
				RefreshValidTargets(newControlledActor);
			}
		}
		else
		{
			ClearValidTargets();
			actorView = null;
		}
		ClearPath();
	}

	private void OnCombatModelChanged(ModelObject m, string changed, object args)
	{
		if (base.PlayerInputManager.ControlledActor != null && changed == "collidersUpdated")
		{
			RefreshValidTargets(base.PlayerInputManager.ControlledActor);
		}
	}

	public override void OnControlledActorPropertiesChanged(string changed, object args)
	{
		OnControlledActorChanged(base.PlayerInputManager.ControlledActor);
	}

	public override bool CanHandleInteraction()
	{
		if (Helpers.IsCombatSkillSelectableStatus())
		{
			return false;
		}
		ActorModel survivorAtMouseCoordinate = base.PlayerInputManager.GetSurvivorAtMouseCoordinate();
		if (!base.PlayerInputManager.IsReconnecting && survivorAtMouseCoordinate != null && base.PlayerInputManager.ControlledActor == survivorAtMouseCoordinate)
		{
			return !base.PlayerInputManager.ControlledActor.TurnComplete;
		}
		return false;
	}

	public override void InteractionStarted()
	{
		GridCoordinate mouseGridCoordinate = base.PlayerInputManager.GetMouseGridCoordinate();
		snapCoordinate = GridCoordinate.Invalid;
		RefreshValidTargets(base.PlayerInputManager.ControlledActor);
		previousCoordinate = mouseGridCoordinate;
		path = GridPath.Create();
		path.AddNode(mouseGridCoordinate);
		base.Combat.Changed -= OnCombatModelChanged;
		base.Combat.Changed += OnCombatModelChanged;
	}

	private bool IsVisibleActorAt(GridCoordinate coordinate)
	{
		ActorModel occupier = base.Combat.GetOccupier(coordinate);
		if (occupier != null && occupier != base.PlayerInputManager.ControlledActor)
		{
			return occupier.IsVisibleToSurvivors;
		}
		return false;
	}

	private void UpdatePath()
	{
		GridCoordinate mouseGridCoordinate = base.PlayerInputManager.GetMouseGridCoordinate();
		GridCoordinate mouseCoordinate = mouseGridCoordinate;
		ActorModel occupier = base.Combat.GetOccupier(mouseGridCoordinate);
		if (GameManager.Instance.gameEconomyData.ConfigData.EnableCombatGridSnap)
		{
			bool valid = validTargets[mouseGridCoordinate].Valid;
			if (valid)
			{
				lastValidCoordinate = mouseGridCoordinate;
			}
			else if (lastValidCoordinate.IsValid && mouseGridCoordinate.DistanceTo(lastValidCoordinate) >= 2L)
			{
				lastValidCoordinate = GridCoordinate.Invalid;
			}
			if (valid)
			{
				if (validTargets[mouseGridCoordinate].InteractiveObject != null || validTargets[mouseGridCoordinate].Target != null || IsVisibleActorAt(mouseGridCoordinate))
				{
					snapCoordinate = mouseGridCoordinate;
				}
				else
				{
					ActorModel controlledActor = base.PlayerInputManager.ControlledActor;
					if (controlledActor == null || controlledActor.SelectedAbility?.Definition.TriggerType != AbilityTriggerType.GridOrTarget)
					{
						ActorModel controlledActor2 = base.PlayerInputManager.ControlledActor;
						if ((controlledActor2 == null || controlledActor2.SelectedAbility?.Definition.TriggerType != AbilityTriggerType.Grid) && (!snapCoordinate.IsValid || !(mouseGridCoordinate.DistanceTo(snapCoordinate) >= 2L)))
						{
							goto IL_01d9;
						}
					}
					snapCoordinate = GridCoordinate.Invalid;
				}
				goto IL_01d9;
			}
			snapCoordinate = GridCoordinate.Invalid;
			if (lastValidCoordinate.IsValid && (occupier == null || occupier.Faction != Faction.Survivor))
			{
				mouseCoordinate = lastValidCoordinate;
			}
		}
		goto IL_0214;
		IL_0214:
		UpdatePath(mouseCoordinate);
		ForceUpdateValidTargets();
		return;
		IL_01d9:
		if (actorView != null && occupier == actorView.Model)
		{
			snapCoordinate = GridCoordinate.Invalid;
		}
		if (snapCoordinate.IsValid)
		{
			mouseCoordinate = snapCoordinate;
		}
		goto IL_0214;
	}

	private GridCoordinate ApplySnap(GridCoordinate coordinate)
	{
		if (GameManager.Instance.gameEconomyData.ConfigData.EnableCombatGridSnap)
		{
			if (snapCoordinate.IsValid)
			{
				return snapCoordinate;
			}
			if (lastValidCoordinate.IsValid)
			{
				return lastValidCoordinate;
			}
		}
		return coordinate;
	}

	private void UpdatePath(GridCoordinate mouseCoordinate)
	{
		if (mouseCoordinate != previousCoordinate)
		{
			ClearExplosionRangeIndicators();
		}
		if (validTargets == null)
		{
			return;
		}
		ActorModel controlledActor = base.PlayerInputManager.ControlledActor;
		AfterMoveAbilityTarget afterMoveAbilityTarget = new AfterMoveAbilityTarget(GridCoordinate.Invalid, GridCoordinate.Invalid);
		if (!validTargets[mouseCoordinate].Valid)
		{
			ClearPath();
			return;
		}
		if (base.Grid.IsCoordinateValid(mouseCoordinate) && mouseCoordinate != previousCoordinate)
		{
			previousCoordinate = mouseCoordinate;
			base.GridView.ClearHighlights();
			ClearForestStalkerTrait();
			ClearHerdSignals(oneWithTheHerdTraitTargetHighlight, oneWithTheHerdTraitRangeHighlight);
			ClearHerdSignals(riotShieldHerdTraitTargetHighlight, riotShieldHerdTraitRangeHighlight);
			ClearPointBlankShotVisualiser();
			ClearGridAttackTargetHighlight();
			ClearPushDirectionIndicators();
			ClearBaitCandidatesHighlights();
			ClearDamageFalloffVisualiser();
			ClearSuppressTraitVisualiser();
			if (activationRangeVisualizer != null)
			{
				activationRangeVisualizer.Clear();
			}
			ClearAbilityAttackRangeVisualizer();
			int moveRange = CombatHelpers.GetMoveRange(controlledActor);
			GridCoordinate to = mouseCoordinate;
			List<ActorModel> list = null;
			EquipmentItemModel selectedEquipment = controlledActor.SelectedEquipment;
			AbilityModel selectedAbility = controlledActor.SelectedAbility;
			if (this.actorView == null)
			{
				this.actorView = GameManager.Instance.GetViewForModel(controlledActor) as ActorView;
			}
			ActorModel occupier = base.Combat.GetOccupier(mouseCoordinate);
			bool flag = !base.Combat.HasPvPRules && occupier != null && occupier.Faction == Faction.Survivor && occupier.IsBleedingOut;
			if (occupier != null && occupier.IsSneak)
			{
				ClearPath();
				return;
			}
			InteractiveObjectModel interactiveObject = validTargets[mouseCoordinate].InteractiveObject;
			bool flag2 = interactiveObject?.CanBeInteracted ?? false;
			if (flag2)
			{
				GridCoordinate gridCoordinate = CombatModel.GetInteractionDirectionNeighbor(mouseCoordinate, base.Combat, controlledActor, interactiveObject.InteractionDirection, interactiveObject.Placement == Placement.Cell);
				if (!gridCoordinate.IsValid)
				{
					gridCoordinate = distanceFieldFromActor.GetClosestFreeNeighbor(mouseCoordinate, controlledActor, moveRange, interactiveObject, checkVisibility: false);
				}
				if (gridCoordinate.IsValid)
				{
					to = gridCoordinate;
				}
			}
			else if ((occupier != null && occupier != controlledActor && selectedEquipment != null && selectedEquipment.Definition.Category == EquipmentCategory.MeleeWeapon) || flag)
			{
				GridCoordinate closestFreeNeighbor = distanceFieldFromActor.GetClosestFreeNeighbor(mouseCoordinate, controlledActor, moveRange, null, checkVisibility: true, edgeCheck: false);
				if (closestFreeNeighbor.IsValid)
				{
					to = closestFreeNeighbor;
				}
			}
			GridPath gridPath = base.Combat.FindPath(controlledActor, controlledActor.GridCoordinate, to);
			bool flag3 = false;
			GridCoordinate resultTrapFlameEndGridCoordinate = GridCoordinate.Invalid;
			TrapFlameAreaManager model = base.Combat.GetModel<TrapFlameAreaManager>();
			if (model != null)
			{
				GridPath originGridPath = GridPath.Create(gridPath);
				if (model.RecalculatePathEndGridCoordinateBecauseOfTrapFlameArea(base.Combat, controlledActor, originGridPath, ref resultTrapFlameEndGridCoordinate, out var reducePathCount))
				{
					gridPath.ClipTo(resultTrapFlameEndGridCoordinate);
					UpdateValidTargets(controlledActor.GridCoordinate, CombatHelpers.GetMoveRange(controlledActor) - reducePathCount);
					flag3 = true;
					if (!gridPath.IsValid || !gridPath.End.IsValid || !mouseCoordinate.IsValid)
					{
						ClearPath();
						return;
					}
				}
			}
			if (gridPath.IsValid && validTargets[gridPath.End].Valid)
			{
				path = gridPath;
			}
			else
			{
				path.Clear();
				path.AddNode(controlledActor.GridCoordinate);
			}
			GridCoordinate gridCoordinate2 = (path.IsValid ? path.End : controlledActor.GridCoordinate);
			bool flag4 = base.Grid.AreNeighbors(gridCoordinate2, mouseCoordinate);
			bool flag5 = selectedEquipment.Definition.Category == EquipmentCategory.Utility && selectedAbility.Definition.TriggerType == AbilityTriggerType.Targetted && selectedAbility.Definition.AbilityRange <= 1L;
			bool flag6 = selectedEquipment.Definition.Category == EquipmentCategory.Utility && selectedAbility.Definition.TriggerType == AbilityTriggerType.Targetted && selectedAbility.Definition.AbilityRange > 1L;
			bool flag7 = selectedAbility.Definition.TriggerType == AbilityTriggerType.Grid || selectedAbility.Definition.TriggerType == AbilityTriggerType.GridOrTarget;
			if (flag2 && flag4 && validTargets[mouseCoordinate].Valid)
			{
				path.TargetCoordinate = mouseCoordinate;
				if (base.Combat.CanUseInteractiveObject(controlledActor, interactiveObject))
				{
					path.Clear();
					path.AddNode(controlledActor.GridCoordinate);
					path.TargetCoordinate = mouseCoordinate;
				}
				ActorView actorView = GameManager.Instance.GetViewForModel(controlledActor) as ActorView;
				if (actorView != null)
				{
					SurvivorAnimationController survivorAnimationController = actorView.CharacterAnimationController as SurvivorAnimationController;
					if (survivorAnimationController != null && survivorAnimationController.CurrentWeaponPose != WeaponPose.Lowered && survivorAnimationController.CurrentWeaponPose != WeaponPose.BeingLowered)
					{
						VisualizationQueue.Instance.Add(new ChangeWeaponPoseVisualizationTask(controlledActor, WeaponPose.Lowered));
					}
				}
				ClearAbilityAttackRangeVisualizer();
			}
			else if ((IsVisibleActorAt(mouseCoordinate) || flag7) && validTargets[mouseCoordinate].Valid)
			{
				if (selectedEquipment != null && selectedAbility != null)
				{
					if ((selectedEquipment.Definition.Category == EquipmentCategory.MeleeWeapon && selectedAbility.Definition.AbilityRange == 1L) || flag5 || flag)
					{
						path.TargetCoordinate = mouseCoordinate;
						if (base.Combat.IsGridCellVisible(controlledActor.GridCoordinate, mouseCoordinate) && base.Grid.AreNeighbors(controlledActor.GridCoordinate, mouseCoordinate))
						{
							path.Clear();
							path.AddNode(controlledActor.GridCoordinate);
							path.TargetCoordinate = mouseCoordinate;
						}
					}
					else if (selectedEquipment.Definition.Category == EquipmentCategory.RangeWeapon || selectedAbility.Definition.AbilityRange > 1L || flag7 || flag6)
					{
						if (selectedAbility.CanAbilityBePerformedOnGridCell(base.Combat, controlledActor, controlledActor.GridCoordinate, mouseCoordinate) == AbilityResult.Success)
						{
							path.Clear();
							ClearMoveActionIndicator();
							path.AddNode(controlledActor.GridCoordinate);
							path.TargetCoordinate = mouseCoordinate;
						}
						else
						{
							bool flag8 = false;
							if (flag3 && path != null)
							{
								_ = path.End;
								if (path.End.IsValid)
								{
									flag8 = true;
								}
							}
							for (int i = 0; i < afterMoveAttackTargets.Count; i++)
							{
								if (flag8)
								{
									break;
								}
								if (afterMoveAttackTargets[i].AbilityTarget == mouseCoordinate)
								{
									afterMoveAbilityTarget = afterMoveAttackTargets[i];
								}
							}
							if (afterMoveAbilityTarget.MoveCoordinate.IsValid)
							{
								path.Clear();
								path = base.Combat.FindPath(controlledActor, controlledActor.GridCoordinate, afterMoveAbilityTarget.MoveCoordinate);
								path.TargetCoordinate = mouseCoordinate;
							}
							else
							{
								GridPath gridPath2 = GridPath.Create();
								for (int j = 0; j < path.Path.Count; j++)
								{
									if (selectedAbility.CanAbilityBePerformedOnGridCell(base.Combat, controlledActor, path.Path[j], mouseCoordinate) == AbilityResult.Success)
									{
										gridPath2 = base.Combat.FindPath(controlledActor, controlledActor.GridCoordinate, path.Path[j]);
										gridPath2.TargetCoordinate = mouseCoordinate;
										break;
									}
								}
								if (gridPath2.IsValid)
								{
									path = gridPath2;
								}
							}
						}
					}
					else
					{
						path.ClearTargetCoordinate();
					}
					bool attracted;
					bool flag9 = CheckForFlareAbility(selectedAbility, mouseCoordinate, out attracted);
					if (!flag9)
					{
						list = base.Combat.AbilityManager.GetListOfActorsToBeTargetted(selectedAbility, controlledActor, path.End, mouseCoordinate);
					}
					if (list != null && list.Count > 0 && !flag9)
					{
						FixedPoint value = 0.0;
						base.Combat.AbilityManager.VisitParameter(AbilityModifierIncreaseSecondaryHitsChance.SecondaryHitsChance, ref value, controlledActor);
						if (selectedAbility.Definition.SecondaryTargetsHitChance * (1.0 + value) < 1L && list != null && list.Count > 0)
						{
							list = list.GetRange(0, 1);
						}
						highlightCoordinates.Clear();
						highlightColors.Clear();
						highlightIndices.Clear();
						for (int num = list.Count - 1; num >= 0; num--)
						{
							ActorModel actorModel = list[num];
							if (actorModel.IsVisibleToSurvivors)
							{
								GridCoordinate item = actorModel.GridCoordinate;
								bool flag10 = mouseCoordinate == actorModel.GridCoordinate;
								if (actorModel.IsMultiCell)
								{
									flag10 = actorModel.GetOccupiedCells()?.Contains(mouseCoordinate) ?? false;
									item = (flag10 ? mouseCoordinate : ((!(actorModel is TankActorModel tankActorModel)) ? actorModel.GetClosestOccupiedCell(controlledActor.GridCoordinate) : tankActorModel.GetVisualCenterCell()));
								}
								highlightCoordinates.Add(item);
								if (flag10)
								{
									highlightColors.Add(actorModel.IsMultiCell ? Color.green : Color.red);
									highlightIndices.Add(1);
								}
								else
								{
									highlightColors.Add(Color.red);
									highlightIndices.Add(0);
								}
								if (controlledActor is SurvivorModel survivorModel && (survivorModel.IsRangedClass || survivorModel.SelectedEquipment.Definition.Category == EquipmentCategory.RangeWeapon || (survivorModel.SelectedEquipment.Definition.Category == EquipmentCategory.Utility && survivorModel.SelectedEquipment.Definition.Type == EquipmentType.Grenade)) && actorModel.GetTraitWithTag("Explosive") != null)
								{
									TraitDefinition traitWithTag = actorModel.GetTraitWithTag("Explosive");
									WalkerExplosionDefinition walkerExplosionDefinition = GameManager.Instance.gameEconomyData.GetWalkerExplosionDefinition(traitWithTag.Identifier);
									if (walkerExplosionDefinition != null)
									{
										WeaponRangeVisualization weaponRangeVisualization = (GameManager.Instance.GetViewForModel(actorModel) as ActorView).ActivationRangeVisualizer;
										Vector3 start = base.GridView.GetPosition(actorModel.GridCoordinate).ToVector3();
										float radius = (float)walkerExplosionDefinition.GetParameter<FixedPoint>(1) * (float)base.Combat.Grid.CellSize.X + (float)base.Combat.Grid.CellSize.X / 2f;
										weaponRangeVisualization.SetCircle(start, radius);
										if (explosionRangeVisualizers == null)
										{
											explosionRangeVisualizers = new List<WeaponRangeVisualization>();
										}
										explosionRangeVisualizers.Add(weaponRangeVisualization);
									}
								}
							}
							else
							{
								list.RemoveAt(num);
							}
						}
						base.GridView.HighlightCoordinates(highlightCoordinates, highlightColors, highlightIndices);
						ActorView actorView2 = GameManager.Instance.GetViewForModel(controlledActor) as ActorView;
						if (actorView2 != null && !controlledActor.IsInvisible)
						{
							SurvivorAnimationController survivorAnimationController2 = actorView2.CharacterAnimationController as SurvivorAnimationController;
							if (path.MoveDistance == 0.0 && survivorAnimationController2 != null && survivorAnimationController2.CurrentWeaponPose != WeaponPose.Raised && survivorAnimationController2.CurrentWeaponPose != WeaponPose.BeingRaised)
							{
								VisualizationQueue.Instance.Add(new TurnToTargetVisualizationTask(controlledActor, actorView2.transform.position, base.GridView.GetPosition(mouseCoordinate).ToVector3()));
								VisualizationQueue.Instance.Add(new ChangeWeaponPoseVisualizationTask(controlledActor, WeaponPose.Raised));
							}
						}
					}
					if (flag9 && !controlledActor.IsInvisible && this.actorView != null)
					{
						SurvivorAnimationController survivorAnimationController3 = this.actorView.CharacterAnimationController as SurvivorAnimationController;
						if (attracted && path.MoveDistance == 0.0 && survivorAnimationController3 != null && survivorAnimationController3.CurrentWeaponPose != WeaponPose.Raised && survivorAnimationController3.CurrentWeaponPose != WeaponPose.BeingRaised)
						{
							VisualizationQueue.Instance.Add(new TurnToTargetVisualizationTask(controlledActor, this.actorView.transform.position, base.GridView.GetPosition(mouseCoordinate).ToVector3()));
							VisualizationQueue.Instance.Add(new ChangeWeaponPoseVisualizationTask(controlledActor, WeaponPose.Raised));
						}
						else if ((!attracted || path.MoveDistance > 0.0) && survivorAnimationController3 != null && survivorAnimationController3.CurrentWeaponPose != WeaponPose.Lowered && survivorAnimationController3.CurrentWeaponPose != WeaponPose.BeingLowered)
						{
							VisualizationQueue.Instance.Add(new ChangeWeaponPoseVisualizationTask(controlledActor, WeaponPose.Lowered));
						}
						if (abilityRangeVisualizer != null)
						{
							abilityRangeVisualizer.ClearDangerIndicator();
							float radius2 = (float)selectedAbility.Definition.AbilityTargetAreaRadius * (float)base.Combat.Grid.CellSize.X + (float)base.Combat.Grid.CellSize.X / 2f;
							abilityRangeVisualizer.SetCircle(base.GridView.GetPosition(mouseCoordinate).ToVector3(), radius2);
							ClearMoveActionIndicator();
						}
					}
					if (this.actorView != null && abilityRangeVisualizer != null && list != null && list.Count > 0)
					{
						abilityRangeVisualizer.ClearDangerIndicator();
						Vector3 vector = (path.IsValid ? base.GridView.GetPosition(path.End).ToVector3() : this.actorView.transform.position);
						Vector3 vector2 = base.GridView.GetPosition(mouseCoordinate).ToVector3();
						if (selectedAbility.PushEffect != null)
						{
							CheckForPushDirectionIndicators(selectedAbility.PushEffect, list, mouseCoordinate);
						}
						if (selectedAbility.Definition.AbilityTargetArea == AbilityTargetAreaType.Circle || flag)
						{
							float radius3 = (float)base.Combat.AbilityManager.GetDamageAreaBlockEffectiveAreaRadius(selectedAbility, mouseCoordinate, (int)selectedAbility.Definition.AbilityTargetAreaRadius) * (float)base.Combat.Grid.CellSize.X + (float)base.Combat.Grid.CellSize.X / 2f;
							abilityRangeVisualizer.SetCircle(vector2, radius3);
							CheckForPointBlankShot(list, vector2, vector, selectedAbility.Definition);
							CheckForGridAttackTargetHighlight(selectedAbility, mouseCoordinate);
							ClearMoveActionIndicator();
						}
						else if (selectedAbility.Definition.AbilityTargetArea == AbilityTargetAreaType.Diamond)
						{
							int damageAreaBlockEffectiveAreaRadius = base.Combat.AbilityManager.GetDamageAreaBlockEffectiveAreaRadius(selectedAbility, mouseCoordinate, (int)selectedAbility.Definition.AbilityTargetAreaRadius);
							List<GridCoordinate> diamondCoordinates = base.Combat.GetDiamondCoordinates(mouseCoordinate, damageAreaBlockEffectiveAreaRadius);
							Color fillColor = ((abilityRangeVisualizer != null) ? abilityRangeVisualizer.GetFillColor() : Color.green);
							base.GridView.HighlightCoordinatesWithFill(diamondCoordinates, fillColor);
							CheckForPointBlankShot(list, vector2, vector, selectedAbility.Definition);
							ClearMoveActionIndicator();
						}
						else if (selectedAbility.Definition.AbilityTargetArea == AbilityTargetAreaType.LineSeparated || selectedAbility.Definition.AbilityTargetArea == AbilityTargetAreaType.Line)
						{
							Vector3 vector3 = vector2 - vector;
							Vector3 normalized = vector3.normalized;
							float num2 = vector3.magnitude + (float)base.Combat.Grid.CellSize.X / 2f;
							Vector3 extendedTarget = vector + normalized * num2;
							FixedPoint value2 = 0.4000000059604645;
							if (!selectedAbility.IsConsumableAbility)
							{
								base.Combat.AbilityManager.VisitParameter("AbilityModifierIncreaseBulletWidth", ref value2, controlledActor);
							}
							if (controlledActor.FocusModeState && !selectedAbility.IsChargeAttack)
							{
								base.Combat.AbilityManager.VisitParameter("AbilityModifierFocusModeAttackWidth", ref value2, controlledActor);
							}
							bool canBeBlocked = selectedAbility.Definition.CanBeBlocked;
							if (!TryDrawTridentSeparatedAttackLines(controlledActor, selectedAbility, vector, vector2, (float)value2, canBeBlocked))
							{
								DrawAbilityAimLine(abilityRangeVisualizer, controlledActor, vector, extendedTarget, (float)value2, canBeBlocked);
							}
							CheckForPointBlankShot(list, vector2, vector, selectedAbility.Definition);
							CheckForDamageFalloff(vector2, vector, selectedAbility);
						}
						else if (selectedAbility.Definition.AbilityTargetArea == AbilityTargetAreaType.ConeRight || selectedAbility.Definition.AbilityTargetArea == AbilityTargetAreaType.ConeLeft)
						{
							FixedPoint value3 = selectedAbility.Definition.AbilityTargetAreaAngle;
							if (!selectedAbility.IsConsumableAbility)
							{
								base.Combat.AbilityManager.VisitParameter("AbilityModifierIncreaseConeAngle", ref value3, controlledActor);
								base.Combat.AbilityManager.VisitParameter("AbilityModifierThreatArcUpgrade", ref value3, controlledActor);
							}
							FixedPoint range = selectedAbility.Definition.AbilityRange;
							if (!selectedAbility.IsConsumableAbility)
							{
								CombatHelpers.CalculateRangeExtension(ref range, controlledActor, base.Combat.AbilityManager);
							}
							float num3 = ((selectedAbility.Definition.AbilityTargetArea == AbilityTargetAreaType.ConeRight) ? ((float)value3 * -0.5f) : ((float)value3 * 0.5f));
							Vector3 normalized2 = (vector2 - vector).normalized;
							FixedPoint radians = num3 * FixedPoint.PI / 180.0;
							float x = (float)(normalized2.x * FixedPoint.Cos(radians) - normalized2.z * FixedPoint.Sin(radians));
							float z = (float)(normalized2.x * FixedPoint.Sin(radians) + normalized2.z * FixedPoint.Cos(radians));
							normalized2.x = x;
							normalized2.z = z;
							FixedPoint fixedPoint = range * base.GridView.Model.CellSize.X + base.Combat.Grid.CellSize.X / 2.0;
							Vector3 end = vector + normalized2 * (float)fixedPoint;
							bool canBeBlocked2 = selectedAbility.Definition.CanBeBlocked;
							GridCoordinate coordinate = base.Combat.Grid.GetCoordinate(vector.ToFixedVec3());
							List<Vector3> list2 = new List<Vector3>();
							CollectAimTrajectoryBlockCenters(controlledActor, coordinate, list, canBeBlocked2, list2);
							float sectorAngle = (float)value3 + 25f;
							if (list2.Count > 0)
							{
								abilityRangeVisualizer.SetBrokenSector(vector, end, sectorAngle, list2, (float)base.Combat.Grid.CellSize.X * 0.5f);
							}
							else
							{
								abilityRangeVisualizer.SetSector(vector, end, sectorAngle);
							}
						}
						else
						{
							bool canBeBlocked3 = selectedAbility.Definition.CanBeBlocked;
							Vector3 normalized3 = (vector2 - vector).normalized;
							FixedPoint range2 = selectedAbility.Definition.AbilityRange;
							if (!selectedAbility.IsConsumableAbility)
							{
								CombatHelpers.CalculateRangeExtension(ref range2, controlledActor, base.Combat.AbilityManager);
							}
							FixedPoint fixedPoint2 = range2 * base.GridView.Model.CellSize.X;
							if ((float)range2 % 1f == 0f)
							{
								fixedPoint2 += base.Combat.Grid.CellSize.X / 2.0;
							}
							Vector3 vector4 = vector + normalized3 * (float)fixedPoint2;
							FixedPoint value4 = selectedAbility.Definition.AbilityTargetAreaAngle;
							if (!selectedAbility.IsConsumableAbility)
							{
								if (IsAreaAngleEffective(selectedAbility.Definition))
								{
									base.Combat.AbilityManager.VisitParameter("AbilityModifierIncreaseConeAngle", ref value4, controlledActor);
								}
								base.Combat.AbilityManager.VisitParameter("AbilityModifierThreatArcUpgrade", ref value4, controlledActor);
							}
							CheckForPointBlankShot(list, vector2, vector, selectedAbility.Definition);
							CheckForDamageFalloff(vector2, vector, selectedAbility);
							if (value4 <= 1L)
							{
								FixedPoint value5 = 0.4000000059604645;
								if (!selectedAbility.IsConsumableAbility)
								{
									base.Combat.AbilityManager.VisitParameter("AbilityModifierIncreaseBulletWidth", ref value5, controlledActor);
								}
								if (controlledActor.FocusModeState && !selectedAbility.IsChargeAttack)
								{
									base.Combat.AbilityManager.VisitParameter("AbilityModifierFocusModeAttackWidth", ref value5, controlledActor);
								}
								if (!TryDrawTridentSeparatedAttackLines(controlledActor, selectedAbility, vector, vector2, (float)value5, canBeBlocked3))
								{
									DrawAbilityAimLine(abilityRangeVisualizer, controlledActor, vector, vector4, (float)value5, canBeBlocked3);
								}
							}
							else
							{
								GridCoordinate coordinate2 = base.Combat.Grid.GetCoordinate(vector.ToFixedVec3());
								List<Vector3> list3 = new List<Vector3>();
								CollectAimTrajectoryBlockCenters(controlledActor, coordinate2, list, canBeBlocked3, list3);
								if (list3.Count > 0)
								{
									abilityRangeVisualizer.SetBrokenSector(vector, vector4, (float)value4, list3, (float)base.Combat.Grid.CellSize.X * 0.5f);
								}
								else
								{
									abilityRangeVisualizer.SetSector(vector, vector4, (float)value4);
								}
							}
							CheckForGridAttackTargetHighlight(selectedAbility, mouseCoordinate);
							ClearMoveActionIndicator();
						}
						if (selectedAbility.Definition.HasFriendlyFire)
						{
							for (int k = 0; k < list.Count; k++)
							{
								if (list[k].IsFriendlyHuman)
								{
									abilityRangeVisualizer.SetDangerIndicator();
									break;
								}
							}
						}
						CheckSuppressTrait(path, mouseCoordinate);
					}
					else if (!flag9)
					{
						SurvivorAnimationController survivorAnimationController4 = (SurvivorAnimationController)this.actorView.CharacterAnimationController;
						if (survivorAnimationController4 != null && survivorAnimationController4.CurrentWeaponPose != WeaponPose.Lowered && survivorAnimationController4.DesiredWeaponPose != WeaponPose.Lowered)
						{
							VisualizationQueue.Instance.Add(new ChangeWeaponPoseVisualizationTask(controlledActor, WeaponPose.Lowered));
						}
						ClearAbilityAttackRangeVisualizer();
					}
				}
				else
				{
					path.ClearTargetCoordinate();
				}
			}
			else
			{
				path.ClearTargetCoordinate();
				ActorView actorView3 = GameManager.Instance.GetViewForModel(controlledActor) as ActorView;
				if (actorView3 != null)
				{
					SurvivorAnimationController survivorAnimationController5 = actorView3.CharacterAnimationController as SurvivorAnimationController;
					if (survivorAnimationController5 != null && survivorAnimationController5.CurrentWeaponPose != WeaponPose.Lowered && survivorAnimationController5.CurrentWeaponPose != WeaponPose.BeingLowered)
					{
						VisualizationQueue.Instance.Add(new ChangeWeaponPoseVisualizationTask(controlledActor, WeaponPose.Lowered));
					}
				}
				ClearAbilityAttackRangeVisualizer();
			}
			bool flag11 = false;
			if (path != null && path.IsValid)
			{
				flag11 = path.MoveDistance > controlledActor.MoveRange;
			}
			GridPath gridPath3 = path;
			if (path == null || !path.IsValid)
			{
				gridPath3 = GridPath.Create();
				gridPath3.AddNode(controlledActor.GridCoordinate);
				gridPath3.AddNode(controlledActor.GridCoordinate);
				gridPath3.TargetCoordinate = mouseCoordinate;
			}
			GridCoordinate targetCoordinate = gridPath3.TargetCoordinate;
			CheckEnemyActivationRange(gridPath3, mouseCoordinate, list);
			CheckEnemyFreeAttackRange(gridPath3, mouseCoordinate);
			CheckSurvivorForestStalkerTrait(gridPath3);
			CheckSurvivorOneWithTheHerdTrait(gridPath3);
			if (path.HasTargetCoordinate)
			{
				InteractiveObjectModel interactiveObject2 = validTargets[path.TargetCoordinate].InteractiveObject;
				if (interactiveObject2 != null)
				{
					MoveActionType type = MoveActionType.Loot;
					int turnsToComplete = interactiveObject2.TurnsToComplete;
					InteractiveObjectView interactiveObjectView = GameManager.Instance.GetViewForModel(interactiveObject2) as InteractiveObjectView;
					if (interactiveObjectView != null)
					{
						if (interactiveObjectView.IndicatorType == IndicatorType.Examine)
						{
							type = MoveActionType.Examine;
						}
						else if (interactiveObjectView.IndicatorType == IndicatorType.Loot)
						{
							type = MoveActionType.Loot;
						}
						else if (interactiveObjectView.IndicatorType == IndicatorType.Interact)
						{
							type = MoveActionType.Interact;
						}
					}
					CombatView.Instance.CombatHUD.SetActionMoveIndicator(targetCoordinate, type, turnsToComplete, 0, GridCoordinate.Invalid);
					CombatView.Instance.CombatHUD.HideCoverMoveIndicator();
				}
				else
				{
					ActorModel occupier2 = base.Combat.GetOccupier(targetCoordinate);
					GridCoordinate to2 = base.Combat.ResolveMultiCellTargetCell(controlledActor.GridCoordinate, targetCoordinate);
					if (occupier2 != null && controlledActor.IsEnemy(occupier2) && base.Combat.IsGridCellVisible(controlledActor.GridCoordinate, to2))
					{
						CheckSurvivorRiotShieldHerdTrait(targetCoordinate);
						if (controlledActor.SelectedEquipment.Definition.Category == EquipmentCategory.MeleeWeapon)
						{
							CombatView.Instance.CombatHUD.SetActionMoveIndicator(targetCoordinate, MoveActionType.Melee, 0, 0, GridCoordinate.Invalid);
							CombatView.Instance.CombatHUD.HideCoverMoveIndicator();
						}
						else
						{
							if (afterMoveAbilityTarget.MoveCoordinate.IsValid && base.Combat.HasCover(afterMoveAbilityTarget.MoveCoordinate))
							{
								CombatView.Instance.CombatHUD.SetCoverMoveIndicator(afterMoveAbilityTarget.MoveCoordinate);
							}
							else
							{
								CombatView.Instance.CombatHUD.HideCoverMoveIndicator();
							}
							CombatView.Instance.CombatHUD.SetActionMoveIndicator(targetCoordinate, MoveActionType.Shoot, 0, 0, afterMoveAbilityTarget.MoveCoordinate);
						}
					}
					else if (occupier2 != null && controlledActor.IsEnemy(occupier2))
					{
						CheckSurvivorRiotShieldHerdTrait(targetCoordinate);
					}
					else
					{
						CombatView.Instance.CombatHUD.HideCoverMoveIndicator();
						if (occupier2 != null && occupier2.IsBleedingOut)
						{
							CombatView.Instance.CombatHUD.SetActionMoveIndicator(targetCoordinate, MoveActionType.BuddyAid, 0, 0, GridCoordinate.Invalid);
						}
					}
				}
			}
			else
			{
				CombatView.Instance.CombatHUD.HideCoverMoveIndicator();
				ClearHerdSignals(riotShieldHerdTraitTargetHighlight, riotShieldHerdTraitRangeHighlight);
				ClearAbilityAttackRangeVisualizer();
				if (gridAttackTargetHighlight != null)
				{
					ClearGridAttackTargetHighlight();
				}
				if (pushDirectionIndicators != null)
				{
					ClearPushDirectionIndicators();
				}
				if (mouseCoordinate != controlledActor.GridCoordinate)
				{
					int apCount = ((!controlledActor.MoveCompleted && !flag11) ? 1 : 0);
					GridCoordinate coordinate3 = mouseCoordinate;
					if (flag3 && path != null)
					{
						_ = path.End;
						if (path.End.IsValid)
						{
							coordinate3 = path.End;
						}
					}
					if (base.Combat.GetCoveredDirections(mouseCoordinate) == 0)
					{
						CombatView.Instance.CombatHUD.SetActionMoveIndicator(coordinate3, flag11 ? MoveActionType.MoveSprint : MoveActionType.Move, 0, apCount, GridCoordinate.Invalid);
					}
					else
					{
						CombatView.Instance.CombatHUD.SetActionMoveIndicator(coordinate3, MoveActionType.Cover, 0, apCount, GridCoordinate.Invalid);
					}
				}
				else
				{
					CombatView.Instance.CombatHUD.HideMoveActionIndicator();
				}
			}
			NotifyDrawnPathChanged(flag11);
		}
		if (path != null && path.IsValid)
		{
			CurrentInteractionTarget = (path.HasTargetCoordinate ? validTargets[path.TargetCoordinate].InteractiveObject : validTargets[path.End].InteractiveObject);
		}
		else
		{
			CurrentInteractionTarget = null;
		}
	}

	public void UpdatePathHasTargetCoordinate()
	{
	}

	public void UpdatePathNotHasTargetCoordinate()
	{
	}

	public override bool UpdateInteraction(float deltaTime)
	{
		if (validTargets == null)
		{
			return true;
		}
		UpdatePath();
		return true;
	}

	private void NotifyDrawnPathChanged(bool doubleMove)
	{
		DrawnPathChanged?.Invoke(path, doubleMove);
	}

	private void NotifyInteractiveObjectChanged()
	{
		InteractiveObjectChanged?.Invoke(CurrentInteractionTarget);
	}

	private void ExecuteOrder()
	{
		if (base.PlayerInputManager == null)
		{
			return;
		}
		ActorModel controlledActor = base.PlayerInputManager.ControlledActor;
		if (controlledActor == null)
		{
			return;
		}
		ActorView actorView = this.actorView;
		if (path == null || (!path.IsValid && !path.HasTargetCoordinate))
		{
			return;
		}
		GridPath gridPath = path;
		if (base.Combat.HasForcedInteractionTarget)
		{
			GridCoordinate gridCoordinate = (gridPath.HasTargetCoordinate ? gridPath.TargetCoordinate : gridPath.End);
			if (base.Combat.SuggestedInteractionTargetCoordinate != gridCoordinate)
			{
				return;
			}
		}
		InteractiveObjectModel interactiveObjectModel = (gridPath.HasTargetCoordinate ? validTargets[gridPath.TargetCoordinate].InteractiveObject : null);
		if (gridPath.IsValid)
		{
			if (GameManager.Instance.gameEconomyData.GetFeature("CombatOfflineModeFix").Enabled && !GameManager.Instance.CheckConnectionReachability(showPopup: true, "MoveCommand"))
			{
				VisualizationQueue.Instance.GameDisconnected();
			}
			Helpers.ExecuteCommand(new MoveCommand(controlledActor, gridPath));
		}
		bool dashTraitAttackFlag = controlledActor.dashTraitAttackFlag;
		if (dashTraitAttackFlag)
		{
			Helpers.ExecuteCommand(new ModifyDashTraitCommand(controlledActor));
		}
		if (interactiveObjectModel != null)
		{
			MovableModel movableForInteractiveObject = base.Combat.GetMovableForInteractiveObject(interactiveObjectModel);
			if (movableForInteractiveObject != null && movableForInteractiveObject != null && !movableForInteractiveObject.CheckClearance())
			{
				actorView.Say("Actor.MovableObjectBlocked");
				return;
			}
		}
		if (controlledActor.GridCoordinate == gridPath.End)
		{
			if (interactiveObjectModel != null)
			{
				Helpers.ExecuteCommand(new UseInteractiveObjectCommand(controlledActor, interactiveObjectModel));
			}
			else if (gridPath.HasTargetCoordinate)
			{
				AbilityModel selectedAbility = controlledActor.SelectedAbility;
				if (!controlledActor.GetIsUsingAdditionalAttacks() && selectedAbility != null && selectedAbility.CanAbilityBePerformedOnGridCell(base.Combat, controlledActor, controlledActor.GridCoordinate, gridPath.TargetCoordinate) == AbilityResult.Success)
				{
					if (!GameManager.Instance.gameEconomyData.GetFeature("AbilityCommandErrorFix").Enabled || !controlledActor.AbilityCompleted || selectedAbility.Definition.IsFreeAction)
					{
						if (GameManager.Instance.gameEconomyData.GetFeature("CombatOfflineModeFix").Enabled && !GameManager.Instance.CheckConnectionReachability(showPopup: true, "AbilityCommand"))
						{
							VisualizationQueue.Instance.GameDisconnected();
						}
						ActorModel occupier = base.Combat.GetOccupier(gridPath.TargetCoordinate);
						if ((!dashTraitAttackFlag || (controlledActor.SelectedEquipment == controlledActor.GetConsumableEquipment() && controlledActor.SelectedEquipment.IsConsumable) || (occupier != null && (occupier == null || !occupier.IsDead))) && (occupier != null || selectedAbility.Definition.TriggerType != AbilityTriggerType.Targetted))
						{
							Helpers.ExecuteCommand(new AbilityCommand(controlledActor, selectedAbility, gridPath.TargetCoordinate));
						}
					}
				}
				else if (selectedAbility != null && selectedAbility.CanAbilityBeTargetedOnGridCell(base.Combat, controlledActor, controlledActor.GridCoordinate, gridPath.End) && (gridPath.Length <= 1 || controlledActor.PassByAttackedOnMove))
				{
					if (!GameManager.Instance.gameEconomyData.GetFeature("AbilityCommandErrorFix").Enabled || !controlledActor.AbilityCompleted || selectedAbility.Definition.IsFreeAction)
					{
						if (GameManager.Instance.gameEconomyData.GetFeature("CombatOfflineModeFix").Enabled && !GameManager.Instance.CheckConnectionReachability(showPopup: true, "AbilityCommand"))
						{
							VisualizationQueue.Instance.GameDisconnected();
						}
						ActorModel occupier2 = base.Combat.GetOccupier(gridPath.TargetCoordinate);
						if ((!dashTraitAttackFlag || (controlledActor.SelectedEquipment == controlledActor.GetConsumableEquipment() && controlledActor.SelectedEquipment.IsConsumable) || (occupier2 != null && (occupier2 == null || !occupier2.IsDead))) && (occupier2 != null || selectedAbility.Definition.TriggerType != AbilityTriggerType.Targetted))
						{
							Helpers.ExecuteCommand(new AbilityCommand(controlledActor, selectedAbility, gridPath.TargetCoordinate));
						}
					}
				}
				else
				{
					ActorModel occupier3 = base.Combat.GetOccupier(gridPath.TargetCoordinate);
					if (!base.Combat.HasPvPRules && occupier3 != null && occupier3.Faction == Faction.Survivor && occupier3.IsBleedingOut)
					{
						Helpers.ExecuteCommand(new SaveBleedingOutCommand(controlledActor, occupier3));
					}
				}
			}
		}
		EndTurnRoutine(controlledActor, base.Combat);
	}

	public static void EndTurnRoutine(ActorModel activeActor, CombatModel combatModel)
	{
		CombatView.Instance.CheckEndTurnButtonHighlight();
		if (!activeActor.TurnComplete)
		{
			return;
		}
		bool flag = false;
		Faction faction = (activeActor.ActorFactionChangedInCombat ? activeActor.OriginalFaction : activeActor.Faction);
		List<ActorModel> factionActors = combatModel.GetFactionActors(faction);
		for (int i = 0; i < factionActors.Count; i++)
		{
			ActorModel actorModel = factionActors[i];
			if (actorModel != activeActor && !actorModel.TurnComplete)
			{
				Helpers.ExecuteCommand(new SetActiveActorCommand(actorModel));
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			CombatView.Instance.CombatHUD.SetSkipTurnEnabled(enabled: false);
		}
	}

	private void ClearPath()
	{
		previousCoordinate = GridCoordinate.Invalid;
		path = GridPath.Create();
		NotifyDrawnPathChanged(doubleMove: false);
		base.GridView.ClearHighlights();
		ClearEnemyActivationRangeVisualization();
		ClearEnemyFreeAttackRangeVisualization();
		ClearMoveActionIndicator();
		if (activationRangeVisualizer != null)
		{
			activationRangeVisualizer.Clear();
		}
		ClearAbilityAttackRangeVisualizer();
		ClearForestStalkerTrait();
		ClearHerdSignals(oneWithTheHerdTraitTargetHighlight, oneWithTheHerdTraitRangeHighlight);
		ClearHerdSignals(riotShieldHerdTraitTargetHighlight, riotShieldHerdTraitRangeHighlight);
		ClearPointBlankShotVisualiser();
		ClearExplosionRangeIndicators();
		ClearGridAttackTargetHighlight();
		ClearPushDirectionIndicators();
		ClearBaitCandidatesHighlights();
		ClearDamageFalloffVisualiser();
		ClearSuppressTraitVisualiser();
	}

	public override void Reset()
	{
		base.Reset();
		ClearPath();
		ClearValidTargets();
	}

	public override void InteractionStopped()
	{
		if (base.PlayerInputManager != null)
		{
			GridCoordinate previousDragCoordinate = base.PlayerInputManager.GetPreviousDragCoordinate(DragEndGestureDelay);
			previousDragCoordinate = ApplySnap(previousDragCoordinate);
			if (previousDragCoordinate != GridCoordinate.Invalid)
			{
				UpdatePath(previousDragCoordinate);
				ForceUpdateValidTargets();
				ExecuteOrder();
			}
		}
		ClearPath();
		base.Combat.Changed -= OnCombatModelChanged;
	}

	private void UpdateValidTargets(GridCoordinate startCoordinate, FixedPoint remainingMoveRange)
	{
		if (validTargets == null)
		{
			validTargets = new GridField<CellValidity>(base.Grid.Width, base.Grid.Height, new CellValidity(CellStatus.Invalid, null, null));
		}
		bool flag = true;
		if (base.Combat.gameEconomyData != null)
		{
			flag = base.Combat.gameEconomyData.GetFeature("PathfindingImprovements").Enabled;
		}
		if (flag && threatField == null)
		{
			threatField = new GridField<FixedPoint>(base.Combat.manager.GridModel.Width, base.Combat.manager.GridModel.Height, 0L);
		}
		else if (!flag)
		{
			threatField = null;
		}
		CombatHelpers.GetValidTargets(base.Combat, base.PlayerInputManager.ControlledActor, startCoordinate, remainingMoveRange, ref validTargets, ref afterMoveAttackTargets, ref threatField);
		UpdateBooleanField();
	}

	private void UpdateBooleanField()
	{
		bool flag = false;
		if (validStatus == null)
		{
			validStatus = new GridField<CellStatus>(base.Grid.Width, base.Grid.Height, CellStatus.Invalid);
		}
		if (validTargets.IsClear)
		{
			flag = true;
			validStatus.Clear();
		}
		else
		{
			for (int i = 0; i < base.Grid.NumCells; i++)
			{
				GridCoordinate coordinate = base.Grid.GetCoordinate(i);
				validStatus[coordinate] = validTargets[coordinate].Status;
			}
		}
		bool flag2 = PlayerInputManager.Instance.ControlledActor != null && PlayerInputManager.Instance.ControlledActor.Faction == Faction.Survivor;
		if (!flag && VisualizationQueue.Instance.HasTaskOfType<FactionChangeVisualizationTask>() && flag2)
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(null, delegate
			{
				base.PlayerInputManager.SetValidTargets(validStatus);
			}));
		}
		else
		{
			base.PlayerInputManager.SetValidTargets(validStatus);
		}
	}

	public void ClearValidTargets(bool notify = true)
	{
		if (validTargets != null && !validTargets.IsClear)
		{
			validTargets.Clear();
			if (notify)
			{
				UpdateBooleanField();
			}
		}
		if (threatField != null && !threatField.IsClear)
		{
			threatField.Clear();
		}
		ClearAbilityAttackRangeVisualizer();
	}

	private void OnAbilitySelected(AbilityModel ability, ActorModel sourceActor)
	{
		if (sourceActor != null)
		{
			UpdateValidTargets(sourceActor.GridCoordinate, CombatHelpers.GetMoveRange(sourceActor));
		}
	}

	private void CheckEnemyActivationRange(GridPath path, GridCoordinate targetCoordinate, List<ActorModel> targetActors)
	{
		if (path == null || !path.IsValid)
		{
			return;
		}
		ActorModel controlledActor = base.PlayerInputManager.ControlledActor;
		AbilityModel selectedAbility = controlledActor.SelectedAbility;
		int num = 0;
		if (targetCoordinate.IsValid && selectedAbility != null && path.HasTargetCoordinate && validTargets[targetCoordinate].InteractiveObject == null && base.Combat.GetOccupier(targetCoordinate) != controlledActor)
		{
			GridCoordinate other = new GridCoordinate(path.End.X + selectedAbility.Definition.NoiseRange, path.End.Y);
			num = path.End.SquaredDistanceTo(other);
		}
		List<ActorModel> enemyFactionsActors = base.Combat.GetEnemyFactionsActors(Faction.Survivor);
		for (int i = 0; i < enemyFactionsActors.Count; i++)
		{
			if (enemyFactionsActors[i].ActivationRange == 0)
			{
				continue;
			}
			bool flag = enemyFactionsActors[i] == base.Combat.GetOccupier(targetCoordinate);
			bool flag2 = targetActors?.Contains(enemyFactionsActors[i]) ?? false;
			GridCoordinate other2 = new GridCoordinate(enemyFactionsActors[i].GridCoordinate.X + enemyFactionsActors[i].ActivationRange, enemyFactionsActors[i].GridCoordinate.Y);
			bool flag3 = false;
			for (int j = 0; j < path.Path.Count; j++)
			{
				int num2 = enemyFactionsActors[i].GridCoordinate.SquaredDistanceTo(path.Path[j]);
				int num3 = enemyFactionsActors[i].GridCoordinate.SquaredDistanceTo(other2);
				if (((num2 <= num3 && base.Combat.IsGridCellVisible(path.Path[j], enemyFactionsActors[i].GridCoordinate)) || num2 <= num || flag2) && enemyFactionsActors[i].AIController.AIDataModel.Alertness < AIAlertness.Homing)
				{
					flag3 = true;
					break;
				}
			}
			ActorView actorView = GameManager.Instance.GetViewForModel(enemyFactionsActors[i]) as ActorView;
			if (actorView != null)
			{
				actorView.SetActivationRangeVisualization(flag3 && !controlledActor.IsInvisible && !controlledActor.IsCamouflaged);
				if (flag)
				{
					actorView.ShowHealthIndicator(visible: false);
					actorView.SetActivationRangeVisualization(enabled: false);
				}
				else
				{
					actorView.ShowHealthIndicator(visible: true);
				}
			}
		}
	}

	private void CheckSurvivorForestStalkerTrait(GridPath path)
	{
		if (path == null || !path.IsValid)
		{
			return;
		}
		ActorModel controlledActor = base.PlayerInputManager.ControlledActor;
		if (!controlledActor.HasAnyLevelTrait("LeaderBuffForestStalker") || controlledActor.HasAnyLevelTrait("LeaderBuffOneWithTheHerdStalker") || controlledActor.IsInvisible || controlledActor.HasTrait("LeaderBuffOneWithTheHerdStalker"))
		{
			return;
		}
		LeaderBuffForestStalkerTrait.GetIntersectingCoordinate(GameManager.Instance.modelManager, path, out var intersectingCoordinate, out var walkerToAttack);
		if (intersectingCoordinate != GridCoordinate.Invalid && walkerToAttack != null && GameManager.Instance.GetViewForModel(walkerToAttack) as ActorView != null)
		{
			if (controlledActor.SelectedAbility.PushEffect != null && !controlledActor.SelectedAbility.IsConsumableAbility)
			{
				CheckForPushDirectionIndicators(controlledActor.SelectedAbility.PushEffect, new List<ActorModel> { walkerToAttack }, intersectingCoordinate, forceSourceFromMouseCoordinate: true);
			}
			forestStalkerTraitTargetHighlight = base.GridView.AddHighlight(walkerToAttack.GridCoordinate, Color.red, 1);
			ActorView actorView = GameManager.Instance.GetViewForModel(walkerToAttack) as ActorView;
			forestStalkerTraitRangeVisualization = actorView.AbilityRangeVisualizer;
			float radius = (float)base.Combat.Grid.CellSize.X / 2f;
			forestStalkerTraitRangeVisualization.SetCircle(base.GridView.GetPosition(walkerToAttack.GridCoordinate).ToVector3(), radius);
		}
	}

	private void ClearForestStalkerTrait()
	{
		if (forestStalkerTraitRangeVisualization != null)
		{
			forestStalkerTraitRangeVisualization.Clear();
		}
		if (forestStalkerTraitTargetHighlight != null)
		{
			CacheableObject component = forestStalkerTraitTargetHighlight.gameObject.GetComponent<CacheableObject>();
			if (component != null)
			{
				component.Destroy();
			}
			forestStalkerTraitTargetHighlight = null;
		}
	}

	private void CheckSurvivorRiotShieldHerdTrait(GridCoordinate target)
	{
		if (!target.IsValid)
		{
			return;
		}
		ActorModel controlledActor = base.PlayerInputManager.ControlledActor;
		if (!controlledActor.HasAnyLevelTrait("Equipment_Active_RiotShield_Herd") || controlledActor.IsInvisible || !controlledActor.HasAnyLevelTrait("Equipment_Active_RiotShield_Herd") || !controlledActor.SelectedEquipment.IsChargeEquipment)
		{
			return;
		}
		FixedPoint fixedPoint = 0L;
		TraitDefinition traitDefinition = controlledActor.Modifiers.gameEconomyData.TraitDefinitions.First((TraitDefinition x) => x.Identifier == "Equipment_Active_RiotShield_Herd");
		if (traitDefinition != null)
		{
			fixedPoint = traitDefinition.GetParameter<FixedPoint>(0);
		}
		List<ActorModel> enemiesByDistance = path.End.GetEnemiesByDistance(target, base.Combat, (int)fixedPoint);
		for (int num = 0; num < enemiesByDistance.Count; num++)
		{
			if (GameManager.Instance.GetViewForModel(enemiesByDistance[num]) as ActorView != null)
			{
				riotShieldHerdTraitTargetHighlight.Add(base.GridView.AddHighlight(enemiesByDistance[num].GridCoordinate, Color.red, 1));
				ActorView actorView = GameManager.Instance.GetViewForModel(enemiesByDistance[num]) as ActorView;
				riotShieldHerdTraitRangeHighlight.Add(actorView.AbilityRangeVisualizer);
				float radius = (float)base.Combat.Grid.CellSize.X * 0.5f;
				riotShieldHerdTraitRangeHighlight[riotShieldHerdTraitRangeHighlight.Count - 1].SetCircle(base.GridView.GetPosition(enemiesByDistance[num].GridCoordinate).ToVector3(), radius);
				riotShieldHerdTraitRangeHighlight[riotShieldHerdTraitRangeHighlight.Count - 1].SetHerdIndicator();
				if (enemiesByDistance[num].Faction != Faction.Walker)
				{
					riotShieldHerdTraitRangeHighlight[riotShieldHerdTraitRangeHighlight.Count - 1].Clear();
				}
				if (controlledActor.SelectedAbility.PushEffect != null && !controlledActor.SelectedAbility.IsConsumableAbility)
				{
					CheckForPushDirectionIndicators(controlledActor.SelectedAbility.PushEffect, new List<ActorModel> { enemiesByDistance[num] }, enemiesByDistance[num].GridCoordinate, forceSourceFromMouseCoordinate: true);
				}
			}
		}
	}

	private void CheckSurvivorOneWithTheHerdTrait(GridPath path)
	{
		if (path == null || !path.IsValid)
		{
			return;
		}
		ActorModel controlledActor = base.PlayerInputManager.ControlledActor;
		if (!controlledActor.HasAnyLevelTrait("LeaderBuffOneWithTheHerd") || controlledActor.IsInvisible || !controlledActor.HasAnyLevelTrait("LeaderBuffOneWithTheHerd"))
		{
			return;
		}
		bool flag = controlledActor.HasAnyLevelTrait("LeaderBuffOneWithTheHerdStalker");
		LeaderBuffOneWithTheHerdTrait.GetIntersectingCoordinates(GameManager.Instance.modelManager, path, out var intersectingCoordinateList, out var walkersToAddToTheHerd, flag);
		for (int i = 0; i < walkersToAddToTheHerd.Count; i++)
		{
			GridCoordinate gridCoordinate = intersectingCoordinateList[i];
			ActorModel actorModel = walkersToAddToTheHerd[i];
			if (gridCoordinate != GridCoordinate.Invalid && actorModel != null && GameManager.Instance.GetViewForModel(actorModel) as ActorView != null)
			{
				if (oneWithTheHerdTraitTargetHighlight == null)
				{
					oneWithTheHerdTraitTargetHighlight = new List<GridTargetHighlight>();
				}
				oneWithTheHerdTraitTargetHighlight.Add(base.GridView.AddHighlight(actorModel.GridCoordinate, Color.red, 1));
				ActorView actorView = GameManager.Instance.GetViewForModel(actorModel) as ActorView;
				if (oneWithTheHerdTraitRangeHighlight == null)
				{
					oneWithTheHerdTraitRangeHighlight = new List<WeaponRangeVisualization>();
				}
				oneWithTheHerdTraitRangeHighlight.Add(actorView.AbilityRangeVisualizer);
				float radius = (float)base.Combat.Grid.CellSize.X / 2f;
				oneWithTheHerdTraitRangeHighlight[oneWithTheHerdTraitRangeHighlight.Count - 1].SetCircle(base.GridView.GetPosition(actorModel.GridCoordinate).ToVector3(), radius);
				if (i > 0 || !flag)
				{
					oneWithTheHerdTraitRangeHighlight[oneWithTheHerdTraitRangeHighlight.Count - 1].SetHerdIndicator();
				}
				else if (i == 0 && flag && controlledActor.SelectedAbility.PushEffect != null && !controlledActor.SelectedAbility.IsConsumableAbility)
				{
					CheckForPushDirectionIndicators(controlledActor.SelectedAbility.PushEffect, new List<ActorModel> { actorModel }, gridCoordinate, forceSourceFromMouseCoordinate: true);
				}
			}
		}
	}

	private void ClearHerdSignals(List<GridTargetHighlight> highlights, List<WeaponRangeVisualization> weaponRangeVisualizations)
	{
		if (highlights != null)
		{
			for (int i = 0; i < highlights.Count; i++)
			{
				CacheableObject component = highlights[i].gameObject.GetComponent<CacheableObject>();
				if (component != null)
				{
					component.Destroy();
				}
			}
			highlights.Clear();
		}
		if (weaponRangeVisualizations != null)
		{
			for (int j = 0; j < weaponRangeVisualizations.Count; j++)
			{
				weaponRangeVisualizations[j].ClearDangerIndicator();
				weaponRangeVisualizations[j].Clear();
			}
			weaponRangeVisualizations.Clear();
		}
	}

	private void CheckForPointBlankShot(List<ActorModel> actorsAffected, Vector3 targetCellPosition, Vector3 movePosition, AbilityDefinition abilityDefinition)
	{
		if (abilityDefinition == null)
		{
			return;
		}
		int equipmentBreakThroughTraitParam = actorView.Model.GetEquipmentBreakThroughTraitParam("PointBlankShot", 0);
		if (equipmentBreakThroughTraitParam <= 0)
		{
			return;
		}
		Vector3 normalized = (targetCellPosition - movePosition).normalized;
		float num = (float)equipmentBreakThroughTraitParam + (float)base.Combat.Grid.CellSize.X / 2f + 1f;
		if (abilityDefinition.AbilityTargetArea == AbilityTargetAreaType.Cone)
		{
			num = (float)equipmentBreakThroughTraitParam * (float)base.Combat.Grid.CellSize.X;
		}
		Vector3 end = movePosition + normalized * num;
		if (pointBlankShotVisualiser == null)
		{
			PrefabResource prefabResource = UnityUtils.LoadFromAssetBundle<PrefabResource>("Combat/WeaponRangeIndicator", "scriptableobjects");
			pointBlankShotVisualiser = Object.Instantiate(prefabResource.GetPrefab()).GetComponent<WeaponRangeVisualization>();
			pointBlankShotVisualiser.gameObject.name = "PointPlankShotWeaponVisualizer";
			pointBlankShotVisualiser.SetPointBlankIndicator();
		}
		pointBlankShotVisualiser.transform.parent = actorView.transform;
		pointBlankShotVisualiser.transform.localPosition = new Vector3(0f, 0f, 0f);
		switch (abilityDefinition.AbilityTargetArea)
		{
		default:
			return;
		case AbilityTargetAreaType.Circle:
		case AbilityTargetAreaType.Line:
		case AbilityTargetAreaType.LineMax:
		case AbilityTargetAreaType.Diamond:
		case AbilityTargetAreaType.LineSeparated:
			pointBlankShotVisualiser.SetLine(movePosition, end, 0.8f);
			break;
		case AbilityTargetAreaType.Cone:
		{
			FixedPoint value = abilityDefinition.AbilityTargetAreaAngle;
			base.Combat.AbilityManager.VisitParameter("AbilityModifierIncreaseConeAngle", ref value, actorView.Model);
			base.Combat.AbilityManager.VisitParameter("AbilityModifierThreatArcUpgrade", ref value, actorView.Model);
			pointBlankShotVisualiser.SetSector(movePosition, end, (float)value);
			break;
		}
		case AbilityTargetAreaType.Chained:
		case AbilityTargetAreaType.ConeLeft:
		case AbilityTargetAreaType.ConeRight:
			return;
		}
		pointBlankShotVisualiser.SetPointBlankIndicator();
		float radius = (float)base.Combat.Grid.CellSize.X / 2f;
		for (int i = 0; i < actorsAffected.Count; i++)
		{
			if (CombatHelpers.IsWithinRange(base.Combat, equipmentBreakThroughTraitParam, actorView.Model.GridCoordinate, actorsAffected[i].GridCoordinate))
			{
				if (pointBlankShotTargetHighlight == null)
				{
					pointBlankShotTargetHighlight = new List<WeaponRangeVisualization>();
				}
				pointBlankShotTargetHighlight.Add((GameManager.Instance.GetViewForModel(actorsAffected[i]) as ActorView).AbilityRangeVisualizer);
				pointBlankShotTargetHighlight[pointBlankShotTargetHighlight.Count - 1].SetCircle(base.GridView.GetPosition(actorsAffected[i].GridCoordinate).ToVector3(), radius);
			}
		}
	}

	private void ClearPointBlankShotVisualiser()
	{
		if (pointBlankShotVisualiser != null)
		{
			pointBlankShotVisualiser.Clear();
		}
		if (pointBlankShotTargetHighlight != null)
		{
			for (int i = 0; i < pointBlankShotTargetHighlight.Count; i++)
			{
				pointBlankShotTargetHighlight[i].Clear();
			}
			pointBlankShotTargetHighlight = null;
		}
	}

	private void CheckForGridAttackTargetHighlight(AbilityModel selectedAbility, GridCoordinate mouseCoordinate)
	{
		if (selectedAbility.Definition.TriggerType == AbilityTriggerType.GridOrTarget && base.Combat.GetOccupier(mouseCoordinate) == null)
		{
			gridAttackTargetHighlight = base.GridView.AddHighlight(mouseCoordinate, Color.yellow, 1);
		}
	}

	private void ClearGridAttackTargetHighlight()
	{
		if (gridAttackTargetHighlight != null)
		{
			CacheableObject component = gridAttackTargetHighlight.gameObject.GetComponent<CacheableObject>();
			if (component != null)
			{
				component.Destroy();
			}
			gridAttackTargetHighlight = null;
		}
	}

	private void CheckEnemyFreeAttackRange(GridPath path, GridCoordinate targetCoordinate)
	{
		if (path == null || !path.IsValid)
		{
			return;
		}
		ActorModel controlledActor = base.PlayerInputManager.ControlledActor;
		FixedPoint value = 0.0;
		base.Combat.AbilityManager.VisitParameter("AbilityModifierCarolCannotAttackedChance", ref value, controlledActor);
		bool flag = value >= 1.0 && controlledActor.IsSneak;
		if (controlledActor.HasAnyLevelTrait("LeaderBuffForestStalker") || flag || controlledActor.HasAnyLevelTrait("LeaderBuffOneWithTheHerd") || controlledActor.IsInvisible || !controlledActor.CanReceiveOOT || HelpersModel.IsDodge)
		{
			return;
		}
		bool isDanger = false;
		List<ActorModel> enemyFactionsActors = base.Combat.GetEnemyFactionsActors(Faction.Survivor);
		List<int> list = new List<int>();
		foreach (ActorModel item in enemyFactionsActors)
		{
			list.Add(item.ModelId);
		}
		Helpers.ExecuteCommand(new EquipmentActiveLightResetCommand(controlledActor, list));
		Helpers.ExecuteCommand(new FreeOWResetCommand(controlledActor, list));
		for (int i = 0; i < enemyFactionsActors.Count; i++)
		{
			bool flag2 = enemyFactionsActors[i] == base.Combat.GetOccupier(targetCoordinate);
			bool flag3 = false;
			_ = (FixedPoint)0.0;
			if (enemyFactionsActors[i].Faction == Faction.Walker && enemyFactionsActors[i].HasTrait("FreeAttack") && enemyFactionsActors[i].CanPerformOOT)
			{
				int num = 0;
				foreach (GridCoordinate item2 in base.Combat.Grid.Neighbors(enemyFactionsActors[i].GridCoordinate))
				{
					if (path.Contains(item2) && base.Combat.GetOccupier(item2) == null && base.Combat.CanTraverse(null, item2, enemyFactionsActors[i].GridCoordinate) && item2 != path.End)
					{
						num++;
					}
				}
				flag3 = num >= 2;
			}
			if ((enemyFactionsActors[i].Faction == Faction.Walker || enemyFactionsActors[i].Faction == Faction.Raider) && (enemyFactionsActors[i].HasTrait("FreeAttack") || enemyFactionsActors[i].HasTrait("Overwatch")) && enemyFactionsActors[i].CanPerformOOT)
			{
				foreach (GridCoordinate item3 in base.Combat.Grid.Neighbors(enemyFactionsActors[i].GridCoordinate))
				{
					if (path.Contains(item3) && base.Combat.GetOccupier(item3) == null && base.Combat.CanTraverse(null, item3, enemyFactionsActors[i].GridCoordinate) && ((controlledActor.HasAnyLevelTrait("Equipment_Active_Light") && Helpers.ExecuteCommand(new EquipmentActiveLightCommand(controlledActor, enemyFactionsActors[i].ModelId)) == TWDModelResult.Error) || (controlledActor.HasTraitsThatContains("Equipment_Passive_FreeOW") && Helpers.ExecuteCommand(new FreeOWCommand(controlledActor, enemyFactionsActors[i].ModelId)) == TWDModelResult.Error)))
					{
						return;
					}
				}
			}
			bool activeLightState = enemyFactionsActors[i].GetActiveLightState();
			ActorView actorView = GameManager.Instance.GetViewForModel(enemyFactionsActors[i]) as ActorView;
			actorView.SetFreeAttackWarningVisualization(flag3);
			if (activeLightState)
			{
				actorView.SetFreeAttackWarningVisualization(enabled: false);
				if (flag2)
				{
					actorView.ShowHealthIndicator(visible: false);
					actorView.SetFreeAttackWarningVisualization(enabled: false);
				}
				else
				{
					actorView.ShowHealthIndicator(visible: true);
				}
			}
			else if (flag2)
			{
				actorView.ShowHealthIndicator(visible: false);
				actorView.SetFreeAttackWarningVisualization(enabled: false);
			}
			else
			{
				actorView.ShowHealthIndicator(visible: true);
			}
			if (flag3)
			{
				isDanger = true;
			}
		}
		path.IsDanger = isDanger;
	}

	private void ClearEnemyActivationRangeVisualization()
	{
		if (CombatView.Instance == null)
		{
			return;
		}
		List<ActorModel> enemyFactionsActors = base.Combat.GetEnemyFactionsActors(Faction.Survivor);
		for (int i = 0; i < enemyFactionsActors.Count; i++)
		{
			ActorView actorView = GameManager.Instance.GetViewForModel(enemyFactionsActors[i]) as ActorView;
			if (actorView != null)
			{
				actorView.SetActivationRangeVisualization(enabled: false);
			}
		}
	}

	private void ClearEnemyFreeAttackRangeVisualization()
	{
		if (CombatView.Instance == null)
		{
			return;
		}
		List<ActorModel> enemyFactionsActors = base.Combat.GetEnemyFactionsActors(Faction.Survivor);
		for (int i = 0; i < enemyFactionsActors.Count; i++)
		{
			ActorView actorView = GameManager.Instance.GetViewForModel(enemyFactionsActors[i]) as ActorView;
			if (actorView != null)
			{
				actorView.SetFreeAttackWarningVisualization(enabled: false);
			}
		}
	}

	private void ClearMoveActionIndicator()
	{
		if (CombatView.Instance == null)
		{
			return;
		}
		CombatView.Instance.CombatHUD.HideMoveActionIndicator();
		CombatView.Instance.CombatHUD.HideCoverMoveIndicator();
		List<ActorModel> enemyFactionsActors = base.Combat.GetEnemyFactionsActors(Faction.Survivor);
		for (int i = 0; i < enemyFactionsActors.Count; i++)
		{
			ActorView actorView = GameManager.Instance.GetViewForModel(enemyFactionsActors[i]) as ActorView;
			if (actorView != null)
			{
				actorView.ShowHealthIndicator(visible: true);
				actorView.RefreshUI(updateAll: true);
			}
		}
	}

	private void ClearExplosionRangeIndicators()
	{
		if (explosionRangeVisualizers != null)
		{
			for (int i = 0; i < explosionRangeVisualizers.Count; i++)
			{
				explosionRangeVisualizers[i].Clear();
			}
			explosionRangeVisualizers.Clear();
		}
	}

	private void CheckForPushDirectionIndicators(AbilityEffectPush push, List<ActorModel> actorsAffected, GridCoordinate mouseCoordinate, bool forceSourceFromMouseCoordinate = false)
	{
		if (mouseCoordinate == actorView.Model.GridCoordinate || push.IsDisablePushDirectionIndicators)
		{
			return;
		}
		foreach (ActorModel item in actorsAffected)
		{
			GridCoordinate coordinate = (forceSourceFromMouseCoordinate ? mouseCoordinate : ((push.ForceSourceFromProjectile && mouseCoordinate != item.GridCoordinate) ? mouseCoordinate : ((!path.IsValid || (push.ForceSourceFromProjectile && !actorView.IsMeleeWeaponEquipped)) ? actorView.Model.GridCoordinate : path.End)));
			FixedVec3 position = base.GridView.GetPosition(coordinate);
			FixedVec3 position2 = base.GridView.GetPosition(item.GridCoordinate);
			FixedVec3 fixedVec = FixedVec3.Normalize(position2 - position);
			FixedPoint radians = -FixedPoint.DegToRad(push.ForceAngle);
			fixedVec = new FixedVec3(FixedPoint.Cos(radians) * fixedVec.X - FixedPoint.Sin(radians) * fixedVec.Z, fixedVec.Y, FixedPoint.Cos(radians) * fixedVec.Z + FixedPoint.Sin(radians) * fixedVec.X);
			GridCoordinate coordinate2 = base.Combat.Grid.GetCoordinate(position2 + fixedVec * (push.Distance * base.Combat.Grid.CellSize.X));
			fixedVec = FixedVec3.Normalize(base.GridView.GetPosition(coordinate2) - position2);
			float y = 57.29578f * Mathf.Atan2((float)fixedVec.Z, (float)fixedVec.X) * -1f;
			if (pushDirectionIndicatorPrefab == null)
			{
				PrefabResource prefabResource = UnityUtils.LoadFromAssetBundle<PrefabResource>("Combat/PushDirectionIndicator", "scriptableobjects");
				pushDirectionIndicatorPrefab = prefabResource.GetPrefab();
			}
			GameObject gameObject = SingularityMonoBehaviour<ObjectPoolManager>.Instance.FetchObject(pushDirectionIndicatorPrefab, (GameManager.Instance.GetViewForModel(item) as ActorView).transform);
			gameObject.transform.position = position2.ToVector3();
			gameObject.transform.eulerAngles = new Vector3(0f, y, 0f);
			if (pushDirectionIndicators == null)
			{
				pushDirectionIndicators = new List<CacheableObject>();
			}
			pushDirectionIndicators.Add(gameObject.GetComponent<CacheableObject>());
		}
	}

	private void ClearPushDirectionIndicators()
	{
		if (pushDirectionIndicators == null)
		{
			return;
		}
		foreach (CacheableObject pushDirectionIndicator in pushDirectionIndicators)
		{
			pushDirectionIndicator.Destroy();
		}
		pushDirectionIndicators.Clear();
	}

	private bool CheckForFlareAbility(AbilityModel selectedAbility, GridCoordinate gridCoordinate, out bool attracted)
	{
		if (selectedAbility.Effects.Exists((AbilityEffect t) => t.GetType() == typeof(AbilityEffectFlareConsumable)))
		{
			if (baitCandidatesTargetHighlight == null)
			{
				baitCandidatesTargetHighlight = new List<GridTargetHighlight>();
			}
			foreach (ActorModel item in CombatHelpers.GetClosestWalkersToLure(base.Combat, gridCoordinate, preview: true))
			{
				if (base.Combat.IsGridCellVisibleByAnySurvivor(item.GridCoordinate))
				{
					baitCandidatesTargetHighlight.Add(base.GridView.AddHighlight(item.GridCoordinate, Color.red, 1));
				}
			}
			attracted = baitCandidatesTargetHighlight.Count > 0;
			return true;
		}
		attracted = false;
		return false;
	}

	private void ClearBaitCandidatesHighlights()
	{
		if (baitCandidatesTargetHighlight == null)
		{
			return;
		}
		for (int i = 0; i < baitCandidatesTargetHighlight.Count; i++)
		{
			CacheableObject component = baitCandidatesTargetHighlight[i].gameObject.GetComponent<CacheableObject>();
			if (component != null)
			{
				component.Destroy();
			}
		}
		baitCandidatesTargetHighlight.Clear();
	}

	private void CheckForDamageFalloff(Vector3 targetCellPosition, Vector3 movePosition, AbilityModel ability)
	{
		if (ability.IsConsumableAbility || !actorView)
		{
			return;
		}
		string text = actorView.Model?.GetWeaponEquipment()?.GetEquipmentActiveTraits()?.FirstOrDefault((string trait) => trait.Contains("RangedDamageFalloff"));
		if (!string.IsNullOrEmpty(text))
		{
			TraitDefinition traitDefinition = actorView.Model.gameEconomyData.GetTraitDefinition(text);
			AbilityDefinition definition = ability.Definition;
			Vector3 normalized = (targetCellPosition - movePosition).normalized;
			float num = (float)traitDefinition.GetParameter<int>(0) + (float)base.Combat.Grid.CellSize.X / 2f + 1f;
			Vector3 end = movePosition + normalized * num;
			if (damageFalloffVisualiser == null)
			{
				PrefabResource prefabResource = UnityUtils.LoadFromAssetBundle<PrefabResource>("Combat/WeaponRangeIndicator", "scriptableobjects");
				damageFalloffVisualiser = Object.Instantiate(prefabResource.GetPrefab()).GetComponent<WeaponRangeVisualization>();
				damageFalloffVisualiser.gameObject.name = "RangedDamageFalloffWeaponVisualizer";
				damageFalloffVisualiser.SetPointBlankIndicator();
			}
			damageFalloffVisualiser.transform.parent = actorView.transform;
			damageFalloffVisualiser.transform.localPosition = new Vector3(0f, 0f, 0f);
			if (definition.AbilityTargetArea == AbilityTargetAreaType.Line || definition.AbilityTargetArea == AbilityTargetAreaType.LineMax || definition.AbilityTargetArea == AbilityTargetAreaType.LineSeparated)
			{
				damageFalloffVisualiser.SetLine(movePosition, end, 0.5f);
				damageFalloffVisualiser.SetPointBlankIndicator();
			}
		}
	}

	private void ClearDamageFalloffVisualiser()
	{
		if ((bool)damageFalloffVisualiser)
		{
			damageFalloffVisualiser.Clear();
		}
	}

	private void ClearAbilityAttackRangeVisualizer()
	{
		if (abilityRangeVisualizer != null)
		{
			abilityRangeVisualizer.Clear();
		}
		ClearTridentSeparatedLineVisualizers();
	}

	private void ClearTridentSeparatedLineVisualizers()
	{
		if (tridentLeftLineVisualizer != null)
		{
			tridentLeftLineVisualizer.Clear();
		}
		if (tridentRightLineVisualizer != null)
		{
			tridentRightLineVisualizer.Clear();
		}
	}

	private bool TryDrawTridentSeparatedAttackLines(ActorModel source, AbilityModel ability, Vector3 movePosition, Vector3 aimWorldPosition, float lineWidth, bool abilityCanBeBlocked)
	{
		if (!AbilityRangeTridentSkill.ShouldApplySeparatedAttackLines(source, ability) || abilityRangeVisualizer == null)
		{
			ClearTridentSeparatedLineVisualizers();
			return false;
		}
		AbilityRangeTridentSkill activeSkill = AbilityRangeTridentSkill.GetActiveSkill(source);
		FixedPoint angleDegrees = AbilityRangeTridentSkill.ResolveSeparatedAngleDegrees(source, ability);
		FixedPoint value = 0L;
		FixedPoint value2 = 0L;
		if (activeSkill != null)
		{
			value = activeSkill.GetEffectiveMiddleExtraRange();
			value2 = activeSkill.GetEffectiveSideExtraRange();
		}
		else if (base.Combat.AbilityManager != null)
		{
			base.Combat.AbilityManager.VisitParameter("AbilityModifierLineSeparatedMiddleRangePlus", ref value, source);
			base.Combat.AbilityManager.VisitParameter("AbilityModifierLineSeparatedSideRangePlus", ref value2, source);
		}
		FixedVec3 sourcePos = movePosition.ToFixedVec3();
		FixedVec3 aimPos = aimWorldPosition.ToFixedVec3();
		AbilityRangeTridentSkill.GetSeparatedLineWorldEnds(base.Combat, ability, source, sourcePos, aimPos, angleDegrees, value, value2, out var middleEnd, out var leftEnd, out var rightEnd, activeSkill != null);
		EnsureTridentSeparatedLineVisualizers();
		if (tridentLeftLineVisualizer == null || tridentRightLineVisualizer == null)
		{
			return false;
		}
		float extra = (float)base.Combat.Grid.CellSize.X / 2f;
		Vector3 extendedTarget = ExtendWorldLineEnd(movePosition, middleEnd.ToVector3(), extra);
		Vector3 extendedTarget2 = ExtendWorldLineEnd(movePosition, leftEnd.ToVector3(), extra);
		Vector3 extendedTarget3 = ExtendWorldLineEnd(movePosition, rightEnd.ToVector3(), extra);
		DrawAbilityAimLine(abilityRangeVisualizer, source, movePosition, extendedTarget, lineWidth, abilityCanBeBlocked);
		DrawAbilityAimLine(tridentLeftLineVisualizer, source, movePosition, extendedTarget2, lineWidth, abilityCanBeBlocked);
		DrawAbilityAimLine(tridentRightLineVisualizer, source, movePosition, extendedTarget3, lineWidth, abilityCanBeBlocked);
		return true;
	}

	private void DrawAbilityAimLine(WeaponRangeVisualization visualizer, ActorModel source, Vector3 movePosition, Vector3 extendedTarget, float lineWidth, bool abilityCanBeBlocked)
	{
		if (!(visualizer == null))
		{
			GridCoordinate coordinate = base.Combat.Grid.GetCoordinate(movePosition.ToFixedVec3());
			GridCoordinate coordinate2 = base.Combat.Grid.GetCoordinate(extendedTarget.ToFixedVec3());
			GridCoordinate firstAimTrajectoryBlockCoordinate = base.Combat.GetFirstAimTrajectoryBlockCoordinate(coordinate, coordinate2, abilityCanBeBlocked, source);
			if (firstAimTrajectoryBlockCoordinate != GridCoordinate.Invalid)
			{
				visualizer.SetBrokenLine(movePosition, extendedTarget, base.Combat.Grid.GetPosition(firstAimTrajectoryBlockCoordinate).ToVector3(), lineWidth);
			}
			else
			{
				visualizer.SetLine(movePosition, extendedTarget, lineWidth);
			}
		}
	}

	private void CollectAimTrajectoryBlockCenters(ActorModel source, GridCoordinate shooterCoordinate, List<ActorModel> actorsAffected, bool abilityCanBeBlocked, List<Vector3> blockedCenters)
	{
		if (actorsAffected == null || blockedCenters == null)
		{
			return;
		}
		for (int i = 0; i < actorsAffected.Count; i++)
		{
			ActorModel actorModel = actorsAffected[i];
			if (actorModel != null && actorModel != source && ((actorModel.HasDamageAreaBlock && actorModel.IsEnemy(source)) || (abilityCanBeBlocked && actorModel.IsImpenetrable)))
			{
				GridCoordinate coordinate = (actorModel.IsMultiCell ? actorModel.GetClosestOccupiedCell(shooterCoordinate) : actorModel.GridCoordinate);
				blockedCenters.Add(base.Combat.Grid.GetPosition(coordinate).ToVector3());
			}
		}
	}

	private static Vector3 ExtendWorldLineEnd(Vector3 start, Vector3 end, float extra)
	{
		Vector3 vector = end - start;
		vector.y = 0f;
		float magnitude = vector.magnitude;
		if (magnitude <= 0.0001f)
		{
			return end;
		}
		return start + vector / magnitude * (magnitude + extra);
	}

	private void EnsureTridentSeparatedLineVisualizers()
	{
		WeaponRangeVisualization weaponRangeVisualization = abilityRangeVisualizer;
		if (!(weaponRangeVisualization == null) && !(actorView == null))
		{
			if (tridentLeftLineVisualizer == null)
			{
				tridentLeftLineVisualizer = CreateTridentSideLineVisualizer(weaponRangeVisualization, "TridentLeftLineVisualizer");
			}
			if (tridentRightLineVisualizer == null)
			{
				tridentRightLineVisualizer = CreateTridentSideLineVisualizer(weaponRangeVisualization, "TridentRightLineVisualizer");
			}
			tridentLeftLineVisualizer.transform.parent = actorView.transform;
			tridentLeftLineVisualizer.transform.localPosition = new Vector3(0f, 0f, 0f);
			tridentLeftLineVisualizer.SetPointBlankIndicator();
			tridentRightLineVisualizer.transform.parent = actorView.transform;
			tridentRightLineVisualizer.transform.localPosition = new Vector3(0f, 0f, 0f);
			tridentRightLineVisualizer.SetPointBlankIndicator();
		}
	}

	private WeaponRangeVisualization CreateTridentSideLineVisualizer(WeaponRangeVisualization template, string name)
	{
		WeaponRangeVisualization component = Object.Instantiate(template.gameObject).GetComponent<WeaponRangeVisualization>();
		component.gameObject.name = name;
		component.transform.parent = actorView.transform;
		component.transform.localPosition = Vector3.zero;
		component.transform.localScale = Vector3.one;
		component.transform.rotation = Quaternion.identity;
		component.Clear();
		return component;
	}

	private void CheckSuppressTrait(GridPath path, GridCoordinate mouseCoordinate)
	{
		if (!path.HasTargetCoordinate)
		{
			return;
		}
		ActorModel controlledActor = base.PlayerInputManager.ControlledActor;
		List<string> list = controlledActor?.SelectedEquipment?.GetEquipmentActiveTraits();
		string text = list?.FirstOrDefault((string trait) => trait.Contains("Equipment_Active_Suppress_1"));
		if (string.IsNullOrEmpty(text))
		{
			text = list?.FirstOrDefault((string trait) => trait.Contains("Equipment_Active_Suppress_2"));
		}
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		Vector3 start = base.GridView.GetPosition(mouseCoordinate).ToVector3();
		ActorModel occupier = base.Combat.GetOccupier(mouseCoordinate);
		if (occupier != null && !controlledActor.IsEnemy(occupier))
		{
			return;
		}
		TraitDefinition traitDefinition = GameManager.Instance.gameEconomyData.GetTraitDefinition(text);
		if (traitDefinition != null)
		{
			if (suppressTraitVisualiser == null)
			{
				PrefabResource prefabResource = UnityUtils.LoadFromAssetBundle<PrefabResource>("Combat/WeaponRangeIndicator", "scriptableobjects");
				suppressTraitVisualiser = Object.Instantiate(prefabResource.GetPrefab()).GetComponent<WeaponRangeVisualization>();
				suppressTraitVisualiser.gameObject.name = "SuppressTraitVisualizer";
			}
			suppressTraitVisualiser.transform.parent = actorView.transform;
			suppressTraitVisualiser.transform.localPosition = new Vector3(0f, 0f, 0f);
			float radius = (float)traitDefinition.GetParameter<int>(0) * (float)base.Combat.Grid.CellSize.X + (float)base.Combat.Grid.CellSize.X / 2f;
			suppressTraitVisualiser.SetCircle(start, radius);
			suppressTraitVisualiser.SetSuppressIndicator();
		}
	}

	private void ClearSuppressTraitVisualiser()
	{
		if (suppressTraitVisualiser != null)
		{
			suppressTraitVisualiser.Clear();
		}
	}

	private bool IsAreaAngleEffective(AbilityDefinition definition)
	{
		bool result = false;
		AbilityTargetAreaType abilityTargetArea = definition.AbilityTargetArea;
		if (abilityTargetArea == AbilityTargetAreaType.Cone || (uint)(abilityTargetArea - 5) <= 1u)
		{
			result = true;
		}
		return result;
	}

	private void ForceUpdateValidTargets()
	{
		ActorModel controlledActor = base.PlayerInputManager.ControlledActor;
		if (controlledActor != null)
		{
			UpdateValidTargets(controlledActor.GridCoordinate, CombatHelpers.GetMoveRange(controlledActor));
		}
	}
}
