using Newtonsoft.Json;

namespace TWDModel
{
	public class SetMissionObjectiveModel : TWDModelObjectWithViewId, TriggerReceiver
	{
		public string ObjectiveText;

		public string CustomText1;

		public string CustomText2;

		private bool isTriggered;

		public const string triggeredEvent = "triggerStateChanged";

		[JsonIgnore]
		public bool IsTriggered
		{
			get
			{
				return isTriggered;
			}
			set
			{
				isTriggered = value;
			}
		}

		public SetMissionObjectiveModel()
		{
		}

		public SetMissionObjectiveModel(string viewId, string text, string customText1, string customText2)
		{
			base.ViewId = viewId;
			ObjectiveText = text;
			CustomText1 = customText1;
			CustomText2 = customText2;
		}

		public void OnTriggered(ActorModel instigator)
		{
			NotifyChange("triggerStateChanged");
			isTriggered = true;
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
