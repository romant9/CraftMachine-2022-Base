using System.Linq.Expressions;

namespace BaseModel
{
	public interface IAnnotationProperties
	{
		ExpressionType[] Operator { get; set; }

		string Name { get; set; }

		AllowedDataType AllowedDataTypes { get; set; }

		bool Unique { get; set; }

		SegmentalPropertyHandlerType SegmentalPropertyHandlerType { get; set; }
	}
}
