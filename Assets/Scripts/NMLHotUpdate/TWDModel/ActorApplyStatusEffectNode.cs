using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class ActorApplyStatusEffectNode : NodeBase
	{
		[GraphItVariable("Turns")]
		public int Turns = 1;

		[GraphItVariable("StatusEffect")]
		public TimedEffectType Effect;

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

		public ActorApplyStatusEffectNode()
		{
		}

		public ActorApplyStatusEffectNode(ActorApplyStatusEffectNode node)
			: base(node)
		{
			Turns = node.Turns;
			Effect = node.Effect;
		}

		public override NodeBase RecordValue()
		{
			return new ActorApplyStatusEffectNode(this);
		}

		[GraphItInput("Give Status Effect", "")]
		public void GiveStatusEffect()
		{
			List<ActorModel> targetActors = TargetActors;
			if (targetActors != null)
			{
				for (int i = 0; i < targetActors.Count; i++)
				{
					ActorModel actorModel = targetActors[i];
					switch (Effect)
					{
					case TimedEffectType.Stun:
						actorModel.Stun(Turns, actorModel);
						break;
					case TimedEffectType.Root:
						actorModel.Root(Turns, actorModel);
						break;
					case TimedEffectType.Invisible:
						actorModel.SetInvisible(Turns, actorModel);
						break;
					case TimedEffectType.Crippled:
						actorModel.Cripple(Turns, actorModel);
						break;
					default:
						base.Manager.Debug.LogError($"Status Effect {Effect} not supported");
						break;
					case TimedEffectType.Herd:
						break;
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
