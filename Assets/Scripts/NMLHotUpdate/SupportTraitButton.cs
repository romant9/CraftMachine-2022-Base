using UnityEngine;

public class SupportTraitButton : MonoBehaviour
{
	[SerializeField]
	private UISprite emptyIcon;

	[SerializeField]
	private UISprite lockIcon;

	[SerializeField]
	private UISprite traitIcon;

	[SerializeField]
	private UIButton traitButton;

	public void SetContent(SupportTraitType type, string spriteName = null)
	{
		switch (type)
		{
		case SupportTraitType.Lock:
			Helpers.GameObjectSetActive(emptyIcon.gameObject, value: false);
			Helpers.GameObjectSetActive(lockIcon.gameObject, value: true);
			Helpers.GameObjectSetActive(traitIcon.gameObject, value: false);
			break;
		case SupportTraitType.Empty:
			Helpers.GameObjectSetActive(emptyIcon.gameObject, value: true);
			Helpers.GameObjectSetActive(lockIcon.gameObject, value: false);
			Helpers.GameObjectSetActive(traitIcon.gameObject, value: false);
			break;
		case SupportTraitType.Trait:
			Helpers.GameObjectSetActive(emptyIcon.gameObject, value: false);
			Helpers.GameObjectSetActive(lockIcon.gameObject, value: false);
			Helpers.GameObjectSetActive(traitIcon.gameObject, value: true);
			traitButton.normalSprite = spriteName;
			break;
		}
	}
}
