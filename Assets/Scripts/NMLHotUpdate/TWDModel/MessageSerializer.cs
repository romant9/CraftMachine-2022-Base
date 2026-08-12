using System;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Threading;
using BaseModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TWDModel
{
	public sealed class MessageSerializer : IMessageSerializer
	{
		private readonly JsonSerializerSettings settings = new JsonSerializerSettings();

		private readonly ThreadLocal<JsonSerializer> _threadSerializer;

		public MessageSerializer()
		{
			settings.Binder = new TWDSerializationBinder();
			settings.TypeNameHandling = TypeNameHandling.Auto;
			settings.TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple;
			settings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
			settings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
			settings.Converters.Add(new StringEnumConverter());
			settings.Converters.Add(new CustomFloatConverter());
			settings.Converters.Add(new IsoDateTimeConverter());
			settings.Converters.Add(new FixedPointConverter());
			settings.ContractResolver = new TWDContractResolver();
			settings.NullValueHandling = NullValueHandling.Ignore;
			settings.DefaultValueHandling = DefaultValueHandling.Include;
			settings.MaxDepth = 512;
			_threadSerializer = new ThreadLocal<JsonSerializer>(() => JsonSerializer.Create(settings));
		}

		public string Serialize(object value, bool indent = false)
		{
			JsonSerializer value2 = _threadSerializer.Value;
			Formatting formatting = (indent ? Formatting.Indented : Formatting.None);
			using StringWriter stringWriter = new StringWriter();
			using JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter)
			{
				Formatting = formatting
			};
			value2.Serialize(jsonWriter, value);
			return stringWriter.ToString();
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
