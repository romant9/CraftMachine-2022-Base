using System;

namespace BaseModel
{
	public interface IMessageSerializer
	{
		string SerializeObject(object value);

		string Serialize(object value, bool indent = false);

		T DeserializeObject<T>(string value);

		T Deserialize<T>(string value);

		object DeserializeObject(Type type, string value);
	}
}
