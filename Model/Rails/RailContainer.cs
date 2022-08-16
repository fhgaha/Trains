using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Builders;

namespace Trains
{
	public class RailContainer : Spatial
	{
		public List<RailPath> Rails { get; private set; } = new List<RailPath>();

		internal void AddRail(RailPath rail)
		{
			Rails.Add(rail);
			AddChild(rail);
			rail.GetNode<CSGPolygon>("CSGPolygon").UseCollision = true;
		}

		internal void Add(RailPath path)
		{
			Rails.Add(path);
		}

		internal void RemovePath(RailPath path)
		{
			Rails.Remove(path);
		}

	}
}
