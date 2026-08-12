using System;
using System.Collections.Generic;
using Client.Tweener;
using TWDModel;
using UnityEngine;

public class RadioCallCardEffect : MonoBehaviourExtended
{
	[SerializeField]
	private UIButtonExtended Button;

	[SerializeField]
	private Transform FrontIconParent;

	[SerializeField]
	private Transform BackIconParent;

	[SerializeField]
	private Transform BackStarsParent;

	[SerializeField]
	private Transform FrontStarsParent;

	[SerializeField]
	private GameObject StarPrefab;

	[SerializeField]
	private Vector2 StarsSize = new Vector2(0.5f, 0.5f);

	[SerializeField]
	private SurvivorClassIconsData[] IconsList;

	[SerializeField]
	private GameObject SpinEffectPrefabL;

	[SerializeField]
	private GameObject SpinEffectPrefabR;

	[SerializeField]
	private GameObject CommonEffectPrefab;

	[SerializeField]
	private GameObject CommonEffectPrefabParented;

	[SerializeField]
	private GameObject RareEffectPrefab;

	[SerializeField]
	private GameObject RareEffectPrefabParented;

	[SerializeField]
	private GameObject EpicEffectPrefab;

	[SerializeField]
	private GameObject EpicEffectPrefabParented;

	[SerializeField]
	private GameObject LegendaryEffectPrefab;

	[SerializeField]
	private GameObject LegendaryEffectPrefabParented;

	[SerializeField]
	private GameObject TokenEffectPrefab;

	[SerializeField]
	private GameObject TokenEffectPrefabParented;

	[SerializeField]
	private GameObject IconVanishEffectPrefab;

	private ParticleSystem SpinEffectPS;

	private Dictionary<string, GameObject> IconsPool = new Dictionary<string, GameObject>();

	private List<SurvivorClass> UnlockedSurvivorClasses = new List<SurvivorClass>();

	private List<GameObject> StarsPool = new List<GameObject>();

	private Animator AnimatorRef;

	private int StartSpinningHash = Animator.StringToHash("StartSpinning");

	private int StopSpinningHash = Animator.StringToHash("StopSpinning");

	private Callback EffectCompleteCallback;

	private float startTime;

	private Material effectMaterial;

	private bool hideRequested;

	private SurvivorClass survivorClassInternal = SurvivorClass.None;

	private int survivorRarityLevelInternal = -1;

	private int MinRarityInt = -1;

	private bool IsToken;

	private bool AnimCompleteInternal;

	private GameObject StarsTempObj;

	private Tweener ScaleTweener = new Tweener();

	private Vector3 InitScale = Vector3.one;

	private void Awake()
	{
		DebugIdString = "RadioCallCardEffect";
		InitScale = base.transform.localScale;
		GameObject gameObject = null;
		if (FrontIconParent != null && BackIconParent != null)
		{
			Transform[] array = new Transform[2] { FrontIconParent, BackIconParent };
			if (IconsList != null && IconsPool != null && IconsPool.Count < 1)
			{
				for (int i = 0; i < array.Length; i++)
				{
					for (int j = 0; j < IconsList.Length; j++)
					{
						if (IconsList[j] != null && IconsList[j].Prefab != null)
						{
							if (!IsClassUnlocked(IconsList[j].id))
							{
								continue;
							}
							gameObject = Helpers.InstantiateToParentAndLayer(IconsList[j].Prefab, array[i].gameObject);
							if (gameObject != null)
							{
								Helpers.GameObjectSetActive(gameObject, value: false);
								IconsPool[GetIconsPoolKey(array[i], IconsList[j].id)] = gameObject;
								if (!UnlockedSurvivorClasses.Contains(IconsList[j].id))
								{
									UnlockedSurvivorClasses.Add(IconsList[j].id);
								}
							}
						}
						else
						{
							DebugLogError("Content is NULL!");
						}
					}
				}
			}
			else
			{
				DebugLogError("Content is NULL!");
			}
		}
		if (StarsPool != null && StarsPool.Count < 1)
		{
			for (int k = 0; k < 5; k++)
			{
				gameObject = Helpers.InstantiateToParentAndLayer(StarPrefab, base.gameObject);
				Helpers.GameObjectSetActive(gameObject, value: false);
				StarsPool.Add(gameObject);
			}
		}
		effectMaterial = base.gameObject.GetComponent<MeshRenderer>().material;
		effectMaterial.SetTextureScale("_MaskTex", new Vector2(0.5f, 1f));
		effectMaterial.SetTextureOffset("_MaskTex", new Vector2(UnityEngine.Random.Range(0f, 0.5f), 0f));
	}

	private void Start()
	{
		if (Button != null)
		{
			Button.SetClickCallback(OnObjectClickedUI);
			Button.SetOnPressCallback(OnObjectClickedUI);
			Button.SetOnDragOverCallback(OnObjectClickedUI);
		}
	}

