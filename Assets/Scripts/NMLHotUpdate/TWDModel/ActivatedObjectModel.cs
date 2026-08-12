using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ActivatedObjectModel : TWDSpatialModelObject
	{
		private int turnActivated;

		private int activationsLeft;

		private Dictionary<string, int> activationChances;

		public ActivatedObjectType ActivationType { get; set; }

		public ThreatState ActivationThreatState { get; set; }

		public int ActivationDelay { get; set; }

		public ActivatedObjectState State { get; set; }

		public FixedPoint AwokenPercentage => (FixedPoint)(long)(State - 2) / (FixedPoint)4.0;

		[JsonIgnore]
		public bool Awoken
		{
			get
			{
				if (State != ActivatedObjectState.AwokenLow && State != ActivatedObjectState.AwokenMedium && State != ActivatedObjectState.AwokenHigh && State != ActivatedObjectState.AwokenFull)
				{
					return State == ActivatedObjectState.Active;
				}
				return true;
			}
		}

		[JsonIgnore]
		public bool Active => State == ActivatedObjectState.Active;

		public int ActivationCount { get; set; }

		public int ActivationRate { get; set; }

		[JsonIgnore]
		public bool Deactivated => State == ActivatedObjectState.Deactivated;

		[JsonIgnore]
		public bool IsEmpty => State == ActivatedObjectState.Empty;

		public void Activate()
		{
			State = ActivatedObjectState.Active;
		}

		public void Deactivate()
		{
			State = ActivatedObjectState.Deactivated;
		}

		public ActivatedObjectModel()
		{
		}

		public ActivatedObjectModel(string viewId)
		{
			base.ViewId = viewId;
		}

		public void IncreaseState()
		{
			if (State >= ActivatedObjectState.Dormant && State < ActivatedObjectState.AwokenFull)
			{
				State++;
			}
		}

		public void SetState(ActivatedObjectState state)
		{
			State = state;
		}

		public void Reset()
		{
			State = ActivatedObjectState.Dormant;
			turnActivated = 0;
			activationsLeft = ActivationCount;
		}

		public override void Initialize()
		{
			base.Initialize();
			Reset();
		}

		public override void Start()
		{
			base.Start();
			activationChances = new Dictionary<string, int>();
			foreach (NoiseActivatedObjectData noiseActivatedObject in base.manager.GameEconomyData.NoiseActivatedObjects)
			{
				activationChances.Add(noiseActivatedObject.State, noiseActivatedObject.ActivationChance);
			}
			if (ActivationType == ActivatedObjectType.Instant && State == ActivatedObjectState.Dormant)
			{
				turnActivated = -1;
				Activate();
				CheckAction();
			}
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
		}

		public override bool IsValid()
		{
			return base.manager.Player.Grid.IsCoordinateValid(base.Location.Coordinate);
		}

		public void CheckAction()
		{
			if (Awoken && !Active)
			{
				CheckActivationChance();
			}
			if (ActivationType == ActivatedObjectType.Delayed && base.manager.Player.Combat.TurnManager.TurnCount == ActivationDelay && !IsEmpty)
			{
				Activate();
			}
			if (!Active || activationsLeft <= 0)
			{
				return;
			}
			int turnCount = base.manager.Player.Combat.TurnManager.TurnCount;
			if (turnCount - turnActivated >= ActivationRate)
			{
				if (DoAction(null))
				{
					turnActivated = turnCount;
					activationsLeft--;
				}
				if (activationsLeft <= 0)
				{
					State = ActivatedObjectState.Empty;
				}
			}
		}

		public virtual bool DoAction(ActorModel instigator)
		{
			return true;
		}

		private void CheckActivationChance()
		{
			int num = base.manager.CombatModel.RollCombatDice(RollDiceType.ActivateChance, 100);
			int num2 = 0;
			string name = Enum.GetName(typeof(ActivatedObjectState), State);
			if (activationChances.ContainsKey(name))
			{
				num2 = activationChances[name];
			}
			if (num <= num2)
			{
				turnActivated = base.manager.CombatModel.TurnManager.TurnCount;
				Activate();
			}
		}
	}
}
