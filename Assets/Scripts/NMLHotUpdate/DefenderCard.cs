using TWDModel;
using UnityEngine;

public class DefenderCard : MonoBehaviour
{
	[Header("Header")]
	[SerializeField]
	private UILabel NameLabel;

	[SerializeField]
	private UISprite ClassIconSprite;

	[SerializeField]
	private UILabel LevelLabel;

	[SerializeField]
	private UISprite Border;

	private SurvivorModel LimitedSurvivorInternal = new SurvivorModel();

	public SurvivorModel LimitedSurvivorModel
	{
		get
		{
			if (LimitedSurvivorInternal == null)
			{
				LimitedSurvivorInternal = new SurvivorModel();
			}
			return LimitedSurvivorInternal;
		}
		set
		{
			LimitedSurvivorInternal = value;
		}
	}

	public void UpdateUI()
	{
		if (LimitedSurvivorModel == null)
		{
			Debug.LogWarning("DefenderCard.UpdateUI() Cant be run LimitedSurvivorInternal is NULL");
		}
		if (NameLabel != null)
		{
			NameLabel.text = GameManager.Instance.GetFilteredText(LimitedSurvivorInternal.Name);
		}
		if (ClassIconSprite != null)
		{
			ClassIconSprite.spriteName = HelpersGfx.GetSurvivorClassIconName(LimitedSurvivorInternal.SurvivorClass.ToString(), LimitedSurvivorInternal.SurvivorRarityLevel);
		}
		if (LevelLabel != null)
		{
			LevelLabel.text = LimitedSurvivorInternal.Level.ToString();
		}
		if (Border != null)
		{
			Border.spriteName = HelpersGfx.GetRarityBorderSpriteName(LimitedSurvivorInternal.SurvivorRarityLevel);
		}
	}
}