	public bool AnimationComplete()
	{
		return AnimCompleteInternal;
	}

	public void SetSurvivorClass(SurvivorClass survivorClass)
	{
		survivorClassInternal = survivorClass;
	}

	public void SetSurvivorRarityLevel(int rarityLevel)
	{
		survivorRarityLevelInternal = rarityLevel;
	}

	public void SetDropType(DropType drop)
	{
		MinRarityInt = (int)drop;
	}

	public void SetIsToken(bool isToken)
	{
		IsToken = isToken;
	}

	public void AddCompleteCallback(Callback callback)
	{
		EffectCompleteCallback = (Callback)Delegate.Remove(EffectCompleteCallback, callback);
		EffectCompleteCallback = (Callback)Delegate.Combine(EffectCompleteCallback, callback);
	}

	public void StartAnimation()
	{
		AnimCompleteInternal = false;
		SetAnimationState(StartSpinningHash, value: true);
	}

	public void StartEndAnimation()
	{
		SetAnimationState(StopSpinningHash, value: true);
	}

	public void Show()
	{
		base.gameObject.SetActive(value: true);
	}

	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}

	public void RequestHide()
	{
		hideRequested = true;
		startTime = Time.time;
		if (SpinEffectPrefabL != null)
		{
			SpinEffectPS = SpinEffectPrefabL.GetComponent<ParticleSystem>();
			if (SpinEffectPS != null)
			{
				ParticleSystem.EmissionModule emission = SpinEffectPS.emission;
				emission.enabled = false;
			}
		}
		if (SpinEffectPrefabR != null)
		{
			SpinEffectPS = SpinEffectPrefabR.GetComponent<ParticleSystem>();
			if (SpinEffectPS != null)
			{
				ParticleSystem.EmissionModule emission2 = SpinEffectPS.emission;
				emission2.enabled = false;
			}
		}
	}

	public void Update()
	{
		if (ScaleTweener != null && ScaleTweener.animating)
		{
			ScaleTweener.update();
			base.transform.localScale = ScaleTweener.progression;
		}
		float num = Time.time - startTime;
		if (hideRequested && num > 0f)
		{
			effectMaterial.SetFloat("_Cutoff", num * 1f);
		}
		if (hideRequested && num > 0.5f)
		{
			Helpers.GameObjectSetActive(FrontIconParent.gameObject, value: false);
			Helpers.GameObjectSetActive(BackIconParent.gameObject, value: false);
			Helpers.GameObjectSetActive(FrontStarsParent.gameObject, value: false);
			Helpers.GameObjectSetActive(BackStarsParent.gameObject, value: false);
		}
		if (hideRequested && num > 1.5f)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void AnimationEventComplete()
	{
		AnimCompleteInternal = true;
		if (EffectCompleteCallback != null)
		{
			EffectCompleteCallback();
			EffectCompleteCallback = null;
		}
	}

	public void AnimationEventBackHidden()
	{
		UpdateWithIcon(BackIconParent);
	}

	public void AnimationEventFrontHidden()
	{
		UpdateWithIcon(FrontIconParent);
	}

	public void AnimationEventFrontHiddenLast()
	{
		UpdateWithIcon(BackIconParent, survivorClassInternal);
		if (survivorClassInternal != SurvivorClass.None)
		{
			UpdateParentWithRating(BackStarsParent, survivorRarityLevelInternal);
		}
	}

	public void AnimationEventClimax()
	{
		GameObject gameObject = null;
		GameObject gameObject2 = null;
		switch (survivorRarityLevelInternal)
		{
		case 0:
			gameObject = CommonEffectPrefab;
			gameObject2 = CommonEffectPrefabParented;
			break;
		case 1:
			gameObject = CommonEffectPrefab;
			gameObject2 = CommonEffectPrefabParented;
			break;
		case 2:
			gameObject = RareEffectPrefab;
			gameObject2 = RareEffectPrefabParented;
			break;
		case 3:
			gameObject = EpicEffectPrefab;
			gameObject2 = EpicEffectPrefabParented;
			break;
		case 4:
			gameObject = LegendaryEffectPrefab;
			gameObject2 = LegendaryEffectPrefabParented;
			break;
		}
		if (gameObject2 != null && !IsToken)
		{
			Helpers.InstantiateToParentAndLayer(gameObject2, base.gameObject);
		}
		if (gameObject != null && !IsToken)
		{
			UnityEngine.Object.Instantiate(gameObject).transform.localPosition = base.transform.position;
		}
		if (TokenEffectPrefab != null && IsToken)
		{
			UnityEngine.Object.Instantiate(TokenEffectPrefab).transform.localPosition = base.transform.position;
		}
		if (TokenEffectPrefabParented != null && IsToken)
		{
			Helpers.InstantiateToParentAndLayer(TokenEffectPrefabParented, base.gameObject);
		}
		if (IconVanishEffectPrefab != null)
		{
			GameObject obj = UnityEngine.Object.Instantiate(IconVanishEffectPrefab);
			obj.transform.localPosition = base.transform.position;
			obj.transform.localRotation = base.transform.rotation;
		}
	}

	public void PlaySound(string identifier = "")
	{
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			if (identifier == "flip")
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/card_flip");
			}
			else if (identifier == "shake")
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/card_shake");
			}
			else
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/card_spin");
			}
		}
	}

	public void FakeUserClick()
	{
		if (Button != null)
		{
			OnObjectClicked(Button, isUserClick: false);
		}
	}

	public override void Clear()
	{
		base.Clear();
		if (Button != null)
		{
			Button.Clear();
		}
	}

	private void OnObjectClickedUI(UIButtonExtended button)
	{
		OnObjectClicked(button);
	}

	private void OnObjectClicked(UIButtonExtended button, bool isUserClick = true)
	{
		AnimateTouchDown();
		EventManager.NotifyClick("SearchOver");
		Button.Clear();
		Button.isEnabled = false;
		StartAnimation();
		StartEndAnimation();
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			if (isUserClick)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/card_click");
			}
			SingularityMonoBehaviour<AudioManager>.Instance.OnRadioCallCardClicked();
		}
	}

	private void AnimateTouchDown()
	{
		if (ScaleTweener == null || !ScaleTweener.animating)
		{
			ScaleTweener = new Tweener();
			Vector4 vector = base.transform.localScale;
			Vector4 to = InitScale * 0.8f;
			ScaleTweener.easeFromTo(vector, to, 0.1f, EasingFunctions.BackEaseOut, AnimateTouchBack);
		}
	}

	private void AnimateTouchBack()
	{
		ScaleTweener = new Tweener();
		Vector4 vector = base.transform.localScale;
		Vector4 to = InitScale;
		ScaleTweener.easeFromTo(vector, to, 0.1f, EasingFunctions.BackEaseIn);
	}

	private void UpdateParentWithRating(Transform parent, int rarityLevel = -1)
	{
		int num = 0;
		if (rarityLevel == -1)
		{
			int maxRarityLevel = GameManager.Instance.gameEconomyData.ConfigData.MaxRarityLevel;
			num = UnityEngine.Random.Range(MinRarityInt, maxRarityLevel + 1);
		}
		else
		{
			num = rarityLevel + 1;
		}
		for (int i = 0; i < StarsPool.Count; i++)
		{
			StarsTempObj = StarsPool[i];
			if (StarsTempObj != null)
			{
				Helpers.ChangeParent(StarsTempObj, parent.gameObject);
				StarsTempObj.transform.localPosition = HelpersUI.GetRowPositionX(i, num + 1, StarsSize);
				if (i < num)
				{
					Helpers.GameObjectSetActive(StarsTempObj, value: true);
				}
				else
				{
					Helpers.GameObjectSetActive(StarsTempObj, value: false);
				}
			}
		}
	}

	private void UpdateWithIcon(Transform iconParent, SurvivorClass survivorClass = SurvivorClass.None)
	{
		GameObject obj = null;
		if (iconParent != null && TryGetRandomClassIcon(out obj, iconParent, survivorClass))
		{
			NGUITools.SetActiveChildren(iconParent.gameObject, state: false);
			Helpers.GameObjectSetActive(obj, value: true);
		}
	}

	private bool TryGetRandomClassIcon(out GameObject obj, Transform iconParent, SurvivorClass classOverride = SurvivorClass.None)
	{
		int num = 0;
		SurvivorClass survivorClass = classOverride;
		if (classOverride == SurvivorClass.None && UnlockedSurvivorClasses != null && UnlockedSurvivorClasses.Count > 0)
		{
			num = UnityEngine.Random.Range(0, UnlockedSurvivorClasses.Count);
			survivorClass = UnlockedSurvivorClasses[num];
		}
		if (IconsPool.TryGetValue(GetIconsPoolKey(iconParent, survivorClass), out obj))
		{
			return true;
		}
		return false;
	}

	private void SetAnimationState(int hash, bool value)
	{
		if (AnimatorRef == null)
		{
			AnimatorRef = base.gameObject.GetComponent<Animator>();
		}
		if (AnimatorRef != null)
		{
			AnimatorRef.SetBool(hash, value);
		}
		else
		{
			DebugLogError("Could not find animtor in object: " + base.gameObject.name);
		}
	}

	private bool IsClassUnlocked(SurvivorClass survivorClass)
	{
		if (GameManager.Instance != null && GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.SurvivorContainer != null)
		{
			return GameManager.Instance.playerModel.SurvivorContainer.IsSurvivorClassUnlocked(survivorClass);
		}
		Debug.LogError("GameManager.Instance was NULL!");
		return false;
	}

	private string GetIconsPoolKey(Transform obj, SurvivorClass survivorClass)
	{
		if (obj != null)
		{
			return obj.name + "_" + survivorClass;
		}
		return "";
	}
}
