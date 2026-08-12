using System;
using TWDModel;
using UnityEngine;

public class CollectAnimation : HUDElement
{
	[SerializeField]
	[Tooltip("Offset for the end position of the amount of currency animation")]
	private Vector3 amountEndOffset;

	[SerializeField]
	[Tooltip("Duration of the animation")]
	private float duration = 1f;

	[SerializeField]
	[Tooltip("Randomness in the sprite animation curve")]
	private float randomness = 100f;

	[SerializeField]
	[Tooltip("How much the sprite scales up in the middle of it's trajectory")]
	private float scaleBoost = 0.5f;

	[SerializeField]
	[Tooltip("Force this as the minimum frame deltaTime")]
	private float minimumFrameDeltaTime;

	[SerializeField]
	private UIWidget currencySprite;

	[SerializeField]
	private UILabel amountLabel;

	[SerializeField]
	private bool amountLabelFollowSprite;

	[SerializeField]
	private UISprite currencyUISprite;

	private Vector3 currencyEndPosition = Vector3.zero;

	private float time;

	private AnimComplete animComplete;

	private float random_seed;

	private Vector2 randomV;

	private float distance;

	private bool isFirst = true;

	private CurrencyType animCurrencyType;

	public void LateUpdate()
	{
		float num = time / duration;
		time += Mathf.Min(Time.deltaTime, minimumFrameDeltaTime);
		if (time > duration)
		{
			if (animComplete != null && isFirst)
			{
				animComplete(isComplete: true, animCurrencyType);
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
		float f = Mathf.Sin(num * MathF.PI * 0.5f) * MathF.PI * 0.5f;
		Vector3 vector = base.transform.InverseTransformPoint(currencyEndPosition);
		float num2 = Mathf.Lerp(0f, vector.x, 1f - Mathf.Cos(f));
		float num3 = Mathf.Lerp(0f, vector.y, Mathf.Sin(f));
		randomV = new Vector2(Mathf.PerlinNoise(random_seed + num2 * 0.876f / distance, random_seed + num3 * 0.777f / distance) - 0.5f, Mathf.PerlinNoise(random_seed + num2 * 0.759f / distance, random_seed + num3 * 0.883f / distance) - 0.5f);
		float num4 = Mathf.SmoothStep(0f, 1f, num) * Mathf.SmoothStep(1f, 0f, num);
		float num5 = 10f * randomness * num4 * randomV.x;
		float num6 = 10f * randomness * num4 * randomV.y;
		num2 += num5;
		num3 += num6;
		float num7 = 1f + 2f * scaleBoost * num4;
		if (amountLabelFollowSprite)
		{
			currencySprite.cachedTransform.localPosition = new Vector3(num2, num3, 0f);
			currencySprite.cachedTransform.localScale = new Vector3(num7, num7, num7);
		}
		else
		{
			currencySprite.cachedTransform.localPosition = new Vector3(num2, num3, 0f);
			currencySprite.cachedTransform.localScale = new Vector3(num7, num7, num7);
			amountLabel.cachedTransform.localPosition = Vector3.Lerp(Vector3.zero, amountEndOffset, num);
		}
	}

	public void StartAnimation(Vector3 position, int amount, CurrencyType currencyType, AnimComplete animComplete)
	{
		this.animComplete = animComplete;
		StartAnimation(amount, currencyType);
	}

	public void StartAnimation(int amount, CurrencyType currencyType, AnimComplete animComplete = null, bool isFirst = true)
	{
		this.animComplete = animComplete;
		this.isFirst = isFirst;
		HelpersUI.SetSprite(currencyUISprite, HelpersGfx.GetCurrencyIconName(currencyType, GameManager.Instance.playerModel));
		CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud) as CampHUD;
		animCurrencyType = currencyType;
		if (currencyType == CurrencyType.GuildBattleRP && GuildWarHelper.IsBattleOnGoing())
		{
			InitAnimation(amount, SingularityMonoBehaviour<GuildWarManager>.Instance.GetGvGMapFlyingCurrencyTargetPosition(campHUD.GetCollectAnimationDestination(currencyType), useCameraOffsetPosition: true));
		}
		else
		{
			InitAnimation(amount, campHUD.GetCollectAnimationDestination(currencyType));
		}
	}

	public void StartAnimation(int amount, Transform destination)
	{
		InitAnimation(amount, destination);
	}

	public void StartAnimation(int amount, Vector3 destination, AnimComplete animComplete = null, bool isFirst = true)
	{
		this.isFirst = isFirst;
		if (animComplete != null)
		{
			this.animComplete = animComplete;
		}
		InitAnimation(amount, destination);
	}

	public void StartAnimation(int amount, CurrencyType currencyType, Transform destination, AnimComplete animComplete = null, bool isFirst = true)
	{
		this.isFirst = isFirst;
		HelpersUI.SetSprite(currencyUISprite, HelpersGfx.GetCurrencyIconName(currencyType, GameManager.Instance.playerModel));
		this.animComplete = animComplete;
		InitAnimation(amount, destination);
	}

	public void SetSprite(CurrencyType currencyType)
	{
		HelpersUI.SetSprite(currencySprite?.GetComponent<UISprite>(), HelpersGfx.GetCurrencyIconName(currencyType, GameManager.Instance.playerModel));
	}

	public void StartAnimationXp(int amount)
	{
		InitAnimation(amount, CampView.Instance.Hud.CollectAnimationDestinationForXp);
	}

	public void SetLabelVisible(bool visible)
	{
		isFirst = visible;
	}

	private void InitAnimation(int amount, Transform currencyIconDestination)
	{
		if (currencyIconDestination != null)
		{
			InitAnimation(amount, currencyIconDestination.position);
		}
	}

	private void InitAnimation(int amount, Vector3 currencyIconDestination)
	{
		currencyEndPosition = currencyIconDestination;
		random_seed = UnityEngine.Random.value * 1024f;
		duration *= 1f + 1f * UnityEngine.Random.value;
		distance = base.transform.InverseTransformPoint(currencyEndPosition).magnitude + 1f;
		if (isFirst)
		{
			if (amountLabel != null)
			{
				amountLabel.text = ((amount > 0) ? amount.ToString() : "");
			}
			Transform transform = base.transform.FindInChildren("Glow");
			if (transform != null)
			{
				transform.gameObject.SetActive(value: true);
			}
		}
		else if (amountLabel != null)
		{
			amountLabel.gameObject.SetActive(value: false);
		}
	}

	private void OnDisable()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
