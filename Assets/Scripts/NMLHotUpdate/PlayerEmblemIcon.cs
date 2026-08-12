using BaseModel;
using TWDModel;
using UnityEngine;

public class PlayerEmblemIcon : MonoBehaviour
{
	[SerializeField]
	private UISprite emblemIcon;

	[SerializeField]
	private UITexture emblemIconTexture;

	[SerializeField]
	private UISprite emblemIconEffect;

	[SerializeField]
	private UISprite emblemBorder;

	[SerializeField]
	private UITexture emblemBorderTexture;

	[SerializeField]
	private UISprite emblemBorderEffect;

	[SerializeField]
	private UISprite emblemBackground;

	[SerializeField]
	private int ImageLoadCompleteTweenGroup = 10;

	[SerializeField]
	private bool ownPlayer;

	public void OnEnable()
	{
		if (ownPlayer)
		{
			SetEmblem(GameManager.Instance.playerModel.PlayerEmblem);
			GameManager.Instance.playerModel.Changed += OnPlayerModelChanged;
		}
	}

	public void OnDisable()
	{
		if (ownPlayer)
		{
			GameManager.Instance.playerModel.Changed -= OnPlayerModelChanged;
		}
	}

	public void SetEmblem(PlayerEmblem emblem)
	{
		if (emblem == null)
		{
			emblem = new PlayerEmblem();
		}
		Helpers.GameObjectSetActive(emblemIconTexture, value: false);
		Helpers.GameObjectSetActive(emblemIcon, value: false);
		Helpers.GameObjectSetActive(emblemIconEffect, value: false);
		AvatarsDefinition avatarsDefinition = GameManager.Instance.gameEconomyData.GetAvatarsDefinition(emblem.IconIndex);
		if (avatarsDefinition != null)
		{
			if (!string.IsNullOrEmpty(avatarsDefinition.LocalImg))
			{
				HelpersUI.SetSprite(emblemIcon, avatarsDefinition.LocalImg);
				Helpers.GameObjectSetActive(emblemIcon, value: true);
				LoadImageFromCdn.LoadImageToTarget(emblemIconTexture, "", clearLocalCachedUrls: false, ImageLoadCompleteTweenGroup);
			}
			else
			{
				LoadImageFromCdn.LoadImageToTarget(emblemIconTexture, avatarsDefinition.Image, clearLocalCachedUrls: false, ImageLoadCompleteTweenGroup);
			}
			Helpers.GameObjectSetActive(emblemIconEffect, avatarsDefinition.LocalEffectType > 0);
		}
		Helpers.GameObjectSetActive(emblemBorderTexture, value: false);
		Helpers.GameObjectSetActive(emblemBorder, value: false);
		Helpers.GameObjectSetActive(emblemBorderEffect, value: false);
		BordersDefinition bordersDefinition = GameManager.Instance.gameEconomyData.GetBordersDefinition(emblem.BorderIndex);
		if (bordersDefinition != null)
		{
			if (!string.IsNullOrEmpty(bordersDefinition.LocalImg))
			{
				HelpersUI.SetSprite(emblemBorder, bordersDefinition.LocalImg);
				Helpers.GameObjectSetActive(emblemBorder, value: true);
				Helpers.GameObjectSetActive(emblemBorderEffect, bordersDefinition.LocalEffectType > 0);
				LoadImageFromCdn.LoadImageToTarget(emblemBorderTexture, "", clearLocalCachedUrls: false, ImageLoadCompleteTweenGroup);
			}
			else
			{
				LoadImageFromCdn.LoadImageToTarget(emblemBorderTexture, bordersDefinition.Image, clearLocalCachedUrls: false, ImageLoadCompleteTweenGroup);
			}
		}
		AvatarColorsDefinition avatarColorsDefinition = GameManager.Instance.gameEconomyData.GetAvatarColorsDefinition(emblem.ColorIndex);
		if (ColorUtility.TryParseHtmlString("#" + avatarColorsDefinition.ColorCode, out var color))
		{
			HelpersUI.SetColor(emblemBackground, color);
		}
	}

	public void OnPlayerModelChanged(ModelObject model, string changed, object args)
	{
		if (changed == "PlayerEmblemChanged")
		{
			SetEmblem((model as PlayerModel).PlayerEmblem);
		}
	}
}
