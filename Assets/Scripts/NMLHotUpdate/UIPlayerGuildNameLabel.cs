using UnityEngine;

public class UIPlayerGuildNameLabel : MonoBehaviourExtended
{
	[SerializeField]
	private UILabel label;

	public virtual void OnEnable()
	{
		UpdateUI();
	}

	public virtual void UpdateUI()
	{
		if (label != null && GameManager.Instance != null && GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.HasGuild && GameManager.Instance.playerModel.GuildModel != null && GameManager.Instance.playerModel.IsGuildMember && !string.IsNullOrEmpty(GameManager.Instance.playerModel.GuildModel.Name))
		{
			label.text = GameManager.Instance.playerModel.GuildModel.Name;
			Helpers.GameObjectSetActive(base.gameObject, value: true);
		}
		else
		{
			Helpers.GameObjectSetActive(base.gameObject, value: false);
		}
	}
}
