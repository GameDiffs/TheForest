using System;
using System.IO;

namespace Pathfinding.Serialization
{
	public class GraphSerializationContext
	{
		private readonly GraphNode[] id2NodeMapping;

		public readonly BinaryReader reader;

		public readonly BinaryWriter writer;

		public readonly int graphIndex;

		public GraphSerializationContext(BinaryReader reader, GraphNode[] id2NodeMapping, int graphIndex)
		{
			this.reader = reader;
			this.id2NodeMapping = id2NodeMapping;
			this.graphIndex = graphIndex;
		}

		public GraphSerializationContext(BinaryWriter writer)
		{
			this.writer = writer;
		}

		public int GetNodeIdentifier(GraphNode node)
		{
			return (node != null) ? node.NodeIndex : -1;
		}

		public GraphNode GetNodeFromIdentifier(int id)
		{
			if (this.id2NodeMapping == null)
			{
				throw new Exception("Calling GetNodeFromIdentifier when serializing");
			}
			if (id == -1)
			{
				return null;
			}
			GraphNode graphNode = this.id2NodeMapping[id];
			if (graphNode == null)
			{
				throw new Exception("Invalid id");
			}
			return graphNode;
		}
	}
}
