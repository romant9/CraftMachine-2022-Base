using System;

public class OSVersion
{
	private readonly int[] version;

	public int this[int index] => version[index];

	public int Value
	{
		get
		{
			int num = 0;
			int num2 = 24;
			for (int i = 0; i < version.Length; i++)
			{
				num += version[i] << num2;
				num2 -= 8;
			}
			return num;
		}
	}

	public OSVersion(string version)
	{
		this.version = Array.ConvertAll(version.Split('.'), delegate(string s)
		{
			int.TryParse(s, out var result);
			return result;
		});
	}

	public static implicit operator OSVersion(string versionString)
	{
		return new OSVersion(versionString);
	}

	public static bool operator <(OSVersion a, OSVersion b)
	{
		return (a?.Value ?? (-1)) < (b?.Value ?? (-1));
	}

	public static bool operator >(OSVersion a, OSVersion b)
	{
		return (a?.Value ?? (-1)) > (b?.Value ?? (-1));
	}

	public static bool operator ==(OSVersion a, OSVersion b)
	{
		return (a?.Value ?? (-1)) == (b?.Value ?? (-1));
	}

	public static bool operator !=(OSVersion a, OSVersion b)
	{
		return (a?.Value ?? (-1)) != (b?.Value ?? (-1));
	}

	public static bool operator >=(OSVersion a, OSVersion b)
	{
		return (a?.Value ?? (-1)) >= (b?.Value ?? (-1));
	}

	public static bool operator <=(OSVersion a, OSVersion b)
	{
		return (a?.Value ?? (-1)) <= (b?.Value ?? (-1));
	}

	public override bool Equals(object other)
	{
		if (other is OSVersion || other is string)
		{
			return Value == ((OSVersion)other).Value;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override string ToString()
	{
		return string.Concat(version, '.');
	}
}
