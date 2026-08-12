using System;
using System.Collections;
using System.Collections.Generic;

namespace TWDModel
{
	public class GridColliderData
	{
		public static bool DISABLE_DYNAMIC_COLLIDERS;

		private GridModel Grid;

		public int VisibilityColliderCount;

		public int MovementColliderCount;

		private BitArray visibilityBits;

		private BitArray movementBits;

		private int VisibilityBitsPerEntry => ((!DISABLE_DYNAMIC_COLLIDERS) ? VisibilityColliderCount : 0) + 1;

		private int VisiblityBitsPerCell => VisibilityBitsPerEntry * Grid.NumCells;

		private int VisibilityBits => VisiblityBitsPerCell * Grid.NumCells;

		private int VisibilityBlockedBits => VisibilityBitsPerEntry * Grid.NumCells;

		public GridColliderData(GridModel grid, int visibilityColliderCount, int moveColliderCount)
		{
			Grid = grid;
			VisibilityColliderCount = visibilityColliderCount;
			MovementColliderCount = moveColliderCount;
			int length = Pad(Grid.Width * Grid.Height * Grid.Width * Grid.Height) * (1 + visibilityColliderCount);
			int length2 = Pad(Grid.Width * Grid.Height * 9) * (1 + moveColliderCount);
			visibilityBits = new BitArray(length);
			movementBits = new BitArray(length2);
		}

		public GridColliderData(GridModel grid, int visibilityColliderCount, int moveColliderCount, BitArray visibilityBits, BitArray movementBits)
		{
			Grid = grid;
			VisibilityColliderCount = visibilityColliderCount;
			MovementColliderCount = moveColliderCount;
			this.visibilityBits = visibilityBits;
			this.movementBits = movementBits;
		}

		public GridColliderData(GridModel grid, int visibilityColliderCount, int moveColliderCount, string visibilityData, string movementData)
		{
			Grid = grid;
			VisibilityColliderCount = visibilityColliderCount;
			MovementColliderCount = moveColliderCount;
			byte[] bytes = Convert.FromBase64String(visibilityData);
			visibilityBits = new BitArray(bytes);
			byte[] bytes2 = Convert.FromBase64String(movementData);
			movementBits = new BitArray(bytes2);
			if (movementBits.Count < grid.Width * grid.Height * 9 * (1 + moveColliderCount))
			{
				int num = 1 + moveColliderCount;
				int num2 = movementBits.Count / (grid.Width * grid.Height * 9);
				throw new ArgumentException("Movement data mismatch (excepted: " + num + " bits, actual: " + num2 + " bits)");
			}
			if (visibilityBits.Count < grid.Width * grid.Height * grid.Width * grid.Height * (1 + visibilityColliderCount))
			{
				int num3 = 1 + visibilityColliderCount;
				int num4 = movementBits.Count / (grid.Width * grid.Height * grid.Width * grid.Height);
				throw new ArgumentException("Visibility data mismatch (excepted: " + num3 + " bits, actual: " + num4 + " bits)");
			}
		}

		public void Combine(GridColliderData other, int[] visibilityBitMapping, int[] movementBitMapping)
		{
			for (int i = 0; i < Grid.Height; i++)
			{
				for (int j = 0; j < Grid.Width; j++)
				{
					GridCoordinate fromCoordinate = new GridCoordinate(j, i);
					for (int k = 0; k < 9; k++)
					{
						int movementBitOffset = GetMovementBitOffset(fromCoordinate, k);
						int movementBitOffset2 = other.GetMovementBitOffset(fromCoordinate, k);
						movementBits[movementBitOffset] |= other.movementBits[movementBitOffset2];
						for (int l = 0; l < movementBitMapping.Length; l++)
						{
							movementBits[movementBitOffset + movementBitMapping[l] + 1] |= other.movementBits[movementBitOffset2 + l + 1];
						}
					}
					for (int m = 0; m < Grid.Height; m++)
					{
						for (int n = 0; n < Grid.Width; n++)
						{
							GridCoordinate toCoordinate = new GridCoordinate(n, m);
							int visibilityBitOffset = GetVisibilityBitOffset(fromCoordinate, toCoordinate, 0);
							int visibilityBitOffset2 = other.GetVisibilityBitOffset(fromCoordinate, toCoordinate, 0);
							visibilityBits[visibilityBitOffset] |= other.visibilityBits[visibilityBitOffset2];
							for (int num = 0; num < visibilityBitMapping.Length; num++)
							{
								visibilityBits[visibilityBitOffset + visibilityBitMapping[num] + 1] |= other.visibilityBits[visibilityBitOffset2 + num + 1];
							}
						}
					}
				}
			}
		}

