using Godot;
using System;
using System.Linq;
using Trains.Model.Builders;

namespace Trains
{
	public class DebugHelper : Spatial
	{
		[Export]
		private bool showCurvePointsPosition = false;
		private MeshInstance helperInst;
		private Path currentPath;

		public bool ShowCurvePointsPosition
		{
			get => showCurvePointsPosition;
			set
			{
				if (value && currentPath != null)
					Draw(currentPath);
				else if (!value)
					ClearDrawn();

				showCurvePointsPosition = value;
			}
		}

		public override void _Ready()
		{
			helperInst = GetNode<MeshInstance>("curvePoint");
		}

		public void DrawHelpers(Path currentPath)
		{
			this.currentPath = currentPath;
			ClearDrawn();
			if (showCurvePointsPosition)
				Draw(currentPath);
		}

		private void Draw(Path currentPath)
		{
			var curve = RailCurve.GetFrom(currentPath);

			// DrawUsingActualPoints(curve);
			// DrawUsingTesselatedPoints(curve);
			DrawUsingBakedPoints(curve);
		}

		private void ClearDrawn()
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
