using BaseModel;
using Client.Utils;
using TWDModel;
using UnityEngine;

public class FortificationsCoverView : CombatModelView
{
	[SerializeField]
	private float fadeOutDuration = 0.45f;

	private bool isFadingOut;

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
		FortificationsCoverModel fortificationsCoverModel = (FortificationsCoverModel)base.Model;
		base.transform.localScale = Vector3.one * 0.5f;
		base.transform.position = GridView.Instance.GetPosition(fortificationsCoverModel.GridCoordinate).ToVector3();
		float y = ((fortificationsCoverModel.Facing != FacingDirection.Any) ? FacingDirections.ToRotationY(fortificationsCoverModel.Facing) : GetOwnerRotationY(fortificationsCoverModel));
		base.transform.eulerAngles = new Vector3(0f, y, 0f);
	}

	public override void Kill()
	{
		VisualizationQueue.Instance.Add(new CustomVisualizationTask(delegate
		{
			base.Kill();
		}));
	}

	internal void DestroyAfterFade()
	{
		base.Kill();
	}

	private static float GetOwnerRotationY(FortificationsCoverModel cover)
	{
		ActorView actorView = ((cover.Owner != null) ? (GameManager.Instance.GetViewForModel(cover.Owner) as ActorView) : null);
		if (!(actorView != null))
		{
			return 0f;
		}
		return actorView.transform.eulerAngles.y;
	}
}
