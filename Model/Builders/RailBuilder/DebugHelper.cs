using Godot;
using System;
using System.Linq;
using Trains.Model.Builders;

namespace Trains
{
	public class DebugHelper : Spatial
	{
		private MeshInstance helperInst;

		public override void _Ready()
		{
			helperInst = GetNode<MeshInstance>("curvePont");
		}

		public void DrawHelpers(Path currentPath)
		{
			ClearDawn();

			var curve = RailCurve.GetFrom(currentPath);

			// DrawUsingActualPoints(curve);
			// DrawUsingTesselatedPoints(curve);
			DrawUsingBakedPoints(curve);
		}

		private void ClearDawn()
		{
			var drawnHelpers = GetChildren().Cast<Node>().Where(node => node.Name.Contains("curvePoint"));
			foreach (var item in drawnHelpers)
				item.QueueFree();
		}

		private void DrawUsingBakedPoints(RailCurve curve)
		{
			var points = curve.GetBakedPoints();
			foreach (var p in points)
			{
				var helper = (MeshInstance)helperInst.Duplicate();
				AddChild(helper);
				helper.Translation = p + curve.Origin;
			}
		}

		private void DrawUsingTesselatedPoints(RailCurve curve)
		{
			var points = curve.Tessellate(5, 100);
			foreach (var p in points)
			{
				var helper = (MeshInstance)helperInst.Duplicate();
				AddChild(helper);
				helper.Translation = p + curve.Origin;
			}
		}

		private void DrawUsingActualPoints(RailCurve curve)
		{
			var amount = curve.Tessellate().Length;
			for (int i = 0; i < amount; i++)
			{
				var helper = (MeshInstance)helperInst.Duplicate();
				AddChild(helper);
				helper.Translation = curve.GetPointPosition(i) + curve.Origin;
			}
		}
	}
}