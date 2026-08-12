using System;
using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;

public class PlayerEmblemEditor : UIToggleContent
{
	public enum EmblemType
	{
		Icon = 0,
		Border = 1,
		Color = 2
	}

	[Header("Icon Selector")]
	[SerializeField]
	private UILabel currentIconNumber;

	[SerializeField]
	private UISprite currentIconSprite;

	[SerializeField]
	private UISprite currentIconSpriteEffect;

	[SerializeField]
	private UITexture currentIconTexture;

	[SerializeField]
	private GameObject iconLock;

	[Header("Border Selector")]
	[SerializeField]
	private UILabel currentBorderNumber;

	[SerializeField]
	private UISprite currentBorderSprite;

	[SerializeField]
	private UISprite currentBorderSpriteEffect;

	[SerializeField]
	private GameObject borderLock;

	[SerializeField]
	private UITexture currentBorderTexture;

	[Header("Color Selector")]
	[SerializeField]
	private UILabel currentColorNumber;

	[SerializeField]
	private UISprite currentColorSprite;

	[SerializeField]
	private GameObject colorLock;

	[Header("Preview")]
	[SerializeField]
	private PlayerEmblemIcon previewSmall;

	[SerializeField]
	private PlayerEmblemIcon previewMedium;

	[SerializeField]
	private PlayerEmblemIcon previewBig;

	[SerializeField]
	private GameObject applyButton;

	[SerializeField]
	private GameObject lockButton;

	[SerializeField]
	private int ImageLoadCompleteTweenGroup = 10;

	[SerializeField]
	private Color lockColor;

	private PlayerEmblem tempEmblem;

	private int selectedIconIndex;

	private int selectedBorderIndex;

	private int selectedColorIndex;

	private int totalIconAmount;

	private int totalBorderAmount;

	private int totalColorsAmount;

	private PlayerEmblemResourcesMap _playerEmblemResources;

	[SerializeField]
	[Tooltip("Label show when there is some info")]
	private UILabel infoLabel;

	[SerializeField]
	private UISprite infoSprite;

	[SerializeField]
	[Tooltip("Time to show in seconds")]
	private float timeToShow = 2f;

	[SerializeField]
	private Color errorColor;

	[SerializeField]
	private Color normalColor;

	private PlayerEmblemResourcesMap playerEmblemResources
	{
		get
		{
			if (_playerEmblemResources == null)
			{
				_playerEmblemResources = GameManager.Instance.PlayerEmblemResources;
			}
			return _playerEmblemResources;
		}
	}

	public override void Activate()
	{
		base.Activate();
		tempEmblem = new PlayerEmblem(GameManager.Instance.playerModel.PlayerEmblem);
		selectedIconIndex = tempEmblem.IconIndex;
		selectedBorderIndex = tempEmblem.BorderIndex;
		selectedColorIndex = tempEmblem.ColorIndex;
		totalIconAmount = GameManager.Instance.gameEconomyData.AvatarsDefinitions.Count;
		totalBorderAmount = GameManager.Instance.gameEconomyData.BordersDefinitions.Count;
		totalColorsAmount = GameManager.Instance.gameEconomyData.AvatarColorsDefinitions.Count;
		UpdateUI();
	}

