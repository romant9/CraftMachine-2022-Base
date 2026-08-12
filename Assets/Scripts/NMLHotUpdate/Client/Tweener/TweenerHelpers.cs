namespace Client.Tweener
{
	public class TweenerHelpers
	{
		public static Tweener.TweenDelegate getGetByEnum(Easing.All easing)
		{
			return easing switch
			{
				Easing.All.BackEaseIn => EasingFunctions.BackEaseIn, 
				Easing.All.BackEaseOut => EasingFunctions.BackEaseOut, 
				Easing.All.BackEaseInOut => EasingFunctions.BackEaseInOut, 
				Easing.All.BackEaseOutIn => EasingFunctions.BackEaseOutIn, 
				Easing.All.CubicEaseOut => EasingFunctions.CubicEaseOut, 
				Easing.All.CubicEaseInOut => EasingFunctions.CubicEaseInOut, 
				Easing.All.CubicEaseOutIn => EasingFunctions.CubicEaseOutIn, 
				Easing.All.ExpoEaseOut => EasingFunctions.ExpoEaseOut, 
				Easing.All.ExpoEaseIn => EasingFunctions.ExpoEaseIn, 
				Easing.All.ExpoEaseInOut => EasingFunctions.ExpoEaseInOut, 
				Easing.All.ExpoEaseOutIn => EasingFunctions.ExpoEaseOutIn, 
				Easing.All.BounceEaseOutIn => EasingFunctions.BounceEaseOutIn, 
				Easing.All.BounceEaseInOut => EasingFunctions.BounceEaseInOut, 
				Easing.All.BounceEaseIn => EasingFunctions.BounceEaseIn, 
				Easing.All.BounceEaseOut => EasingFunctions.BounceEaseOut, 
				Easing.All.ElasticEaseOutIn => EasingFunctions.ElasticEaseOutIn, 
				Easing.All.ElasticEaseInOut => EasingFunctions.ElasticEaseInOut, 
				Easing.All.ElasticEaseIn => EasingFunctions.ElasticEaseIn, 
				Easing.All.ElasticEaseOut => EasingFunctions.ElasticEaseOut, 
				_ => EasingFunctions.Linear, 
			};
		}
	}
}
