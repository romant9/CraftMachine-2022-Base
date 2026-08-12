using BaseModel;
using TWDModel;
using UnityEngine;

public abstract class LeaderBuffStateIndicator : MonoBehaviour
{
	[SerializeField]
	protected UISprite chargeState;

	[SerializeField]
	protected UILabel buffCountText;

	[SerializeField]
	protected GameObject fullStateEffect;

	protected ActorModel actor;

	protected AbilityManagerModel abilityManager;

	private void OnEnable()
	{
		if (actor != null)
		{
			actor.Changed += OnActorModelChanged;
			UpdateState();
		}
	}

	private void OnDisable()
	{
		if (actor != null)
		{
			actor.Changed -= OnActorModelChanged;
		}
	}

	public void SetActorModel(ActorModel actorModel)
	{
		if (actor != null)
		{
			actor.Changed -= OnActorModelChanged;
		}
		if (actorModel == null)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		actor = actorModel;
		actor.Changed += OnActorModelChanged;
		base.gameObject.SetActive(value: true);
		UpdateState();
	}

	public abstract void OnActorModelChanged(ModelObject model, string changed, object args);

	public abstract void UpdateState();
}
