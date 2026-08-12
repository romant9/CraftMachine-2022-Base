using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class AddTutorialTraitNode : NodeBase
	{
		[GraphItVariable("Trait ID")]
		public string TraitIdentifier = "";

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

		public AddTutorialTraitNode()
		{
		}

		public AddTutorialTraitNode(AddTutorialTraitNode node)
			: base(node)
		{
			TraitIdentifier = node.TraitIdentifier;
		}

		public override NodeBase RecordValue()
		{
			return new AddTutorialTraitNode(this);
		}

		[GraphItInput("Add Trait", "")]
		public void AddTrait()
		{
			List<ActorModel> actors = Actors;
			if (actors == null)
			{
				return;
			}
			for (int i = 0; i < actors.Count; i++)
			{
				TraitDefinition traitDefinition = actors[i].manager.GameEconomyData.GetTraitDefinition(TraitIdentifier);
				if (traitDefinition != null)
				{
					if (traitDefinition.HasTag("Tutorial"))
					{
						actors[i].AddTrait(TraitIdentifier);
					}
					else
					{
						actors[i].manager.Debug.LogError("Trying to give trait '" + TraitIdentifier + "' which does not have tag 'Tutorial'!");
					}
				}
				else
				{
					actors[i].manager.Debug.LogError("Trying to give trait '" + TraitIdentifier + "' which cannot be found from the trait definitions in GED!");
				}
			}
		}
	}
}
