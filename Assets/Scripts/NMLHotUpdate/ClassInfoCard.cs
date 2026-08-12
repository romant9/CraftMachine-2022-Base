using TWDModel;
using UnityEngine;

public class ClassInfoCard : UIListCard<SurvivorModel>
{
	[SerializeField]
	private UILabel className;

	[SerializeField]
	private UISprite classIcon;

	[SerializeField]
	private UILabel tokenAmount;

	[SerializeField]
	private UIButton buttonInfo;

	[SerializeField]
	private UITexture classTexture;

	private SurvivorClass selectedClass;

	public override int GetSortValue()
	{
		return UIListCard<SurvivorModel>.GetSortIntFor(SurvivorSortOrder.SurvivorClassInfoCard, 1000);
	}

	public void SetClassInfo(SurvivorClass survivorClass, int amountTokens)
	{
		selectedClass = survivorClass;
		HelpersUI.SetContentToLabel(className, HelpersLocalization.GetSurvivorClassName(survivorClass));
		HelpersUI.SetContentToLabel(tokenAmount, amountTokens.ToString());
		classIcon.spriteName = HelpersGfx.GetCurrencyIconName(SurvivorToken.GetClassAsCurrency(survivorClass));
		HelpersGfx.SetSurvivorClassMaterial(classTexture, survivorClass);
	}

	public void OnInfoClick()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.UnlockClassPopup) as UnlockClassPopup).OpenSingleInfo(selectedClass);
	}
}
