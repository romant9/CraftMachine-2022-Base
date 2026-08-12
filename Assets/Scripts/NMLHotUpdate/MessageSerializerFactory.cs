using System;
using BaseModel;
using TWDModel;

public class MessageSerializerFactory : IMessageSerializerFactory
{
	public IMessageSerializer CreateSerializer(SerializerType type)
	{
		return type switch
		{
			SerializerType.NewtonSoft => new MessageSerializer(), 
			SerializerType.Unity => new UnityCompatibleMessageSerializer(), 
			_ => throw new NotImplementedException(string.Format("MessageSerializerFactory: Unsupported serializer type requested, type='%s'", type)), 
		};
	}
}
