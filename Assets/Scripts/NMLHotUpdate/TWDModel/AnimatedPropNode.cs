using System;
using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class AnimatedPropNode : NodeBase
	{
		[GraphItVariable("Animation ID")]
		public string AnimationId = "";

		[GraphItVariable("Animation speed")]
		public float AnimationSpeed = 1f;

		[IgnoreModelProperty]
		public AnimatedPropModel AnimatedPropModel { get; set; }

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

		public AnimatedPropNode()
		{
		}

		public AnimatedPropNode(AnimatedPropNode node)
			: base(node)
		{
			AnimatedPropModel = node.AnimatedPropModel;
			CombatColliderModel = node.CombatColliderModel;
			AnimationId = node.AnimationId;
			AnimationSpeed = node.AnimationSpeed;
			LastInstigator = node.LastInstigator;
		}

		public override NodeBase RecordValue()
		{
			return new AnimatedPropNode(this);
		}

		public override void Start()
		{
			AnimatedPropModel animatedPropModel = AnimatedPropModel;
			AnimatedPropModel = null;
			base.Start();
			AnimatedPropModel = animatedPropModel;
			base.manager.RegisterDelayedEventListener(AnimatedPropModel, OnModelChanged);
		}

		public override void ClearListener()
		{
			base.ClearListener();
			base.manager.UnregisterDelayedEventListener(AnimatedPropModel, OnModelChanged);
		}

		private void OnModelChanged(ModelObject model, string changed, object args)
		{
			if (changed == "Animate")
			{
				LastInstigator = args as ActorModel;
				if (CombatColliderModel != null)
				{
					CombatColliderModel.OnTriggered(LastInstigator);
				}
			}
		}

		[GraphItInput("Animate", "Animate")]
		public void Animate()
		{
			AnimatedPropModel.Animate(Instigator, AnimationId, AnimationSpeed);
		}

		[GraphItOutput("Animated", "")]
		public void Animated()
		{
			Fire("Animated");
		}
	}
}
