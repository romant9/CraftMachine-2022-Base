using UnityEngine;

public class UIPlayerNameLabel : MonoBehaviourExtended
{
	[SerializeField]
	private UILabel label;

	public virtual void OnEnable()
	{
		UpdateUI();
	}

	public virtual void UpdateUI()
	{
		if (label != null && GameManager.Instance != null && GameManager.Instance.playerModel != null)
		{
			label.text = GameManager.Instance.playerModel.Name;
		}
	}
}
