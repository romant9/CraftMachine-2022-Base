using UnityEngine;

public class AnimationWeapon : MonoBehaviour
{
	public void WeaponActive(int isActive)
	{
		if (SingularityMonoBehaviour<FullscreenActorOverlay>.Instance != null)
		{
			if (isActive == 1)
			{
				SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.ShowWeapon();
			}
			else
			{
				SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.HideWeapon();
			}
		}
	}
}
