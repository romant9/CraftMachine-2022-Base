using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Client.Tweener;
using NextGames.Sdk.AssetBundleManager;
using TWDModel;
using UnityEngine;

public class FullscreenActorOverlay : SingularityMonoBehaviour<FullscreenActorOverlay>
{
	[Serializable]
	private class HeroResourceData
	{
		public string AnimationName;

		public string BackdropName;

		public AnimationClip LoadAnimation()
		{
			return AssetBundleManager.Instance.LoadAsset<AnimationClip>(AnimationName, "herounlock");
		}

		public GameObject LoadBackdrop()
		{
			return AssetBundleManager.Instance.LoadAsset<GameObject>(BackdropName, "herounlock");
		}
	}

	public enum BackgroundType
	{
		Survivor = 0,
		Walker = 1,
		Hero = 2,
		AltHero = 3
	}

	[SerializeField]
	private HeroResourceData[] heroResources;

	[SerializeField]
	[Tooltip("Camera")]
	private Camera cameraUsed;

	[SerializeField]
	private Animation HeroUnlockCameraAnimation;

	[SerializeField]
	[Tooltip("UI Name Targets")]
	public Transform[] NameTargets;

	[SerializeField]
	[Tooltip("Actor Panels")]
	private FullscreenActorPanel[] panels;

	[SerializeField]
	[Header("Offset Tween")]
	[Header("Positions")]
	private Transform UpgradePosition;

	[SerializeField]
	[Header("Duration")]
	private float TweenDuration;

	[SerializeField]
	[Header("Duration")]
	private Transform ObjectToTween;

	[Header("Backgrounds")]
	[SerializeField]
	private GameObject survivorBackgroundContainer;

	[SerializeField]
	private GameObject walkerBackgroundContainer;

	[SerializeField]
	private GameObject heroBackgroundContainer;

	[SerializeField]
	private GameObject heroAltBackgroundContainer;

	[Header("Lights Walker")]
	[SerializeField]
	private GameObject unlockedLightsContainer;

	[SerializeField]
	private GameObject lockedLightsContainer;

	[SerializeField]
	private GameObject heroUnlockGoreEffectsRoot;

	[SerializeField]
	private GameObject heroUnlockNonGoreEffectsRoot;

	[SerializeField]
	private GameObject levelUpCelebrationEffect;

	private Tweener tweener = new Tweener();

	private Vector3 startPosition;

	private Vector3 startRotation;

	private ActorModel openedModel;

	private AmplifyColorEffect vignette;

	protected override void AwakeInternal()
	{
		startPosition = ObjectToTween.localPosition;
		startRotation = ObjectToTween.localEulerAngles;
		close();
	}

	private void Update()
	{
		if (tweener != null && ObjectToTween != null && tweener.animating)
		{
			tweener.update();
			ObjectToTween.localPosition = tweener.progression;
		}
	}

	[ContextMenu("AnimateOffset")]
	public void AnimateOffset()
	{
		if (ObjectToTween != null)
		{
			tweener = new Tweener();
			Vector4 vector = ObjectToTween.localPosition;
			vector.w = ObjectToTween.localEulerAngles.y;
			Vector4 to = UpgradePosition.localPosition;
			vector.w = UpgradePosition.localEulerAngles.y;
			tweener.easeFromTo(vector, to, TweenDuration, EasingFunctions.BackEaseOut);
		}
		else
		{
			Debug.LogError("Could Not Tween");
		}
	}

	[ContextMenu("AnimateBack")]
	public void AnimateBack()
	{
		if (ObjectToTween != null)
		{
			tweener = new Tweener();
			Vector4 vector = ObjectToTween.localPosition;
			vector.w = ObjectToTween.localEulerAngles.y;
			Vector4 to = startPosition;
			vector.w = startRotation.y;
			tweener.easeFromTo(vector, to, TweenDuration, EasingFunctions.BackEaseOut);
		}
		else
		{
			Debug.LogError("Could Not Tween");
		}
	}

	public void SetToOffset()
	{
		if (ObjectToTween != null)
		{
			ObjectToTween.localPosition = UpgradePosition.localPosition;
			tweener = null;
		}
	}

