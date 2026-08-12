using System;
using System.Collections.Generic;
using System.Linq;

namespace MIConvexHull
{
	internal class ConvexHullInternal
	{
		private const double PlaneDistanceTolerance = 1E-07;

		private bool Computed;

		private readonly int Dimension;

		private List<VertexWrap> InputVertices;

		private List<VertexWrap> ConvexHull;

		private FaceList UnprocessedFaces;

		private List<ConvexFaceInternal> ConvexFaces;

		private VertexWrap CurrentVertex;

		private double MaxDistance;

		private VertexWrap FurthestVertex;

		private double[] Center;

		private ConvexFaceInternal[] UpdateBuffer;

		private int[] UpdateIndices;

		private Stack<ConvexFaceInternal> TraverseStack;

		private VertexBuffer EmptyBuffer;

		private VertexBuffer BeyondBuffer;

		private List<ConvexFaceInternal> AffectedFaceBuffer;

		private List<DeferredFace> ConeFaceBuffer;

		private HashSet<VertexWrap> SingularVertices;

		private const int ConnectorTableSize = 2017;

		private ConnectorList[] ConnectorTable;

		private ObjectManager ObjectManager;

		private MathHelper MathHelper;

		private void Initialize()
		{
			ConvexHull = new List<VertexWrap>();
			UnprocessedFaces = new FaceList();
			ConvexFaces = new List<ConvexFaceInternal>();
			ObjectManager = new ObjectManager(Dimension);
			MathHelper = new MathHelper(Dimension);
			Center = new double[Dimension];
			TraverseStack = new Stack<ConvexFaceInternal>();
			UpdateBuffer = new ConvexFaceInternal[Dimension];
			UpdateIndices = new int[Dimension];
			EmptyBuffer = new VertexBuffer();
			AffectedFaceBuffer = new List<ConvexFaceInternal>();
			ConeFaceBuffer = new List<DeferredFace>();
			SingularVertices = new HashSet<VertexWrap>();
			BeyondBuffer = new VertexBuffer();
			ConnectorTable = (from _ in Enumerable.Range(0, 2017)
				select new ConnectorList()).ToArray();
		}

		private int DetermineDimension()
		{
			Random random = new Random();
			int count = InputVertices.Count;
			List<int> list = new List<int>();
			for (int i = 0; i < 10; i++)
			{
				list.Add(InputVertices[random.Next(count)].Vertex.Position.Length);
			}
			int num = list.Min();
			if (num != list.Max())
			{
				throw new ArgumentException("Invalid input data (non-uniform dimension).");
			}
			return num;
		}

		private ConvexFaceInternal[] InitiateFaceDatabase()
		{
			ConvexFaceInternal[] array = new ConvexFaceInternal[Dimension + 1];
			int i;
			for (i = 0; i < Dimension + 1; i++)
			{
				VertexWrap[] array2 = ConvexHull.Where((VertexWrap _, int j) => i != j).ToArray();
				ConvexFaceInternal convexFaceInternal = new ConvexFaceInternal(Dimension, new VertexBuffer());
				convexFaceInternal.Vertices = array2;
				Array.Sort(array2, VertexWrapComparer.Instance);
				CalculateFacePlane(convexFaceInternal);
				array[i] = convexFaceInternal;
			}
			for (int num = 0; num < Dimension; num++)
			{
				for (int num2 = num + 1; num2 < Dimension + 1; num2++)
				{
					UpdateAdjacency(array[num], array[num2]);
				}
			}
			return array;
		}

		private bool CalculateFacePlane(ConvexFaceInternal face)
		{
			VertexWrap[] vertices = face.Vertices;
			double[] normal = face.Normal;
			MathHelper.FindNormalVector(vertices, normal);
			if (double.IsNaN(normal[0]))
			{
				return false;
			}
			double num = 0.0;
			double num2 = 0.0;
			double[] positionData = vertices[0].PositionData;
			for (int i = 0; i < Dimension; i++)
			{
				double num3 = normal[i];
				num += num3 * positionData[i];
				num2 += num3 * Center[i];
			}
			face.Offset = 0.0 - num;
			num2 -= num;
			if (num2 > 0.0)
			{
				for (int j = 0; j < Dimension; j++)
				{
					normal[j] = 0.0 - normal[j];
				}
				face.Offset = num;
				face.IsNormalFlipped = true;
			}
			else
			{
				face.IsNormalFlipped = false;
			}
			return true;
		}

