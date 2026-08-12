using System;
using BaseModel;
using TWDModel;
using UnityEngine;

public class SmallSupportCard : SupportCard
{
	[SerializeField]
	private GameObject removeHolder;

	[SerializeField]
	private GameObject supportNormalEffectGo;

	[SerializeField]
	private GameObject supportEffectGo;

	[SerializeField]
	private UISprite bg;

	private Action removeClicked;

	private void Update()
	{
		//SetBgColor();
		//mycod.Перенесено в OnEnable
	}

	private void OnEnable()
	{
		SetBgColor();
	}

	private void SetBgColor()
	{
		if (!(bg == null))
		{
			if (base.Item.definition.Category == 1)
			{
				bg.color = new Color(0.77254903f, 0.2509804f, 8f / 51f, 1f);
			}
			if (base.Item.definition.Category == 0)
			{
				bg.color = new Color(0.11764706f, 0.3372549f, 0.4117647f, 1f);
			}
		}
	}

	public void Initialize(SupportModel model, Action onClick, Action onInfoClick, Action onRemoveClick, MapCategory mapCategory = MapCategory.None)
	{
		Initialize(model, onClick, onInfoClick, mapCategory);
		if (base.Item != null)
		{
			base.Item.Changed += OnSupportModelChanged;
		}
		removeClicked = onRemoveClick;
		removeHolder.SetActive(removeClicked != null);
	}

	private void OnDestroy()
	{
		if (base.Item != null)
		{
			base.Item.Changed -= OnSupportModelChanged;
		}
	}

	private void OnSupportModelChanged(ModelObject m, string changed, object args)
	{
		if (changed == "SupportUpgraded" && args is SupportModel supportModel && base.Item == supportModel)
		{
			Refresh();
		}
	}

	protected override void InitializeEmpty()
	{
		base.gameObject.SetActive(value: false);
	}

	protected override void InitializeRegular()
	{
		base.gameObject.SetActive(value: true);
		base.InitializeRegular();
		Helpers.GameObjectSetActive(supportEffectGo, base.Item.Level > 5);
		Helpers.GameObjectSetActive(supportNormalEffectGo, base.Item.Level <= 5);
	}

	public void RemoveClick()
	{
		removeClicked?.Invoke();
	}
}
