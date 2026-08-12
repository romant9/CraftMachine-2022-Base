using System;

namespace BaseModel
{
	[Flags]
	public enum LoadMode
	{
		None = 0,
		Server = 1,
		Client = 2
	}
}
