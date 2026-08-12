using System;
using System.Globalization;

namespace TWDModel
{
	[Serializable]
	public struct FixedPoint
	{
		public long Value;

		public static int Scale = 65536;

		public static FixedPoint MaxValue = new FixedPoint(long.MaxValue / Scale);

		public static FixedPoint MinValue = new FixedPoint(long.MinValue / Scale);

		public static FixedPoint PI => new FixedPoint(3.1416015625);

		public FixedPoint(FixedPoint other)
		{
			Value = other.Value;
		}

		public FixedPoint(long integerValue)
		{
			Value = integerValue * Scale;
		}

		public FixedPoint(int integerValue)
		{
			Value = (long)integerValue * (long)Scale;
		}

		public FixedPoint(double floatingValue)
		{
			Value = (long)(floatingValue * (double)Scale);
		}

		public FixedPoint(string stringValue)
		{
			if (stringValue != null)
			{
				double result = 0.0;
				if (double.TryParse(stringValue, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
				{
					Value = (long)(result * (double)Scale);
				}
				else
				{
					Value = 0L;
				}
			}
			else
			{
				Value = 0L;
			}
		}

		public static implicit operator FixedPoint(long integerValue)
		{
			return new FixedPoint(integerValue);
		}

		public static implicit operator FixedPoint(double floatingValue)
		{
			return new FixedPoint(floatingValue);
		}

		public static explicit operator FixedPoint(string stringValue)
		{
			return new FixedPoint(stringValue);
		}

		public static explicit operator int(FixedPoint fixedPoint)
		{
			return (int)(fixedPoint.Value / Scale);
		}

		public static explicit operator long(FixedPoint fixedPoint)
		{
			return fixedPoint.Value / Scale;
		}

		public static explicit operator float(FixedPoint fixedPoint)
		{
			return (float)((double)fixedPoint.Value / (double)Scale);
		}

		public static explicit operator double(FixedPoint fixedPoint)
		{
			return (double)fixedPoint.Value / (double)Scale;
		}

		public static bool operator ==(FixedPoint a, FixedPoint b)
		{
			return a.Value == b.Value;
		}

		public static bool operator !=(FixedPoint a, FixedPoint b)
		{
			return a.Value != b.Value;
		}

		public static bool operator <(FixedPoint a, FixedPoint b)
		{
			return a.Value < b.Value;
		}

		public static bool operator >(FixedPoint a, FixedPoint b)
		{
			return a.Value > b.Value;
		}

		public static bool operator <=(FixedPoint a, FixedPoint b)
		{
			return a.Value <= b.Value;
		}

		public static bool operator >=(FixedPoint a, FixedPoint b)
		{
			return a.Value >= b.Value;
		}

		public override int GetHashCode()
		{
			return (int)Value;
		}

		public override bool Equals(object other)
		{
			if (other is FixedPoint)
			{
				return Value == ((FixedPoint)other).Value;
			}
			return false;
		}

		public override string ToString()
		{
			return ((double)this).ToString(CultureInfo.InvariantCulture);
		}

		public static FixedPoint operator +(FixedPoint a, FixedPoint b)
		{
			FixedPoint result = default(FixedPoint);
			result.Value = a.Value + b.Value;
			return result;
		}

		public static FixedPoint operator -(FixedPoint a, FixedPoint b)
		{
			FixedPoint result = default(FixedPoint);
			result.Value = a.Value - b.Value;
			return result;
		}

		public static FixedPoint operator -(FixedPoint a)
		{
			FixedPoint result = default(FixedPoint);
			result.Value = -a.Value;
			return result;
		}

		public static FixedPoint operator *(FixedPoint a, FixedPoint b)
		{
			FixedPoint result = default(FixedPoint);
			result.Value = a.Value * b.Value / Scale;
			return result;
		}

		public static FixedPoint operator /(FixedPoint a, FixedPoint b)
		{
			FixedPoint result = default(FixedPoint);
			result.Value = a.Value * Scale / b.Value;
			return result;
		}

		public static FixedPoint Abs(FixedPoint value)
		{
			if (value.Value < 0)
			{
				return -value;
			}
			return value;
		}

		public static FixedPoint Min(FixedPoint a, FixedPoint b)
		{
			if (a.Value < b.Value)
			{
				return a;
			}
			return b;
		}

		public static FixedPoint Max(FixedPoint a, FixedPoint b)
		{
			if (a.Value > b.Value)
			{
				return a;
			}
			return b;
		}

		public static FixedPoint Clamp(FixedPoint a, FixedPoint min, FixedPoint max)
		{
			return Min(Max(a, min), max);
		}

		public static FixedPoint Floor(FixedPoint value)
		{
			FixedPoint result = default(FixedPoint);
			result.Value = value.Value & ~((long)Scale - 1L);
			return result;
		}

		public static FixedPoint Round(FixedPoint value)
		{
			FixedPoint result = default(FixedPoint);
			result.Value = (value.Value + Scale / 2) & ~((long)Scale - 1L);
			return result;
		}

		public static FixedPoint Ceiling(FixedPoint value)
		{
			FixedPoint result = default(FixedPoint);
			result.Value = (value.Value + Scale - 1) & ~((long)Scale - 1L);
			return result;
		}

		public static FixedPoint Sqrt(FixedPoint value)
		{
			return Math.Sqrt((double)value);
		}

		public static FixedPoint Sin(FixedPoint radians)
		{
			FixedPoint fixedPoint = PI * 2L;
			if (radians.Value >= fixedPoint.Value)
			{
				radians.Value %= fixedPoint.Value;
			}
			else if (radians.Value < 0)
			{
				radians.Value %= fixedPoint.Value;
			}
			return Math.Sin((double)radians);
		}

		public static FixedPoint Cos(FixedPoint radians)
		{
			return Sin(radians + PI / 2L);
		}

		public static FixedPoint Asin(FixedPoint d)
		{
			return Math.Asin((double)d);
		}

		public static FixedPoint Acos(FixedPoint d)
		{
			return Math.Acos((double)d);
		}

		public static FixedPoint DegToRad(FixedPoint deg)
		{
			return deg * PI / 180L;
		}

		public static FixedPoint RadToDeg(FixedPoint rad)
		{
			return rad * 180L / PI;
		}
	}
}
