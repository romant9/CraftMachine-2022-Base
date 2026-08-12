using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class ActorChangeFactionNode : NodeBase
	{
		[GraphItVariable("Target faction to be set for actor(s).")]
		public Faction TargetFaction;

		[JsonIgnore]
		[GraphItImportData("Target Actors", "")]
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

		public ActorChangeFactionNode()
		{
		}

		public ActorChangeFactionNode(ActorChangeFactionNode node)
			: base(node)
		{
			TargetFaction = node.TargetFaction;
		}

		public override NodeBase RecordValue()
		{
			return new ActorChangeFactionNode(this);
		}

		[GraphItInput("Change Faction", "")]
		public void ChangeFaction()
		{
			bool flag = false;
			if (TargetFaction != Faction.Any)
			{
				List<ActorModel> targetActors = TargetActors;
				if (targetActors != null)
				{
					for (int i = 0; i < targetActors.Count; i++)
					{
						ActorModel actorModel = targetActors[i];
						if (actorModel.Faction != TargetFaction)
						{
							base.manager.CombatModel.ChangeActorFaction(actorModel, TargetFaction);
							flag = true;
						}
					}
				}
			}
			if (flag)
			{
				Success();
			}
			else
			{
				Fail();
			}
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
