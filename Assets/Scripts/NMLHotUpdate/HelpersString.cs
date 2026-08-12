public class HelpersString
{
	public static string FormatNumberWithSign(int number)
	{
		if (number > 0)
		{
			return "+" + number;
		}
		return number.ToString();
	}

	public static string FormatNumberWithToken(int number, int allNumber)
	{
		if (number >= allNumber)
		{
			return $"{number}/{allNumber}";
		}
		return $"[ff0000]{number}[-]/{allNumber}";
	}
}
