using Client.Utils;
using TWDModel;
using UnityEngine;

public class DelayedActionGrenadeAreaInputHandler : PlayerInputHandler
{
	private static DelayedActionGrenadeAreaView selectedView;

	public override int Priority => 2;

	public override bool TapOnly => true;

	public override bool CanHandleInteraction()
	{
		if (Helpers.IsCombatSkillSelectableStatus())
		{
			return false;
		}
		if (!base.PlayerInputManager.PlayerSelectionEnabled || base.PlayerInputManager.GetActorAtMouseCoordinate() != null)
		{
			return false;
		}
		DelayedActionGrenadeArea delayedActionGrenadeAreaAtMouseCoordinate = base.PlayerInputManager.GetDelayedActionGrenadeAreaAtMouseCoordinate();
		if (delayedActionGrenadeAreaAtMouseCoordinate != null)
		{
			return IsBombVisible(delayedActionGrenadeAreaAtMouseCoordinate);
		}
		return false;
	}

	public override void InteractionStarted()
	{
		DelayedActionGrenadeArea delayedActionGrenadeAreaAtMouseCoordinate = base.PlayerInputManager.GetDelayedActionGrenadeAreaAtMouseCoordinate();
		DelayedActionGrenadeAreaView bombView = GetBombView(delayedActionGrenadeAreaAtMouseCoordinate);
		if (!(bombView == null))
		{
			ClearSelectedRange();
			selectedView = bombView;
			bombView.ShowExplosionRange();
			FixedVec3 position = base.Grid.GetPosition(delayedActionGrenadeAreaAtMouseCoordinate.EffectiveAreaGridCoordinate);
			PlayerInputManager.Instance.GetHandler<CameraInputHandler>().FocusCameraOnTargetIfFarFromCenter(position.ToVector3());
		}
	}

	public override void Reset()
	{
		ClearSelectedRange();
		base.Reset();
	}

	public override void Update(float deltaTime)
	{
		if (selectedView == null || !Input.GetMouseButtonDown(0))
		{
			return;
		}
		if (UICamera.isOverUI)
		{
			ClearSelectedRange();
			return;
		}
		DelayedActionGrenadeArea delayedActionGrenadeAreaAtMouseCoordinate = base.PlayerInputManager.GetDelayedActionGrenadeAreaAtMouseCoordinate();
		if (delayedActionGrenadeAreaAtMouseCoordinate == null || !IsBombVisible(delayedActionGrenadeAreaAtMouseCoordinate))
		{
			ClearSelectedRange();
		}
	}

	public static void ClearIfSelected(DelayedActionGrenadeAreaView view)
	{
		if (selectedView == view)
		{
			selectedView = null;
		}
	}

	private static void ClearSelectedRange()
	{
		selectedView?.ClearExplosionRange();
		selectedView = null;
	}

	private static bool IsBombVisible(DelayedActionGrenadeArea bomb)
	{
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		if (combat != null && bomb != null)
		{
			return combat.IsGridCellVisibleByAnySurvivor(bomb.EffectiveAreaGridCoordinate);
		}
		return false;
	}

	private static DelayedActionGrenadeAreaView GetBombView(DelayedActionGrenadeArea bomb)
	{
		if (bomb == null)
		{
			return null;
		}
		return GameManager.Instance.GetViewForModel((TWDModelObject)bomb) as DelayedActionGrenadeAreaView;
	}
}
