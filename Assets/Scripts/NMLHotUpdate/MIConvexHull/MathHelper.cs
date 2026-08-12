using System;

namespace MIConvexHull
{
	internal class MathHelper
	{
		private readonly int Dimension;

		private double[] ntX;

		private double[] ntY;

		private double[] ntZ;

		private double[] nDNormalSolveVector;

		private double[,] nDMatrix;

		private double[][] jaggedNDMatrix;

		private static void GaussElimination(int nDim, double[][] pfMatr, double[] pfVect, double[] pfSolution)
		{
			for (int i = 0; i < nDim - 1; i++)
			{
				double[] array = pfMatr[i];
				double num = Math.Abs(array[i]);
				int num2 = i;
				for (int j = i + 1; j < nDim; j++)
				{
					if (num < Math.Abs(pfMatr[j][i]))
					{
						num = pfMatr[j][i];
						num2 = j;
					}
				}
				if (num2 != i)
				{
					double[] array2 = pfMatr[num2];
					double num3;
					for (int j = i; j < nDim; j++)
					{
						num3 = array[j];
						array[j] = array2[j];
						array2[j] = num3;
					}
					num3 = pfVect[i];
					pfVect[i] = pfVect[num2];
					pfVect[num2] = num3;
				}
				for (int k = i + 1; k < nDim; k++)
				{
					double[] array3 = pfMatr[k];
					double num3 = (0.0 - array3[i]) / array[i];
					for (int j = i; j < nDim; j++)
					{
						array3[j] += num3 * array[j];
					}
					pfVect[k] += num3 * pfVect[i];
				}
			}
			for (int i = nDim - 1; i >= 0; i--)
			{
				double[] array4 = pfMatr[i];
				pfSolution[i] = pfVect[i];
				for (int j = i + 1; j < nDim; j++)
				{
					pfSolution[i] -= array4[j] * pfSolution[j];
				}
				pfSolution[i] /= array4[i];
			}
		}

		public static double LengthSquared(double[] x)
		{
			double num = 0.0;
			foreach (double num2 in x)
			{
				num += num2 * num2;
			}
			return num;
		}

		private void Normalize(double[] x)
		{
			double num = 0.0;
			for (int i = 0; i < Dimension; i++)
			{
				double num2 = x[i];
				num += num2 * num2;
			}
			double num3 = 1.0 / Math.Sqrt(num);
			for (int j = 0; j < Dimension; j++)
			{
				x[j] *= num3;
			}
		}

		public void SubtractFast(double[] x, double[] y, double[] target)
		{
			for (int i = 0; i < Dimension; i++)
			{
				target[i] = x[i] - y[i];
			}
		}

		private void FindNormalVector4D(VertexWrap[] vertices, double[] normal)
		{
			SubtractFast(vertices[1].PositionData, vertices[0].PositionData, ntX);
			SubtractFast(vertices[2].PositionData, vertices[1].PositionData, ntY);
			SubtractFast(vertices[3].PositionData, vertices[2].PositionData, ntZ);
			double[] array = ntX;
			double[] array2 = ntY;
			double[] array3 = ntZ;
			double num = array[3] * (array2[2] * array3[1] - array2[1] * array3[2]) + array[2] * (array2[1] * array3[3] - array2[3] * array3[1]) + array[1] * (array2[3] * array3[2] - array2[2] * array3[3]);
			double num2 = array[3] * (array2[0] * array3[2] - array2[2] * array3[0]) + array[2] * (array2[3] * array3[0] - array2[0] * array3[3]) + array[0] * (array2[2] * array3[3] - array2[3] * array3[2]);
			double num3 = array[3] * (array2[1] * array3[0] - array2[0] * array3[1]) + array[1] * (array2[0] * array3[3] - array2[3] * array3[0]) + array[0] * (array2[3] * array3[1] - array2[1] * array3[3]);
			double num4 = array[2] * (array2[0] * array3[1] - array2[1] * array3[0]) + array[1] * (array2[2] * array3[0] - array2[0] * array3[2]) + array[0] * (array2[1] * array3[2] - array2[2] * array3[1]);
			double num5 = Math.Sqrt(num * num + num2 * num2 + num3 * num3 + num4 * num4);
			double num6 = 1.0 / num5;
			normal[0] = num6 * num;
			normal[1] = num6 * num2;
			normal[2] = num6 * num3;
			normal[3] = num6 * num4;
		}

		private void FindNormalVector3D(VertexWrap[] vertices, double[] normal)
		{
			SubtractFast(vertices[1].PositionData, vertices[0].PositionData, ntX);
			SubtractFast(vertices[2].PositionData, vertices[1].PositionData, ntY);
			double[] array = ntX;
			double[] array2 = ntY;
			double num = array[1] * array2[2] - array[2] * array2[1];
			double num2 = array[2] * array2[0] - array[0] * array2[2];
			double num3 = array[0] * array2[1] - array[1] * array2[0];
			double num4 = Math.Sqrt(num * num + num2 * num2 + num3 * num3);
			double num5 = 1.0 / num4;
			normal[0] = num5 * num;
			normal[1] = num5 * num2;
			normal[2] = num5 * num3;
		}

		private void FindNormalVector2D(VertexWrap[] vertices, double[] normal)
		{
			SubtractFast(vertices[1].PositionData, vertices[0].PositionData, ntX);
			double[] array = ntX;
			double num = 0.0 - array[1];
			double num2 = array[0];
			double num3 = Math.Sqrt(num * num + num2 * num2);
			double num4 = 1.0 / num3;
			normal[0] = num4 * num;
			normal[1] = num4 * num2;
		}

		public void FindNormalVector(VertexWrap[] vertices, double[] normalData)
		{
			switch (Dimension)
			{
			case 2:
				FindNormalVector2D(vertices, normalData);
				return;
			case 3:
				FindNormalVector3D(vertices, normalData);
				return;
			case 4:
				FindNormalVector4D(vertices, normalData);
				return;
			}
			for (int i = 0; i < Dimension; i++)
			{
				nDNormalSolveVector[i] = 1.0;
			}
			for (int j = 0; j < Dimension; j++)
			{
				double[] array = jaggedNDMatrix[j];
				double[] position = vertices[j].Vertex.Position;
				for (int k = 0; k < Dimension; k++)
				{
					array[k] = position[k];
				}
			}
			GaussElimination(Dimension, jaggedNDMatrix, nDNormalSolveVector, normalData);
			Normalize(normalData);
		}

		public double GetVertexDistance(VertexWrap v, ConvexFaceInternal f)
		{
			double[] normal = f.Normal;
			double[] positionData = v.PositionData;
			double num = f.Offset;
			for (int i = 0; i < Dimension; i++)
			{
				num += normal[i] * positionData[i];
			}
			return num;
		}

		public MathHelper(int dimension)
		{
			Dimension = dimension;
			ntX = new double[Dimension];
			ntY = new double[Dimension];
			ntZ = new double[Dimension];
			nDNormalSolveVector = new double[Dimension];
			jaggedNDMatrix = new double[Dimension][];
			for (int i = 0; i < Dimension; i++)
			{
				nDNormalSolveVector[i] = 1.0;
				jaggedNDMatrix[i] = new double[Dimension];
			}
			nDMatrix = new double[Dimension, Dimension];
		}
	}
}
