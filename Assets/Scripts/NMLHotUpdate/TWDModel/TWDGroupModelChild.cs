using System;
using System.Collections.Generic;
using System.Reflection;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public abstract class TWDGroupModelChild
	{
		[JsonIgnore]
		protected List<PropertyInfo> modelProperties;

		[JsonIgnore]
		protected List<TWDGroupModelChild> modelObjects;

		private ICustomLogger _logger;

		private GameEconomyData gedInternal;

		private TWDGroupModelChild rootInternal;

		private ICustomLogger logger
		{
			get
			{
				if (_logger == null)
				{
					_logger = CustomModelLogger<GVGGroupModelChildCustomLogger>.Logger;
				}
				return _logger;
			}
		}

		protected TWDGroupModelChild Debug => this;

		protected GameEconomyData gameEconomyData => gedInternal;

		protected TWDGroupModelChild root => rootInternal;

		public event GroupModelChildChangeEventHandler Changed;

		public TWDGroupModelChild()
		{
		}

		public virtual void Start()
		{
		}

		public virtual void SetPlayerOwnerAndGameEconomyData(GameEconomyData ged, TWDGroupModelChild root, PlayerModel player)
		{
			gedInternal = ged;
			if (root != null)
			{
				rootInternal = root;
			}
			GetModelProperties();
			if (modelObjects != null)
			{
				for (int i = 0; i < modelObjects.Count; i++)
				{
					modelObjects[i].SetPlayerOwnerAndGameEconomyData(ged, root, player);
				}
			}
			Start();
		}

		public void NotifyChange(string changed, object args = null)
		{
			this.Changed?.Invoke(this, changed, args);
		}

		public void UpdateModelObjects()
		{
			if (modelProperties == null)
			{
				return;
			}
			modelObjects = new List<TWDGroupModelChild>();
			for (int i = 0; i < modelProperties.Count; i++)
			{
				TWDGroupModelChild tWDGroupModelChild = (TWDGroupModelChild)modelProperties[i].GetValue(this, null);
				if (tWDGroupModelChild != null)
				{
					modelObjects.Add(tWDGroupModelChild);
				}
			}
		}

		public void Log(object obj)
		{
			logger.Log(null, obj.ToString());
		}

		public void LogWarning(object obj)
		{
			logger.LogWarning(null, obj.ToString());
		}

		public void LogError(object obj)
		{
			logger.LogError(null, obj.ToString());
		}

		protected bool IsModelProperty(PropertyInfo property)
		{
			if (property.PropertyType.IsSubclassOf(typeof(TWDGroupModelChild)) && !Attribute.IsDefined(property, typeof(IgnoreModelPropertyAttribute)))
			{
				return !Attribute.IsDefined(property, typeof(JsonIgnoreAttribute));
			}
			return false;
		}

		protected void GetModelProperties()
		{
			if (modelProperties != null)
			{
				UpdateModelObjects();
				return;
			}
			modelProperties = null;
			PropertyInfo[] properties = GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (IsModelProperty(propertyInfo))
				{
					if (modelProperties == null)
					{
						modelProperties = new List<PropertyInfo>();
					}
					modelProperties.Add(propertyInfo);
				}
			}
			UpdateModelObjects();
		}
	}
}
