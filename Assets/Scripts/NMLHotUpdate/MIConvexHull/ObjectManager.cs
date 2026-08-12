using System.Collections.Generic;

namespace MIConvexHull
{
	internal class ObjectManager
	{
		private readonly int Dimension;

		private Stack<ConvexFaceInternal> RecycledFaceStack;

		private Stack<FaceConnector> ConnectorStack;

		private Stack<VertexBuffer> EmptyBufferStack;

		private Stack<DeferredFace> DeferredFaceStack;

		public void DepositFace(ConvexFaceInternal face)
		{
			for (int i = 0; i < Dimension; i++)
			{
				face.AdjacentFaces[i] = null;
			}
			RecycledFaceStack.Push(face);
		}

		public ConvexFaceInternal GetFace()
		{
			if (RecycledFaceStack.Count == 0)
			{
				return new ConvexFaceInternal(Dimension, GetVertexBuffer());
			}
			return RecycledFaceStack.Pop();
		}

		public void DepositConnector(FaceConnector connector)
		{
			ConnectorStack.Push(connector);
		}

		public FaceConnector GetConnector()
		{
			if (ConnectorStack.Count == 0)
			{
				return new FaceConnector(Dimension);
			}
			return ConnectorStack.Pop();
		}

		public void DepositVertexBuffer(VertexBuffer buffer)
		{
			buffer.Clear();
			EmptyBufferStack.Push(buffer);
		}

		public VertexBuffer GetVertexBuffer()
		{
			if (EmptyBufferStack.Count == 0)
			{
				return new VertexBuffer();
			}
			return EmptyBufferStack.Pop();
		}

		public void DepositDeferredFace(DeferredFace face)
		{
			DeferredFaceStack.Push(face);
		}

		public DeferredFace GetDeferredFace()
		{
			if (DeferredFaceStack.Count == 0)
			{
				return new DeferredFace();
			}
			return DeferredFaceStack.Pop();
		}

		public ObjectManager(int dimension)
		{
			Dimension = dimension;
			RecycledFaceStack = new Stack<ConvexFaceInternal>();
			ConnectorStack = new Stack<FaceConnector>();
			EmptyBufferStack = new Stack<VertexBuffer>();
			DeferredFaceStack = new Stack<DeferredFace>();
		}
	}
}
