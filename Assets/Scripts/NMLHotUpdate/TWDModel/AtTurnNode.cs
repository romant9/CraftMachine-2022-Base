using System;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class AtTurnNode : NodeBase
	{
		[GraphItVariable("Turn to activate this node. -1 means this node will signal 'New Turn' every time turn changes, using values from 0..inf will signal 'New Turn' only when that specified turn becomes active.")]
		public int TurnToCheck = -1;

		public bool HasFiredZeroTurn;

		[JsonIgnore]
		[GraphItExportData("Current Turn", "Current turn, can be used to read turn from logic nodes to perform logic at any point.")]
		public int CurrentTurn => base.manager.Player.Combat.TurnManager.TurnCount;

		public AtTurnNode()
		{
		}

		public AtTurnNode(AtTurnNode node)
			: base(node)
		{
			TurnToCheck = node.TurnToCheck;
			HasFiredZeroTurn = node.HasFiredZeroTurn;
		}

		public override NodeBase RecordValue()
		{
			return new AtTurnNode(this);
		}

		public override void Initialize()
		{
			base.Initialize();
			HasFiredZeroTurn = false;
		}

		public override void Start()
		{
			base.Start();
			base.manager.RegisterDelayedEventListener(base.manager.Player.Combat.TurnManager, TurnManager_OnChanged);
		}

		public override void ClearListener()
		{
			base.ClearListener();
			base.manager.UnregisterDelayedEventListener(base.manager.Player.Combat.TurnManager, TurnManager_OnChanged);
		}

		public override void Update()
		{
			if (!HasFiredZeroTurn)
			{
				HasFiredZeroTurn = true;
				NewTurn();
			}
		}

		private void TurnManager_OnChanged(ModelObject model, string changed, object args)
		{
			if (changed == "TurnCountChanged")
			{
				NewTurn();
			}
		}

		[GraphItOutput("New Turn", "This output pin fires every time turn changes while matching the 'TurnToCheck' property. -1 means this pin will fire every time turn changes and values from 0..inf will limit to that exact turn.")]
		public void NewTurn()
		{
			if (CurrentTurn == TurnToCheck || TurnToCheck == -1)
			{
				Fire("New Turn");
			}
		}
	}
}
