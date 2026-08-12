using BaseModel;

public interface IMessageSerializerFactory
{
	IMessageSerializer CreateSerializer(SerializerType type);
}
