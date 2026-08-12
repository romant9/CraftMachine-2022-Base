using UnityEngine;

public class PolylinePathIterator
{
	private PolylinePath Path;

	private float cachedTotalLength;

	private float DistanceTraveled { get; set; }

	private float DistanceTraveledOnSegment { get; set; }

	private int CurrentSegmentIndex { get; set; }

	public float RemainingDistance => Mathf.Max(0f, TotalLength - DistanceTraveled);

	public float TotalLength
	{
		get
		{
			if (cachedTotalLength < 0f)
			{
				cachedTotalLength = 0f;
				for (int i = 0; i < Path.Segments.Count; i++)
				{
					cachedTotalLength += Path.Segments[i].Length;
				}
			}
			return cachedTotalLength;
		}
	}

	private float NormalizedDistanceTraveledOnSegment
	{
		get
		{
			if (!AtEnd)
			{
				float length = Path.Segments[CurrentSegmentIndex].Length;
				if (!(length > 0f))
				{
					return 1f;
				}
				return DistanceTraveledOnSegment / length;
			}
			return 1f;
		}
	}

	private float RemainingSegmentLength
	{
		get
		{
			if (!AtEnd)
			{
				return Path.Segments[CurrentSegmentIndex].Length - DistanceTraveledOnSegment;
			}
			return 0f;
		}
	}

	public Vector3 End
	{
		get
		{
			if (Path == null || Path.Segments.Count <= 0)
			{
				return new Vector3(0f, 0f, 0f);
			}
			return Path.Segments[Path.Segments.Count - 1].end;
		}
	}

	public Vector3 EndDirection
	{
		get
		{
			if (Path == null || Path.Segments.Count <= 0)
			{
				return new Vector3(0f, 0f, 1f);
			}
			return Path.Segments[Path.Segments.Count - 1].GetDirection(1f);
		}
	}

	public Vector3 Position
	{
		get
		{
			if (CurrentSegmentIndex >= Path.Segments.Count)
			{
				return End;
			}
			return Path.Segments[CurrentSegmentIndex].GetPosition(NormalizedDistanceTraveledOnSegment);
		}
	}

	public Vector3 Direction
	{
		get
		{
			if (CurrentSegmentIndex >= Path.Segments.Count)
			{
				return EndDirection;
			}
			return Path.Segments[CurrentSegmentIndex].GetDirection(NormalizedDistanceTraveledOnSegment);
		}
	}

	public bool AtEnd
	{
		get
		{
			if (Path != null)
			{
				return CurrentSegmentIndex >= Path.Segments.Count;
			}
			return true;
		}
	}

	public PolylinePathIterator(PolylinePath path)
	{
		Clear(path);
	}

	public void Clear(PolylinePath path)
	{
		DistanceTraveled = 0f;
		CurrentSegmentIndex = 0;
		DistanceTraveledOnSegment = 0f;
		Path = path;
		cachedTotalLength = -1f;
	}

	public void Advance(float distance)
	{
		if (AtEnd)
		{
			return;
		}
		float num = distance;
		while (num > 0f)
		{
			float length = Path.Segments[CurrentSegmentIndex].Length;
			float num2 = ((length > 0f) ? (num / length) : 0f);
			float num3 = 1f - NormalizedDistanceTraveledOnSegment;
			if (num2 >= num3)
			{
				num -= RemainingSegmentLength;
				CurrentSegmentIndex++;
				DistanceTraveledOnSegment = 0f;
				if (CurrentSegmentIndex >= Path.Segments.Count)
				{
					DistanceTraveled = TotalLength;
					break;
				}
			}
			else
			{
				num = 0f;
				DistanceTraveledOnSegment += distance;
			}
		}
		DistanceTraveled += distance;
	}

	public Vector3 FuturePosition(float distanceToFuture)
	{
		PolylinePathIterator polylinePathIterator = new PolylinePathIterator(Path);
		polylinePathIterator.Advance(DistanceTraveled + distanceToFuture);
		return polylinePathIterator.Position;
	}
}