	public void ResetPosition()
	{
		if (ObjectToTween != null)
		{
			ObjectToTween.localPosition = startPosition;
			ObjectToTween.localEulerAngles = startRotation;
			tweener = null;
		}
	}

	private void open()
	{
		ResetPosition();
		base.gameObject.SetActive(value: true);
	}

	public void close()
	{
		if ((bool)base.gameObject)
		{
			base.gameObject.SetActive(value: false);
		}
		for (int i = 0; i < panels.Length; i++)
		{
			if (panels[i] != null)
			{
				panels[i].close();
			}
		}
	}

	public void OpenForSelected(ActorModel model, bool locked = false, BackgroundType backgroundType = BackgroundType.Survivor)
	{
		openedModel = model;
		open();
		SetBackgroundType(backgroundType);
		SetWalkerLockStatus(backgroundType == BackgroundType.Walker, locked);
		for (int i = 0; i < panels.Length; i++)
		{
			if (model != null)
			{
				if (i == 1)
				{
					panels[i].open();
					panels[i].InitActor(model);
				}
				else
				{
					panels[i].close();
				}
			}
		}
	}

	public void HideWeapon()
	{
		GameObject gameObject = panels[1].gameObject.FindInChildren("Bind_Spine");
		if (gameObject != null)
		{
			gameObject.gameObject.SetActive(value: false);
		}
	}

	public void ShowWeapon()
	{
		GameObject gameObject = panels[1].gameObject.FindInChildren("Bind_Spine");
		if (gameObject != null)
		{
			gameObject.gameObject.SetActive(value: true);
		}
	}

	public void OpenForSelected(string actorId, bool locked = false, BackgroundType backgroundType = BackgroundType.Survivor)
	{
		open();
		SetBackgroundType(backgroundType);
		SetWalkerLockStatus(backgroundType == BackgroundType.Walker, locked);
		for (int i = 0; i < panels.Length; i++)
		{
			if (actorId != null)
			{
				if (i == 1)
				{
					panels[i].open();
					panels[i].InitActor(actorId);
				}
				else
				{
					panels[i].close();
				}
			}
		}
	}

	private void SetBackgroundType(BackgroundType backgroundType)
	{
		Helpers.GameObjectSetActive(survivorBackgroundContainer, value: false);
		Helpers.GameObjectSetActive(walkerBackgroundContainer, value: false);
		Helpers.GameObjectSetActive(heroBackgroundContainer, value: false);
		Helpers.GameObjectSetActive(heroAltBackgroundContainer, value: false);
		switch (backgroundType)
		{
		case BackgroundType.Hero:
			Helpers.GameObjectSetActive(heroBackgroundContainer, value: true);
			break;
		case BackgroundType.AltHero:
			Helpers.GameObjectSetActive(heroAltBackgroundContainer, value: true);
			break;
		case BackgroundType.Walker:
			Helpers.GameObjectSetActive(walkerBackgroundContainer, value: true);
			break;
		default:
			Helpers.GameObjectSetActive(survivorBackgroundContainer, value: true);
			break;
		}
	}

	private void SetWalkerLockStatus(bool showWalker, bool locked)
	{
		if (showWalker)
		{
			if (unlockedLightsContainer != null)
			{
				unlockedLightsContainer.SetActive(!locked);
			}
			if (lockedLightsContainer != null)
			{
				lockedLightsContainer.SetActive(locked);
			}
		}
	}

	public void RequestShowUpgradeAnim()
	{
		if (panels.Length > 1 && panels[1] != null)
		{
			panels[1].RequestShowUpgradeAnim();
			Helpers.InstantiateToParent(levelUpCelebrationEffect, panels[1].gameObject);
		}
	}

	public void RequestShowUnlockAnim()
	{
		if (panels.Length > 1 && panels[1] != null)
		{
			panels[1].RequestShowUnlockAnim();
		}
	}

	public void RequestSwitchEquipment(EquipmentItemModel equipment)
	{
		if (panels.Length > 1 && panels[1] != null)
		{
			panels[1].RequestSwitchEquipment(equipment);
		}
	}

