using Godot;
using System;
using System.Collections.Generic;
using Trains.Model.Builders;

namespace Trains
{
	public class RailContainer : Spatial
	{
		public List<RailPath> PathList = new List<RailPath>();

		internal void AddPath(RailPath path)
		{
			PathList.Add(path);
			AddChild(path);
		}

		internal void AddExistingPath(RailPath path)
		{
			PathList.Add(path);
		}

		internal void RemovePath(RailPath path)
		{
			PathList.Remove(path);
		}
	}
}