		private void TagAffectedFaces(ConvexFaceInternal currentFace)
		{
			AffectedFaceBuffer.Clear();
			AffectedFaceBuffer.Add(currentFace);
			TraverseAffectedFaces(currentFace);
		}

		private void TraverseAffectedFaces(ConvexFaceInternal currentFace)
		{
			TraverseStack.Clear();
			TraverseStack.Push(currentFace);
			currentFace.Tag = 1;
			while (TraverseStack.Count > 0)
			{
				ConvexFaceInternal convexFaceInternal = TraverseStack.Pop();
				for (int i = 0; i < Dimension; i++)
				{
					ConvexFaceInternal convexFaceInternal2 = convexFaceInternal.AdjacentFaces[i];
					if (convexFaceInternal2.Tag == 0 && MathHelper.GetVertexDistance(CurrentVertex, convexFaceInternal2) >= 1E-07)
					{
						AffectedFaceBuffer.Add(convexFaceInternal2);
						convexFaceInternal2.Tag = 1;
						TraverseStack.Push(convexFaceInternal2);
					}
				}
			}
		}

		private void UpdateAdjacency(ConvexFaceInternal l, ConvexFaceInternal r)
		{
			VertexWrap[] vertices = l.Vertices;
			VertexWrap[] vertices2 = r.Vertices;
			int i;
			for (i = 0; i < Dimension; i++)
			{
				vertices[i].Marked = false;
			}
			for (i = 0; i < Dimension; i++)
			{
				vertices2[i].Marked = true;
			}
			for (i = 0; i < Dimension && vertices[i].Marked; i++)
			{
			}
			if (i == Dimension)
			{
				return;
			}
			for (int j = i + 1; j < Dimension; j++)
			{
				if (!vertices[j].Marked)
				{
					return;
				}
			}
			l.AdjacentFaces[i] = r;
			for (i = 0; i < Dimension; i++)
			{
				vertices[i].Marked = false;
			}
			for (i = 0; i < Dimension && !vertices2[i].Marked; i++)
			{
			}
			r.AdjacentFaces[i] = l;
		}

		private DeferredFace MakeDeferredFace(ConvexFaceInternal face, int faceIndex, ConvexFaceInternal pivot, int pivotIndex, ConvexFaceInternal oldFace)
		{
			DeferredFace deferredFace = ObjectManager.GetDeferredFace();
			deferredFace.Face = face;
			deferredFace.FaceIndex = faceIndex;
			deferredFace.Pivot = pivot;
			deferredFace.PivotIndex = pivotIndex;
			deferredFace.OldFace = oldFace;
			return deferredFace;
		}

		private void ConnectFace(FaceConnector connector)
		{
			uint num = connector.HashCode % 2017;
			ConnectorList connectorList = ConnectorTable[num];
			for (FaceConnector faceConnector = connectorList.First; faceConnector != null; faceConnector = faceConnector.Next)
			{
				if (FaceConnector.AreConnectable(connector, faceConnector, Dimension))
				{
					connectorList.Remove(faceConnector);
					FaceConnector.Connect(faceConnector, connector);
					faceConnector.Face = null;
					connector.Face = null;
					ObjectManager.DepositConnector(faceConnector);
					ObjectManager.DepositConnector(connector);
					return;
				}
			}
			connectorList.Add(connector);
		}

