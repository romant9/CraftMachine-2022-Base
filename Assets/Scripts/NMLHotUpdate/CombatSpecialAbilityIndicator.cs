using TWDModel;
using UnityEngine;

public class CombatSpecialAbilityIndicator : MonoBehaviour
{
	[Tooltip("The special ability display name.")]
	[SerializeField]
	private UILabel displayName;

	[Tooltip("Background Highlight.")]
	[SerializeField]
	private UISprite backgroundHighlight;

	private Color selectedColor = new Color(0.349f, 0.749f, 0.706f, 1f);

	private Color unselectedColor = new Color(0.161f, 0.133f, 0.122f, 1f);

	public AbilityModel Ability;

	private bool selected;

	public bool Selected
	{
		get
		{
			return selected;
		}
		set
		{
			selected = value;
			if (backgroundHighlight != null && displayName != null)
			{
				backgroundHighlight.color = (selected ? selectedColor : unselectedColor);
				displayName.color = (selected ? Color.white : selectedColor);
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (Ability != null && displayName != null)
		{
			displayName.color = (selected ? Color.white : selectedColor);
		}
	}
}
