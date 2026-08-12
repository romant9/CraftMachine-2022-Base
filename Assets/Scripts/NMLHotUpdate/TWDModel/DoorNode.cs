using System;
using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class DoorNode : NodeBase
	{
		[IgnoreModelProperty]
		public DoorModel DoorModel { get; set; }

		[IgnoreModelProperty]
		public CombatColliderModel CombatColliderModel { get; set; }

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

		public DoorNode()
		{
		}

		public DoorNode(DoorNode node)
			: base(node)
		{
			DoorModel = node.DoorModel;
			CombatColliderModel = node.CombatColliderModel;
			LastInstigator = node.LastInstigator;
		}

		public override NodeBase RecordValue()
		{
			return new DoorNode(this);
		}

		public override void Start()
		{
			DoorModel doorModel = DoorModel;
			DoorModel = null;
			base.Start();
			DoorModel = doorModel;
			base.manager.RegisterDelayedEventListener(DoorModel, OnDoorChanged);
		}

		public override void ClearListener()
		{
			base.ClearListener();
			base.manager.UnregisterDelayedEventListener(DoorModel, OnDoorChanged);
		}

		private void OnDoorChanged(ModelObject model, string changed, object args)
		{
			if (changed == "IsOpen")
			{
				LastInstigator = args as ActorModel;
				if (DoorModel.IsOpen)
				{
					Opened();
				}
				else
				{
					Closed();
				}
				Flipped();
				if (CombatColliderModel != null)
				{
					CombatColliderModel.OnTriggered(LastInstigator);
				}
			}
			else if (changed == "IsHidden")
			{
				if (DoorModel.IsHidden)
				{
					Hidden();
				}
				else
				{
					Shown();
				}
			}
		}

		[GraphItInput("Open", "Open door if it is closed. If door is open does nothing.")]
		public void Open()
		{
			if (!DoorModel.IsOpen)
			{
				DoorModel.FlipDoor(Instigator);
			}
		}

		[GraphItInput("Close", "Close door if it is open. If door is closed does nothing.")]
		public void Close()
		{
			if (DoorModel.IsOpen)
			{
				DoorModel.FlipDoor(Instigator);
			}
		}

		[GraphItInput("Hide", "Hide the door.")]
		public void Hide()
		{
			DoorModel.SetHidden(Instigator, hidden: true);
		}

		[GraphItInput("Show", "Show the door.")]
		public void Show()
		{
			DoorModel.SetHidden(Instigator, hidden: false);
		}

		[GraphItInput("Flip", "Flip door between open and closed states.")]
		public void Flip()
		{
			DoorModel.FlipDoor(Instigator);
		}

		[GraphItOutput("Opened", "Door was opened.")]
		public void Opened()
		{
			Fire("Opened");
		}

		[GraphItOutput("Closed", "Door was closed.")]
		public void Closed()
		{
			Fire("Closed");
		}

		[GraphItOutput("Shown", "Door was set to visible.")]
		public void Shown()
		{
			Fire("Shown");
		}

		[GraphItOutput("Hidden", "Door was set to invisible.")]
		public void Hidden()
		{
			Fire("Hidden");
		}

		[GraphItOutput("Flipped", "Door state changed between open and closed.")]
		public void Flipped()
		{
			Fire("Flipped");
		}
	}
}
