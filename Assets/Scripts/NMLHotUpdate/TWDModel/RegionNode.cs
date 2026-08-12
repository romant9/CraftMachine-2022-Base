using System;
using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class RegionNode : NodeBase
	{
		[IgnoreModelProperty]
		public RegionModel Region { get; set; }

		[JsonIgnore]
		[GraphItImportData("Actors", "")]
		public List<ActorModel> Actors
		{
			get
			{
				List<object> list = ImportValues("Actors");
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

		[JsonIgnore]
		[GraphItExportData("Alive Count", "How many actors inside region.")]
		public int AliveCount
		{
			get
			{
				int num = 0;
				List<ActorModel> actors = Actors;
				if (actors != null && Region != null)
				{
					for (int i = 0; i < actors.Count; i++)
					{
						if (Region.Location.Coordinates.Contains(actors[i].GridCoordinate))
						{
							num++;
						}
					}
				}
				return num;
			}
		}

		[JsonIgnore]
		[GraphItExportData("Actors Inside", "Actors inside the region.")]
		public List<ActorModel> ActorsInside
		{
			get
			{
				List<ActorModel> list = new List<ActorModel>();
				List<ActorModel> actors = Actors;
				if (actors != null && Region != null)
				{
					for (int i = 0; i < actors.Count; i++)
					{
						if (Region.Location.Coordinates.Contains(actors[i].GridCoordinate))
						{
							list.Add(actors[i]);
						}
					}
				}
				return list;
			}
		}

		public RegionNode()
		{
		}

		public RegionNode(RegionNode node)
			: base(node)
		{
			Region = node.Region;
		}

		public override NodeBase RecordValue()
		{
			return new RegionNode(this);
		}
	}
}
