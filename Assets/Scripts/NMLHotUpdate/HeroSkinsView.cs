using TWDModel;
using UnityEngine;

public class HeroSkinsView : MonoBehaviour
{
	[Header("Titles")]
	[SerializeField]
	private UILabel MainTitle;

	[SerializeField]
	private UILabel SkinTitle;

	[SerializeField]
	private UILabel SeasonTitle;

	[SerializeField]
	private UIButton CancelButton;

	[SerializeField]
	private HeroSkinList heroSkinList;

	[SerializeField]
	private GameObject skinAvailableForPurchase;

	[HideInInspector]
	public HeroSkinInfo HeroSkinInfo;

	private SurvivorModel currentSurvivorModel;

	public HeroSkinInfo OriginalHeroSkin { get; private set; }

	public void Show(HeroSkinResourceEntry heroSkinResource, SurvivorModel survivorModel)
	{
		base.gameObject.SetActive(value: true);
		HeroSkinInfo = heroSkinResource.GetHeroSkinInfoForSurvivor(survivorModel);
		OriginalHeroSkin = HeroSkinInfo;
		currentSurvivorModel = survivorModel;
		if (heroSkinList != null)
		{
			heroSkinList.CreateItems(heroSkinResource, survivorModel);
		}
		if (MainTitle != null)
		{
			MainTitle.text = LocalizationManager.GetText("Popup.SurvivorInfoPopup.OutfitPreview");
		}
		UpdateUI();
	}

	public void ShowSkinPreview(string skinDefinitionID, SurvivorModel survivorModel)
	{
		base.gameObject.SetActive(value: true);
		HeroSkinInfo = GameManager.Instance.GetHeroSkinInfoEntry(skinDefinitionID);
		if (MainTitle != null)
		{
			MainTitle.text = LocalizationManager.GetText("Popup.SurvivorInfoPopup.OutfitPreview");
		}
		UpdateUI();
	}

	public void UpdateUI()
	{
		if (HeroSkinInfo != null)
		{
			if (SkinTitle != null)
			{
				SkinTitle.text = LocalizationManager.GetText(HeroSkinInfo.SkinNameLocalizationKey);
			}
			if (SeasonTitle != null)
			{
				SeasonTitle.text = LocalizationManager.GetText(HeroSkinInfo.SeasonLocalizationKey);
			}
			Helpers.GameObjectSetActive(skinAvailableForPurchase, HeroSkinInfo.AvailableForPurchase);
		}
		CancelButton.enabled = true;
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "OnNewOutfitSeleted")
		{
			HeroSkinInfo = parameter as HeroSkinInfo;
			SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.PermanentlySwitchToSkin(HeroSkinInfo, null);
			if (GameManager.Instance.modelManager.GetModel<ActorModel>(currentSurvivorModel.ModelId) != null)
			{
				SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.RequestSwitchSkin();
			}
			UpdateUI();
		}
	}

	public void BackButtonPressed()
	{
		bool flag = HeroSkinInfo != null && GameManager.Instance.playerModel.SurvivorContainer.HeroSkinsOwned.Contains(HeroSkinInfo.PrefabId);
		if (OriginalHeroSkin != null && !flag)
		{
			SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.PermanentlySwitchToSkin(OriginalHeroSkin, delegate
			{
				SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.close();
				UIEvent.Send("OnHeroSkinViewClosed", currentSurvivorModel);
			});
		}
		else if (HeroSkinInfo != null)
		{
			SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.PermanentlySwitchToSkin(HeroSkinInfo, delegate
			{
				SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.close();
				UIEvent.Send("OnHeroSkinViewClosed", currentSurvivorModel);
			});
		}
		CancelButton.enabled = false;
	}
}
