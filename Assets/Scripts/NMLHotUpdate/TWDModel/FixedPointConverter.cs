using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TWDModel
{
	public class FixedPointConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(FixedPoint);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.StartObject)
			{
				JObject jObject = serializer.Deserialize<JObject>(reader);
				return new FixedPoint
				{
					Value = jObject.Value<long>("Value")
				};
			}
			return new FixedPoint((float)Convert.ChangeType(serializer.Deserialize<JValue>(reader).Value, typeof(float)));
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			float num = (float)(FixedPoint)value;
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
