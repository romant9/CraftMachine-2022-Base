using System;
using Client.Tweener;
using UnityEngine;

public class RadioWeaponCallCardEffect : MonoBehaviourExtended
{
	[SerializeField]
	private UIButtonExtended Button;

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

	private Animator AnimatorRef;

	private int StartSpinningHash = Animator.StringToHash("StartSpinning");

	private int StopSpinningHash = Animator.StringToHash("StopSpinning");

	private Callback EffectCompleteCallback;

	private float startTime;

	private Material effectMaterial;

	private bool hideRequested;

	private int weaponRarityLevelInternal = -1;

	private bool AnimCompleteInternal;

	private Tweener ScaleTweener = new Tweener();

	private Vector3 InitScale = Vector3.one;

	private void Awake()
	{
		DebugIdString = "RadioWeaponCallCardEffect";
		InitScale = base.transform.localScale;
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

	public void SetWeaponRarityLevel(int rarityLevel)
	{
		weaponRarityLevelInternal = rarityLevel;
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
		if (hideRequested)
		{
			_ = 0.5f;
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
	}

	public void AnimationEventFrontHidden()
	{
	}

	public void AnimationEventFrontHiddenLast()
	{
	}

	public void AnimationEventClimax()
	{
		GameObject gameObject = null;
		GameObject gameObject2 = null;
		switch (weaponRarityLevelInternal)
		{
		case 0:
			gameObject = CommonEffectPrefab;
			gameObject2 = CommonEffectPrefabParented;
			break;
		case 1:
			gameObject = RareEffectPrefab;
			gameObject2 = RareEffectPrefabParented;
			break;
		case 2:
			gameObject = EpicEffectPrefab;
			gameObject2 = EpicEffectPrefabParented;
			break;
		case 3:
			gameObject = TokenEffectPrefab;
			gameObject2 = TokenEffectPrefabParented;
			break;
		case 4:
			gameObject = LegendaryEffectPrefab;
			gameObject2 = LegendaryEffectPrefabParented;
			break;
		}
		if (gameObject2 != null)
		{
			Helpers.InstantiateToParentAndLayer(gameObject2, base.gameObject);
		}
		if (gameObject != null)
		{
			UnityEngine.Object.Instantiate(gameObject).transform.localPosition = base.transform.position;
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
}
