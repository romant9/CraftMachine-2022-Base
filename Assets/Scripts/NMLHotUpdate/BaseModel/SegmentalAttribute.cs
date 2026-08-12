using System;
using System.Linq.Expressions;

namespace BaseModel
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class SegmentalAttribute : Attribute, IAnnotationProperties
	{
		public virtual ExpressionType[] Operator { get; set; }

		public virtual string Name { get; set; }

		public virtual AllowedDataType AllowedDataTypes { get; set; }

		public virtual SegmentalPropertyHandlerType SegmentalPropertyHandlerType { get; set; }

		public virtual bool Unique { get; set; }
	}
}
