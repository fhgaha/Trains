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

		public void AddRail(RailPath rail)
		{
			Rails.Add(rail);
			AddChild(rail);
			rail.GetNode<CSGPolygon>("CSGPolygon").UseCollision = true;
		}

		public void Add(RailPath path)
		{
			Rails.Add(path);
		}

		public void Remove(RailPath path)
		{
			Rails.Remove(path);
		}
	}
}