	private void UpdateUI()
	{
		InternalHide();
		HelpersUI.SetContentToLabel(currentIconNumber, $"{selectedIconIndex + 1}/{totalIconAmount}");
		AvatarsDefinition avatarsDefinition = GameManager.Instance.gameEconomyData.GetAvatarsDefinition(selectedIconIndex);
		Helpers.GameObjectSetActive(currentIconSprite, value: false);
		Helpers.GameObjectSetActive(currentIconSpriteEffect, value: false);
		Helpers.GameObjectSetActive(currentIconTexture, value: false);
		if (avatarsDefinition != null)
		{
			if (!string.IsNullOrEmpty(avatarsDefinition.LocalImg))
			{
				HelpersUI.SetSprite(currentIconSprite, avatarsDefinition.LocalImg);
				currentIconSprite.color = (IsUnlockByType(EmblemType.Icon) ? Color.white : lockColor);
				Helpers.GameObjectSetActive(currentIconSprite, value: true);
				Helpers.GameObjectSetActive(currentIconSpriteEffect, avatarsDefinition.LocalEffectType > 0);
			}
			else
			{
				LoadImageFromCdn.LoadImageToTarget(currentIconTexture, avatarsDefinition.Image, clearLocalCachedUrls: false, ImageLoadCompleteTweenGroup);
				currentIconTexture.color = (IsUnlockByType(EmblemType.Icon) ? Color.white : lockColor);
				Helpers.GameObjectSetActive(currentIconTexture, value: true);
			}
		}
		Helpers.GameObjectSetActive(iconLock, !IsUnlockByType(EmblemType.Icon));
		Helpers.GameObjectSetActive(currentBorderSprite, value: false);
		Helpers.GameObjectSetActive(currentBorderSpriteEffect, value: false);
		Helpers.GameObjectSetActive(currentBorderTexture, value: false);
		HelpersUI.SetContentToLabel(currentBorderNumber, $"{selectedBorderIndex + 1}/{totalBorderAmount}");
		BordersDefinition bordersDefinition = GameManager.Instance.gameEconomyData.GetBordersDefinition(selectedBorderIndex);
		if (bordersDefinition != null)
		{
			if (!string.IsNullOrEmpty(bordersDefinition.LocalImg))
			{
				HelpersUI.SetSprite(currentBorderSprite, bordersDefinition.LocalImg);
				currentBorderSprite.color = (IsUnlockByType(EmblemType.Border) ? Color.white : lockColor);
				Helpers.GameObjectSetActive(currentBorderSprite, value: true);
				Helpers.GameObjectSetActive(currentBorderSpriteEffect, bordersDefinition.LocalEffectType > 0);
			}
			else
			{
				LoadImageFromCdn.LoadImageToTarget(currentBorderTexture, bordersDefinition.Image, clearLocalCachedUrls: false, ImageLoadCompleteTweenGroup);
				currentBorderTexture.color = (IsUnlockByType(EmblemType.Border) ? Color.white : lockColor);
				Helpers.GameObjectSetActive(currentBorderTexture, value: true);
			}
		}
		Helpers.GameObjectSetActive(borderLock, !IsUnlockByType(EmblemType.Border));
		HelpersUI.SetContentToLabel(currentColorNumber, $"{selectedColorIndex + 1}/{totalColorsAmount}");
		AvatarColorsDefinition avatarColorsDefinition = GameManager.Instance.gameEconomyData.GetAvatarColorsDefinition(selectedColorIndex);
		if (ColorUtility.TryParseHtmlString("#" + avatarColorsDefinition.ColorCode, out var color))
		{
			HelpersUI.SetColor(currentColorSprite, color);
		}
		Helpers.GameObjectSetActive(colorLock, !IsUnlockByType(EmblemType.Color));
		Helpers.GameObjectSetActive(applyButton, IsAllUnlock());
		Helpers.GameObjectSetActive(lockButton, !IsAllUnlock());
		UpdatePreviews();
	}

	private void UpdatePreviews()
	{
		if (previewSmall != null)
		{
			previewSmall.SetEmblem(tempEmblem);
		}
		if (previewMedium != null)
		{
			previewMedium.SetEmblem(tempEmblem);
		}
		if (previewBig != null)
		{
			previewBig.SetEmblem(tempEmblem);
		}
	}

	public void OnClickPreviousIcon()
	{
		selectedIconIndex--;
		if (selectedIconIndex < 0)
		{
			selectedIconIndex = totalIconAmount - 1;
		}
		tempEmblem.IconIndex = selectedIconIndex;
		UpdateUI();
	}

	public void OnClickNextIcon()
	{
		selectedIconIndex++;
		if (selectedIconIndex >= totalIconAmount)
		{
			selectedIconIndex = 0;
		}
		tempEmblem.IconIndex = selectedIconIndex;
		UpdateUI();
	}

	public void OnClickPreviousBorder()
	{
		selectedBorderIndex--;
		if (selectedBorderIndex < 0)
		{
			selectedBorderIndex = totalBorderAmount - 1;
		}
		tempEmblem.BorderIndex = selectedBorderIndex;
		UpdateUI();
	}

	public void OnClickNextBorder()
	{
		selectedBorderIndex++;
		if (selectedBorderIndex >= totalBorderAmount)
		{
			selectedBorderIndex = 0;
		}
		tempEmblem.BorderIndex = selectedBorderIndex;
		UpdateUI();
	}

	public void OnClickPreviousColor()
	{
		selectedColorIndex--;
		if (selectedColorIndex < 0)
		{
			selectedColorIndex = totalColorsAmount - 1;
		}
		tempEmblem.ColorIndex = selectedColorIndex;
		UpdateUI();
	}

	public void OnClickNextColor()
	{
		selectedColorIndex++;
		if (selectedColorIndex >= totalColorsAmount)
		{
			selectedColorIndex = 0;
		}
		tempEmblem.ColorIndex = selectedColorIndex;
		UpdateUI();
	}

	public void OnClickApply()
	{
		Helpers.ExecuteCommandDelayed(new SetPlayerEmblemCommand(tempEmblem));
		if (GameManager.Instance.playerModel.IsGuildMember)
		{
			Helpers.ExecuteCommandDelayed(new UpdateMemberInfoCommand());
		}
		Deactivate();
	}

	public void OnClickBack()
	{
		Deactivate();
	}

