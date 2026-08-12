using System;
using System.Collections.Generic;

[Serializable]
public struct SmartTutorialConfiguration
{
	public SmartTutorialType Type;

	public PrefabResource PrefabResource;

	public List<int> IncludeMissionTags;

	public List<int> ExcludeMissionTags;
}
