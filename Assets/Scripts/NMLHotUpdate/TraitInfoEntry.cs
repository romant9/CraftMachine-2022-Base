using TWDModel;
using UnityEngine;

public class TraitInfoEntry : MonoBehaviour
{
	[SerializeField]
	private UILabel Dec;

	[SerializeField]
	private UILabel traitName;

	[SerializeField]
	private UILabel Levelcount;

	[SerializeField]
	private UILabel leftCount;

	[SerializeField]
	private TraitInfoEntryIcon traitInfoEntryIcon;

	private string traitId;

	private ActorModel actor;

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
			traitId = identifier;
			actor = actorModel;
			UpdateUI();
		}
	}

	public void UpdateUI()
	{
		traitName.text = LocalizationManager.GetText("Traits." + traitId);
		Dec.text = LocalizationManager.GetText("Traits." + traitId + ".Description");
		int effectShowBuffLeftCount = actor.GetEffectShowBuffLeftCount(traitId);
		string text = effectShowBuffLeftCount.ToString();
		if (effectShowBuffLeftCount < 0)
		{
			text = LocalizationManager.GetText("UI.Battle.Buff.Des.RemainingRoundsPerpetual");
		}
		Levelcount.text = LocalizationManager.GetText("UI.Battle.Buff.Des.RemainingRounds", text);
		int effectShowBuffLevelCount = actor.GetEffectShowBuffLevelCount(traitId);
		Helpers.GameObjectSetActive(leftCount, effectShowBuffLevelCount > 0);
		leftCount.text = LocalizationManager.GetText("UI.Battle.Buff.Des.Stacks", actor.GetEffectShowBuffLevelCount(traitId));
		traitInfoEntryIcon.SetContent(traitId, actor);
	}
}
