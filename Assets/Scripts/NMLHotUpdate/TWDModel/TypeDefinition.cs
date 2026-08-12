using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class TypeDefinition
	{
		public int Order;

		public ItemType Type;

		public string TypeIcon;

		public List<string> SubType;

		private List<ItemDefinition> items;

		[JsonIgnore]
		public List<ItemDefinition> ItemDefinitions => items;

		public void SetItems(List<ItemDefinition> itemDefinitions)
		{
			items = itemDefinitions;
		}
	}
}
