using UnityEngine;

public class SurvivorInfoStateHeroUnlock : SurvivorInfoStateBase
{
	private float ExitDelay = 20f;

	private float ExitTime;

	private bool ExitAllowed;

	private bool UiShown;

	public override void Init()
	{
		base.Init();
		CurrentState = States.SurvivorHeroUnlock;
	}

	public override void Enter()
	{
		base.Enter();
		ExitAllowed = false;
		UiShown = false;
		if (base.SurvivorModel.Definition.ID == "Hero_Simon")
		{
			SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.HideWeapon();
		}
		LoadAnimation();
	}

	private async void LoadAnimation()
	{
		ExitDelay = await SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.PlayHeroCameraAnimationAsync();
		SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.RequestShowUnlockAnim();
		if (ExitDelay == 0f)
		{
			if (base.UnlockView != null)
			{
				base.UnlockView.ShowUnlock(base.SurvivorModel, IntroDoneCallback);
			}
			UiShown = true;
		}
		SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.AllowRotate(allow: false);
		PlayAnchorTween(base.SurvivorStatistics, TweenAnchorId.Hide);
		Helpers.GameObjectSetActive(base.SurvivorRightSidePanel, value: false);
		ExitAfter(ExitDelay);
	}

	public override bool AllowExit()
	{
		return ExitAllowed;
	}

	public override void Exit()
	{
		base.Exit();
		SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.AllowRotate(allow: true);
	}

	public override void UpdateUI()
	{
		Helpers.GameObjectSetActive(base.CloseButton, value: false);
		if (UiShown)
		{
			UpdateAndShowStats();
			UpdateAndShowTraits();
		}
		Helpers.GameObjectSetActive(base.RarityAndClass, value: false);
	}

	public override void Update()
	{
		base.Update();
		if (ExitTime > 0f && Time.realtimeSinceStartup >= ExitTime)
		{
			ExitAllowed = true;
			ExitTime = 0f;
			if (base.UnlockView != null && !UiShown)
			{
				base.UnlockView.ShowUnlock(base.SurvivorModel, IntroDoneCallback, showFade: false);
				UiShown = true;
			}
		}
	}

	private void IntroDoneCallback()
	{
	}

	private void OnClickedUnlockConfirm(UIButtonExtended button)
	{
		if (button != null)
		{
			button.RemoveClickCallback(OnClickedUnlockConfirm);
			SetState(States.SurvivorOverview);
		}
	}

	private void ExitAfter(float time)
	{
		ExitTime = Time.realtimeSinceStartup + time;
	}
}
