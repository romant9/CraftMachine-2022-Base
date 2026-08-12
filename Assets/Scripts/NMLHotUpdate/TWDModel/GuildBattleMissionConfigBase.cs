namespace TWDModel
{
	public class GuildBattleMissionConfigBase
	{
		public const string Walker = "Walker";

		private const string KeyEmpty = "Empty";

		public bool IsEmpty;

		public virtual bool Parse(ref string wrapperName, ref string stringParams, ref int[] intParams, ref string errorMessage)
		{
			if (string.IsNullOrEmpty(wrapperName) || intParams == null)
			{
				return false;
			}
			return true;
		}

		public virtual bool IsValid()
		{
			return false;
		}

		protected bool EvalEmpty(ref string value)
		{
			IsEmpty = value == "Empty";
			return IsEmpty;
		}

		public static bool TryParseDataFromRow(ref string input, ref string wrapperName, ref string stringParams, ref int[] intParams, ref string errorMessage)
		{
			if (string.IsNullOrEmpty(input))
			{
				errorMessage = "Input NULL Error";
				return false;
			}
			input = input.Trim();
			if (!input.Contains('(') || !input.Contains(')'))
			{
				errorMessage = "Format Error, missing '(' or ')', " + input;
				return false;
			}
			string[] array = input.Split('(');
			if (array.Length != 2)
			{
				errorMessage = "Format Error, after '(', " + input;
				return false;
			}
			if (string.IsNullOrEmpty(array[0]) || string.IsNullOrEmpty(array[1]))
			{
				errorMessage = "Format Error, NULL or Empty value when parsing, " + input;
				return false;
			}
			if (intParams == null || intParams.Length != 2)
			{
				intParams = new int[2];
			}
			for (int i = 0; i < intParams.Length; i++)
			{
				intParams[i] = -1;
			}
			array[1] = array[1].Replace(")", "");
			int num = 0;
			int result = 0;
			string[] array2 = array[1].Split(',');
			for (int j = 0; j < array2.Length; j++)
			{
				if (string.IsNullOrEmpty(array2[j]))
				{
					continue;
				}
				if (int.TryParse(array2[j], out result))
				{
					if (num >= intParams.Length)
					{
						errorMessage = "Format Error, too many int values max: " + intParams.Length + ", " + input;
						return false;
					}
					intParams[num] = result;
					num++;
				}
				else if (string.IsNullOrEmpty(stringParams))
				{
					stringParams = array2[j];
				}
				else
				{
					stringParams = stringParams + ", " + array2[j];
				}
			}
			wrapperName = array[0];
			errorMessage = "";
			return true;
		}
	}
}
