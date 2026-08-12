using System;
using System.Collections.Generic;
using BaseModel.ContentTypes;

public class PlayerHubNewsItem : NewsItem
{
	public enum AttributeTag
	{
		None = 0,
		PrefabSrc = 1,
		PlacementId = 2,
		ArticleId = 3,
		EntryId = 4
	}

	public const string MoreInfo = "MORE_INFO";

	public const string Poll = "POLL";

	public const string Quiz = "QUIZ";

	private bool attributesParsed;

	private Dictionary<int, string> attributeTagsMapping = new Dictionary<int, string>();

	private const char attributeSeperator = ',';

	public const char NewsItemKeyValueSeparator = ':';

	private string[] tempArrayAttributes = new string[2];

	private string[] tempArrayKeyValue = new string[2];

	public bool HasBeenRead { get; set; }

	public bool HasAttribute(AttributeTag tag)
	{
		ParseAttributes();
		if (attributeTagsMapping == null)
		{
			return false;
		}
		return attributeTagsMapping.ContainsKey((int)tag);
	}

	public string GetAttributeValue(AttributeTag tag)
	{
		ParseAttributes();
		if (attributeTagsMapping != null && attributeTagsMapping.ContainsKey((int)tag))
		{
			return attributeTagsMapping[(int)tag];
		}
		return "";
	}

	private void ParseAttributes()
	{
		if (attributesParsed)
		{
			return;
		}
		if (!string.IsNullOrEmpty(base.PromoAttributes))
		{
			attributeTagsMapping = new Dictionary<int, string>();
			base.PromoAttributes = base.PromoAttributes.Replace(" ", "");
			tempArrayAttributes = base.PromoAttributes.Split(',');
			if (tempArrayAttributes.Length != 0)
			{
				for (int i = 0; i < tempArrayAttributes.Length; i++)
				{
					tempArrayKeyValue = tempArrayAttributes[i].Split(':');
					AttributeTag enumValue = AttributeTag.None;
					if (tempArrayKeyValue.Length > 1 && TryParseEnum<AttributeTag>(tempArrayKeyValue[0], out enumValue))
					{
						attributeTagsMapping.Add((int)enumValue, tempArrayKeyValue[1]);
					}
				}
			}
		}
		tempArrayAttributes = new string[2];
		tempArrayKeyValue = new string[2];
		attributesParsed = true;
	}

	private static bool TryParseEnum<TEnum>(string stringValue, out TEnum enumValue)
	{
		if (!Enum.IsDefined(typeof(TEnum), stringValue))
		{
			enumValue = default(TEnum);
			return false;
		}
		enumValue = (TEnum)Enum.Parse(typeof(TEnum), stringValue);
		return true;
	}
}