		private bool CreateCone()
		{
			int index = CurrentVertex.Index;
			ConeFaceBuffer.Clear();
			for (int i = 0; i < AffectedFaceBuffer.Count; i++)
			{
				ConvexFaceInternal convexFaceInternal = AffectedFaceBuffer[i];
				int num = 0;
				for (int j = 0; j < Dimension; j++)
				{
					ConvexFaceInternal convexFaceInternal2 = convexFaceInternal.AdjacentFaces[j];
					if (convexFaceInternal2.Tag == 0)
					{
						UpdateBuffer[num] = convexFaceInternal2;
						UpdateIndices[num] = j;
						num++;
					}
				}
				for (int k = 0; k < num; k++)
				{
					ConvexFaceInternal convexFaceInternal3 = UpdateBuffer[k];
					int pivotIndex = 0;
					ConvexFaceInternal[] adjacentFaces = convexFaceInternal3.AdjacentFaces;
					for (int l = 0; l < Dimension; l++)
					{
						if (convexFaceInternal == adjacentFaces[l])
						{
							pivotIndex = l;
							break;
						}
					}
					int num2 = UpdateIndices[k];
					ConvexFaceInternal face = ObjectManager.GetFace();
					VertexWrap[] vertices = face.Vertices;
					for (int m = 0; m < Dimension; m++)
					{
						vertices[m] = convexFaceInternal.Vertices[m];
					}
					int index2 = vertices[num2].Index;
					int num3;
					if (index < index2)
					{
						num3 = 0;
						int num4 = num2 - 1;
						while (num4 >= 0)
						{
							if (vertices[num4].Index > index)
							{
								vertices[num4 + 1] = vertices[num4];
								num4--;
								continue;
							}
							num3 = num4 + 1;
							break;
						}
					}
					else
					{
						num3 = Dimension - 1;
						for (int n = num2 + 1; n < Dimension; n++)
						{
							if (vertices[n].Index < index)
							{
								vertices[n - 1] = vertices[n];
								continue;
							}
							num3 = n - 1;
							break;
						}
					}
					vertices[num3] = CurrentVertex;
					if (!CalculateFacePlane(face))
					{
						return false;
					}
					ConeFaceBuffer.Add(MakeDeferredFace(face, num3, convexFaceInternal3, pivotIndex, convexFaceInternal));
				}
			}
			return true;
		}

		private void CommitCone()
		{
			ConvexHull.Add(CurrentVertex);
			for (int i = 0; i < ConeFaceBuffer.Count; i++)
			{
				DeferredFace deferredFace = ConeFaceBuffer[i];
				ConvexFaceInternal face = deferredFace.Face;
				ConvexFaceInternal pivot = deferredFace.Pivot;
				ConvexFaceInternal oldFace = deferredFace.OldFace;
				int faceIndex = deferredFace.FaceIndex;
				face.AdjacentFaces[faceIndex] = pivot;
				pivot.AdjacentFaces[deferredFace.PivotIndex] = face;
				for (int j = 0; j < Dimension; j++)
				{
					if (j != faceIndex)
					{
						FaceConnector connector = ObjectManager.GetConnector();
						connector.Update(face, j, Dimension);
						ConnectFace(connector);
					}
				}
				if (pivot.VerticesBeyond.Count < oldFace.VerticesBeyond.Count)
				{
					FindBeyondVertices(face, pivot.VerticesBeyond, oldFace.VerticesBeyond);
				}
				else
				{
					FindBeyondVertices(face, oldFace.VerticesBeyond, pivot.VerticesBeyond);
				}
				if (face.VerticesBeyond.Count == 0)
				{
					ConvexFaces.Add(face);
					UnprocessedFaces.Remove(face);
					ObjectManager.DepositVertexBuffer(face.VerticesBeyond);
					face.VerticesBeyond = EmptyBuffer;
				}
				else
				{
					UnprocessedFaces.Add(face);
				}
				ObjectManager.DepositDeferredFace(deferredFace);
			}
			for (int k = 0; k < AffectedFaceBuffer.Count; k++)
			{
				ConvexFaceInternal face2 = AffectedFaceBuffer[k];
				UnprocessedFaces.Remove(face2);
				ObjectManager.DepositFace(face2);
			}
		}

		private void IsBeyond(ConvexFaceInternal face, VertexBuffer beyondVertices, VertexWrap v)
		{
			double vertexDistance = MathHelper.GetVertexDistance(v, face);
			if (vertexDistance >= 1E-07)
			{
				if (vertexDistance > MaxDistance)
				{
					MaxDistance = vertexDistance;
					FurthestVertex = v;
				}
				beyondVertices.Add(v);
			}
		}

