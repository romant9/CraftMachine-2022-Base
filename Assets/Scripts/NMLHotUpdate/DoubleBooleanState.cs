public struct DoubleBooleanState
{
	public bool FirstState;

	public bool SecondState;

	public bool this[int index]
	{
		get
		{
			if (index == 0)
			{
				return FirstState;
			}
			return SecondState;
		}
		set
		{
			if (index == 0)
			{
				FirstState = value;
			}
			else
			{
				SecondState = value;
			}
		}
	}

	public static bool operator ==(DoubleBooleanState a, DoubleBooleanState b)
	{
		if (a.FirstState == b.FirstState)
		{
			return a.SecondState == b.SecondState;
		}
		return false;
	}

	public static bool operator ==(DoubleBooleanState a, bool value)
	{
		if (a.FirstState == value)
		{
			return a.SecondState == value;
		}
		return false;
	}

	public static bool operator !=(DoubleBooleanState a, DoubleBooleanState b)
	{
		if (a.FirstState == b.FirstState)
		{
			return a.SecondState != b.SecondState;
		}
		return true;
	}

	public static bool operator !=(DoubleBooleanState a, bool value)
	{
		if (a.FirstState == value)
		{
			return a.SecondState != value;
		}
		return true;
	}

	public static implicit operator bool(DoubleBooleanState value)
	{
		if (value.FirstState)
		{
			return value.SecondState;
		}
		return false;
	}

	public static implicit operator DoubleBooleanState(bool value)
	{
		return new DoubleBooleanState
		{
			FirstState = value,
			SecondState = value
		};
	}

	public override bool Equals(object obj)
	{
		if (obj is DoubleBooleanState)
		{
			return this == (DoubleBooleanState)obj;
		}
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return ((FirstState ? 1 : 0) << 1) + (SecondState ? 1 : 0);
	}

	public override string ToString()
	{
		return "{ FirstState = " + FirstState + ", SecondState = " + SecondState + " = " + (bool)this + " }";
	}
}
