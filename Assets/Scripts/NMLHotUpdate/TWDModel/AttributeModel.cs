using System.Collections.Generic;

namespace TWDModel
{
	public class AttributeModel : TWDModelObject
	{
		public Dictionary<string, FixedPoint> CustomProperties;

		public FixedPoint zong()
		{
			return 1L;
		}

		public override void Start()
		{
			base.Start();
			List<AttributeDefinition> attributeDefinition = base.manager.GameEconomyData.GetAttributeDefinition();
			if (attributeDefinition == null)
			{
				base.manager.Debug.LogError("AttributeDefinitions GED Is Null ");
				return;
			}
			foreach (AttributeDefinition item in attributeDefinition)
			{
				if (item == null)
				{
					base.manager.Debug.LogError("AttributeDefinition 为空，跳过该元素");
					continue;
				}
				if (item.ID == null)
				{
					base.manager.Debug.LogError("AttributeDefinition ID 为空，跳过该元素");
					continue;
				}
				if (CustomProperties == null)
				{
					CustomProperties = new Dictionary<string, FixedPoint>();
				}
				if (!CustomProperties.ContainsKey(item.ID))
				{
					CustomProperties.Add(item.ID, item.StartValue);
				}
			}
		}

		public bool UpdateResetAttributeModelValue(string Id, FixedPoint numer)
		{
			AttributeDefinition attributeDefinitionById = base.manager.GameEconomyData.GetAttributeDefinitionById(Id);
			if (attributeDefinitionById != null && CustomProperties != null)
			{
				CustomProperties[Id] = UtilsMath.Clamp((float)numer, attributeDefinitionById.Min, attributeDefinitionById.Max);
				return true;
			}
			base.manager.Debug.LogError("AttributeDefinitions GED Is Null ID:" + Id);
			return false;
		}

		public void UpdateAttributeModelValueTotalization(string Id, FixedPoint numer)
		{
			CustomProperties[Id] = numer;
		}

		public void UpdateAttributeModelValueTotalizationNew(string Id, FixedPoint numer)
		{
			CustomProperties[Id] -= numer;
		}

		public FixedPoint GetAttributeModelValue(string Id)
		{
			if (CustomProperties != null)
			{
				return CustomProperties[Id];
			}
			return 0L;
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