		private void FindBeyondVertices(ConvexFaceInternal face)
		{
			VertexBuffer verticesBeyond = face.VerticesBeyond;
			MaxDistance = double.NegativeInfinity;
			FurthestVertex = null;
			int count = InputVertices.Count;
			for (int i = 0; i < count; i++)
			{
				IsBeyond(face, verticesBeyond, InputVertices[i]);
			}
			face.FurthestVertex = FurthestVertex;
		}

		private void FindBeyondVertices(ConvexFaceInternal face, VertexBuffer beyond, VertexBuffer beyond1)
		{
			VertexBuffer beyondBuffer = BeyondBuffer;
			MaxDistance = double.NegativeInfinity;
			FurthestVertex = null;
			int count = beyond1.Count;
			for (int i = 0; i < count; i++)
			{
				beyond1[i].Marked = true;
			}
			CurrentVertex.Marked = false;
			count = beyond.Count;
			for (int j = 0; j < count; j++)
			{
				VertexWrap vertexWrap = beyond[j];
				if (vertexWrap != CurrentVertex)
				{
					vertexWrap.Marked = false;
					IsBeyond(face, beyondBuffer, vertexWrap);
				}
			}
			count = beyond1.Count;
			for (int k = 0; k < count; k++)
			{
				VertexWrap vertexWrap = beyond1[k];
				if (vertexWrap.Marked)
				{
					IsBeyond(face, beyondBuffer, vertexWrap);
				}
			}
			face.FurthestVertex = FurthestVertex;
			VertexBuffer verticesBeyond = face.VerticesBeyond;
			face.VerticesBeyond = beyondBuffer;
			if (verticesBeyond.Count > 0)
			{
				verticesBeyond.Clear();
			}
			BeyondBuffer = verticesBeyond;
		}

		private void UpdateCenter()
		{
			int num = ConvexHull.Count + 1;
			for (int i = 0; i < Dimension; i++)
			{
				Center[i] *= num - 1;
			}
			double num2 = 1.0 / (double)num;
			for (int j = 0; j < Dimension; j++)
			{
				Center[j] = num2 * (Center[j] + CurrentVertex.PositionData[j]);
			}
		}

		private void RollbackCenter()
		{
			int num = ConvexHull.Count + 1;
			for (int i = 0; i < Dimension; i++)
			{
				Center[i] *= num;
			}
			double num2 = 1.0 / (double)(num - 1);
			for (int j = 0; j < Dimension; j++)
			{
				Center[j] = num2 * (Center[j] - CurrentVertex.PositionData[j]);
			}
		}

		private void InitConvexHull()
		{
			List<VertexWrap> list = FindExtremes();
			foreach (VertexWrap item2 in FindInitialPoints(list))
			{
				VertexWrap item = (CurrentVertex = item2);
				UpdateCenter();
				ConvexHull.Add(CurrentVertex);
				InputVertices.Remove(item);
				list.Remove(item);
			}
			ConvexFaceInternal[] array = InitiateFaceDatabase();
			foreach (ConvexFaceInternal convexFaceInternal in array)
			{
				FindBeyondVertices(convexFaceInternal);
				if (convexFaceInternal.VerticesBeyond.Count == 0)
				{
					ConvexFaces.Add(convexFaceInternal);
				}
				else
				{
					UnprocessedFaces.Add(convexFaceInternal);
				}
			}
		}