	public void RequestSwitchOutfit(OutfitDefinition outfit)
	{
		if (panels.Length > 1 && panels[1] != null)
		{
			panels[1].RequestSwitchOutfit(outfit);
		}
	}

	public void RequestSwitchSkin()
	{
		if (panels.Length > 1 && panels[1] != null)
		{
			panels[1].RequestSwitchSkin();
		}
	}

	public void PermanentlySwitchToOutfit(OutfitDefinition outfit, PortraitManager.PortraitRenderedCallback portraitRenderedCallback)
	{
		if (panels.Length > 1 && panels[1] != null)
		{
			panels[1].PermanentlySwitchToOutfit(outfit, portraitRenderedCallback);
		}
	}

	public void PermanentlySwitchToSkin(HeroSkinInfo heroSkin, PortraitManager.PortraitRenderedCallback portraitRenderedCallback)
	{
		if (panels.Length > 1 && panels[1] != null)
		{
			panels[1].PermanentlySwitchToSkin(heroSkin, portraitRenderedCallback);
		}
	}

	public void PermanentlySwitchBackToDefault(PortraitManager.PortraitRenderedCallback portraitRenderedCallback)
	{
		if (panels.Length > 1 && panels[1] != null)
		{
			panels[1].PermanentlySwitchBackToDefault(portraitRenderedCallback);
		}
	}

	public void AllowRotate(bool allow)
	{
		if (panels.Length > 1 && panels[1] != null)
		{
			panels[1].allowRotate = allow;
		}
	}

	public void SetActorVisibility(bool visible)
	{
		if (panels.Length > 1 && panels[1] != null)
		{
			panels[1].SetActorVisibility(visible);
		}
	}

	public async Task<float> PlayHeroCameraAnimationAsync()
	{
		HeroResourceData heroData = ((openedModel != null) ? heroResources.FirstOrDefault((HeroResourceData data) => data.AnimationName == openedModel.ActorDefinitionID) : null);
		cameraUsed.enabled = false;
		AnimationClip animationClip;
		if (heroData != null)
		{
			SingularityMonoBehaviour<AssetBundleController>.Instance.LoadAssetBundle("herounlock");
			while (SingularityMonoBehaviour<AssetBundleController>.Instance.LoadingAssetBundles)
			{
				await Task.Yield();
			}
			animationClip = heroData.LoadAnimation();
			HeroUnlockCameraAnimation.AddClip(animationClip, animationClip.name);
			GameObject gameObject = null;
			if (!string.IsNullOrEmpty(heroData.BackdropName))
			{
				gameObject = UnityEngine.Object.Instantiate(heroData.LoadBackdrop(), HeroUnlockCameraAnimation.transform);
				gameObject.name = heroData.BackdropName;
			}
			StartCoroutine(DelayedReleaseHeroUnlockAssets(animationClip.length, gameObject, animationClip));
		}
		else
		{
			animationClip = HeroUnlockCameraAnimation.GetClip("Hero_Unlock_Camera");
		}
		cameraUsed.enabled = true;
		string text = animationClip.name;
		HeroUnlockCameraAnimation.Play(text);
		foreach (Transform item in heroUnlockNonGoreEffectsRoot.transform)
		{
			item.gameObject.SetActive(item.gameObject.name == text);
		}
		bool flag = GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.GoreDisabled");
		heroUnlockGoreEffectsRoot.SetActive(!flag);
		if (!flag)
		{
			foreach (Transform item2 in heroUnlockGoreEffectsRoot.transform)
			{
				item2.gameObject.SetActive(item2.gameObject.name == text);
			}
		}
		return animationClip.length;
	}

	private IEnumerator DelayedReleaseHeroUnlockAssets(float delay, GameObject target, AnimationClip clip)
	{
		yield return new WaitForSeconds(delay);
		if (target != null)
		{
			UnityEngine.Object.Destroy(target);
		}
		HeroUnlockCameraAnimation.RemoveClip(clip);
		SingularityMonoBehaviour<AssetBundleController>.Instance.UnloadAssetBundle("herounlock");
		Helpers.ClearUnusedMemory();
	}
}
