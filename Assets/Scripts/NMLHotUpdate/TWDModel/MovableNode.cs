using System;
using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class MovableNode : NodeBase
	{
		[IgnoreModelProperty]
		public MovableModel MovableModel { get; set; }

		[JsonIgnore]
		[GraphItExportData("Last Instigator", "Actor that performed the last operation on this door.")]
		public ActorModel LastInstigator { get; set; }

		[JsonIgnore]
		[GraphItImportData("Instigator", "Actor that performs the operation on this door.")]
		public ActorModel Instigator
		{
			get
			{
				object obj = Import("Instigator");
				if (obj is ActorModel result)
				{
					return result;
				}
				if (obj is List<ActorModel> { Count: >0 } list)
				{
					return list[0];
				}
				return null;
			}
		}

		public MovableNode()
		{
		}

		public MovableNode(MovableNode node)
			: base(node)
		{
			MovableModel = node.MovableModel;
			LastInstigator = node.LastInstigator;
		}

		public override NodeBase RecordValue()
		{
			return new MovableNode(this);
		}

		public override void Start()
		{
			MovableModel movableModel = MovableModel;
			MovableModel = null;
			base.Start();
			MovableModel = movableModel;
			base.manager.RegisterDelayedEventListener(MovableModel, OnMovableChanged);
		}

		public override void ClearListener()
		{
			base.ClearListener();
			base.manager.UnregisterDelayedEventListener(MovableModel, OnMovableChanged);
		}

		private void OnMovableChanged(ModelObject model, string changed, object args)
		{
			if (changed == "IsMoved")
			{
				LastInstigator = args as ActorModel;
				if (MovableModel.IsMoved)
				{
					Moved();
				}
			}
		}

		[GraphItInput("Move", "Move movable.")]
		public void Move()
		{
			if (!MovableModel.IsMoved && Instigator != null)
			{
				if (MovableModel.CheckClearance())
				{
					MovableModel.Move(Instigator);
				}
				else
				{
					Failed();
				}
			}
		}

		[GraphItInput("Reset", "Reset state, sets movable to be in not moved state.")]
		public void Reset()
		{
			if (MovableModel.IsMoved)
			{
				MovableModel.Reset();
			}
		}

		[GraphItOutput("Moved", "Movable object was moved.")]
		public void Moved()
		{
			Fire("Moved");
		}

		[GraphItOutput("Failed", "Failed to move object.")]
		public void Failed()
		{
			Fire("Failed");
		}
	}
}
