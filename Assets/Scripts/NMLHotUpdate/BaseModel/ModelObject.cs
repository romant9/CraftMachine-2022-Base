using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using TwdCustomMod;

namespace BaseModel
{
	public abstract class ModelObject : IModelObject
	{
		[JsonIgnore]
		protected bool started;

		[JsonIgnore]
		protected List<PropertyInfo> modelProperties;

		[JsonIgnore]
		protected List<ModelObject> modelObjects;

		[JsonIgnore]
		public int ModelId { get; protected set; }

		[JsonIgnore]
		public ModelManager Manager
		{
			get
			{
				return OfflineManager.IsLoadDataManager && modelManager == null ? DataManager.Instance.ModelManager : modelManager;
			}
			protected set
			{
				modelManager = value;
			}
		}

		[JsonIgnore]
		private ModelManager modelManager;

		[JsonIgnore]
		public IModelDebug Debug => Manager.Debug;

		public event ModelChangeEventHandler Changed;

		public ModelObject()
		{
		}

		public virtual void Initialize()
		{
		}

		public abstract bool IsValid();

		public virtual void SetManager(ModelManager manager)
		{
			Manager = manager;
			GetModelProperties();
			if (modelObjects != null)
			{
				for (int i = 0; i < modelObjects.Count; i++)
				{
					modelObjects[i].SetManager(manager);
				}
			}
		}

		public virtual void Start()
		{
			if (started)
			{
				throw new Exception("Model object " + ToString() + " already started.");
			}
			if (Manager == null)
			{
				throw new Exception("Starting object with NULL ModelManager");
			}
			if (Manager.StartState == ModelManager.ModelManagerStartState.Initial)
			{
				if (!OfflineManager.IsLoadDataManager) throw new Exception("Trying to start object before manager is started.");
				else global::DebugTWD.LogError("Fuck off the Exception", DebugType.Error);
			}
			if (Manager != null)
			{
				ModelId = Manager.RegisterModel(this);
			}
			GetModelProperties();
			if (modelObjects != null)
			{
				for (int i = 0; i < modelObjects.Count; i++)
				{
					ModelObject modelObject = modelObjects[i];
					if (modelObject.started)
					{
						throw new Exception("Model object " + modelObject.ToString() + " already started (referenced from " + ToString() + ")");
					}
					modelObject.Start();
				}
			}
			started = true;
		}

		public virtual void Tick(long deltaTime)
		{
			if (modelObjects != null)
			{
				for (int i = 0; i < modelObjects.Count; i++)
				{
					modelObjects[i].Tick(deltaTime);
				}
			}
		}

		public bool Validate()
		{
			if (modelObjects != null)
			{
				for (int i = 0; i < modelObjects.Count; i++)
				{
					if (!modelObjects[i].Validate())
					{
						return false;
					}
				}
			}
			if (started && IsValid())
			{
				return true;
			}
			Debug.LogError(ToString() + " failed validation!");
			return false;
		}

		public void NotifyChange(string changed, object args = null)
		{
			if (started && this.Changed != null)
			{
				this.Changed(this, changed, args);
			}
		}

		protected bool IsModelProperty(PropertyInfo property)
		{
			if (property.PropertyType.IsSubclassOf(typeof(ModelObject)) && !Attribute.IsDefined(property, typeof(IgnoreModelPropertyAttribute)))
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

		public void UpdateModelObjects()
		{
			if (modelProperties == null)
			{
				return;
			}
			modelObjects = new List<ModelObject>();
			for (int i = 0; i < modelProperties.Count; i++)
			{
				ModelObject modelObject = (ModelObject)modelProperties[i].GetValue(this, null);
				if (modelObject != null)
				{
					modelObjects.Add(modelObject);
				}
			}
		}

		public override string ToString()
		{
			return "[" + GetType().ToString() + " Id=" + ModelId + "]";
		}

		#region mycode
		public void SetID(int ID)
		{
			ModelId = ID;
		}
		#endregion
	}
}