		private List<VertexWrap> FindInitialPoints(List<VertexWrap> extremes)
		{
			List<VertexWrap> list = new List<VertexWrap>();
			VertexWrap item = null;
			VertexWrap item2 = null;
			double num = 0.0;
			double[] array = new double[Dimension];
			for (int i = 0; i < extremes.Count - 1; i++)
			{
				VertexWrap vertexWrap = extremes[i];
				for (int j = i + 1; j < extremes.Count; j++)
				{
					VertexWrap vertexWrap2 = extremes[j];
					MathHelper.SubtractFast(vertexWrap.PositionData, vertexWrap2.PositionData, array);
					double num2 = MathHelper.LengthSquared(array);
					if (num2 > num)
					{
						item = vertexWrap;
						item2 = vertexWrap2;
						num = num2;
					}
				}
			}
			list.Add(item);
			list.Add(item2);
			for (int k = 2; k <= Dimension; k++)
			{
				double num3 = 1E-06;
				VertexWrap vertexWrap3 = null;
				for (int l = 0; l < extremes.Count; l++)
				{
					VertexWrap vertexWrap4 = extremes[l];
					if (!list.Contains(vertexWrap4))
					{
						double squaredDistanceSum = GetSquaredDistanceSum(vertexWrap4, list);
						if (squaredDistanceSum > num3)
						{
							num3 = squaredDistanceSum;
							vertexWrap3 = vertexWrap4;
						}
					}
				}
				if (vertexWrap3 != null)
				{
					list.Add(vertexWrap3);
					continue;
				}
				int count = InputVertices.Count;
				for (int m = 0; m < count; m++)
				{
					VertexWrap vertexWrap5 = InputVertices[m];
					if (!list.Contains(vertexWrap5))
					{
						double squaredDistanceSum2 = GetSquaredDistanceSum(vertexWrap5, list);
						if (squaredDistanceSum2 > num3)
						{
							num3 = squaredDistanceSum2;
							vertexWrap3 = vertexWrap5;
						}
					}
				}
				if (vertexWrap3 != null)
				{
					list.Add(vertexWrap3);
				}
				else
				{
					ThrowSingular();
				}
			}
			return list;
		}

		private double GetSquaredDistanceSum(VertexWrap pivot, List<VertexWrap> initialPoints)
		{
			int count = initialPoints.Count;
			double num = 0.0;
			for (int i = 0; i < count; i++)
			{
				VertexWrap vertexWrap = initialPoints[i];
				for (int j = 0; j < Dimension; j++)
				{
					double num2 = vertexWrap.PositionData[j] - pivot.PositionData[j];
					num += num2 * num2;
				}
			}
			return num;
		}

		private List<VertexWrap> FindExtremes()
		{
			List<VertexWrap> list = new List<VertexWrap>(2 * Dimension);
			int count = InputVertices.Count;
			for (int i = 0; i < Dimension; i++)
			{
				double num = double.MaxValue;
				double num2 = double.MinValue;
				int num3 = 0;
				int num4 = 0;
				for (int j = 0; j < count; j++)
				{
					double num5 = InputVertices[j].PositionData[i];
					if (num5 < num)
					{
						num = num5;
						num3 = j;
					}
					if (num5 > num2)
					{
						num2 = num5;
						num4 = j;
					}
				}
				if (num3 != num4)
				{
					list.Add(InputVertices[num3]);
					list.Add(InputVertices[num4]);
				}
				else
				{
					list.Add(InputVertices[num3]);
				}
			}
			return list;
		}

		private void ThrowSingular()
		{
			throw new InvalidOperationException("ConvexHull: Singular input data (i.e. trying to triangulate a data that contain a regular lattice of points).\nIntroducing some noise to the data might resolve the issue.");
		}

		private void HandleSingular()
		{
			RollbackCenter();
			SingularVertices.Add(CurrentVertex);
			for (int i = 0; i < AffectedFaceBuffer.Count; i++)
			{
				ConvexFaceInternal convexFaceInternal = AffectedFaceBuffer[i];
				VertexBuffer verticesBeyond = convexFaceInternal.VerticesBeyond;
				for (int j = 0; j < verticesBeyond.Count; j++)
				{
					SingularVertices.Add(verticesBeyond[j]);
				}
				ConvexFaces.Add(convexFaceInternal);
				UnprocessedFaces.Remove(convexFaceInternal);
				ObjectManager.DepositVertexBuffer(convexFaceInternal.VerticesBeyond);
				convexFaceInternal.VerticesBeyond = EmptyBuffer;
			}
		}

		private void FindConvexHull()
		{
			InitConvexHull();
			while (UnprocessedFaces.First != null)
			{
				ConvexFaceInternal first = UnprocessedFaces.First;
				CurrentVertex = first.FurthestVertex;
				UpdateCenter();
				TagAffectedFaces(first);
				if (!SingularVertices.Contains(CurrentVertex) && CreateCone())
				{
					CommitCone();
				}
				else
				{
					HandleSingular();
				}
				int count = AffectedFaceBuffer.Count;
				for (int i = 0; i < count; i++)
				{
					AffectedFaceBuffer[i].Tag = 0;
				}
			}
		}

