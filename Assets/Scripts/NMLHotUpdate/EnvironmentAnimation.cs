using System.Collections.Generic;
using UnityEngine;

public class EnvironmentAnimation : MonoBehaviour
{
	public List<EnvironmentAnimationEntry> Animations;

	public List<EnvironmentAnimationLocation> Locations;

	public EnvironmentAnimationLocation GetClosestLocation(Vector3 position)
	{
		EnvironmentAnimationLocation result = null;
		if (Locations != null)
		{
			float num = float.MaxValue;
			for (int i = 0; i < Locations.Count; i++)
			{
				EnvironmentAnimationLocation environmentAnimationLocation = Locations[i];
				float num2 = Vector3.Distance(position, environmentAnimationLocation.GetWorldPosition(base.transform));
				if (num2 < num)
				{
					num = num2;
					result = environmentAnimationLocation;
				}
			}
		}
		return result;
	}

	public AnimationClip GetAnimationClip(EnvironmentAnimationType animationType)
	{
		if (Animations != null)
		{
			for (int i = 0; i < Animations.Count; i++)
			{
				if (Animations[i].AnimationType == animationType)
				{
					return Animations[i].Animation;
				}
			}
		}
		return null;
	}
}
