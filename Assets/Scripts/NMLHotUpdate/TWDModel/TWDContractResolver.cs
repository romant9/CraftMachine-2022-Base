using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TWDModel
{
	public class TWDContractResolver : DefaultContractResolver
	{
		private static Dictionary<string, JsonProperty> properties = new Dictionary<string, JsonProperty>();

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			lock (properties)
			{
				string key = member.DeclaringType.Name + "." + member.Name;
				if (properties.TryGetValue(key, out var value))
				{
					return value;
				}
				JsonProperty jsonProperty = base.CreateProperty(member, memberSerialization);
				if (!jsonProperty.Writable)
				{
					PropertyInfo propertyInfo = member as PropertyInfo;
					if (propertyInfo != null)
					{
						bool writable = propertyInfo.GetSetMethod(nonPublic: true) != null;
						jsonProperty.Writable = writable;
					}
				}
				properties.Add(key, jsonProperty);
				return jsonProperty;
			}
		}
	}
}
