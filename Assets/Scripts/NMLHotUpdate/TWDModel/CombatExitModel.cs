using System.Collections.Generic;

namespace TWDModel
{
	public class CombatExitModel : TWDModelObject, TriggerReceiver
	{
		public const string enableStateChangedEvent = "enableStateChanged";

		private bool enabled;

		public List<GridCoordinate> GridCoordinates { get; set; }

		public bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				if (enabled != value)
				{
					enabled = value;
					NotifyChange("enableStateChanged");
					if (base.manager != null && base.manager.CombatModel != null && enabled)
					{
						base.manager.CombatModel.OnExitEnabled();
					}
				}
			}
		}

		public CombatExitModel()
		{
		}

		public CombatExitModel(List<GridCoordinate> coordinates)
		{
			GridCoordinates = coordinates;
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public void OnTriggered(ActorModel instigator)
		{
			Enabled = true;
		}

		public bool IsActorInExit(ActorModel actor)
		{
			for (int i = 0; i < GridCoordinates.Count; i++)
			{
				if (actor.GridCoordinate == GridCoordinates[i])
				{
					return true;
				}
			}
			return false;
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
