using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HelpersUI
{
	private const string CdnTextureChildName = "CdnTexture";

	public static bool SetContentToLabel(UILabel label, string content, bool setActive = true)
	{
		if (label != null && label.gameObject != null)
		{
			label.text = content;
			Helpers.GameObjectSetActive(label.gameObject, setActive);
			return true;
		}
		return false;
	}

	public static bool SetSprite(UISprite sprite, string spriteName, bool setActive = true)
	{
		if (sprite != null && sprite.atlas != null)
		{
			if (sprite.atlas.GetSprite(spriteName) != null)
			{
				sprite.spriteName = spriteName;
				Helpers.GameObjectSetActive(sprite.gameObject, setActive);
				return true;
			}
			Helpers.GameObjectSetActive(sprite.gameObject, value: false);
		}
		return false;
	}

	public static bool SetSpriteAndAtlas(UISprite sprite, string spriteName, UIAtlas atlas, bool setActive = true)
	{
		if (sprite != null && atlas != null)
		{
			if ((UIAtlas)sprite.atlas != atlas)
			{
				sprite.atlas = atlas;
			}
			return SetSprite(sprite, spriteName, setActive);
		}
		return false;
	}

	public static bool SetColor(UIWidget widget, Color color, bool setActive = true)
	{
		if (widget != null)
		{
			Helpers.GameObjectSetActive(widget, setActive);
			widget.color = color;
			return true;
		}
		return false;
	}

	public static Vector3 GetRowPositionX(int index, int count, float width, float height, Vector3 currentPosition = default(Vector3))
	{
		float num = width * (float)(count - 1) * 0.5f;
		currentPosition.x = width * (float)index + 1f - num;
		return currentPosition;
	}

	public static Vector3 GetRowPositionX(int index, int count, Vector2 size, Vector3 currentPosition = default(Vector3))
	{
		return GetRowPositionX(index, count, size.x, size.y, currentPosition);
	}

	public static GameObject GetTouchedUIObject()
	{
		if (UICamera.currentTouch != null && UICamera.currentTouch.isOverUI)
		{
			return UICamera.currentTouch.current;
		}
		int i = 0;
		for (int count = UICamera.activeTouches.Count; i < count; i++)
		{
			UICamera.MouseOrTouch mouseOrTouch = UICamera.activeTouches[i];
			if (mouseOrTouch.pressed != null && mouseOrTouch.pressed != UICamera.fallThrough && NGUITools.FindInParents<UIRoot>(mouseOrTouch.pressed) != null)
			{
				return mouseOrTouch.pressed;
			}
		}
		if (UICamera.controller.pressed != null && UICamera.controller.pressed != UICamera.fallThrough && NGUITools.FindInParents<UIRoot>(UICamera.controller.pressed) != null)
		{
			return UICamera.controller.pressed;
		}
		return null;
	}

	public static void ResizeWidgetToFullscreen(UIWidget widget, bool keepAspectRatio = true)
	{
		if (!(widget != null))
		{
			return;
		}
		Vector2 vector = Helpers.CalculateNguiScreenSize(widget.gameObject);
		if (keepAspectRatio)
		{
			if (Mathf.Max(vector.y, vector.x) / Mathf.Min(vector.y, vector.x) > widget.aspectRatio)
			{
				widget.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnWidth;
				widget.width = (int)vector.x;
			}
			else
			{
				widget.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnHeight;
				widget.height = (int)vector.y;
			}
		}
		else
		{
			widget.keepAspectRatio = UIWidget.AspectRatioSource.Free;
			widget.width = (int)vector.x;
			widget.height = (int)vector.y;
		}
	}

	public static string GetRarityName(int rarityLevel)
	{
		string result = "";
		if (rarityLevel < 5)
		{
			switch (rarityLevel)
			{
			case 0:
				result = "Common";
				break;
			case 1:
				result = "Uncommon";
				break;
			case 2:
				result = "Rare";
				break;
			case 3:
				result = "Epic";
				break;
			case 4:
				result = "Legendary";
				break;
			}
		}
		else
		{
			result = "Legendary";
		}
		return result;
	}

	public static bool TryClearListOf<T>(ref List<T> list, bool clearList = false, bool nullList = false) where T : MonoBehaviourExtended
	{
		if (list != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] != null)
				{
					list[i].Clear();
				}
			}
			if (clearList)
			{
				list.Clear();
			}
			if (nullList)
			{
				list = null;
			}
			return true;
		}
		return false;
	}

	public static void AnimateLabel(UILabel label, int fromValue, int toValue, float duration, Action onComplete = null)
	{
		GameManager.Instance.StartCoroutine(AnimateLabelCoroutine(label, fromValue, toValue, duration, onComplete));
	}

	private static IEnumerator AnimateLabelCoroutine(UILabel label, int fromValue, int toValue, float duration, Action onComplete = null)
	{
		float elapsed = 0f;
		float currentValue = fromValue;
		float updateSpeed = (float)Mathf.Abs(toValue - fromValue) / duration;
		while (elapsed < duration)
		{
			elapsed += Time.deltaTime;
			float num = updateSpeed * Time.deltaTime;
			if (currentValue > (float)toValue)
			{
				currentValue -= num;
				if (currentValue < (float)toValue)
				{
					break;
				}
			}
			else
			{
				currentValue += num;
				if (currentValue > (float)toValue)
				{
					break;
				}
			}
			if (!(label != null))
			{
				break;
			}
			label.text = ((int)currentValue).ToString();
			yield return null;
		}
		if (label != null)
		{
			int num2 = toValue;
			label.text = num2.ToString();
		}
		onComplete?.Invoke();
	}

	public static void ScrollToTheEndHorizontal(UIPanel panel)
	{
		float x = panel.GetViewSize().x;
		Transform transform = panel.transform;
		if (transform.childCount != 0)
		{
			BoxCollider component = transform.GetChild(0).GetComponent<BoxCollider>();
			if (!(component == null))
			{
				float num = component.size.x / 2f;
				float num2 = (float)(transform.childCount * 2) * num;
				float x2 = (0f - num2) / 2f + num - (num2 - x) / 2f;
				SpringPanel.Begin(panel.gameObject, new Vector3(x2, transform.localPosition.y, transform.localPosition.z), 5f);
			}
		}
	}

	public static void SetButtonState(UIButton button, UIButtonColor.State state)
	{
		button.SetState(state, true);
		button.isEnabled = state != UIButtonColor.State.Disabled;
	}

	public static bool SetTextureMaterial(UITexture texture, Material material, bool setActive = true)
	{
		if (texture != null && material != null)
		{
			texture.material = material;
			Helpers.GameObjectSetActive(texture.gameObject, setActive);
			return true;
		}
		return false;
	}

	public static LoadImageFromCdn SetTraitsIconOnSprite(UISprite sprite, string localSpriteName, string cdnContentPath, bool clearLocalCachedUrls = false, int tweenGroupOnLoadComplete = -1)
	{
		if (string.IsNullOrEmpty(cdnContentPath))
		{
			ClearCdnTextureOnSprite(sprite);
			SetSprite(sprite, localSpriteName);
			return null;
		}
		return LoadCdnImageOnSprite(sprite, cdnContentPath, clearLocalCachedUrls, tweenGroupOnLoadComplete);
	}

	public static LoadImageFromCdn LoadCdnImageOnSprite(UISprite sprite, string contentPath, bool clearLocalCachedUrls = false, int tweenGroupOnLoadComplete = -1)
	{
		if (sprite == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(contentPath))
		{
			ClearCdnTextureOnSprite(sprite);
			return null;
		}
		UITexture orCreateCdnTextureOnSprite = GetOrCreateCdnTextureOnSprite(sprite);
		orCreateCdnTextureOnSprite.width = sprite.width;
		orCreateCdnTextureOnSprite.height = sprite.height;
		orCreateCdnTextureOnSprite.depth = sprite.depth;
		orCreateCdnTextureOnSprite.pivot = sprite.pivot;
		orCreateCdnTextureOnSprite.transform.localPosition = Vector3.zero;
		sprite.enabled = false;
		orCreateCdnTextureOnSprite.enabled = true;
		return LoadImageFromCdn.LoadImageToTarget(orCreateCdnTextureOnSprite, contentPath, clearLocalCachedUrls, tweenGroupOnLoadComplete);
	}

	private static UITexture GetOrCreateCdnTextureOnSprite(UISprite sprite)
	{
		ClearLegacyCdnTextureOnSprite(sprite);
		Transform transform = sprite.transform.Find("CdnTexture");
		GameObject gameObject;
		if (transform != null)
		{
			gameObject = transform.gameObject;
		}
		else
		{
			gameObject = new GameObject("CdnTexture");
			gameObject.layer = sprite.gameObject.layer;
			Transform transform2 = gameObject.transform;
			transform2.SetParent(sprite.transform, worldPositionStays: false);
			transform2.localPosition = Vector3.zero;
			transform2.localRotation = Quaternion.identity;
			transform2.localScale = Vector3.one;
		}
		return Helpers.AddComponent<UITexture>(gameObject);
	}

	private static void ClearCdnTextureOnSprite(UISprite sprite)
	{
		if (!(sprite == null))
		{
			ClearLegacyCdnTextureOnSprite(sprite);
			Transform transform = sprite.transform.Find("CdnTexture");
			if (transform != null)
			{
				DestroyCdnTextureComponents(transform.gameObject);
				Helpers.DestroyOrCache(transform.gameObject);
			}
			sprite.enabled = true;
			Helpers.GameObjectSetActive(sprite.gameObject, value: true);
		}
	}

	private static void ClearLegacyCdnTextureOnSprite(UISprite sprite)
	{
		if (!(sprite.GetComponent<UITexture>() == null))
		{
			DestroyCdnTextureComponents(sprite.gameObject);
		}
	}

	private static void DestroyCdnTextureComponents(GameObject target)
	{
		UITexture component = target.GetComponent<UITexture>();
		if (component != null)
		{
			LoadImageFromCdn.LoadImageToTarget(component, null);
		}
		LoadImageFromCdn component2 = target.GetComponent<LoadImageFromCdn>();
		if (component2 != null)
		{
			UnityEngine.Object.Destroy(component2);
		}
		if (component != null)
		{
			UnityEngine.Object.Destroy(component);
		}
	}

	public static Task<bool> ConfirmationPopupAsync(string titleKey, string infoKey, string confirmButtonKey, GameObject parent = null)
	{
		TaskCompletionSource<bool> completionSource = new TaskCompletionSource<bool>();
		ConfirmationPopup obj = (ConfirmationPopup)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup, parent);
		obj.SetOkButtonLabel(LocalizationManager.GetText(confirmButtonKey));
		obj.SetContent(LocalizationManager.GetText(titleKey), LocalizationManager.GetText(infoKey));
		obj.SetCallbacks(delegate
		{
			completionSource.SetResult(result: true);
		}, delegate
		{
			completionSource.SetResult(result: false);
		});
		obj.Open();
		return completionSource.Task;
	}


	#region mycode
	public static int GetActualRewardValue(RadioCallButton bt, int testAmount)
	{
		bool isObsolete = false;
		if (!isObsolete)
		{
			//DebugTWD.Log("Проверить устаревший метод", DebugType.System);
			return testAmount;
		}
		if (bt.parsedHeroTokensDropNumberValues != null && bt.OriginHeroRarityAmountsValues != null && bt.parsedHeroTokensDropNumberValues.Count == bt.OriginHeroRarityAmountsValues.Count)
		{
			if (bt.OriginHeroRarityAmountsValues.Contains(testAmount))
				return bt.parsedHeroTokensDropNumberValues[bt.OriginHeroRarityAmountsValues.IndexOf(testAmount)];
			else return testAmount;
		}
		else return testAmount;
	}
	#endregion
}
