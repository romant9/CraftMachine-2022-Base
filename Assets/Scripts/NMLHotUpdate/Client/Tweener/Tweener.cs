using UnityEngine;

namespace Client.Tweener
{
	public class Tweener
	{
		public delegate void CallBackDelegate();

		public delegate float TweenDelegate(float t, float b, float c, float d);

		private CallBackDelegate _Callback;

		private TweenDelegate _Easing;

		private Vector4 _From = Vector4.zero;

		private Vector4 _To = Vector4.zero;

		private float _ProgressPct;

		private bool _Animating;

		private float _TimeElapsed;

		private float _Duration = 1f;

		private Vector4 _Progression = Vector4.zero;

		public float progressPct => _ProgressPct;

		public bool animating => _Animating;

		public Vector4 progression => _Progression;

		public Vector4 from
		{
			get
			{
				return _From;
			}
			set
			{
				_From = value;
			}
		}

		public Vector4 to
		{
			get
			{
				return _To;
			}
			set
			{
				_To = value;
			}
		}

		public CallBackDelegate currectCallback
		{
			get
			{
				return _Callback;
			}
			set
			{
				_Callback = value;
			}
		}

		public void easeFromTo(Vector4 from, Vector4 to, float duration = 1f, TweenDelegate easing = null, CallBackDelegate callback = null)
		{
			if (easing == null)
			{
				easing = EasingFunctions.Linear;
			}
			_Easing = easing;
			_Callback = callback;
			_From = from;
			_To = to;
			_Duration = duration;
			_TimeElapsed = 0f;
			_ProgressPct = 0f;
			_Animating = true;
		}

		public Vector4 update(bool callCallBack = true)
		{
			if (_Animating)
			{
				if (_TimeElapsed < _Duration)
				{
					if (_Easing != null)
					{
						_Progression.x = _Easing(_TimeElapsed, _From.x, _To.x - _From.x, _Duration);
						_Progression.y = _Easing(_TimeElapsed, _From.y, _To.y - _From.y, _Duration);
						_Progression.z = _Easing(_TimeElapsed, _From.z, _To.z - _From.z, _Duration);
						_Progression.w = _Easing(_TimeElapsed, _From.w, _To.w - _From.w, _Duration);
						_ProgressPct = _TimeElapsed / _Duration;
						_TimeElapsed += Time.deltaTime;
					}
				}
				else
				{
					_Progression = _To;
					_Animating = false;
					_TimeElapsed = 0f;
					_ProgressPct = 1f;
					if (callCallBack && _Callback != null)
					{
						_Callback();
						_Callback = null;
					}
				}
			}
			return _Progression;
		}
	}
}
