using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class FactionChangeNode : NodeBase
	{
		[GraphItVariable("")]
		public Faction FactionToCheck;

		[GraphItVariable("")]
		public int TurnToCheck = -1;

		public bool HasFiredZeroTurn;

		[JsonIgnore]
		[GraphItExportData("Current Turn", "")]
		public int CurrentTurn => base.manager.Player.Combat.TurnManager.TurnCount;

		[JsonIgnore]
		[GraphItExportData("Current Faction", "")]
		public Faction CurrentFaction => base.manager.Player.Combat.TurnManager.ActiveFaction;

		public FactionChangeNode()
		{
		}

		public FactionChangeNode(FactionChangeNode node)
			: base(node)
		{
			FactionToCheck = node.FactionToCheck;
			TurnToCheck = node.TurnToCheck;
			HasFiredZeroTurn = node.HasFiredZeroTurn;
		}

		public override NodeBase RecordValue()
		{
			return new FactionChangeNode(this);
		}

		public override void Initialize()
		{
			base.Initialize();
			HasFiredZeroTurn = false;
		}

		public override void Start()
		{
			base.Start();
			base.manager.Player.Combat.TurnManager.FactionPostChanged += TurnManager_FactionPostChanged;
		}

		public override void Update()
		{
			if (!HasFiredZeroTurn)
			{
				HasFiredZeroTurn = true;
				FactionChanged();
			}
		}

		private void TurnManager_FactionPostChanged(Faction currentFaction, Faction newFaction)
		{
			if (currentFaction != newFaction)
			{
				FactionChanged();
			}
		}

		[GraphItOutput("Faction Changed", "")]
		public void FactionChanged()
		{
			bool num = TurnToCheck == -1 || CurrentTurn == TurnToCheck;
			bool flag = CurrentFaction == FactionToCheck || FactionToCheck == Faction.Any;
			if (num && flag)
			{
				Fire("Faction Changed");
			}
		}
	}
}
