using System;
using System.Collections.Generic;
using System.Reflection;

namespace TWDModel
{
	public class ReflectionUtils
	{
		private static Dictionary<Type, List<Type>> cachedDerivedTypes = new Dictionary<Type, List<Type>>();

		public static List<Type> GetDerivedTypes(Type baseType)
		{
			if (!cachedDerivedTypes.ContainsKey(baseType))
			{
				List<Type> list = new List<Type>();
				Type[] types = Assembly.GetExecutingAssembly().GetTypes();
				foreach (Type type in types)
				{
					if (type.IsSubclassOf(baseType) && type != baseType)
					{
						list.Add(type);
					}
				}
				cachedDerivedTypes[baseType] = list;
			}
			return cachedDerivedTypes[baseType];
		}

		public static Type FindDerivedType(Type baseType, string identifier)
		{
			List<Type> derivedTypes = GetDerivedTypes(baseType);
			if (derivedTypes != null)
			{
				foreach (Type item in derivedTypes)
				{
					if (item.Name == identifier)
					{
						return item;
					}
				}
			}
			return null;
		}

		public static Type FindDerivedTypeStartingWith(Type baseType, string identifier)
		{
			List<Type> derivedTypes = GetDerivedTypes(baseType);
			if (derivedTypes != null)
			{
				foreach (Type item in derivedTypes)
				{
					if (item.Name.StartsWith(identifier))
					{
						return item;
					}
				}
			}
			return null;
		}

		public static Type FindDerivedTypeOrInterfaceStartingWith(Type baseType, string identifier)
		{
			Type[] types = Assembly.GetExecutingAssembly().GetTypes();
			foreach (Type type in types)
			{
				if ((type.IsSubclassOf(baseType) || baseType.IsAssignableFrom(type)) && type != baseType && type.Name.StartsWith(identifier))
				{
					return type;
				}
			}
			return null;
		}

		public static object Instantiate(Type type, List<string> inConstructorParams)
		{
			int num = inConstructorParams?.Count ?? 0;
			try
			{
				if (type != null)
				{
					ConstructorInfo[] constructors = type.GetConstructors();
					for (int i = 0; i < constructors.Length; i++)
					{
						ParameterInfo[] parameters = constructors[i].GetParameters();
						if (((parameters != null) ? parameters.Length : 0) != num)
						{
							continue;
						}
						List<object> list = new List<object>();
						for (int j = 0; j < parameters.Length; j++)
						{
							if (parameters[j].ParameterType == typeof(FixedPoint) && inConstructorParams[j].GetType() != typeof(FixedPoint))
							{
								list.Add(new FixedPoint(inConstructorParams[j].ToString()));
							}
							else
							{
								list.Add(Convert.ChangeType(inConstructorParams[j], parameters[j].ParameterType));
							}
						}
						object obj = null;
						try
						{
							obj = Activator.CreateInstance(type, list.ToArray());
						}
						catch (Exception)
						{
						}
						if (obj != null)
						{
							return obj;
						}
					}
				}
			}
			catch (Exception)
			{
			}
			return null;
		}
	}
}
