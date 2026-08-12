using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class ActorExplodeNode : NodeBase
	{
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

		public ActorExplodeNode()
		{
		}

		public ActorExplodeNode(ActorExplodeNode node)
			: base(node)
		{
		}

		public override NodeBase RecordValue()
		{
			return new ActorExplodeNode(this);
		}

		[GraphItInput("Explode", "")]
		public void Explode()
		{
			List<ActorModel> targetActors = TargetActors;
			for (int i = 0; i < (targetActors?.Count ?? 0); i++)
			{
				targetActors[i].Explode("Explosive", "LeaderBuffExplosiveBullets");
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
