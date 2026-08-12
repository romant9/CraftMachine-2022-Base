using System;
using TWDModel;
using UnityEngine;

public class RegularSupportCard : SupportCard
{
	[SerializeField]
	private UISprite rarityBorder;

	[SerializeField]
	private GameObject equippedObject;

	[SerializeField]
	private UISprite enhancedSprite;

	[SerializeField]
	private GameObject supportNormalEffectGo;

	[SerializeField]
	private GameObject supportEffectGo;

	[SerializeField]
	private UISprite bg;

	private bool isEquipped;

	private int indexInSourcePanel;

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

	protected override void InitializeRegular()
	{
		base.InitializeRegular();
		if (base.Item.definition.Category == 1)
		{
			Helpers.GameObjectSetActive(enhancedSprite.gameObject, value: true);
			nameLabel.leftAnchor.Set(0f, 47f);
		}
		else
		{
			Helpers.GameObjectSetActive(enhancedSprite.gameObject, value: false);
			nameLabel.leftAnchor.Set(0f, 8f);
		}
		rarityBorder.spriteName = HelpersGfx.GetSupportRarityBorderSpriteName(base.Item.Level);
		equippedObject.SetActive(base.Item.manager.Player.GetEquippedSupportIndex(base.Item.SupportId) >= 0);
		Helpers.GameObjectSetActive(supportEffectGo, base.Item.Level > 5);
		Helpers.GameObjectSetActive(supportNormalEffectGo, base.Item.Level <= 5);
	}

	public void Initialize(SupportModel model, bool alreadyEquipped, Action onClick, Action onInfoClick = null, int indexInPanel = -1, MapCategory mapCategory = MapCategory.None)
	{
		Initialize(model, onClick, onInfoClick, mapCategory);
		isEquipped = alreadyEquipped;
		indexInSourcePanel = indexInPanel;
	}

	public override int GetSortValue()
	{
		PlayerModel player = base.Item.manager.Player;
		int num = player.SupportModels.Count - player.SupportModels.IndexOf(base.Item);
		if (!base.Item.Unlocked)
		{
			num -= 1024;
		}
		if (!isEquipped)
		{
			num += 512;
		}
		if (isEquipped)
		{
			num += 1024;
			num -= indexInSourcePanel * 100;
		}
		return num;
	}
}
