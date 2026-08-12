using System.Collections.Generic;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(UILabel))]
[AddComponentMenu("NGUI/Interaction/Typewriter Effect")]
public class TypewriterEffect : MonoBehaviour
{
	private struct FadeEntry
	{
		public int index;

		public string text;

		public float alpha;
	}

	public static TypewriterEffect current;

	[Tooltip("How many characters will be printed per second.")]
	public int charsPerSecond = 20;

	[Tooltip("How long it takes for each character to fade in.")]
	public float fadeInTime;

	[Tooltip("How long to pause when a period is encountered (in seconds).")]
	public float delayOnPeriod;

	[Tooltip("How long to pause when a new line character is encountered (in seconds).")]
	public float delayOnNewLine;

	[Tooltip("If a scroll view is specified, its UpdatePosition() function will be called every time the text is updated.")]
	public UIScrollView scrollView;

	[Tooltip("If set to 'true', the label's dimensions will be that of a fully faded-in content.")]
	public bool keepFullDimensions;

	public List<EventDelegate> onFinished = new List<EventDelegate>();

	private UILabel mLabel;

	private string mFullText;

	private string mMyText;

	private int mCurrentOffset;

	private float mNextChar;

	private bool mReset = true;

	private bool mActive;

	private BetterList<FadeEntry> mFade = new BetterList<FadeEntry>();

	public bool isActive => mActive;

	public void ResetToBeginning()
	{
		if (mActive && mLabel != null && !string.IsNullOrEmpty(mFullText) && mMyText == mLabel.text)
		{
			mMyText = mFullText;
			mLabel.text = mMyText;
		}
		mCurrentOffset = 0;
		mReset = true;
		mActive = true;
		mFade.Clear();
		Update();
	}

	public void Finish()
	{
		if (!mActive)
		{
			return;
		}
		mActive = false;
		if (!string.IsNullOrEmpty(mFullText))
		{
			if (!mReset && mLabel != null && mMyText == mLabel.text)
			{
				mLabel.text = mFullText;
			}
			mMyText = mFullText;
			mCurrentOffset = mFullText.Length;
		}
		mFade.Clear();
		if (keepFullDimensions && scrollView != null)
		{
			scrollView.UpdatePosition();
		}
		current = this;
		EventDelegate.Execute(onFinished);
		current = null;
	}

	private void OnEnable()
	{
		mReset = true;
		mActive = true;
	}

	private void OnDisable()
	{
		Finish();
	}

	private void OnApplicationQuit()
	{
		onFinished = null;
	}

	private void Update()
	{
		if (!mActive)
		{
			return;
		}
		if (mLabel != null && mLabel.text != mMyText)
		{
			mReset = true;
		}
		if (mReset)
		{
			mReset = false;
			mNextChar = 0f;
			mCurrentOffset = 0;
			mLabel = GetComponent<UILabel>();
			mFullText = mLabel.processedText;
			mMyText = mFullText;
			mFade.Clear();
			if (keepFullDimensions && scrollView != null)
			{
				scrollView.UpdatePosition();
			}
		}
		if (string.IsNullOrEmpty(mFullText))
		{
			return;
		}
		int length = mFullText.Length;
		while (mCurrentOffset < length && mNextChar <= RealTime.time)
		{
			int num = mCurrentOffset;
			charsPerSecond = Mathf.Max(1, charsPerSecond);
			if (mLabel.supportEncoding)
			{
				while (NGUIText.ParseSymbol(mFullText, ref mCurrentOffset))
				{
				}
			}
			mCurrentOffset++;
			if (mCurrentOffset > length)
			{
				break;
			}
			float num2 = 1f / (float)charsPerSecond;
			char c = ((num < length) ? mFullText[num] : '\n');
			char c2 = ((num + 1 < length) ? mFullText[num + 1] : '\n');
			if (c == '\n')
			{
				if (num > 0)
				{
					switch (mFullText[num - 1])
					{
					case '\n':
						num2 += delayOnNewLine;
						break;
					case '.':
					case ']':
						num2 += Mathf.Max(0f, delayOnNewLine - delayOnPeriod);
						break;
					}
				}
			}
			else if (num + 1 == length || c2 <= ' ' || c2 == '[')
			{
				switch (c)
				{
				case '.':
					if (num + 2 < length && c2 == '.' && mFullText[num + 2] == '.')
					{
						num2 += delayOnPeriod * 3f;
						num += 2;
					}
					else
					{
						num2 += delayOnPeriod;
					}
					break;
				case '!':
				case '?':
					num2 += delayOnPeriod;
					break;
				}
			}
			if (mNextChar == 0f)
			{
				mNextChar = RealTime.time + num2;
			}
			else
			{
				mNextChar += num2;
			}
			if (fadeInTime != 0f)
			{
				FadeEntry item = default(FadeEntry);
				item.index = num;
				item.alpha = 0f;
				item.text = mFullText.Substring(num, mCurrentOffset - num);
				mFade.Add(item);
			}
			else
			{
				mMyText = (keepFullDimensions ? (mFullText.Substring(0, mCurrentOffset) + "[00]" + mFullText.Substring(mCurrentOffset)) : mFullText.Substring(0, mCurrentOffset));
				mLabel.text = mMyText;
				if (!keepFullDimensions && scrollView != null)
				{
					scrollView.UpdatePosition();
				}
			}
		}
		if (mCurrentOffset >= length && mFade.size == 0)
		{
			mMyText = mFullText;
			mLabel.text = mMyText;
			current = this;
			EventDelegate.Execute(onFinished);
			current = null;
			mActive = false;
		}
		else
		{
			if (mFade.size == 0)
			{
				return;
			}
			int num3 = 0;
			while (num3 < mFade.size)
			{
				FadeEntry fadeEntry = mFade.buffer[num3];
				fadeEntry.alpha += RealTime.deltaTime / fadeInTime;
				if (fadeEntry.alpha < 1f)
				{
					mFade.buffer[num3] = fadeEntry;
					num3++;
				}
				else
				{
					mFade.RemoveAt(num3);
				}
			}
			if (mFade.size == 0)
			{
				if (mCurrentOffset < length)
				{
					if (keepFullDimensions)
					{
						mMyText = mFullText.Substring(0, mCurrentOffset) + "[00]" + mFullText.Substring(mCurrentOffset);
					}
					else
					{
						mMyText = mFullText.Substring(0, mCurrentOffset);
					}
					mLabel.text = mMyText;
				}
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < mFade.size; i++)
			{
				FadeEntry fadeEntry2 = mFade.buffer[i];
				if (i == 0)
				{
					stringBuilder.Append(mFullText.Substring(0, fadeEntry2.index));
				}
				stringBuilder.Append('[');
				stringBuilder.Append(NGUIText.EncodeAlpha(fadeEntry2.alpha));
				stringBuilder.Append(']');
				stringBuilder.Append(fadeEntry2.text);
			}
			if (keepFullDimensions && mCurrentOffset < length)
			{
				stringBuilder.Append("[00]");
				stringBuilder.Append(mFullText.Substring(mCurrentOffset));
			}
			mMyText = stringBuilder.ToString();
			mLabel.text = mMyText;
		}
	}
}
