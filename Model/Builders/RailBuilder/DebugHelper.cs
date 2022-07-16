using Godot;
using System;
using System.Linq;
using Trains.Model.Builders;

namespace Trains
{
	public class DebugHelper : Spatial
	{
		[Export] PackedScene helper;

		[Export]
		public bool ShowActualPoints
		{
			get => showActualPoints;
			set
			{
				ClearDrawn();
				if (value && currentPath != null)
					DrawUsingActualPoints((RailCurve)currentPath.Curve);
				showActualPoints = value;
			}
		}

		[Export]
		public bool ShowBakedPoints
		{
			get => showBakedPoints;
			set
			{
				ClearDrawn();
				if (value && currentPath != null)
					DrawUsingBakedPoints((RailCurve)currentPath.Curve);
				showBakedPoints = value;
			}
		}

		[Export]
		public bool ShowTesselatedPoints
		{
			get => showTesselatedPoints;
			set
			{
				ClearDrawn();
				if (value && currentPath != null)
					DrawUsingTesselatedPoints((RailCurve)currentPath.Curve);
				showTesselatedPoints = value;
			}
		}

		[Export]
		public int MaxStages
		{
			get => maxStages;
			set
			{
				ClearDrawn();
				if (currentPath != null)
				{
					maxStages = value;
					DrawUsingTesselatedPoints((RailCurve)currentPath.Curve);
				}
			}
		}
		[Export]
		public float ToleranceDegrees
		{
			get => toleranceDegrees;
			set
			{
				ClearDrawn();
				if (currentPath != null)
				{
					toleranceDegrees = value;
					DrawUsingTesselatedPoints((RailCurve)currentPath.Curve);
				}
			}
		}

		private int maxStages = 0;
		private float toleranceDegrees = 0;

		private bool showActualPoints = false;
		private bool showBakedPoints = false;
		private bool showTesselatedPoints = false;

		private MeshInstance helperInst;
		private Path currentPath;

		public override void _Ready()
		{
			helperInst = helper.Instance<MeshInstance>();
		}

		public void SetPath(Path currentPath)
		{
			this.currentPath = currentPath;
		}

		private void ClearDrawn()
		{
			var drawnHelpers = GetChildren().Cast<Node>().Where(node => node.Name.Contains("curvePoint"));
			foreach (var item in drawnHelpers)
				item.QueueFree();
		}

		private void DrawUsingActualPoints(RailCurve curve)
		{
			var amount = curve.GetPointCount();
			for (int i = 0; i < amount; i++)
			{
				var helper = (MeshInstance)helperInst.Duplicate();
				AddChild(helper);
				helper.Translation = curve.GetPointPosition(i) + curve.Origin;
			}
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
			var points = curve.Tessellate(MaxStages, ToleranceDegrees);
			foreach (var p in points)
			{
				var helper = (MeshInstance)helperInst.Duplicate();
				AddChild(helper);
				helper.Translation = p + curve.Origin;
			}
		}
	}
}
