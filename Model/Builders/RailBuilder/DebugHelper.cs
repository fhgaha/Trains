using Godot;
using System;

namespace Trains
{
	public class DebugHelper : Spatial
	{
		private void DrawHelpers()
		{
			var existingHelpers = GetChildren().Cast<Node>().Where(node => node.Name.Contains("curvePoint"));
			foreach (var item in existingHelpers)
			{
				item.QueueFree();
			}

			var curve = RailCurve.GetFrom(currentPath);

			//using actual points
			// var amount = curve.Tessellate().Length;
			// for (int i = 0; i < amount; i++)
			// {
			// 	var helper = (MeshInstance)helperInst.Duplicate();
			// 	AddChild(helper);
			// 	helper.Translation = curve.GetPointPosition(i) + curve.Origin;
			// }

			//using tesselated points
			//var points = curve.Tessellate(5, 100);
			// foreach (var p in points)
			// {
			// 	var helper = (MeshInstance)helperInst.Duplicate();
			// 	AddChild(helper);
			// 	helper.Translation = p + curve.Origin;
			// }

			//using bakedPoints
			var points = curve.GetBakedPoints();
			foreach (var p in points)
			{
				var helper = (MeshInstance)helperInst.Duplicate();
				AddChild(helper);
				helper.Translation = p + curve.Origin;
			}
		}
	}