		public bool HasVisibilityData()
		{
			if (visibilityBits != null)
			{
				return visibilityBits.Count > 0;
			}
			return false;
		}

		public bool HasMovementData()
		{
			if (movementBits != null)
			{
				return movementBits.Count > 0;
			}
			return false;
		}

		public string GetVisibilityAsString()
		{
			byte[] array = new byte[Pad(visibilityBits.Count) / 8];
			visibilityBits.CopyTo(array, 0);
			return Convert.ToBase64String(array);
		}

		public string GetMovementAsString()
		{
			byte[] array = new byte[Pad(movementBits.Count) / 8];
			movementBits.CopyTo(array, 0);
			return Convert.ToBase64String(array);
		}

		public static string ConvertToString(List<int> data)
		{
			byte[] array = new byte[data.Count * 4];
			Buffer.BlockCopy(data.ToArray(), 0, array, 0, data.Count * 4);
			return Convert.ToBase64String(array);
		}

		public bool IsBlocked(GridCoordinate c, int colliderIndex)
		{
			int blockedBitOffset = GetBlockedBitOffset(c);
			return movementBits[blockedBitOffset + ((!DISABLE_DYNAMIC_COLLIDERS) ? colliderIndex : 0)];
		}

		public bool IsMovementBlocked(GridCoordinate fromCoordinate, GridCoordinate toCoordinate, int colliderIndex)
		{
			if (!Grid.IsCoordinateValid(fromCoordinate) || !Grid.IsCoordinateValid(toCoordinate))
			{
				return false;
			}
			if (fromCoordinate == toCoordinate)
			{
				return true;
			}
			int coordinateNeighborIndex = Grid.GetCoordinateNeighborIndex(fromCoordinate, toCoordinate);
			if (coordinateNeighborIndex == -1)
			{
				return false;
			}
			int movementBitOffset = GetMovementBitOffset(fromCoordinate, coordinateNeighborIndex);
			return movementBits[movementBitOffset + ((!DISABLE_DYNAMIC_COLLIDERS) ? colliderIndex : 0)];
		}

		public bool IsVisibilityBlocked(GridCoordinate c, int colliderIndex)
		{
			int blockedVisibilityBitOffset = GetBlockedVisibilityBitOffset(c, colliderIndex);
			return visibilityBits[blockedVisibilityBitOffset];
		}

		public bool IsVisibilityBlocked(GridCoordinate from, GridCoordinate to, int colliderIndex)
		{
			if (Grid.IsCoordinateValid(from) && Grid.IsCoordinateValid(to) && HasVisibilityData())
			{
				int visibilityBitOffset = GetVisibilityBitOffset(from, to, colliderIndex);
				return visibilityBits[visibilityBitOffset];
			}
			return false;
		}

		private int Pad(int value)
		{
			return (value + 7) & -8;
		}

		private int GetVisibilityBitOffset(GridCoordinate fromCoordinate, GridCoordinate toCoordinate, int colliderIndex)
		{
			int coordinateOffset = Grid.GetCoordinateOffset(fromCoordinate);
			int coordinateOffset2 = Grid.GetCoordinateOffset(toCoordinate);
			return (coordinateOffset * Grid.NumCells + coordinateOffset2) * VisibilityBitsPerEntry + ((!DISABLE_DYNAMIC_COLLIDERS) ? colliderIndex : 0);
		}

		private int GetBlockedVisibilityBitOffset(GridCoordinate fromCoordinate, int colliderIndex)
		{
			int coordinateOffset = Grid.GetCoordinateOffset(fromCoordinate);
			return VisibilityBlockedBits + coordinateOffset * VisibilityBitsPerEntry + ((!DISABLE_DYNAMIC_COLLIDERS) ? colliderIndex : 0);
		}

		private int GetMovementBitOffset(GridCoordinate fromCoordinate, int neighborIndex)
		{
			return ((fromCoordinate.Y * Grid.Width + fromCoordinate.X) * 9 + neighborIndex) * (((!DISABLE_DYNAMIC_COLLIDERS) ? MovementColliderCount : 0) + 1);
		}

		private int GetBlockedBitOffset(GridCoordinate fromCoordinate)
		{
			return ((fromCoordinate.Y * Grid.Width + fromCoordinate.X) * 9 + 8) * (((!DISABLE_DYNAMIC_COLLIDERS) ? MovementColliderCount : 0) + 1);
		}
	}
}
