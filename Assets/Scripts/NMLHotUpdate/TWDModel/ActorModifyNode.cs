using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class ActorModifyNode : NodeBase
	{
		[GraphItImportData("Target Actors", "")]
		[JsonIgnore]
		public List<ActorModel> TargetActors
		{
			get
			{
				List<object> list = ImportValues("Target Actors");
				if (list != null)
				{
					List<ActorModel> list2 = new List<ActorModel>();
					for (int i = 0; i < list.Count; i++)
					{
						object obj = list[i];
						if (obj != null)
						{
							if (obj is List<ActorModel> collection)
							{
								list2.AddRange(collection);
							}
							else if (obj is ActorModel item)
							{
								list2.Add(item);
							}
						}
					}
					return list2;
				}
				return null;
			}
		}

		public ActorModifyNode()
		{
		}

		public ActorModifyNode(ActorModifyNode node)
			: base(node)
		{
		}

		public override NodeBase RecordValue()
		{
			return new ActorModifyNode(this);
		}

		[GraphItInput("Enable AI", "")]
		public void EnableAI()
		{
			List<ActorModel> targetActors = TargetActors;
			if (targetActors != null)
			{
				for (int i = 0; i < targetActors.Count; i++)
				{
					targetActors[i].AIController.Enabled = true;
				}
			}
			Out();
		}

		[GraphItInput("Disable AI", "")]
		public void DisableAI()
		{
			List<ActorModel> targetActors = TargetActors;
			if (targetActors != null)
			{
				for (int i = 0; i < targetActors.Count; i++)
				{
					targetActors[i].AIController.Enabled = false;
				}
			}
			Out();
		}

		[GraphItInput("Enable Ctrl", "")]
		public void Enable()
		{
			List<ActorModel> targetActors = TargetActors;
			if (targetActors != null)
			{
				for (int i = 0; i < targetActors.Count; i++)
				{
					targetActors[i].SetUserCanControl(value: true);
				}
			}
			Out();
		}

		[GraphItInput("New Turn", "")]
		public void NewTurn()
		{
			List<ActorModel> targetActors = TargetActors;
			if (targetActors != null)
			{
				for (int i = 0; i < targetActors.Count; i++)
				{
					if (!targetActors[i].UserCanControl)
					{
						targetActors[i].SetUserCanControl(value: true);
					}
					targetActors[i].NewTurn();
				}
			}
			Out();
		}

		[GraphItInput("Give AP", "")]
		public void GiveAP()
		{
			List<ActorModel> targetActors = TargetActors;
			if (targetActors != null)
			{
				for (int i = 0; i < targetActors.Count; i++)
				{
					targetActors[i].EnsureExtraAction("", dueToLuck: false);
				}
			}
			Out();
		}

		[GraphItInput("Disable Ctrl", "")]
		public void Disable()
		{
			List<ActorModel> targetActors = TargetActors;
			if (targetActors != null)
			{
				for (int i = 0; i < targetActors.Count; i++)
				{
					targetActors[i].SetUserCanControl(value: false, "ActorModifyNode.Disable");
					targetActors[i].EndAction();
				}
			}
			Out();
		}

		[GraphItInput("End Turn", "")]
		public void EndTurn()
		{
			List<ActorModel> targetActors = TargetActors;
			if (targetActors != null)
			{
				for (int i = 0; i < targetActors.Count; i++)
				{
					targetActors[i].EndAction();
				}
			}
			Out();
		}

		[GraphItInput("Kill", "")]
		public void Kill()
		{
			List<ActorModel> targetActors = TargetActors;
			if (targetActors != null)
			{
				for (int i = 0; i < targetActors.Count; i++)
				{
					targetActors[i].Kill();
				}
			}
			Out();
		}

		[GraphItInput("Give Charge Point", "")]
		public void GiveChargePoint()
		{
			List<ActorModel> targetActors = TargetActors;
			if (targetActors != null)
			{
				for (int i = 0; i < targetActors.Count; i++)
				{
					if (targetActors[i].ChargeMeter != null)
					{
						targetActors[i].AddChargePoints(1);
					}
				}
			}
			Out();
		}

		[GraphItOutput("Out", "")]
		public void Out()
		{
			Fire("Out");
		}
	}
}
