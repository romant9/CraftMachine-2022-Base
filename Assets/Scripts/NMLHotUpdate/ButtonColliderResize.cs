using System;
using UnityEngine;

[ExecuteInEditMode]
public class ButtonColliderResize : MonoBehaviour
{
	[SerializeField]
	private UISprite bgSprite;

	private void Start()
	{
		if (bgSprite == null)
		{
			GameObject tweenTarget = GetComponent<UIButton>().tweenTarget;
			if (tweenTarget != null)
			{
				bgSprite = tweenTarget.GetComponent<UISprite>();
			}
		}
		if (bgSprite != null)
		{
			UISprite uISprite = bgSprite;
			uISprite.onChange = (UIWidget.OnDimensionsChanged)Delegate.Combine(uISprite.onChange, new UIWidget.OnDimensionsChanged(OnDimensionsChanged));
			UISprite uISprite2 = bgSprite;
			uISprite2.OnAnchorDimensionsChangedChange = (UIWidget.OnAnchorDimensionsChanged)Delegate.Combine(uISprite2.OnAnchorDimensionsChangedChange, new UIWidget.OnAnchorDimensionsChanged(OnDimensionsChanged));
		}
	}

	private void Destroy()
	{
		if (bgSprite != null)
		{
			UISprite uISprite = bgSprite;
			uISprite.onChange = (UIWidget.OnDimensionsChanged)Delegate.Remove(uISprite.onChange, new UIWidget.OnDimensionsChanged(OnDimensionsChanged));
			UISprite uISprite2 = bgSprite;
			uISprite2.OnAnchorDimensionsChangedChange = (UIWidget.OnAnchorDimensionsChanged)Delegate.Remove(uISprite2.OnAnchorDimensionsChangedChange, new UIWidget.OnAnchorDimensionsChanged(OnDimensionsChanged));
		}
	}

	private void OnDimensionsChanged()
	{
		BoxCollider component = base.gameObject.GetComponent<BoxCollider>();
		if (component != null)
		{
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(base.gameObject.transform, considerInactive: false);
			component.center = bounds.center;
			component.size = new Vector3(bounds.size.x, bounds.size.y, 0f);
		}
	}
}
