using System;

namespace Trains.Model.Common.GraphRelated
{
	public class Edge
	{
		public readonly Node From;
		public readonly Node To;
		public Edge(Node first, Node second)
		{
			this.From = first;
			this.To = second;
		}
		public bool IsIncident(Node node)
		{
			return From == node || To == node;
		}
		public Node OtherNode(Node node)
		{
			if (!IsIncident(node)) throw new ArgumentException();
			if (From == node) return To;
			return From;
		}

		public override bool Equals(object obj)
		{
			
			if (obj == null || GetType() != obj.GetType())
				return false;
			var otherEdge = (Edge)obj;
			return From == otherEdge.From && To == otherEdge.To;
		}
		
		public override int GetHashCode()
		{
			return From.GetHashCode() + To.GetHashCode();
		}

		public override string ToString()
		{
			return $"Edge [{From} - {To}]";
		}
	}
}
