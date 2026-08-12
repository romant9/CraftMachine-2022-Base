using UnityEngine;

public class SurvivorUpgradePanel : MonoBehaviourExtended
{
	[SerializeField]
	private UILabel NewDamageLabel;

	[SerializeField]
	private UILabel NewHealthLabel;

	[SerializeField]
	private UILabel NewLevelLabel;

	private void Awake()
	{
		DebugIdString = "SurvivorUpgradePanel";
	}

	public void SetDamageValue(int value)
	{
		SetValue(NewDamageLabel, value);
	}

	public void SetHealthValue(int value)
	{
		SetValue(NewHealthLabel, value);
	}

	public void SetLevelValue(int value)
	{
		SetValue(NewLevelLabel, value);
	}

	private void SetValue(UILabel label, int value)
	{
		if (IsNotNull(label, "SetValue"))
		{
			if (value > 0)
			{
				label.text = value.ToString();
				Helpers.GameObjectSetActive(label.transform.parent.gameObject, value: true);
			}
			else
			{
				Helpers.GameObjectSetActive(label.transform.parent.gameObject, value: false);
			}
		}
	}
}
