using System;
using System.Collections.Generic;
using System.Text;

namespace TWDModel
{
	public class GuildSearchInfo
	{
		public enum SearchType
		{
			Suggestions = 0,
			KeywordSearch = 1,
			SuggestionPopup = 2
		}

		public struct CountStruct
		{
			public int Ad;

			public int New;

			public int SameCountry;

			public int NearLevel;

			public int Fallback;

			public int Keyword;

			public void Initialize()
			{
				Ad = -1;
				New = -1;
				SameCountry = -1;
				NearLevel = -1;
				Fallback = -1;
				Keyword = -1;
			}

			public string ToSuggestionsString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(Ad.ToString());
				stringBuilder.Append(",");
				stringBuilder.Append(New.ToString());
				stringBuilder.Append(",");
				stringBuilder.Append(SameCountry.ToString());
				stringBuilder.Append(",");
				stringBuilder.Append(NearLevel.ToString());
				stringBuilder.Append(",");
				stringBuilder.Append(Fallback.ToString());
				return stringBuilder.ToString();
			}

			public string ToKeywordSearchString()
			{
				return Keyword.ToString();
			}

			public string ToSuggestionPopupString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(ToSuggestionsString());
				stringBuilder.Append(",");
				stringBuilder.Append(Keyword.ToString());
				return stringBuilder.ToString();
			}
		}

		public string SearchId { get; set; }

		public SearchType Type { get; set; }

		public string SearchKeyword { get; set; }

		public CountStruct GuildCountsQueried { get; set; }

		public CountStruct GuildCountsSelected { get; set; }

		public int SelectedCount { get; set; }

		public List<int> SelectedGuildSizes { get; set; }

		public List<int> SelectedGuildQueryIds { get; set; }

		public List<string> SelectedGuildCountryCodes { get; set; }

		public List<int> SelectedGuildAvgPlayerLevels { get; set; }

		public int PlayerLevel { get; set; }

		public string PlayerCountryCode { get; set; }

		public int SearchDuration { get; set; }

		public void Initialize(SearchType searchType, string searchKeyword, int playerLevel, string playerCountryCode)
		{
			GuildCountsQueried = default(CountStruct);
			GuildCountsSelected = default(CountStruct);
			SelectedGuildSizes = new List<int>();
			SelectedGuildQueryIds = new List<int>();
			SelectedGuildCountryCodes = new List<string>();
			SelectedGuildAvgPlayerLevels = new List<int>();
			SearchId = Guid.NewGuid().ToString();
			Type = searchType;
			if (string.IsNullOrEmpty(searchKeyword))
			{
				SearchKeyword = null;
			}
			else
			{
				SearchKeyword = searchKeyword;
			}
			CountStruct guildCountsQueried = GuildCountsQueried;
			guildCountsQueried.Initialize();
			GuildCountsQueried = guildCountsQueried;
			guildCountsQueried = GuildCountsSelected;
			guildCountsQueried.Initialize();
			GuildCountsSelected = guildCountsQueried;
			PlayerLevel = playerLevel;
			PlayerCountryCode = playerCountryCode;
		}

		private string IntListToCSV(List<int> list)
		{
			string[] array = new string[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				array[i] = list[i].ToString();
			}
			return string.Join(",", array);
		}

		private string StringListToCSV(List<string> list)
		{
			return string.Join(",", list.ToArray());
		}

		public string GetGuildCountsQueried()
		{
			return Type switch
			{
				SearchType.KeywordSearch => GuildCountsQueried.ToKeywordSearchString(), 
				SearchType.Suggestions => GuildCountsQueried.ToSuggestionsString(), 
				SearchType.SuggestionPopup => GuildCountsQueried.ToSuggestionPopupString(), 
				_ => null, 
			};
		}

		public string GetSearchPositions()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < SelectedCount; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(i.ToString());
			}
			return stringBuilder.ToString();
		}

		public string GetSearchType()
		{
			return Type switch
			{
				SearchType.KeywordSearch => "Search", 
				SearchType.Suggestions => "Suggest", 
				SearchType.SuggestionPopup => "SuggestPopup", 
				_ => null, 
			};
		}

		public string GetGuildCountsSelected()
		{
			return Type switch
			{
				SearchType.KeywordSearch => GuildCountsSelected.ToKeywordSearchString(), 
				SearchType.Suggestions => GuildCountsSelected.ToSuggestionsString(), 
				SearchType.SuggestionPopup => GuildCountsSelected.ToSuggestionPopupString(), 
				_ => null, 
			};
		}

		public string GetSelectedGuildSizes()
		{
			return IntListToCSV(SelectedGuildSizes);
		}

		public string GetSelectedGuildQueryIds()
		{
			return IntListToCSV(SelectedGuildQueryIds);
		}

		public string GetSelectedGuildCountryCodes()
		{
			return StringListToCSV(SelectedGuildCountryCodes);
		}

		public string GetSelectedGuildAvgPlayerLevels()
		{
			return IntListToCSV(SelectedGuildAvgPlayerLevels);
		}
	}
}
