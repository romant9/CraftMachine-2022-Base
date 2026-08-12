using System.Collections;
using TWDModel;

public class GenericAbilityVisualizationTask : ActorVisualizationTask
{
	private bool started;

	protected string traitIdentifier;

	public float Delay { get; set; }

	public ActorModel TargetActor { get; set; }

	private ActorView TargetView { get; set; }

	private IEnumerator ShowVisualHandle { get; set; }

	private bool Done { get; set; }

	public GenericAbilityVisualizationTask(GenericAbilityAction action)
		: base(action)
	{
		base.Actor = GameManager.Instance.modelManager.GetModel<ActorModel>(action.ModelId);
		base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		TargetActor = action.TargetActor;
		if (TargetActor != null)
		{
			AddDependency(TargetActor, reserve: false);
			TargetView = GameManager.Instance.GetViewForModel(TargetActor) as ActorView;
		}
		if (base.Actor != null)
		{
			AddActorDependency(base.Actor);
		}
	}

	public void StartVisualization()
	{
		Start();
		ReleaseDependency(base.Actor);
		ReleaseDependency(TargetActor);
		Done = false;
		ShowVisualHandle = ShowVisual();
		GameManager.Instance.StartCoroutine(ShowVisualHandle);
		Done = true;
	}

	private IEnumerator ShowVisual()
	{
		GenericAbilityAction genericAbilityAction = (GenericAbilityAction)base.Action;
		if (genericAbilityAction != null)
		{
			if (string.IsNullOrEmpty(traitIdentifier))
			{
				traitIdentifier += "Ui_Icon_Trait_";
				traitIdentifier += genericAbilityAction.NotificationKey.Substring(genericAbilityAction.NotificationKey.IndexOf('.') + 1);
			}
			base.ActorView.AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(genericAbilityAction.NotificationKey), traitIdentifier));
		}
		Done = true;
		yield return null;
	}

	public override void Stop()
	{
		if (ShowVisualHandle != null && GameManager.Instance != null)
		{
			GameManager.Instance.StopCoroutine(ShowVisualHandle);
		}
	}

	public override bool Update(float deltaTime)
	{
		Delay -= deltaTime;
		if (Delay > 0f)
		{
			return true;
		}
		if (!started)
		{
			StartVisualization();
			started = true;
		}
		return !Done;
	}
}
