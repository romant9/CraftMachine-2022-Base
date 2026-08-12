using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class BlackMarketSlotDefinition
	{
		public int SlotId;

		public string ActorDefinitionID;

		public string ItemCategory;

		[JsonIgnore]
		private List<string> _categories;

		[JsonIgnore]
		public List<string> Categories
		{
			get
			{
				if (_categories == null)
				{
					_categories = (string.IsNullOrWhiteSpace(ItemCategory) ? new List<string>() : (from s in ItemCategory.Split(',')
						select s.Trim()).ToList());
				}
				return _categories;
			}
		}
	}
}
