using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Builders;

namespace Trains
{
	public class VisibleRailContainer : Spatial
	{
		private readonly List<RailPath> rails = new List<RailPath>();
		public IEnumerable<RailPath> Rails
		{
			get
			{
				foreach (var r in rails)
					yield return r;
			}
		}

		public void AddRail(RailPath rail)
		{
			AddChild(rail);
			rails.Add(rail);
			// rail.GetNode<CSGPolygon>("CSGPolygon").UseCollision = true;
			rail.ConvertCsgToMeshInstance();
		}

		public void Add(RailPath path)
		{
			rails.Add(path);
		}

		public void Remove(RailPath path)
		{
			rails.Remove(path);
		}

		public void UpdateMeshInstanceOf(RailPath path)
		{
			path.UpdateMeshInstance();
		}
	}
}
