using UnityEngine;

public class WalkerTurnSpeedInputHandler : PlayerInputHandler
{
	private bool restoredSpeed = true;

	public override int Priority => 1000;

	private CombatHUD CombatHUD { get; set; }

	public override bool CanHandleInteraction()
	{
		return false;
	}

	public override void Reset()
	{
		base.Reset();
		Time.timeScale = 1f;
	}

	public override void Update(float deltaTime)
	{
		if (CanHandleInteraction())
		{
			restoredSpeed = false;
			Time.timeScale = 1f + Mathf.Clamp(base.PlayerInputManager.MouseDragDelta.y / 100f, 0f, 2f);
			if (CombatHUD == null)
			{
				CombatHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatHUD) as CombatHUD;
			}
			if (CombatHUD != null)
			{
				CombatHUD.ShowSpeedUp(show: true);
			}
		}
		else if (!restoredSpeed)
		{
			restoredSpeed = true;
			Time.timeScale = 1f;
			if (CombatHUD != null)
			{
				CombatHUD.ShowSpeedUp(show: false);
			}
		}
	}
}
