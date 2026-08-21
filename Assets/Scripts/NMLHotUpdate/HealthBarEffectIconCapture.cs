using UnityEngine;

public static class HealthBarEffectIconCapture
{
	public static ActorEffectCapture CaptureContainer(GameObject container)
	{
		if (container == null)
		{
			return default(ActorEffectCapture);
		}
		return CaptureFromSprite(ResolveIconSprite(container), container);
	}

	public static ActorEffectCapture CaptureFromSprite(UISprite iconSprite, GameObject searchRoot)
	{
		return new ActorEffectCapture
		{
			Icon = ActorEffectSprite.From(iconSprite),
			Bg = ResolveBg(searchRoot, iconSprite)
		};
	}

	private static UISprite ResolveIconSprite(GameObject container)
	{
		UISprite component = container.GetComponent<UISprite>();
		if (component != null && !string.IsNullOrEmpty(component.spriteName) && !IsEffectBgName(component.gameObject.name))
		{
			return component;
		}
		UISprite[] componentsInChildren = container.GetComponentsInChildren<UISprite>(includeInactive: true);
		foreach (UISprite uISprite in componentsInChildren)
		{
			if (!(uISprite == null) && !string.IsNullOrEmpty(uISprite.spriteName) && !IsDecorSprite(uISprite.gameObject.name))
			{
				return uISprite;
			}
		}
		return component;
	}

	private static ActorEffectSprite ResolveBg(GameObject searchRoot, UISprite iconSprite)
	{
		if (iconSprite == null)
		{
			return default(ActorEffectSprite);
		}
		ActorEffectSprite result = FindBgAmongSiblings(iconSprite);
		if (result.IsValid)
		{
			return result;
		}
		Transform parent = iconSprite.transform.parent;
		while (parent != null)
		{
			if (IsEffectBgName(parent.gameObject.name))
			{
				UISprite component = parent.GetComponent<UISprite>();
				if (component != null && !string.IsNullOrEmpty(component.spriteName))
				{
					return ActorEffectSprite.From(component);
				}
			}
			if (searchRoot != null && parent.gameObject == searchRoot)
			{
				break;
			}
			parent = parent.parent;
		}
		return default(ActorEffectSprite);
	}

	private static ActorEffectSprite FindBgAmongSiblings(UISprite iconSprite)
	{
		Transform parent = iconSprite.transform.parent;
		if (parent == null)
		{
			return default(ActorEffectSprite);
		}
		for (int i = 0; i < parent.childCount; i++)
		{
			UISprite component = parent.GetChild(i).GetComponent<UISprite>();
			if (!(component == null) && !(component == iconSprite) && !string.IsNullOrEmpty(component.spriteName) && IsEffectBgName(component.gameObject.name))
			{
				return ActorEffectSprite.From(component);
			}
		}
		return default(ActorEffectSprite);
	}

	private static bool IsDecorSprite(string objectName)
	{
		if (!IsEffectBgName(objectName))
		{
			return IsForegroundName(objectName);
		}
		return true;
	}

	private static bool IsEffectBgName(string objectName)
	{
		return objectName == "bg";
	}

	private static bool IsForegroundName(string objectName)
	{
		if (!string.IsNullOrEmpty(objectName))
		{
			return objectName.ToLowerInvariant().Contains("foreground");
		}
		return false;
	}
}
