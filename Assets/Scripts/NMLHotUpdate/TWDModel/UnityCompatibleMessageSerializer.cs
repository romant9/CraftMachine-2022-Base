using System;
using BaseModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TWDModel
{
	public sealed class UnityCompatibleMessageSerializer : IMessageSerializer
	{
		private readonly JsonSerializerSettings settings = new JsonSerializerSettings();

		public UnityCompatibleMessageSerializer()
		{
			settings.TypeNameHandling = TypeNameHandling.None;
			settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
			settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
			settings.Converters.Add(new CustomFloatConverter());
			settings.Converters.Add(new IsoDateTimeConverter());
			settings.NullValueHandling = NullValueHandling.Ignore;
			settings.DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;
		}

		public string Serialize(object value, bool indent = false)
		{
			return JsonConvert.SerializeObject(value, indent ? Formatting.Indented : Formatting.None, settings);
		}

		public string SerializeObject(object value)
		{
			return Serialize(value);
		}

		public T Deserialize<T>(string value)
		{
			return JsonConvert.DeserializeObject<T>(value, settings);
		}

		public T DeserializeObject<T>(string value)
		{
			return Deserialize<T>(value);
		}

		public object DeserializeObject(Type type, string value)
		{
			return JsonConvert.DeserializeObject(value, type, settings);
		}
	}
}
