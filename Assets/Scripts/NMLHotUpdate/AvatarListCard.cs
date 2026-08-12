using TWDModel;
using UnityEngine;

public class AvatarListCard : UIListCard<AvatarBaseDefinition>
{
	[SerializeField]
	private UITexture emblemIconTexture;

	[SerializeField]
	private UISprite emblemLocalIconSprite;

	[SerializeField]
	private UISprite emblemIconEffect;

	[SerializeField]
	private UITexture emblemBorderTexture;

	[SerializeField]
	private UISprite emblemLocalBorderSprite;

	[SerializeField]
	private UISprite emblemBorderEffect;

	[SerializeField]
	private UISprite emblemBackground;

	[SerializeField]
	private GameObject selectObj;

	[SerializeField]
	private Color lockColor;

	[SerializeField]
	private UISprite lockTexture;

	[SerializeField]
	private int ImageLoadCompleteTweenGroup = 10;

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEventHandler;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEventHandler;
	}

	private void OnUIEventHandler(string type, object parameter)
	{
		if (type == "OnUpdateAvatarSelectCard" && parameter is AvatarBaseDefinition avatarBaseDefinition)
		{
			Helpers.GameObjectSetActive(selectObj, base.Item.Index == avatarBaseDefinition.Index);
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		UpdateAvatar(base.Item);
	}

	public void UpdateAvatar(AvatarBaseDefinition data, bool isForceHideLockIcon = false)
	{
		if (data == null)
		{
			return;
		}
		bool flag = false;
		PlayerModel playerModel = GameManager.Instance.playerModel;
		Helpers.GameObjectSetActive(emblemIconTexture, value: false);
		Helpers.GameObjectSetActive(emblemLocalIconSprite, value: false);
		Helpers.GameObjectSetActive(emblemIconEffect, value: false);
		Helpers.GameObjectSetActive(emblemBorderTexture, value: false);
		Helpers.GameObjectSetActive(emblemLocalBorderSprite, value: false);
		Helpers.GameObjectSetActive(emblemBorderEffect, value: false);
		Helpers.GameObjectSetActive(emblemBackground, value: false);
		if (!(data is AvatarsDefinition))
		{
			if (!(data is BordersDefinition))
			{
				if (data is AvatarColorsDefinition)
				{
					flag = playerModel.ColorIndexs.Contains(data.Index);
					AvatarColorsDefinition avatarColorsDefinition = GameManager.Instance.gameEconomyData.GetAvatarColorsDefinition(data.Index);
					if (ColorUtility.TryParseHtmlString("#" + avatarColorsDefinition.ColorCode, out var color))
					{
						HelpersUI.SetColor(emblemBackground, color);
					}
					Helpers.GameObjectSetActive(emblemBackground, value: true);
				}
			}
			else
			{
				flag = playerModel.BorderIndexs.Contains(data.Index);
				BordersDefinition bordersDefinition = GameManager.Instance.gameEconomyData.GetBordersDefinition(data.Index);
				if (bordersDefinition != null)
				{
					if (!string.IsNullOrEmpty(bordersDefinition.LocalImg))
					{
						emblemLocalBorderSprite.spriteName = bordersDefinition.LocalImg;
						emblemLocalBorderSprite.color = (flag ? Color.white : lockColor);
						if (isForceHideLockIcon)
						{
							emblemLocalBorderSprite.color = Color.white;
						}
						Helpers.GameObjectSetActive(emblemLocalBorderSprite, value: true);
						Helpers.GameObjectSetActive(emblemBorderEffect, bordersDefinition.LocalEffectType > 0);
					}
					else
					{
						LoadImageFromCdn.LoadImageToTarget(emblemBorderTexture, bordersDefinition?.Image, clearLocalCachedUrls: false, ImageLoadCompleteTweenGroup);
						emblemBorderTexture.color = (flag ? Color.white : lockColor);
						if (isForceHideLockIcon)
						{
							emblemBorderTexture.color = Color.white;
						}
					}
				}
			}
		}
		else
		{
			flag = playerModel.IconIndexs.Contains(data.Index);
			AvatarsDefinition avatarsDefinition = GameManager.Instance.gameEconomyData.GetAvatarsDefinition(data.Index);
			if (avatarsDefinition != null)
			{
				if (!string.IsNullOrEmpty(avatarsDefinition.LocalImg))
				{
					emblemLocalIconSprite.spriteName = avatarsDefinition.LocalImg;
					emblemLocalIconSprite.color = (flag ? Color.white : lockColor);
					if (isForceHideLockIcon)
					{
						emblemLocalIconSprite.color = Color.white;
					}
					Helpers.GameObjectSetActive(emblemLocalIconSprite, value: true);
				}
				else
				{
					LoadImageFromCdn.LoadImageToTarget(emblemIconTexture, avatarsDefinition?.Image, clearLocalCachedUrls: false, ImageLoadCompleteTweenGroup);
					emblemIconTexture.color = (flag ? Color.white : lockColor);
					if (isForceHideLockIcon)
					{
						emblemIconTexture.color = Color.white;
					}
				}
				Helpers.GameObjectSetActive(emblemIconEffect, avatarsDefinition.LocalEffectType > 0);
			}
		}
		Helpers.GameObjectSetActive(lockTexture, !flag);
		if (isForceHideLockIcon)
		{
			Helpers.GameObjectSetActive(lockTexture, value: false);
		}
		Helpers.GameObjectSetActive(selectObj, value: false);
	}

	public void OnClickSelect()
	{
		if (base.Item != null)
		{
			UIEvent.Send("OnUpdateAvatarSelectCard", base.Item);
		}
	}
}
