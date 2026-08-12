using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ActorAttributeContainerModel : AttributeContainerAbstract
	{
		[IgnoreModelProperty]
		public IAttributeActorSystem SupportModel { get; private set; }

		[JsonIgnore]
		private List<IAttributeActorSystem> ActorAttributeSystems { get; set; }

		[JsonIgnore]
		private Dictionary<AttributeType, MethodInfo> AttributeActorMethods { get; set; }

		public void SetSupportModel(IAttributeActorSystem supportModel)
		{
			SupportModel = supportModel;
			RegisterAttributeTypes();
		}

		public override void Initialize()
		{
			base.Initialize();
			RegisterAttributeTypes();
		}

		public FixedPoint GetAttributeValueByAttributeType(AttributeType attributeType)
		{
			if (ActorAttributeSystems.Count == 0)
			{
				return 0.0;
			}
			if (AttributeActorMethods.Count == 0)
			{
				return 0.0;
			}
			if (!AttributeActorMethods.TryGetValue(attributeType, out var value))
			{
				return 0.0;
			}
			FixedPoint result = 0.0;
			foreach (IAttributeActorSystem actorAttributeSystem in ActorAttributeSystems)
			{
				result += (FixedPoint)value.Invoke(actorAttributeSystem, null);
			}
			return result;
		}

		public override void RegisterAttributeTypes()
		{
			ActorAttributeSystems = new List<IAttributeActorSystem>();
			AttributeActorMethods = AttributeTypeHelper.GetAttributeTypeMethodDict(typeof(IAttributeActorSystem));
			foreach (PropertyInfo item2 in from p in typeof(ActorAttributeContainerModel).GetProperties()
				where typeof(IAttributeActorSystem).IsAssignableFrom(p.PropertyType)
				select p)
			{
				if (item2.GetValue(this) is IAttributeActorSystem item)
				{
					ActorAttributeSystems.Add(item);
				}
			}
		}
	}
}
