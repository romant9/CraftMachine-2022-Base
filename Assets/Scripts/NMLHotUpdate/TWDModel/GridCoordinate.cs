using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public struct GridCoordinate
	{
		[NonSerialized]
		[JsonIgnore]
		public static GridCoordinate Invalid = new GridCoordinate(-1, -1);

		public int X { get; set; }

		public int Y { get; set; }

		[JsonIgnore]
		public bool IsValid => this != Invalid;

		public GridCoordinate(int x, int y)
		{
			this = default(GridCoordinate);
			X = x;
			Y = y;
		}

		public void Add(int x, int y)
		{
			X += x;
			Y += y;
		}

		public void Set(int x, int y)
		{
			X = x;
			Y = y;
		}

		public FixedPoint DistanceTo(GridCoordinate other)
		{
			return FixedPoint.Sqrt(((FixedPoint)X - (FixedPoint)other.X) * ((FixedPoint)X - (FixedPoint)other.X) + ((FixedPoint)Y - (FixedPoint)other.Y) * ((FixedPoint)Y - (FixedPoint)other.Y));
		}

		public int SquaredDistanceTo(GridCoordinate other)
		{
			return (X - other.X) * (X - other.X) + (Y - other.Y) * (Y - other.Y);
		}

		public bool CheckGridInWidthAndHeightRange(GridCoordinate other, int range)
		{
			if (Math.Abs(other.X - X) <= range)
			{
				return Math.Abs(other.Y - Y) <= range;
			}
			return false;
		}

		public int ChebyshevDistance(GridCoordinate other)
		{
			int num = Math.Abs(other.X - X);
			int num2 = Math.Abs(other.Y - Y);
			return num + num2 - Math.Min(num, num2);
		}

		public List<ActorModel> GetEnemiesByDistance(GridCoordinate target, CombatModel combatModel, int distance)
		{
			List<ActorModel> list = new List<ActorModel>();
			list.AddRange(combatModel.GetFactionActors(Faction.Raider));
			list.AddRange(combatModel.GetFactionActors(Faction.Walker));
			return list.FindAll((ActorModel x) => x.GridCoordinate.ChebyshevDistance(target) - 1 < distance);
		}

		public List<ActorModel> GetEnemiesByDistanceAndFaction(GridCoordinate targetGridCoordinate, CombatModel combatModel, int distance, Faction faction)
		{
			List<ActorModel> list = new List<ActorModel>();
			list.AddRange(combatModel.GetEnemyFactionsActors(faction));
			return list.FindAll((ActorModel x) => x.GridCoordinate.ChebyshevDistance(targetGridCoordinate) - 1 < distance);
		}

		public override string ToString()
		{
			return "{ X: " + X + " Y: " + Y + " }";
		}

		public FixedVec2 ToVector2()
		{
			return new FixedVec2(X, Y);
		}

		public static bool operator ==(GridCoordinate a, GridCoordinate b)
		{
			if (a.X == b.X)
			{
				return a.Y == b.Y;
			}
			return false;
		}

		public static bool operator !=(GridCoordinate a, GridCoordinate b)
		{
			if (a.X == b.X)
			{
				return a.Y != b.Y;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (obj is GridCoordinate)
			{
				return this == (GridCoordinate)obj;
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return (Y << 16) + X;
		}

		public static GridCoordinate operator +(GridCoordinate a, GridCoordinate b)
		{
			return new GridCoordinate(a.X + b.X, a.Y + b.Y);
		}

		public static GridCoordinate operator -(GridCoordinate a, GridCoordinate b)
		{
			return new GridCoordinate(a.X - b.X, a.Y - b.Y);
		}
	}
}