		private ConvexHullInternal(IEnumerable<IVertex> vertices)
		{
			InputVertices = new List<VertexWrap>(vertices.Select((IVertex v, int i) => new VertexWrap
			{
				Vertex = v,
				PositionData = v.Position,
				Index = i
			}));
			Dimension = DetermineDimension();
			Initialize();
		}

		private IEnumerable<TVertex> GetConvexHullInternal<TVertex>(bool onlyCompute = false) where TVertex : IVertex
		{
			if (Computed)
			{
				if (!onlyCompute)
				{
					return ConvexHull.Select((VertexWrap v) => (TVertex)v.Vertex).ToArray();
				}
				return null;
			}
			if (Dimension < 2)
			{
				throw new ArgumentException("Dimension of the input must be 2 or greater.");
			}
			FindConvexHull();
			Computed = true;
			if (!onlyCompute)
			{
				return ConvexHull.Select((VertexWrap v) => (TVertex)v.Vertex).ToArray();
			}
			return null;
		}

		private IEnumerable<TFace> GetConvexFacesInternal<TVertex, TFace>() where TVertex : IVertex where TFace : ConvexFace<TVertex, TFace>, new()
		{
			if (!Computed)
			{
				GetConvexHullInternal<TVertex>(onlyCompute: true);
			}
			List<ConvexFaceInternal> convexFaces = ConvexFaces;
			int count = convexFaces.Count;
			TFace[] array = new TFace[count];
			for (int i = 0; i < count; i++)
			{
				ConvexFaceInternal convexFaceInternal = convexFaces[i];
				TVertex[] array2 = new TVertex[Dimension];
				for (int j = 0; j < Dimension; j++)
				{
					array2[j] = (TVertex)convexFaceInternal.Vertices[j].Vertex;
				}
				array[i] = new TFace
				{
					Vertices = array2,
					Adjacency = new TFace[Dimension],
					Normal = convexFaceInternal.Normal
				};
				convexFaceInternal.Tag = i;
			}
			for (int k = 0; k < count; k++)
			{
				ConvexFaceInternal convexFaceInternal2 = convexFaces[k];
				TFace val = array[k];
				for (int l = 0; l < Dimension; l++)
				{
					if (convexFaceInternal2.AdjacentFaces[l] != null)
					{
						val.Adjacency[l] = array[convexFaceInternal2.AdjacentFaces[l].Tag];
					}
				}
				if (convexFaceInternal2.IsNormalFlipped)
				{
					TVertex val2 = val.Vertices[0];
					val.Vertices[0] = val.Vertices[Dimension - 1];
					val.Vertices[Dimension - 1] = val2;
					TFace val3 = val.Adjacency[0];
					val.Adjacency[0] = val.Adjacency[Dimension - 1];
					val.Adjacency[Dimension - 1] = val3;
				}
			}
			return array;
		}

		internal static List<ConvexFaceInternal> GetConvexFacesInternal<TVertex, TFace>(IEnumerable<TVertex> data) where TVertex : IVertex where TFace : ConvexFace<TVertex, TFace>, new()
		{
			ConvexHullInternal convexHullInternal = new ConvexHullInternal(data.Cast<IVertex>());
			convexHullInternal.GetConvexHullInternal<TVertex>(onlyCompute: true);
			return convexHullInternal.ConvexFaces;
		}

		internal static void GetConvexHullAndFaces<TVertex, TFace>(IEnumerable<IVertex> data, out IEnumerable<TVertex> points, out IEnumerable<TFace> faces) where TVertex : IVertex where TFace : ConvexFace<TVertex, TFace>, new()
		{
			ConvexHullInternal convexHullInternal = new ConvexHullInternal(data);
			points = convexHullInternal.GetConvexHullInternal<TVertex>();
			faces = convexHullInternal.GetConvexFacesInternal<TVertex, TFace>();
		}
	}
}
