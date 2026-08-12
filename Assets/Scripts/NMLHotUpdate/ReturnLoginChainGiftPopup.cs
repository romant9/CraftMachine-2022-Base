using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ReturnLoginChainGiftPopup : MonoBehaviour
{
	private const int ChainGiftSlotCount = 7;

	[SerializeField]
	private Transform[] chainGiftTransform;

	[SerializeField]
	private ReturnLoginChainGiftItem returnLoginChainGiftItem;

	[SerializeField]
	private GameObject rewardContent;

	[SerializeField]
	private float chainGiftMoveDuration = 0.35f;

	[SerializeField]
	private float chainGiftFadeDuration = 0.28f;

	private List<ReturnLoginChainGiftItem> returnLoginShopBoxList = new List<ReturnLoginChainGiftItem>();

	private List<ReturnLoginChainGiftItem> animateLoginShopBoxList = new List<ReturnLoginChainGiftItem>();

	private bool chainGiftAnimRunning;

	private int returnLoginShopBoxWindowStart;

	public List<ReturnLoginChainGiftItem> ReturnLoginShopBoxList => returnLoginShopBoxList;

	public void Open()
	{
		Helpers.GameObjectSetActive(base.gameObject, value: true);
		RebuildChainGiftItems();
		returnLoginShopBoxWindowStart = 0;
		RefreshAnimateShopBoxListFromSource();
		LayoutAnimateBoxesToSlots();
	}

	private void RebuildChainGiftItems()
	{
		ClearRewardContentChainGiftItems();
		ReturnEndlessDealModel model = GetModel();
		if (returnLoginChainGiftItem == null || rewardContent == null || model == null)
		{
			return;
		}
		List<ReturnEndlessDealDefinition> currentDefinitions = model.CurrentDefinitions;
		for (int i = model.CurrentPackIndex; i < currentDefinitions.Count; i++)
		{
			GameObject gameObject = Object.Instantiate(returnLoginChainGiftItem.gameObject, rewardContent.transform, worldPositionStays: false);
			ReturnLoginChainGiftItem component = gameObject.GetComponent<ReturnLoginChainGiftItem>();
			if (component == null)
			{
				Object.Destroy(gameObject);
				continue;
			}
			component.Bind(currentDefinitions[i], i == model.CurrentPackIndex);
			returnLoginShopBoxList.Add(component);
			Helpers.GameObjectSetActive(gameObject, value: true);
		}
	}

	private void ClearRewardContentChainGiftItems()
	{
		for (int num = returnLoginShopBoxList.Count - 1; num >= 0; num--)
		{
			ReturnLoginChainGiftItem returnLoginChainGiftItem = returnLoginShopBoxList[num];
			if (!(returnLoginChainGiftItem == null) && (!(this.returnLoginChainGiftItem != null) || !(returnLoginChainGiftItem == this.returnLoginChainGiftItem)))
			{
				Object.Destroy(returnLoginChainGiftItem.gameObject);
			}
		}
		returnLoginShopBoxList.Clear();
	}

	public void Close()
	{
		Helpers.GameObjectSetActive(base.gameObject, value: false);
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		StopAllCoroutines();
		chainGiftAnimRunning = false;
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "ReturnLoginChainItemClickEvent")
		{
			if (base.gameObject.activeInHierarchy)
			{
				AnimateChainGift();
			}
		}
		else if (type == "OnBundleBought" && base.gameObject.activeInHierarchy)
		{
			Open();
		}
	}

	private void RefreshAnimateShopBoxListFromSource()
	{
		animateLoginShopBoxList.Clear();
		int num = MaxChainSlots();
		if (num > 0)
		{
			int b = returnLoginShopBoxList.Count - returnLoginShopBoxWindowStart;
			int num2 = Mathf.Min(num, Mathf.Max(0, b));
			for (int i = 0; i < num2; i++)
			{
				animateLoginShopBoxList.Add(returnLoginShopBoxList[returnLoginShopBoxWindowStart + i]);
			}
		}
	}

	private void RefillAnimateShopBoxesFromTail()
	{
		int num = MaxChainSlots();
		while (animateLoginShopBoxList.Count < num)
		{
			int num2 = returnLoginShopBoxWindowStart + animateLoginShopBoxList.Count;
			if (num2 < returnLoginShopBoxList.Count)
			{
				animateLoginShopBoxList.Add(returnLoginShopBoxList[num2]);
				continue;
			}
			break;
		}
	}

	private int MaxChainSlots()
	{
		if (chainGiftTransform == null || chainGiftTransform.Length == 0)
		{
			return 0;
		}
		return Mathf.Min(7, chainGiftTransform.Length);
	}

	private void LayoutAnimateBoxesToSlots()
	{
		if (chainGiftTransform == null)
		{
			return;
		}
		int num = MaxChainSlots();
		for (int i = 0; i < animateLoginShopBoxList.Count && i < num; i++)
		{
			ReturnLoginChainGiftItem returnLoginChainGiftItem = animateLoginShopBoxList[i];
			Transform transform = chainGiftTransform[i];
			if (!(returnLoginChainGiftItem == null) && !(transform == null))
			{
				GameObject obj = returnLoginChainGiftItem.gameObject;
				obj.transform.SetParent(transform, worldPositionStays: false);
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localScale = Vector3.one;
				Helpers.GameObjectSetActive(obj, value: true);
			}
		}
	}

	public void AnimateChainGift()
	{
		if (!chainGiftAnimRunning && animateLoginShopBoxList.Count != 0 && chainGiftTransform != null)
		{
			StartCoroutine(AnimateChainGiftRoutine());
		}
	}

	private IEnumerator AnimateChainGiftRoutine()
	{
		chainGiftAnimRunning = true;
		ReturnLoginChainGiftItem returnLoginChainGiftItem = animateLoginShopBoxList[0];
		GameObject first = ((returnLoginChainGiftItem != null) ? returnLoginChainGiftItem.gameObject : null);
		UIWidget[] firstWidgets = ((first != null) ? first.GetComponentsInChildren<UIWidget>(includeInactive: true) : null);
		float[] firstWidgetStartAlphas = null;
		bool fadeFirstByAlpha = firstWidgets != null && firstWidgets.Length != 0;
		if (fadeFirstByAlpha)
		{
			firstWidgetStartAlphas = new float[firstWidgets.Length];
			for (int i = 0; i < firstWidgets.Length; i++)
			{
				firstWidgetStartAlphas[i] = firstWidgets[i].alpha;
			}
		}
		Vector3 firstStartScale = ((first != null) ? first.transform.localScale : Vector3.one);
		List<Transform> moveTransforms = new List<Transform>();
		List<Vector3> moveStarts = new List<Vector3>();
		List<Vector3> moveEnds = new List<Vector3>();
		List<Transform> moveTargetSlots = new List<Transform>();
		for (int j = 1; j < animateLoginShopBoxList.Count; j++)
		{
			ReturnLoginChainGiftItem returnLoginChainGiftItem2 = animateLoginShopBoxList[j];
			if (!(returnLoginChainGiftItem2 == null))
			{
				int num = j - 1;
				if (num >= chainGiftTransform.Length)
				{
					break;
				}
				Transform transform = chainGiftTransform[num];
				if (!(transform == null))
				{
					Transform transform2 = returnLoginChainGiftItem2.transform;
					moveTransforms.Add(transform2);
					moveStarts.Add(transform2.position);
					moveEnds.Add(transform.TransformPoint(Vector3.zero));
					moveTargetSlots.Add(transform);
				}
			}
		}
		float moveDur = Mathf.Max(0.01f, chainGiftMoveDuration);
		float fadeDur = Mathf.Max(0.01f, chainGiftFadeDuration);
		float elapsed = 0f;
		while (elapsed < Mathf.Max(moveDur, fadeDur))
		{
			elapsed += Time.deltaTime;
			float t = Mathf.Clamp01(elapsed / moveDur);
			float t2 = Mathf.Clamp01(elapsed / fadeDur);
			t = Mathf.SmoothStep(0f, 1f, t);
			t2 = Mathf.SmoothStep(0f, 1f, t2);
			for (int k = 0; k < moveTransforms.Count; k++)
			{
				if (moveTransforms[k] != null)
				{
					moveTransforms[k].position = Vector3.Lerp(moveStarts[k], moveEnds[k], t);
				}
			}
			if (first != null)
			{
				if (fadeFirstByAlpha && firstWidgetStartAlphas != null)
				{
					for (int l = 0; l < firstWidgets.Length; l++)
					{
						if (firstWidgets[l] != null)
						{
							firstWidgets[l].alpha = firstWidgetStartAlphas[l] * (1f - t2);
						}
					}
				}
				else
				{
					first.transform.localScale = Vector3.Lerp(firstStartScale, Vector3.zero, t2);
				}
			}
			yield return null;
		}
		for (int m = 0; m < moveTransforms.Count; m++)
		{
			Transform transform3 = moveTransforms[m];
			Transform transform4 = moveTargetSlots[m];
			if (!(transform3 == null) && !(transform4 == null))
			{
				transform3.SetParent(transform4, worldPositionStays: false);
				transform3.localPosition = Vector3.zero;
				transform3.localScale = Vector3.one;
			}
		}
		if (first != null)
		{
			Helpers.GameObjectSetActive(first, value: false);
			if (fadeFirstByAlpha && firstWidgetStartAlphas != null)
			{
				for (int n = 0; n < firstWidgets.Length; n++)
				{
					if (firstWidgets[n] != null)
					{
						firstWidgets[n].alpha = firstWidgetStartAlphas[n];
					}
				}
			}
			else
			{
				first.transform.localScale = firstStartScale;
			}
		}
		animateLoginShopBoxList.RemoveAt(0);
		returnLoginShopBoxWindowStart++;
		RefillAnimateShopBoxesFromTail();
		LayoutAnimateBoxesToSlots();
		ReturnEndlessDealModel model = GetModel();
		if (animateLoginShopBoxList.Count > 0 && model?.CurrentPack != null)
		{
			animateLoginShopBoxList[0].Bind(model.CurrentPack, isCurrent: true);
		}
		chainGiftAnimRunning = false;
	}

	private static ReturnEndlessDealModel GetModel()
	{
		return GameManager.Instance?.playerModel?.ReturnActivityManager?.ReturnEndlessDeal;
	}
}
