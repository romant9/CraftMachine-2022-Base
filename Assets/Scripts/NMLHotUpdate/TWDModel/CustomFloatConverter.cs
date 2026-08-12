using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TWDModel
{
	public class CustomFloatConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(float);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return Convert.ChangeType(serializer.Deserialize<JValue>(reader).Value, typeof(float));
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			float num = (float)value;
			if ((double)num == Math.Floor(num))
			{
				writer.WriteValue((int)num);
			}
			else
			{
				writer.WriteValue(num);
			}
		}
	}
}
