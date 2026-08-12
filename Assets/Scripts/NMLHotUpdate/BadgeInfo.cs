using System;
using TWDModel;

public class BadgeInfo : IEquatable<BadgeInfo>
{
	public BadgeModel Model;

	public string ModelId;

	public bool MaxSimilarBadgesReached;

	public bool ScrapModeEnabled;

	public bool ScrapSelected;

	public bool SetBonusActive;

	public string OwnerName;

	public BadgeInfo(BadgeModel model, bool maxReached = false)
	{
		Model = model;
		ModelId = ((model != null) ? model.ModelId.ToString() : "");
		MaxSimilarBadgesReached = maxReached;
		ScrapSelected = false;
	}

	public bool Equals(BadgeInfo other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		return string.Equals(ModelId, other.ModelId);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((BadgeInfo)obj);
	}

	public override int GetHashCode()
	{
		if (ModelId == null)
		{
			return 0;
		}
		return ModelId.GetHashCode();
	}
}
