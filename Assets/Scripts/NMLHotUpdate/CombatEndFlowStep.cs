using System.Collections.Generic;
using UnityEngine;

public class CombatEndFlowStep : HUDElement
{
	public delegate void OnFlowStepEndCallback(CombatEndFlowStep step);

	public delegate void OnReturnToCampAllowedCallback(bool allowed);

	public delegate void OnSetBackgroundCallback(bool enabled);

	[SerializeField]
	private GameObject endScreenStatisticLinePrefab;

	[SerializeField]
	private GameObject outpostEndScreenStatisticLinePrefab;

	[SerializeField]
	private GameObject[] endScreenStatisticLinePositions;

	[SerializeField]
	private GameObject[] outpostEndScreenStatisticLinePositions;

	public bool DestroyAfterCompletion;

	protected bool flowStarted;

	protected bool flowEnded;

	protected bool animationEnded;

	protected EndScreenStatisticLine[] statisticLines;

	public bool ReturnToCampAllowed { get; set; }

	public bool ShowNextButton { get; set; }

	public event OnFlowStepEndCallback OnFlowStepEnd;

	public event OnReturnToCampAllowedCallback OnReturnToCampAllowed;

	public event OnSetBackgroundCallback OnSetBackground;

	public CombatEndFlowStep()
	{
		DestroyAfterCompletion = false;
		flowStarted = false;
		flowEnded = false;
		animationEnded = false;
	}

	public override void Open()
	{
		base.Open();
		StartFlow();
	}

	public virtual void StartFlow()
	{
		flowStarted = true;
	}

	protected void AnimationEnded()
	{
		animationEnded = true;
		if (ReturnToCampAllowed)
		{
			ForceFlowEnd();
		}
	}

	public virtual void ForceFlowEnd()
	{
		flowEnded = true;
		if (this.OnFlowStepEnd != null)
		{
			this.OnFlowStepEnd(this);
		}
		if (!ReturnToCampAllowed)
		{
			Close();
		}
		NotifyReturnToCampAllowed();
	}

	protected void NotifyReturnToCampAllowed()
	{
		this.OnReturnToCampAllowed?.Invoke(ReturnToCampAllowed);
	}

	protected void NotifySetBackground(bool enabled)
	{
		this.OnSetBackground?.Invoke(enabled);
	}

	public static void SetupElementsTransformsOnGrid(List<GameObject> inputElements, GameObject grid, ref List<CardsTransform> outTransforms, bool addToContainer, float offset, bool horizontalFill = true, bool useLocalPosition = false)
	{
		UnityUtils.AlignItemsInsideContainerLine(inputElements, grid, offset, addToContainer, -1f, horizontalFill);
		foreach (GameObject inputElement in inputElements)
		{
			CardsTransform cardsTransform = new CardsTransform();
			if (!useLocalPosition)
			{
				cardsTransform.position = inputElement.transform.position;
			}
			else
			{
				cardsTransform.position = inputElement.transform.localPosition;
			}
			cardsTransform.scale = inputElement.transform.localScale;
			if (outTransforms != null)
			{
				outTransforms.Add(cardsTransform);
			}
		}
	}

	public static void SetupElementsTransforms(List<GameObject> inputElements, ref List<CardsTransform> outTransforms, Vector3 position, Vector3 scale)
	{
		int count = inputElements.Count;
		for (int i = 0; i < count; i++)
		{
			CardsTransform cardsTransform = new CardsTransform();
			cardsTransform.position = position;
			cardsTransform.scale = scale;
			outTransforms.Add(cardsTransform);
		}
	}

	public static void InterpolateElements(List<GameObject> elements, List<CardsTransform> startTransforms, List<CardsTransform> endTransforms, float ratio, bool useLocalPosition = false)
	{
		for (int i = 0; i < elements.Count; i++)
		{
			GameObject gameObject = elements[i];
			CardsTransform cardsTransform = startTransforms[i];
			CardsTransform cardsTransform2 = endTransforms[i];
			Vector3 vector = cardsTransform.position + (cardsTransform2.position - cardsTransform.position) * ratio;
			Vector3 localScale = cardsTransform.scale + (cardsTransform2.scale - cardsTransform.scale) * ratio;
			if (!useLocalPosition)
			{
				gameObject.transform.position = vector;
			}
			else
			{
				gameObject.transform.localPosition = vector;
			}
			gameObject.transform.localScale = localScale;
		}
	}

	protected void CreateEndScreenLine(int positionIndex, Callback callback = null)
	{
		GameObject gameObject = Helpers.InstantiateToParent(endScreenStatisticLinePrefab, endScreenStatisticLinePositions[positionIndex]);
		if (gameObject != null)
		{
			statisticLines[positionIndex] = gameObject.GetComponent<EndScreenStatisticLine>();
			statisticLines[positionIndex].Callback = callback;
		}
		for (int i = 0; i < outpostEndScreenStatisticLinePositions.Length; i++)
		{
			outpostEndScreenStatisticLinePositions[i].SetActive(value: false);
		}
	}

	protected void CreateOutpostEndScreenLine(int positionIndex, Callback callback = null)
	{
		outpostEndScreenStatisticLinePositions[positionIndex].SetActive(value: true);
		GameObject gameObject = Helpers.InstantiateToParent(outpostEndScreenStatisticLinePrefab, outpostEndScreenStatisticLinePositions[positionIndex]);
		if (gameObject != null)
		{
			statisticLines[positionIndex] = gameObject.GetComponent<EndScreenStatisticLine>();
			statisticLines[positionIndex].Callback = callback;
		}
		for (int i = 0; i < endScreenStatisticLinePositions.Length; i++)
		{
			endScreenStatisticLinePositions[i].SetActive(value: false);
		}
	}
}
