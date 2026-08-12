using System;
using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class ExplosiveNode : NodeBase
	{
		public ExplosiveModel ExplosiveModel { get; set; }

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

		public ExplosiveNode()
		{
		}

		public ExplosiveNode(ExplosiveNode node)
			: base(node)
		{
			ExplosiveModel = node.ExplosiveModel;
			LastInstigator = node.LastInstigator;
		}

		public override NodeBase RecordValue()
		{
			return new ExplosiveNode(this);
		}

		public override void Start()
		{
			ExplosiveModel explosiveModel = ExplosiveModel;
			ExplosiveModel = null;
			base.Start();
			ExplosiveModel = explosiveModel;
			ExplosiveModel.Changed += OnExplosiveChanged;
		}

		private void OnExplosiveChanged(ModelObject model, string changed, object args)
		{
			if (changed == "Exploded")
			{
				LastInstigator = args as ActorModel;
				if (ExplosiveModel.HasExploded)
				{
					Exploded();
				}
			}
		}

		[GraphItInput("Explode", "Explode explosive.")]
		public void Explode()
		{
			ExplosiveModel.Changed -= OnExplosiveChanged;
			ExplosiveModel.Explode(Instigator);
			ExplosiveModel.Changed += OnExplosiveChanged;
			Exploded();
		}

		[GraphItOutput("Exploded", "Explosive has exploded.")]
		public void Exploded()
		{
			Fire("Exploded");
		}
	}
}
