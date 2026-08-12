using System;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class InteractiveObjectNode : NodeBase, InteractionReceiver
	{
		[IgnoreModelProperty]
		public InteractiveObjectModel InteractiveObjectModel { get; set; }

		[JsonIgnore]
		[GraphItImportData("Turns To Complete", "")]
		public int TurnsToComplete
		{
			get
			{
				object obj = Import("Turns To Complete");
				if (obj != null)
				{
					return (int)obj;
				}
				return 1;
			}
		}

		[JsonIgnore]
		[GraphItExportData("This", "")]
		public InteractiveObjectModel InteractiveObject => InteractiveObjectModel;

		[JsonIgnore]
		[GraphItExportData("Last Instigator", "")]
		public ActorModel LastInstigator { get; set; }

		[GraphItExportData("Interaction Completed", "")]
		public bool InteractionCompleted { get; set; }

		public InteractiveObjectNode()
		{
		}

		public InteractiveObjectNode(InteractiveObjectNode node)
			: base(node)
		{
			InteractiveObjectModel = node.InteractiveObjectModel;
			LastInstigator = node.LastInstigator;
			InteractionCompleted = node.InteractionCompleted;
		}

		public override NodeBase RecordValue()
		{
			return new InteractiveObjectNode(this);
		}

		public override void Start()
		{
			base.Start();
			LastInstigator = null;
			InteractionCompleted = false;
		}

		[GraphItInput("Enable", "")]
		public void Enable()
		{
			InteractiveObjectModel.SetInteractionDisabled(disabled: false);
		}

		[GraphItInput("Disable", "")]
		public void Disable()
		{
			InteractiveObjectModel.SetInteractionDisabled(disabled: true);
		}

		[GraphItInput("Read Turns", "")]
		public void ReadTurnsToComplete()
		{
			InteractiveObjectModel.TurnsToComplete = TurnsToComplete;
		}

		[GraphItInput("Activate", "")]
		public void Activate()
		{
			InteractiveObjectModel.Disabled = false;
			InteractiveObjectModel.CompleteInteraction(null);
		}

		[GraphItOutput("Completed", "")]
		public void Completed(ActorModel instigator)
		{
			InteractionCompleted = true;
			LastInstigator = instigator;
			Fire("Completed");
		}

		[GraphItOutput("Step", "")]
		public void Step(ActorModel instigator)
		{
			LastInstigator = instigator;
			Fire("Step");
		}

		[GraphItOutput("Canceled", "")]
		public void Canceled(ActorModel instigator)
		{
			LastInstigator = instigator;
			NotifyChange("Canceled");
		}

		[GraphItOutput("Attacked", "")]
		public void Attacked(ActorModel instigator)
		{
			LastInstigator = instigator;
			NotifyChange("Attacked");
		}

		[GraphItOutput("Destroyed", "")]
		public void Destroyed(ActorModel instigator)
		{
			LastInstigator = instigator;
			NotifyChange("Destroyed");
		}

		void InteractionReceiver.OnInteractionStep(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
			Step(instigator);
		}

		void InteractionReceiver.OnInteractionCompleted(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
			Completed(instigator);
		}

		void InteractionReceiver.OnInteractionCanceled(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
			Canceled(instigator);
		}

		void InteractionReceiver.OnAttacked(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
			Attacked(instigator);
		}

		void InteractionReceiver.OnDestroyed(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
			Destroyed(instigator);
		}
	}
}
