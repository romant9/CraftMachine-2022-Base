using System;

internal class GEDDateTimeParser
{
	private static bool ParseSingleDigitDateValue(out int result, char c1)
	{
		if (c1 < '0' || c1 > '9')
		{
			result = 0;
			return false;
		}
		result = c1 - 48;
		return true;
	}

	private static bool ParseTwoDigitDateValue(out int result, char c1, char c2)
	{
		if (c1 < '0' || c1 > '9')
		{
			result = 0;
			return false;
		}
		if (c2 < '0' || c2 > '9')
		{
			result = 0;
			return false;
		}
		result = c2 - 48 + (c1 - 48) * 10;
		return true;
	}

	private static bool ParseFourDigitDateValue(out int result, char c1, char c2, char c3, char c4)
	{
		if (c1 < '0' || c1 > '9')
		{
			result = 0;
			return false;
		}
		if (c2 < '0' || c2 > '9')
		{
			result = 0;
			return false;
		}
		if (c3 < '0' || c3 > '9')
		{
			result = 0;
			return false;
		}
		if (c4 < '0' || c4 > '9')
		{
			result = 0;
			return false;
		}
		result = c4 - 48 + (c3 - 48) * 10 + (c2 - 48) * 100 + (c1 - 48) * 1000;
		return true;
	}

	public static bool TryParseDateTimeOptimized(string value, ref DateTime result)
	{
		if (value == null)
		{
			return false;
		}
		if (value == "")
		{
			return false;
		}
		if (value.Length < 19 || value.Length > 22)
		{
			return false;
		}
		int num = 0;
		int num2 = 1;
		if (value[num + 1] == '/')
		{
			num2 = 1;
		}
		else
		{
			if (value[num + 2] != '/')
			{
				return false;
			}
			num2 = 2;
		}
		int result2;
		if (num2 == 2)
		{
			if (!ParseTwoDigitDateValue(out result2, value[num], value[num + 1]))
			{
				return false;
			}
		}
		else if (!ParseSingleDigitDateValue(out result2, value[num]))
		{
			return false;
		}
		num += num2 + 1;
		int num3 = 1;
		if (value[num + 1] == '/')
		{
			num3 = 1;
		}
		else
		{
			if (value[num + 2] != '/')
			{
				return false;
			}
			num3 = 2;
		}
		int result3;
		if (num3 == 2)
		{
			if (!ParseTwoDigitDateValue(out result3, value[num], value[num + 1]))
			{
				return false;
			}
		}
		else if (!ParseSingleDigitDateValue(out result3, value[num]))
		{
			return false;
		}
		num += num3 + 1;
		if (!ParseFourDigitDateValue(out var result4, value[num], value[num + 1], value[num + 2], value[num + 3]))
		{
			return false;
		}
		num += 4;
		if (value[num] != ' ')
		{
			return false;
		}
		num++;
		int num4 = 1;
		if (value[num + 1] == ':')
		{
			num4 = 1;
		}
		else
		{
			if (value[num + 2] != ':')
			{
				return false;
			}
			num4 = 2;
		}
		int result5;
		if (num4 == 2)
		{
			if (!ParseTwoDigitDateValue(out result5, value[num], value[num + 1]))
			{
				return false;
			}
		}
		else if (!ParseSingleDigitDateValue(out result5, value[num]))
		{
			return false;
		}
		num += num4 + 1;
		if (value[num + 2] != ':')
		{
			return false;
		}
		if (!ParseTwoDigitDateValue(out var result6, value[num], value[num + 1]))
		{
			return false;
		}
		num += 3;
		if (!ParseTwoDigitDateValue(out var result7, value[num], value[num + 1]))
		{
			return false;
		}
		num += 2;
		if (value.Length != num + 3)
		{
			return false;
		}
		if (value[num] != ' ')
		{
			return false;
		}
		num++;
		bool flag = false;
		if (value[num] == 'P' && value[num + 1] == 'M')
		{
			flag = true;
		}
		else if (value[num] != 'A' || value[num + 1] != 'M')
		{
			return false;
		}
		if (!flag && result5 == 12)
		{
			result5 = 0;
		}
		result = new DateTime(result4, result2, result3, result5, result6, result7);
		if (flag && result5 != 12)
		{
			result = result.AddHours(12.0);
		}
		return true;
	}
}
