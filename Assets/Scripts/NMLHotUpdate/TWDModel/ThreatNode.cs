using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class ThreatNode : NodeBase
	{
		[JsonIgnore]
		[GraphItExportData("ThreatLevel", "")]
		public int CurrentThreatLevel => base.manager.Player.Combat.ThreatMeter.ThreatLevel;

		[JsonIgnore]
		[GraphItImportData("Value", "")]
		public int Value
		{
			get
			{
				object obj = Import("Value");
				if (obj != null)
				{
					return (int)obj;
				}
				return 0;
			}
		}

		public ThreatNode()
		{
		}

		public ThreatNode(ThreatNode node)
			: base(node)
		{
		}

		public override NodeBase RecordValue()
		{
			return new ThreatNode(this);
		}

		[GraphItInput("Trigger Threat", "Trigger accumulated threat.")]
		public void TriggerThreat()
		{
			CombatModel combat = base.manager.Player.Combat;
			if (combat != null)
			{
				combat.ThreatMeter.TriggerWaveImmediately();
				OnTriggered();
			}
		}

		[GraphItInput("Set Threat", "Read Value and assign to threat level")]
		public void SetThreatLevel()
		{
			CombatModel combat = base.manager.Player.Combat;
			if (combat != null)
			{
				int value = Value - combat.ThreatMeter.ThreatLevel;
				combat.ThreatMeter.ChangeThreatLevel(value, ThreatInstigator.TurnCount);
				combat.NotifyChange("threatMeterValueChanged", combat.ThreatMeter.ThreatLevel);
				OnThreatChanged();
			}
		}

		[GraphItInput("Set Turn Count", "Read Value and assign to turn count")]
		public void SetThreatTurnCount()
		{
			CombatModel combat = base.manager.Player.Combat;
			if (combat != null)
			{
				combat.ThreatMeter.SetTurnCount(Value);
				combat.NotifyChange("threatMeterValueChanged", combat.ThreatMeter.ThreatLevel);
				OnTurnCountChanged();
			}
		}

		[GraphItOutput("Triggered", "")]
		public void OnTriggered()
		{
			Fire("Triggered");
		}

		[GraphItOutput("ThreatChanged", "")]
		public void OnThreatChanged()
		{
			Fire("ThreatChanged");
		}

		[GraphItOutput("CountChanged", "")]
		public void OnTurnCountChanged()
		{
			Fire("CountChanged");
		}
	}
}
