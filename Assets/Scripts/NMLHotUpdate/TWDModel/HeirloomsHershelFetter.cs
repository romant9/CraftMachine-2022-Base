namespace TWDModel
{
	public class HeirloomsHershelFetter
	{
		public FixedPoint Roundm { get; set; }

		public FixedPoint Floor { get; set; }

		public FixedPoint BurnBeRatio { get; set; }

		public FixedPoint BurnBeDmgRatio { get; set; }

		public FixedPoint BurnRefRatio { get; set; }

		public bool UpdateAttributeValueNew(ActorModel target, ActorModel source, FixedPoint burn_be_dmg_ratio, FixedPoint burn_be_ratio, FixedPoint burn_ref_ratio)
		{
			bool result = true;
			(string, FixedPoint)[] array = new(string, FixedPoint)[3]
			{
				("burn_be_ratio", burn_be_ratio),
				("burn_be_dmg_ratio", burn_be_dmg_ratio),
				("burn_ref_ratio", burn_ref_ratio)
			};
			for (int i = 0; i < array.Length; i++)
			{
				(string, FixedPoint) tuple = array[i];
				string item = tuple.Item1;
				FixedPoint item2 = tuple.Item2;
				AttributeDefinition attributeDefinitionById = source.manager.GameEconomyData.GetAttributeDefinitionById(item);
				float num = (float)target.AttributeModel.GetAttributeModelValue(item);
				float num2 = UtilsMath.Clamp((float)item2 + num, attributeDefinitionById.Min, attributeDefinitionById.Max);
				if (item == "burn_be_ratio")
				{
					FixedPoint fixedPoint = num2 - num;
					BurnBeRatio += fixedPoint;
				}
				if (item == "burn_be_dmg_ratio")
				{
					FixedPoint fixedPoint2 = (FixedPoint)num2 - (FixedPoint)num;
					BurnBeDmgRatio += fixedPoint2;
				}
				if (item == "burn_ref_ratio")
				{
					FixedPoint fixedPoint3 = (FixedPoint)num2 - (FixedPoint)num;
					BurnRefRatio += fixedPoint3;
				}
				target.AttributeModel.UpdateAttributeModelValueTotalization(item, num2);
			}
			return result;
		}
	}
}
