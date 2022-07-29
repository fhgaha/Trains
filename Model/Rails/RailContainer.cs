using Godot;
using System;
using System.Collections.Generic;
using Trains.Model.Builders;

namespace Trains
{
	public class RailContainer : Spatial
	{
		public List<RailPath> PathList = new List<RailPath>();

		internal void AddRailPath(RailPath path)
		{
			PathList.Add(path);
			AddChild(path);
		}

		internal void AddToPathList(RailPath path)
		{
			PathList.Add(path);
		}

		internal void RemovePath(RailPath path)
		{
			PathList.Remove(path);
		}
	}
}
