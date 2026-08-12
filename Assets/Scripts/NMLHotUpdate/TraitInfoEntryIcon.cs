using TWDModel;
using UnityEngine;

public class TraitInfoEntryIcon : MonoBehaviour
{
	[SerializeField]
	private UILabel Levelcount;

	[SerializeField]
	private UILabel leftCount;

	[SerializeField]
	private UISprite icon;

	[SerializeField]
	private UISprite iconBG;

	[SerializeField]
	private UISprite iconArrow;

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		_ = type == "BreakThroughed";
	}

	public void SetContent(string identifier, ActorModel actorModel)
	{
		if (GameManager.Instance.gameEconomyData.GetTraitDefinition(identifier) != null)
		{
			Helpers.GameObjectSetActive(Levelcount, value: false);
			Helpers.GameObjectSetActive(leftCount, value: false);
			int effectShowBuffLevelCount = actorModel.GetEffectShowBuffLevelCount(identifier);
			if (effectShowBuffLevelCount > 0)
			{
				Levelcount.text = effectShowBuffLevelCount.ToString();
				Helpers.GameObjectSetActive(Levelcount, value: true);
			}
			int effectShowBuffLeftCount = actorModel.GetEffectShowBuffLeftCount(identifier);
			if (effectShowBuffLeftCount > 0)
			{
				Helpers.GameObjectSetActive(leftCount, value: true);
				leftCount.text = effectShowBuffLeftCount.ToString();
			}
			icon.spriteName = "Ui_Icon_Trait_" + identifier;
		}
	}
}
