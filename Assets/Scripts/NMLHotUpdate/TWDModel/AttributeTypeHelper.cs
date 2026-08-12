using System;
using System.Collections.Generic;
using System.Reflection;

namespace TWDModel
{
	public static class AttributeTypeHelper
	{
		public static Dictionary<AttributeType, MethodInfo> GetAttributeTypeMethodDict(Type interfaceType)
		{
			Dictionary<AttributeType, MethodInfo> dictionary = new Dictionary<AttributeType, MethodInfo>();
			if (!interfaceType.IsInterface)
			{
				return dictionary;
			}
			List<MethodInfo> methods = new List<MethodInfo>();
			CollectMethods(interfaceType);
			foreach (MethodInfo item in methods)
			{
				AttributeTypeAttribute customAttribute = item.GetCustomAttribute<AttributeTypeAttribute>();
				if (customAttribute != null)
				{
					dictionary[customAttribute.AttributeType] = item;
				}
			}
			return dictionary;
			void CollectMethods(Type type)
			{
				methods.AddRange(type.GetMethods());
				Type[] interfaces = type.GetInterfaces();
				for (int i = 0; i < interfaces.Length; i++)
				{
					CollectMethods(interfaces[i]);
				}
			}
		}
	}
}
