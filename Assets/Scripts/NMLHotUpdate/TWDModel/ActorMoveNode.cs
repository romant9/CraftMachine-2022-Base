using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class ActorMoveNode : NodeBase
	{
		public GridCoordinate TargetCoordinate;

		[GraphItVariable("Should actor end turn after this move.")]
		public bool EndTurn;

		[GraphItVariable("Should visualization be globally blocking.")]
		public bool GloballyBlocking;

		[JsonIgnore]
		[GraphItImportData("Target Actors", "")]
		public List<ActorModel> TargetActors => Import("Target Actors") as List<ActorModel>;

		public ActorMoveNode()
		{
		}

		public ActorMoveNode(ActorMoveNode node)
			: base(node)
		{
			TargetCoordinate = node.TargetCoordinate;
			GloballyBlocking = node.GloballyBlocking;
			EndTurn = node.EndTurn;
		}

		public override NodeBase RecordValue()
		{
			return new ActorMoveNode(this);
		}

		[GraphItInput("Move", "")]
		public void Move()
		{
			CombatModel combat = base.manager.Player.Combat;
			if (combat != null && combat.Grid.IsCoordinateValid(TargetCoordinate) && TargetActors != null && TargetActors.Count == 1)
			{
				ActorModel actorModel = TargetActors[0];
				if (actorModel != null)
				{
					GridPath gridPath = combat.FindPath(actorModel, actorModel.GridCoordinate, TargetCoordinate);
					if (gridPath.IsValid && MoveCommand.PerformActions(base.manager, actorModel, gridPath, GloballyBlocking))
					{
						Success();
						if (EndTurn)
						{
							actorModel.EndAction();
						}
						return;
					}
					if (EndTurn)
					{
						actorModel.EndAction();
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
