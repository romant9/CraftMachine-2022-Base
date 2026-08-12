using BaseModel;
using TWDModel;
using UnityEngine;

public class AnimatedPropView : ModelView<AnimatedPropModel>, IRunLocationItem
{
	[Tooltip("List of animators to play selected animation")]
	public Animator[] Animators;

	public override bool AutoGenerateViewID => true;

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
		model.Changed += OnModelChanged;
	}

	public TWDModelObject Apply(IRunLocationItemContainer runLocation, IRunLocationErrorContext errors)
	{
		AnimatedPropModel animatedPropModel = new AnimatedPropModel(ViewId);
		runLocation.AddModelObject(animatedPropModel);
		return animatedPropModel;
	}

	public void OnModelChanged(ModelObject model, string changed, object args)
	{
		AnimatedPropModel animatedPropModel = (AnimatedPropModel)model;
		if (animatedPropModel == null || string.IsNullOrEmpty(animatedPropModel.AnimationId))
		{
			return;
		}
		for (int i = 0; i < ((Animators != null) ? Animators.Length : 0); i++)
		{
			Animator animator = Animators[i];
			if (animator != null)
			{
				animator.speed = animatedPropModel.AnimationSpeed;
				animator.Play(animatedPropModel.AnimationId);
			}
		}
	}
}
