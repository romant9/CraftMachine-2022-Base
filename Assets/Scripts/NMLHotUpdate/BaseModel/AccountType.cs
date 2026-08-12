using System;

namespace BaseModel
{
	public enum AccountType
	{
		GameCenter = 0,
		GooglePlay = 1,
		WindowsEditor = 3,
		Steam = 4,
		[Obsolete("Not used anymore")]
		Facebook = 2
	}
}
