using System.Collections.Generic;

namespace TWDModel
{
	public class CommonwealthArmorTrait : ActionModifier
	{
		private bool immuneForThisTurn;

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			TraitEntry trait = actor.TraitContainer.GetTrait("CommonwealthArmorActive");
			if (trait == null)
			{
				return ActionListClearFlag.Keep;
			}
			if (!(action is ChangeTurnAction))
			{
				if (!(action is StunAction stunAction))
				{
					if (!(action is BleedingOutAction bleedingOutAction))
					{
						if (!(action is RootAction rootAction))
						{
							if (!(action is CrippleAction crippleAction))
							{
								if (!(action is BurningOutAction burningOutAction))
								{
									if (!(action is StaggerAction staggerAction))
									{
										if (!(action is StruggleAction struggleAction))
										{
											if (!(action is DamageAction damageAction))
											{
												if (!(action is ElectricShockAction electricShockAction))
												{
													if (action is QuantunAction quantunAction && actor == quantunAction.TargetActor)
													{
														quantunAction.Avoided = true;
													}
												}
												else if (actor == electricShockAction.TargetActor)
												{
													electricShockAction.Avoided = true;
												}
											}
											else if (immuneForThisTurn)
											{
												damageAction.Dodged = true;
											}
										}
										else if (actor == struggleAction.Target)
										{
											struggleAction.Avoided = true;
											DropToRedHealthIfNecessary(actor);
										}
									}
									else if (actor == staggerAction.TargetActor)
									{
										staggerAction.Avoided = true;
									}
								}
								else if (actor == burningOutAction.TargetActor)
								{
									burningOutAction.Avoided = true;
									DropToRedHealthIfNecessary(actor);
								}
							}
							else if (actor == crippleAction.TargetActor)
							{
								crippleAction.Avoided = true;
							}
						}
						else if (actor == rootAction.TargetActor)
						{
							rootAction.Avoided = true;
						}
					}
					else if (actor == bleedingOutAction.Target)
					{
						bleedingOutAction.Avoided = true;
						DropToRedHealthIfNecessary(actor);
					}
				}
				else if (actor == stunAction.TargetActor)
				{
					stunAction.Avoided = true;
				}
			}
			else
			{
				immuneForThisTurn = false;
				if (actor.manager.CombatModel.TurnManager.ActiveFaction == actor.Faction)
				{
					trait.TraitDuration--;
					if (trait.TraitDuration <= 0)
					{
						actor.RemoveTrait("CommonwealthArmorActive");
					}
				}
			}
			return ActionListClearFlag.Keep;
		}

		private void DropToRedHealthIfNecessary(ActorModel actor)
		{
			if (actor.Hitpoints == actor.manager.GameEconomyData.ConfigData.StruggleBaseThreshold && actor.StrugglesLeft > 0)
			{
				actor.SetHitPoints(actor.MaxHitPoints, actor.MaxHitPoints);
				actor.OnRedHealthBar = true;
				actor.StrugglesLeft--;
				immuneForThisTurn = true;
			}
		}
	}
}