	private bool IsAllUnlock()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel == null)
		{
			return false;
		}
		if (playerModel.IconIndexs.Contains(selectedIconIndex) && playerModel.BorderIndexs.Contains(selectedBorderIndex))
		{
			return playerModel.ColorIndexs.Contains(selectedColorIndex);
		}
		return false;
	}

	private bool IsUnlockByType(EmblemType type)
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel == null)
		{
			return false;
		}
		return type switch
		{
			EmblemType.Icon => playerModel.IconIndexs.Contains(selectedIconIndex),
			EmblemType.Border => playerModel.BorderIndexs.Contains(selectedBorderIndex),
			EmblemType.Color => playerModel.ColorIndexs.Contains(selectedColorIndex),
			_ => false,
		};
	}

	public void OnClickLock()
	{
		ShowInfo(LocalizationManager.GetText("Achievement.CooperationUnlock.Title"));
	}

	public void OnClickBorder()
	{
		try
		{
			AvatarPopup avatarPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.AvatarPopup) as AvatarPopup;
			if ((bool)avatarPopup)
			{
				List<BordersDefinition> bordersDefinitions = GameManager.Instance.gameEconomyData.BordersDefinitions;
				PlayerModel playerModel = GameManager.Instance.playerModel;
				List<BordersDefinition> data = bordersDefinitions.OrderByDescending((BordersDefinition b) => playerModel.BorderIndexs.Contains(b.Index)).ToList();
				avatarPopup.Show(data);
			}
		}
		catch (Exception arg)
		{
			Debug.LogError($"[PlayerEmblemEditor] click border fail:{arg}");
		}
	}

	public void OnClickIcon()
	{
		try
		{
			AvatarPopup avatarPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.AvatarPopup) as AvatarPopup;
			if ((bool)avatarPopup)
			{
				List<AvatarsDefinition> avatarsDefinitions = GameManager.Instance.gameEconomyData.AvatarsDefinitions;
				PlayerModel playerModel = GameManager.Instance.playerModel;
				List<AvatarsDefinition> data = avatarsDefinitions.OrderByDescending((AvatarsDefinition b) => playerModel.IconIndexs.Contains(b.Index)).ToList();
				avatarPopup.Show(data);
			}
		}
		catch (Exception arg)
		{
			Debug.LogError($"[PlayerEmblemEditor] click icon fail:{arg}");
		}
	}

	public void OnClickColor()
	{
		try
		{
			AvatarPopup avatarPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.AvatarPopup) as AvatarPopup;
			if ((bool)avatarPopup)
			{
				List<AvatarColorsDefinition> avatarColorsDefinitions = GameManager.Instance.gameEconomyData.AvatarColorsDefinitions;
				PlayerModel playerModel = GameManager.Instance.playerModel;
				List<AvatarColorsDefinition> data = avatarColorsDefinitions.OrderByDescending((AvatarColorsDefinition b) => playerModel.ColorIndexs.Contains(b.Index)).ToList();
				avatarPopup.Show(data);
			}
		}
		catch (Exception arg)
		{
			Debug.LogError($"[PlayerEmblemEditor] click color fail:{arg}");
		}
	}

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
		if (!(type == "OnApplyAvatarSelectCard"))
		{
			if (type == "OnAvatarInfo" && parameter is string text)
			{
				ShowInfo(text);
			}
			return;
		}
		if (!(parameter is AvatarsDefinition avatarsDefinition))
		{
			if (!(parameter is BordersDefinition bordersDefinition))
			{
				if (parameter is AvatarColorsDefinition avatarColorsDefinition)
				{
					selectedColorIndex = avatarColorsDefinition.Index;
					tempEmblem.ColorIndex = selectedColorIndex;
				}
			}
			else
			{
				selectedBorderIndex = bordersDefinition.Index;
				tempEmblem.BorderIndex = selectedBorderIndex;
			}
		}
		else
		{
			selectedIconIndex = avatarsDefinition.Index;
			tempEmblem.IconIndex = selectedIconIndex;
		}
		UpdateUI();
	}

	private void ShowInfo(string text, bool isError = false)
	{
		InternalHide();
		SetInfoText(infoLabel, text);
		infoSprite.color = (isError ? errorColor : normalColor);
	}

	private void SetInfoText(UILabel label, string text)
	{
		if (label != null && label.gameObject != null)
		{
			label.gameObject.SetActive(value: true);
			label.text = text;
			CancelInvoke("InternalHide");
			Invoke("InternalHide", timeToShow);
		}
		else
		{
			Debug.LogError("HUDNotification: Could not show notification because label is NULL!");
		}
	}

	private void InternalHide()
	{
		if (infoLabel != null && infoLabel.gameObject != null)
		{
			infoLabel.gameObject.SetActive(value: false);
		}
	}
}
