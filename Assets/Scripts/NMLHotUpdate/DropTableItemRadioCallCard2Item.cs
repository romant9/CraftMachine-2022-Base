using UnityEngine;

public class DropTableItemRadioCallCard2Item : MonoBehaviour
{
	[SerializeField]
	private UILabel amountLabel;

	[SerializeField]
	private UILabel growLabel;

	[SerializeField]
	private UILabel chanceLabel;

	[SerializeField]
	private GameObject bonusIcon;

	public void Setup(string amount, string chance, bool hasBonus)
	{
		amountLabel.text = amount;
		chanceLabel.text = chance;
		ApplyBonusEffect(hasBonus);
	}

	private void ApplyBonusEffect(bool hasBonus)
	{
		if (amountLabel == null)
		{
			return;
		}
		if (hasBonus)
		{
			amountLabel.color = new Color32(byte.MaxValue, 215, 0, byte.MaxValue);
			amountLabel.fontSize = Mathf.RoundToInt((float)amountLabel.fontSize * 1.3f);
			amountLabel.effectStyle = UILabel.Effect.Outline;
			amountLabel.effectColor = new Color32(180, 120, 0, byte.MaxValue);
			amountLabel.effectDistance = new Vector2(1f, 1f);
			growLabel.color = amountLabel.color;
			growLabel.fontSize = amountLabel.fontSize;
			growLabel.effectStyle = amountLabel.effectStyle;
			growLabel.effectColor = amountLabel.effectColor;
			growLabel.effectDistance = amountLabel.effectDistance;
			if (bonusIcon != null)
			{
				NGUITools.SetActive(bonusIcon, state: true);
			}
		}
		else
		{
			amountLabel.color = Color.white;
			amountLabel.effectStyle = UILabel.Effect.None;
			if (bonusIcon != null)
			{
				NGUITools.SetActive(bonusIcon, state: false);
			}
		}
	}
}
