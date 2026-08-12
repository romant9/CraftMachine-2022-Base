using System.Collections.Generic;
using TWDModel;

public class ActorVisualizationTask : VisualizationTask
{
	protected bool IsTaskValid = true;

	private bool updateCoverStates;

	private Dictionary<ActorModel, CoverIconState> coverStates;

	public ActorView ActorView { get; protected set; }

	public ActorModel Actor { get; protected set; }

	public CombatModel Combat { get; private set; }

	public ActorVisualizationTask(ModelAction action, bool affectsCovers = false)
		: base(action)
	{
		Combat = GameManager.Instance.playerModel.Combat;
		updateCoverStates = affectsCovers;
		if (updateCoverStates)
		{
			coverStates = new Dictionary<ActorModel, CoverIconState>();
		}
	}

	public override void Queued()
	{
		base.Queued();
		if (updateCoverStates)
		{
			CacheCoverStates();
		}
	}

	public override void Start()
	{
		base.Start();
	}

	public override void Finished()
	{
		base.Finished();
		if (updateCoverStates)
		{
			UpdateCoverStates();
		}
	}

	public void CacheCoverStates()
	{
		if (Combat == null)
		{
			Combat = GameManager.Instance.playerModel.Combat;
		}
		if (Combat == null || coverStates == null)
		{
			return;
		}
		coverStates.Clear();
		List<ActorModel> allActors = Combat.GetAllActors();
		for (int i = 0; i < allActors.Count; i++)
		{
			ActorModel actorModel = allActors[i];
			CoverIconState value = CoverIconState.None;
			if (!actorModel.IsWalker && !actorModel.IsEnvironmental)
			{
				if (Combat.IsCoverFlanked(actorModel.GridCoordinate, actorModel))
				{
					value = CoverIconState.Flanked;
				}
				else if (Combat.HasCover(actorModel.GridCoordinate))
				{
					value = CoverIconState.HalfCover;
				}
				coverStates.Add(actorModel, value);
			}
		}
	}

	public void UpdateCoverStates()
	{
		if (coverStates == null)
		{
			return;
		}
		foreach (KeyValuePair<ActorModel, CoverIconState> coverState in coverStates)
		{
			ActorModel key = coverState.Key;
			CoverIconState value = coverState.Value;
			ActorView actorView = GameManager.Instance.GetViewForModel(key) as ActorView;
			if (!(actorView != null))
			{
				continue;
			}
			CharacterAnimationController characterAnimationController = actorView.CharacterAnimationController;
			switch (value)
			{
			case CoverIconState.Flanked:
				if (!actorView.FlankedNotificationShown)
				{
					actorView.AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.Flanked")));
					actorView.FlankedNotificationShown = true;
				}
				if (characterAnimationController != null)
				{
					characterAnimationController.SetIdleStance(IdleStance.Stand);
				}
				break;
			case CoverIconState.HalfCover:
				if (characterAnimationController != null)
				{
					characterAnimationController.SetIdleStance(IdleStance.HalfCover);
				}
				break;
			case CoverIconState.None:
				if (characterAnimationController != null)
				{
					characterAnimationController.SetIdleStance(IdleStance.Stand);
				}
				break;
			}
			actorView.SetCoverIconState(value);
		}
	}
}
