using System.Text.RegularExpressions;
using TWDModel;

public class BounsInfo
{
	public int Level;

	public SurvivorModel SurvivorModel;

	public BounsInfoDefinition BounsInfoDefinition;

	public BounsModel BounsModel;

	public bool IsLock => Level <= 0;

	public BounsInfo(int level, SurvivorModel survivorModel, BounsInfoDefinition bounsInfoDefinition, BounsModel bounsModel)
	{
		Level = level;
		SurvivorModel = survivorModel;
		BounsInfoDefinition = bounsInfoDefinition;
		BounsModel = bounsModel;
	}

	public BounsLevelDefinition GetCurrentBounsLevelDefinition()
	{
		if (BounsInfoDefinition == null)
		{
			return null;
		}
		if (Level < 0)
		{
			return null;
		}
		return GameManager.Instance.gameEconomyData.GetBounsLevelDefinition(BounsInfoDefinition.ItemID, Level);
	}

	public BounsLevelDefinition GetNextBounsLevelDefinition()
	{
		if (BounsInfoDefinition == null)
		{
			return null;
		}
		int num = Level + 1;
		if (num <= 0)
		{
			return null;
		}
		return GameManager.Instance.gameEconomyData.GetBounsLevelDefinition(BounsInfoDefinition.ItemID, num);
	}

	public TraitDefinition GetTraitDefinition(string traitId)
	{
		return GameManager.Instance.gameEconomyData.GetTraitDefinition(traitId);
	}

	public string GetTraitDescription(bool isNext, bool isTrait)
	{
		BounsLevelDefinition bounsLevelDefinition = null;
		bounsLevelDefinition = (isNext ? GetNextBounsLevelDefinition() : GetCurrentBounsLevelDefinition());
		if (bounsLevelDefinition == null)
		{
			return "";
		}
		TraitDefinition traitDefinition = GetTraitDefinition(isTrait ? bounsLevelDefinition.TraitsLevel : bounsLevelDefinition.QualityLevel);
		if (traitDefinition == null)
		{
			return "";
		}
		return HelpersLocalization.GetTraitDescription(traitDefinition) ?? "";
	}

	public int GetTraitLevel(string content)
	{
		Match match = Regex.Match(content, "\\d+$");
		if (match.Success)
		{
			return int.Parse(match.Value) + 1;
		}
		return 0;
	}
}
