using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public abstract class TWDModelObject : ModelObject
	{
		[JsonIgnore]
		public TWDModelManager manager => base.Manager as TWDModelManager;

		[JsonIgnore]
		public GameEconomyData gameEconomyData => manager.GameEconomyData;

		public override void Start()
		{
			if (base.ModelId == 0)
			{
				base.Start();
			}
		}

		public static void AddRecursively(Dictionary<int, ModelObject> models, ModelObject model)
		{
			if (models.ContainsKey(model.ModelId))
			{
				return;
			}
			models.Add(model.ModelId, model);
			List<ModelObject> list = new List<ModelObject>();
			PropertyInfo[] properties = model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (PropertyInfo propertyInfo in properties)
			{
				Type propertyType = propertyInfo.PropertyType;
				bool flag = Attribute.IsDefined(propertyInfo, typeof(JsonIgnoreAttribute));
				if (propertyInfo.Name == "Item" || !propertyInfo.CanRead || propertyType.IsPrimitive || flag || propertyType == typeof(string) || propertyType == typeof(bool[]) || propertyType == typeof(EnumerableGrid))
				{
					continue;
				}
				try
				{
					if (typeof(ModelObject).IsAssignableFrom(propertyType))
					{
						if (propertyInfo.GetValue(model, null) is ModelObject item)
						{
							list.Add(item);
						}
						continue;
					}
					Type type = (propertyType.IsGenericType ? propertyType.GetGenericTypeDefinition() : null);
					if (propertyType.IsGenericType && type == typeof(List<>))
					{
						Type[] array = (propertyType.IsGenericType ? propertyType.GetGenericArguments() : null);
						if (!typeof(ModelObject).IsAssignableFrom(array[0]) || !(propertyInfo.GetValue(model, null) is IEnumerable enumerable))
						{
							continue;
						}
						foreach (ModelObject item4 in enumerable)
						{
							list.Add(item4);
						}
						continue;
					}
					if (!typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType) || !(propertyInfo.GetValue(model, null) is IEnumerable enumerable2))
					{
						continue;
					}
					foreach (object item5 in enumerable2)
					{
						if (item5 is ModelObject item3)
						{
							list.Add(item3);
						}
					}
				}
				catch (Exception ex)
				{
					if (model.Manager != null)
					{
						model.Manager.Debug.LogWarning(ex.ToString() + "@" + model.ToString());
					}
				}
			}
			FieldInfo[] fields = model.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.FieldType.IsPrimitive)
				{
					continue;
				}
				if (typeof(IEnumerable).IsAssignableFrom(fieldInfo.FieldType))
				{
					object value = fieldInfo.GetValue(model);
					if (value is IEnumerable)
					{
						foreach (object item6 in value as IEnumerable)
						{
							if (item6 is ModelObject)
							{
								list.Add(item6 as ModelObject);
							}
						}
					}
				}
				if (typeof(ModelObject).IsAssignableFrom(fieldInfo.FieldType))
				{
					object value2 = fieldInfo.GetValue(model);
					if (value2 is ModelObject)
					{
						list.Add(value2 as ModelObject);
					}
				}
			}
			foreach (ModelObject item7 in list)
			{
				AddRecursively(models, item7);
			}
		}
	}
}
