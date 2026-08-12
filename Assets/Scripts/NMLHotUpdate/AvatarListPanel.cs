using System.Collections.Generic;
using TWDModel;

public class AvatarListPanel : ScrollableListPanel<AvatarBaseDefinition>
{
	public void Init<T>(List<T> datas) where T : AvatarBaseDefinition
	{
		SetCards(datas);
	}
}
