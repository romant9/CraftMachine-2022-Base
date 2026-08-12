using UnityEngine;

public class Popup_LevelUp_Base : MonoBehaviour
{
	[SerializeField]
	private UISprite rarityBorderLeft;

	[SerializeField]
	private UISprite rarityBorderRight;

	[Header("Data: Damage")]
	[SerializeField]
	private UILabel damageValueLabel;

	[SerializeField]
	private UILabel damageNextValueLabel;

	[SerializeField]
	private UILabel damageMaxValueLabel;

	[SerializeField]
	private UIProgressBar damageProgressBar;

	[Header("Data: health")]
	[SerializeField]
	private UILabel healthValueLabel;

	[SerializeField]
	private UILabel healthNextValueLabel;

	[SerializeField]
	private UILabel healthMaxValueLabel;

	[SerializeField]
	private UIProgressBar healthProgressBar;

	[Header("Upgrade Path UI")]
	[SerializeField]
	private GameObject upgradePathPrefab;

	[SerializeField]
	private GameObject upgradePathContainer;

	private int damageBarMaxWidth;

	public bool ShowNextLevel { get; set; }

	private void Awake()
	{
		damageBarMaxWidth = damageProgressBar.backgroundWidget.width;
	}

	public void Init(int rarityLevel)
	{
		damageProgressBar.thumb.gameObject.SetActive(ShowNextLevel);
		if (healthProgressBar != null)
		{
			healthProgressBar.thumb.gameObject.SetActive(ShowNextLevel);
		}
		if (rarityBorderLeft != null && rarityBorderRight != null)
		{
			HelpersGfx.UpdateSpriteAndKeepScale(rarityBorderLeft, HelpersGfx.GetRarityBorderSpriteName(rarityLevel));
			HelpersGfx.UpdateSpriteAndKeepScale(rarityBorderRight, HelpersGfx.GetRarityBorderSpriteName(rarityLevel));
		}
	}

	public void InitUpgradePath(UpgradePathData upgradePathData)
	{
	}

	public void UpdateUpgradePath(int level)
	{
	}

	public void SetDamagePanel(int current, int max, int currentLevel, int startLevel, int numberLevels, int maxUpgradeLevels)
	{
		damageValueLabel.text = current.ToString();
		damageMaxValueLabel.text = max.ToString();
		damageProgressBar.value = (float)(currentLevel - startLevel) / (float)maxUpgradeLevels;
		damageProgressBar.GetComponent<UISprite>().width = numberLevels * damageBarMaxWidth / maxUpgradeLevels;
	}

	public void SetNextDamageValue(int nextValue)
	{
		damageNextValueLabel.text = "+" + nextValue;
	}

	public void HideNextDamage()
	{
		damageProgressBar.thumb.gameObject.SetActive(value: false);
	}

	public void SetHealthPanel(int current, int max, int currentLevel, int startLevel, int numberLevels, int maxUpgradeLevels)
	{
		healthValueLabel.text = current.ToString();
		healthMaxValueLabel.text = max.ToString();
		healthProgressBar.value = (float)(currentLevel - startLevel) * 0.1f;
		healthProgressBar.GetComponent<UISprite>().width = numberLevels * damageBarMaxWidth / maxUpgradeLevels;
	}

	public void SetNextHealthValue(int nextValue)
	{
		healthNextValueLabel.text = "+" + nextValue;
	}

	public void HideNextHealth()
	{
		if (healthProgressBar != null)
		{
			healthProgressBar.thumb.gameObject.SetActive(value: false);
		}
	}
}
