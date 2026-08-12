using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class ActorUseNode : NodeBase
	{
		[JsonIgnore]
		[GraphItImportData("Using Actor", "")]
		public ActorModel UsingActor
		{
			get
			{
				List<object> list = ImportValues("Using Actor");
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
					if (list2 == null || list2.Count <= 0)
					{
						return null;
					}
					return list2[0];
				}
				return null;
			}
		}

		[JsonIgnore]
		[GraphItImportData("Use Target", "")]
		public InteractiveObjectModel UseTarget => Import("Use Target") as InteractiveObjectModel;

		public ActorUseNode()
		{
		}

		public ActorUseNode(ActorUseNode node)
			: base(node)
		{
		}

		public override NodeBase RecordValue()
		{
			return new ActorUseNode(this);
		}

		[GraphItInput("Use", "")]
		public void Use()
		{
			CombatModel combat = base.manager.Player.Combat;
			ActorModel usingActor = UsingActor;
			InteractiveObjectModel useTarget = UseTarget;
			if (combat != null && usingActor != null && useTarget != null)
			{
				bool flag = false;
				TWDModelManager tWDModelManager = usingActor.manager;
				if (tWDModelManager.Player.Combat.CanUseInteractiveObject(usingActor, useTarget))
				{
					flag = tWDModelManager.ExecuteAction(new StartInteractiveObjectAction(usingActor, useTarget)) && tWDModelManager.ExecuteAction(new UseInteractiveObjectAction(usingActor, useTarget));
					if (flag)
					{
						usingActor.EndAction();
					}
				}
				if (flag)
				{
					Success();
					return;
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
