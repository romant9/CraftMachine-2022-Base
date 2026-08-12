using TWDModel;
using UnityEngine;

public class SurvivorBadgeSetsPanel : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Fillable icons that indicate how many set badges are equipped")]
	private UISprite[] fillableSetIcons = new UISprite[5];

	[SerializeField]
	[Tooltip("Fill color that's used when the sprite is not fully filled.")]
	private Color fillColor;

	[SerializeField]
	[Tooltip("Fill color that's used when the player has a full set complete.")]
	private Color fillColorFull;

	public void SetInfo(SurvivorModel survivorModel)
	{
		int[] array = new int[5];
		for (int i = 0; i < survivorModel.BadgeContainer.Badges.Count; i++)
		{
			int type = (int)survivorModel.BadgeContainer.Badges[i].Type;
			array[type]++;
		}
		if (fillableSetIcons == null)
		{
			return;
		}
		int num = Mathf.Min(array.Length, fillableSetIcons.Length);
		for (int j = 0; j < num; j++)
		{
			if (fillableSetIcons[j] != null)
			{
				float num2 = Mathf.Clamp((float)array[j] / 4f, 0f, 1f);
				fillableSetIcons[j].fillAmount = num2;
				fillableSetIcons[j].color = ((num2 == 1f) ? fillColorFull : fillColor);
			}
		}
	}
}
