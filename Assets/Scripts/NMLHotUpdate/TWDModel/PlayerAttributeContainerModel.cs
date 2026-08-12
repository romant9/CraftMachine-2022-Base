using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace TWDModel
{
	public class PlayerAttributeContainerModel : AttributeContainerAbstract
	{
		[JsonIgnore]
		private List<IAttributePlayerSystem> PlayerAttributeSystems { get; set; }

		[JsonIgnore]
		private Dictionary<AttributeType, MethodInfo> AttributePlayerMethods { get; set; }

		public FixedPoint GetAttributeValueByAttributeType(AttributeType attributeType)
		{
			if (PlayerAttributeSystems.Count == 0)
			{
				return 0.0;
			}
			if (AttributePlayerMethods.Count == 0)
			{
				return 0.0;
			}
			if (!AttributePlayerMethods.TryGetValue(attributeType, out var value))
			{
				return 0.0;
			}
			FixedPoint result = 0.0;
			foreach (IAttributePlayerSystem playerAttributeSystem in PlayerAttributeSystems)
			{
				result += (FixedPoint)value.Invoke(playerAttributeSystem, null);
			}
			return result;
		}

		public override void RegisterAttributeTypes()
		{
			PlayerAttributeSystems = new List<IAttributePlayerSystem>();
			AttributePlayerMethods = AttributeTypeHelper.GetAttributeTypeMethodDict(typeof(IAttributePlayerSystem));
			foreach (PropertyInfo item2 in from p in typeof(PlayerAttributeContainerModel).GetProperties()
				where typeof(IAttributePlayerSystem).IsAssignableFrom(p.PropertyType)
				select p)
			{
				if (item2.GetValue(this) is IAttributePlayerSystem item)
				{
					PlayerAttributeSystems.Add(item);
				}
			}
		}
	}
}
