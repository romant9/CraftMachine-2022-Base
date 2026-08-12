using UnityEngine;

public class NewDefenseLogEntriesIndicator : MonoBehaviour
{
	[SerializeField]
	private UILabel numberLabel;

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
		UpdateUI();
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "OnDefenseLogSeen")
		{
			UpdateUI();
		}
	}

	public void UpdateUI()
	{
		int num = 0;
		GameManager instance = GameManager.Instance;
		if (instance.playerModel != null)
		{
			num = instance.playerModel.NumNewDefenseLogEntries;
		}
		if (num == 0)
		{
			NGUITools.SetActiveChildren(base.gameObject, state: false);
			return;
		}
		NGUITools.SetActiveChildren(base.gameObject, state: true);
		numberLabel.text = num.ToString();
	}
}
