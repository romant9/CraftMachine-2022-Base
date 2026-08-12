using System;
using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class ActorNode : NodeBase
	{
		public int ActorTagHash;

		public Faction ActorFaction;

		public bool ActorIsBoss;

		private int PreviousTurnEndFired = -1;

		[GraphItExportData("Actors", "")]
		[JsonIgnore]
		public List<ActorModel> Actors
		{
			get
			{
				CombatModel combat = base.manager.Player.Combat;
				if (combat != null)
				{
					if (ActorTagHash == 0)
					{
						return combat.GetFactionActors(ActorFaction, ActorIsBoss);
					}
					return combat.GetActorsWithTag(ActorTagHash, ActorFaction, ActorIsBoss);
				}
				return null;
			}
		}

		[GraphItExportData("Last Instigator", "")]
		[JsonIgnore]
		public ActorModel LastInstigator { get; set; }

		[GraphItExportData("Alive Count", "")]
		[JsonIgnore]
		public int AliveCount
		{
			get
			{
				int num = 0;
				List<ActorModel> actors = Actors;
				if (actors != null)
				{
					for (int i = 0; i < actors.Count; i++)
					{
						if (Actors[i] != null && !Actors[i].IsDead)
						{
							num++;
						}
					}
				}
				return num;
			}
		}

		[GraphItExportData("Health Points", "")]
		[JsonIgnore]
		public int HealthPoints
		{
			get
			{
				int num = 0;
				List<ActorModel> actors = Actors;
				if (actors != null)
				{
					for (int i = 0; i < actors.Count; i++)
					{
						if (Actors[i] != null && !Actors[i].IsDead)
						{
							num += Actors[i].Hitpoints;
						}
					}
				}
				return num;
			}
		}

		public ActorNode()
		{
		}

		public ActorNode(ActorNode node)
			: base(node)
		{
			ActorTagHash = node.ActorTagHash;
			ActorFaction = node.ActorFaction;
			ActorIsBoss = node.ActorIsBoss;
			PreviousTurnEndFired = node.PreviousTurnEndFired;
			LastInstigator = node.LastInstigator;
		}

		public override NodeBase RecordValue()
		{
			return new ActorNode(this);
		}

		public override void Start()
		{
			base.Start();
			List<ActorModel> actors = Actors;
			for (int i = 0; i < actors.Count; i++)
			{
				base.manager.RegisterDelayedEventListener(actors[i], OnActorModelChanged);
			}
			base.manager.RegisterDelayedEventListener(base.manager.Player.Combat, OnCombatModelChanged);
			base.manager.CombatModel.TurnManager.ActorChanged += OnSelectedActorChanged;
		}

		public override void ClearListener()
		{
			base.ClearListener();
			List<ActorModel> actors = Actors;
			for (int i = 0; i < actors.Count; i++)
			{
				base.manager.UnregisterDelayedEventListener(actors[i], OnActorModelChanged);
			}
			base.manager.UnregisterDelayedEventListener(base.manager.Player.Combat, OnCombatModelChanged);
			base.manager.CombatModel.TurnManager.ActorChanged -= OnSelectedActorChanged;
		}

		private void OnSelectedActorChanged(ActorModel actor)
		{
			if (Actors != null && Actors.Contains(actor))
			{
				Selected();
			}
		}

		private void OnActorModelChanged(ModelObject m, string changed, object args)
		{
			LastInstigator = m as ActorModel;
			if (LastInstigator == null)
			{
				return;
			}
			switch (changed)
			{
			case "actorMoveCompleted":
				Moved();
				break;
			case "actorAbilityCompleted":
			case "actorSecondMoveCompleted":
			{
				CombatModel combat = base.manager.Player.Combat;
				if (combat.TurnManager.TurnCount != PreviousTurnEndFired)
				{
					TurnEnded();
					PreviousTurnEndFired = combat.TurnManager.TurnCount;
				}
				break;
			}
			case "actorKilledEvent":
				Killed();
				break;
			case "actorStruggleSaved":
				StruggleSaved();
				break;
			case "actorTimedEffectStart":
				if (args is TimedEffect timedEffect)
				{
					if (timedEffect.Type == TimedEffectType.Struggle)
					{
						Struggled();
					}
					else if (timedEffect.Type == TimedEffectType.Stun)
					{
						Stunned();
					}
				}
				break;
			case "damageDealt":
				Damaged();
				break;
			}
		}

		private void OnCombatModelChanged(ModelObject m, string changed, object args)
		{
			if (changed == "actorCreated")
			{
				ActorModel actorModel = args as ActorModel;
				bool num = actorModel.ActorTag == ActorTagHash || ActorTagHash == 0;
				bool flag = actorModel.Faction == ActorFaction || ActorFaction == Faction.Any;
				bool flag2 = !ActorIsBoss || actorModel.IsBoss;
				if (num && flag && flag2)
				{
					base.manager.RegisterDelayedEventListener(actorModel, OnActorModelChanged);
				}
			}
		}

		[GraphItInput("Enable AI", "")]
		public void EnableAI()
		{
			List<ActorModel> actors = Actors;
			if (actors != null)
			{
				for (int i = 0; i < actors.Count; i++)
				{
					actors[i].AIController.Enabled = true;
				}
			}
			Modified();
		}

		[GraphItInput("Disable AI", "")]
		public void DisableAI()
		{
			List<ActorModel> actors = Actors;
			if (actors != null)
			{
				for (int i = 0; i < actors.Count; i++)
				{
					actors[i].AIController.Enabled = false;
				}
			}
			Modified();
		}

		[GraphItInput("Enable Ctrl", "")]
		public void Enable()
		{
			List<ActorModel> actors = Actors;
			if (actors != null)
			{
				for (int i = 0; i < actors.Count; i++)
				{
					actors[i].SetUserCanControl(value: true);
				}
			}
			Modified();
		}

		[GraphItInput("New Turn", "")]
		public void NewTurn()
		{
			List<ActorModel> actors = Actors;
			if (actors != null)
			{
				for (int i = 0; i < actors.Count; i++)
				{
					if (!actors[i].UserCanControl)
					{
						actors[i].SetUserCanControl(value: true);
					}
					actors[i].NewTurn();
				}
			}
			Modified();
		}

		[GraphItInput("Disable Ctrl", "")]
		public void Disable()
		{
			List<ActorModel> actors = Actors;
			if (actors != null)
			{
				for (int i = 0; i < actors.Count; i++)
				{
					actors[i].SetUserCanControl(value: false, "ActorNode.Disable");
					actors[i].EndAction();
				}
			}
			Modified();
		}

		[GraphItInput("Select", "")]
		public void Select()
		{
			List<ActorModel> actors = Actors;
			if (actors != null)
			{
				for (int i = 0; i < actors.Count; i++)
				{
					if (actors[i].UserCanControl && !actors[i].TurnComplete)
					{
						base.manager.Player.Combat.TurnManager.SelectActor(actors[i]);
						break;
					}
				}
			}
			Modified();
		}

		[GraphItInput("End Turn", "")]
		public void EndTurn()
		{
			List<ActorModel> actors = Actors;
			if (actors != null)
			{
				for (int i = 0; i < actors.Count; i++)
				{
					actors[i].EndAction();
				}
			}
			Modified();
		}

		[GraphItInput("Kill", "")]
		public void Kill()
		{
			List<ActorModel> actors = Actors;
			if (actors != null)
			{
				for (int i = 0; i < actors.Count; i++)
				{
					actors[i].Kill();
				}
			}
			Modified();
		}

		[GraphItOutput("Damaged", "")]
		public void Damaged()
		{
			Fire("Damaged");
		}

		[GraphItOutput("Modified", "")]
		public void Modified()
		{
			Fire("Modified");
		}

		[GraphItOutput("Moved", "")]
		public void Moved()
		{
			Fire("Moved");
		}

		[GraphItOutput("Struggle Saved", "")]
		public void StruggleSaved()
		{
			Fire("Struggle Saved");
		}

		[GraphItOutput("Struggled", "")]
		public void Struggled()
		{
			Fire("Struggled");
		}

		[GraphItOutput("Stunned", "")]
		public void Stunned()
		{
			Fire("Stunned");
		}

		[GraphItOutput("Turn Ended", "")]
		public void TurnEnded()
		{
			Fire("Turn Ended");
		}

		[GraphItOutput("Killed", "")]
		public void Killed()
		{
			Fire("Killed");
		}

		[GraphItOutput("Selected", "")]
		public void Selected()
		{
			Fire("Selected");
		}
	}
}
