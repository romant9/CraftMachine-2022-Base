using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopupQuickTip : HUDElement
{
	[SerializeField]
	private string tipId;

	[SerializeField]
	private GameObject[] stepsPrefab;

	[SerializeField]
	private GameObject stepsContainer;

	[SerializeField]
	private Color bulletDone;

	[SerializeField]
	private Color bulletNotDone;

	[SerializeField]
	private UILabel label;

	[SerializeField]
	private GameObject okButton;

	[SerializeField]
	private UILabel okButtonLabel;

	[SerializeField]
	private GameObject bullet;

	[SerializeField]
	private GameObject bulletContainer;

	[SerializeField]
	private string NextButtonLocalizationKey;

	[SerializeField]
	private string OkButtonLocalizationKey;

	[Header("Next, Prev buttons")]
	[SerializeField]
	private UIButtonExtended prevButton;

	[SerializeField]
	private UIButtonExtended nextButton;

	[SerializeField]
	private float customYforSteps;

	[Header("For custom position")]
	[Header("0.1 is +10%")]
	[SerializeField]
	private float spacingProcentage;

	private List<GameObject> steps;

	private int currentStep;

	private List<UISprite> bullets;

	private bool mousePressed;

	public string TipId { get; set; }

	public int NumberOfSteps
	{
		get
		{
			if (steps == null)
			{
				return 0;
			}
			return steps.Count;
		}
	}

	public override void Open()
	{
		base.Open();
		if (prevButton != null && nextButton != null)
		{
			prevButton.SetClickCallback(OnButtonClickedPrev);
			nextButton.SetClickCallback(OnButtonClickedNext);
		}
		mousePressed = false;
		int num = ((stepsPrefab != null) ? stepsPrefab.Length : 0);
		if (num <= 0)
		{
			return;
		}
		steps = new List<GameObject>();
		stepsContainer.RemoveAllChildren();
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = null;
			int num2 = 0;
			while (gameObject == null && num2 < stepsPrefab.Length)
			{
				if (stepsPrefab[num2].name == TipId + (i + 1) || stepsPrefab[num2].name == tipId + (i + 1))
				{
					gameObject = stepsPrefab[num2];
				}
				num2++;
			}
			if (gameObject != null)
			{
				steps.Add(stepsContainer.AddChild(gameObject));
				steps.Last().transform.localPosition += new Vector3(0f, customYforSteps, 0f);
			}
		}
		bulletContainer.RemoveAllChildren();
		bullets = new List<UISprite>();
		for (int j = 0; j < NumberOfSteps; j++)
		{
			GameObject gameObject2 = Object.Instantiate(bullet);
			gameObject2.transform.SetParent(bulletContainer.transform);
			gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject2.gameObject.transform.localPosition = new Vector3((float)(-(NumberOfSteps - 1 - j)) * 48f, 0f, 0f);
			gameObject2.gameObject.SetActive(value: true);
			bullets.Add(gameObject2.GetComponent<UISprite>());
		}
		Helpers.GameObjectSetActive(okButton, value: true);
		ShowStep(0);
	}

	public override void Close()
	{
		base.Close();
		bulletContainer.RemoveAllChildren();
		stepsContainer.RemoveAllChildren();
		if (prevButton != null && nextButton != null)
		{
			prevButton.Clear();
			nextButton.Clear();
		}
	}

	public void CustomBulletsPosition()
	{
		for (int i = 0; i < ((bullets != null) ? bullets.Count : 0); i++)
		{
			UISprite uISprite = bullets[i];
			if (uISprite != null)
			{
				float num = spacingProcentage + 1f;
				float num2 = (float)uISprite.width * num;
				Vector3 staticVector3Zero = Helpers.staticVector3Zero;
				staticVector3Zero.x = num2 * (float)i;
				staticVector3Zero.x -= (float)bullets.Count * num2 * 0.5f;
				staticVector3Zero.x += num2 * 0.5f;
				uISprite.transform.localPosition = staticVector3Zero;
			}
		}
		Helpers.GameObjectSetActive(bullet, value: false);
	}

	public void OnOkClicked()
	{
		ShowStep(currentStep + 1);
	}

	public void OnButtonClickedPrev(UIButtonExtended button)
	{
		int num = currentStep - 1;
		num = ((num < 0) ? (steps.Count - 1) : num);
		ShowStep(num);
	}

	public void OnButtonClickedNext(UIButtonExtended button)
	{
		int num = currentStep + 1;
		num = ((num < steps.Count) ? num : 0);
		ShowStep(num);
	}

	private void OnEnable()
	{
		mousePressed = false;
	}

	public void ShowStep(int stepNum)
	{
		currentStep = stepNum;
		if (currentStep >= steps.Count)
		{
			Close();
			return;
		}
		foreach (GameObject step in steps)
		{
			if (step != null)
			{
				step.SetActive(value: false);
			}
		}
		for (int i = 0; i < NumberOfSteps; i++)
		{
			bullets[i].color = ((i > currentStep) ? bulletNotDone : bulletDone);
		}
		steps[currentStep].SetActive(value: true);
		if (label != null)
		{
			label.text = LocalizationManager.GetText("Popup.QuickTip." + TipId + (currentStep + 1));
		}
		bool flag = currentStep >= NumberOfSteps - 1;
		if (okButtonLabel != null)
		{
			okButtonLabel.text = LocalizationManager.GetText(flag ? OkButtonLocalizationKey : NextButtonLocalizationKey);
		}
	}

	private void LateUpdate()
	{
		if (base.IsOpen)
		{
			if (Input.GetMouseButtonDown(0))
			{
				mousePressed = true;
			}
			if (Input.GetMouseButtonUp(0))
			{
				_ = mousePressed;
				mousePressed = false;
			}
		}
	}

	private void OnClick()
	{
	}
}
