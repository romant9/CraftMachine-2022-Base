using System.Collections.Generic;

namespace TWDModel
{
	public class CoverModel : TWDModelObjectWithViewId, InteractionReceiver, TriggerReceiver
	{
		public const string IsActiveChanged = "IsActiveChanged";

		public List<GridCoordinate> CoverCoordinates;

		public List<int> CoverDirections;

		public CoverType CoverType;

		public bool IsActiveAtStart = true;

		public bool IsActive { get; set; }

		public CoverModel()
		{
		}

		public CoverModel(string viewId)
		{
			base.ViewId = viewId;
		}

		public override void Initialize()
		{
			base.Initialize();
			IsActive = IsActiveAtStart;
		}

		public CoverDirection GetDirection(GridCoordinate coordinate)
		{
			for (int i = 0; i < CoverCoordinates.Count; i++)
			{
				if (CoverCoordinates[i] == coordinate)
				{
					return (CoverDirection)CoverDirections[i];
				}
			}
			return CoverDirection.None;
		}

		public void OnInteractionStep(InteractiveObjectModel interactiveOject, ActorModel instigator)
		{
		}

		public void OnInteractionCanceled(InteractiveObjectModel interactiveOject, ActorModel instigator)
		{
		}

		public void OnAttacked(InteractiveObjectModel interactiveOject, ActorModel instigator)
		{
		}

		public void OnDestroyed(InteractiveObjectModel interactiveOject, ActorModel instigator)
		{
		}

		public void OnInteractionCompleted(InteractiveObjectModel interactiveOject, ActorModel instigator)
		{
			FlipActiveState(instigator);
		}

		public void OnTriggered(ActorModel instigator)
		{
			FlipActiveState(instigator);
		}

		public void FlipActiveState(ActorModel instigator)
		{
			IsActive = !IsActive;
			NotifyChange("IsActiveChanged", instigator);
			if (base.manager != null && base.manager.Player.Combat != null)
			{
				base.manager.Player.Combat.UpdateCoverField();
			}
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
