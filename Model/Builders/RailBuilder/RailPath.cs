using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Builders;
using Trains.Model.Common;

namespace Trains
{
	public class RailPath : Path
	{
		[Export] public readonly Color BpColor;
		[Export] public readonly Color NotAllowedColor;
		[Export] public Vector3 Start { get => Translation + Curve.First(); set => Start = value; }
		[Export] public Vector3 End { get => Translation + Curve.Last(); set => End = value; }
		[Export] public Vector3[] Points { get => Curve.ToArray(); set => Points = value; }
		[Export] public List<Vector3> Crossings { get; private set; }
		[Export] private bool ShowHelpers 
		{
			get => ShowHelpers; 
			set => ShowHelpers = value;
		}
		public bool IsJoined { get; private set; }

		private CSGPolygon polygon;
		private Curve3D originalBpCurve;

		public RailPath()
		{
			Crossings = new List<Vector3>();
		}

		public Vector3 DirFromStart
		{
			get
			{
				var points = Curve.TakeFirst(2);
				return (points[1] - points[0]).Normalized();
			}
		}
		public Vector3 DirFromEnd
		{
			get
			{
				var points = Curve.TakeLast(2);
				return (points[1] - points[0]).Normalized();
			}
		}

		public override void _Ready()
		{
			originalBpCurve = Curve;
			Curve = RailCurve.GetFrom(Curve);
			if (HasNode("CSGPolygon"))
				polygon = GetNode<CSGPolygon>("CSGPolygon");
		}

		public void InitOnPlacement(RailPath blueprint)
		{
			GlobalTransform = blueprint.GlobalTransform;
			Curve = RailCurve.GetFrom(blueprint.Curve);

			Crossings = new List<Vector3>();
			EnlistCrossing(Start);
			EnlistCrossing(End);
		}

		public void SetOriginalBpCurve()
		{
			Curve = originalBpCurve;
		}

		public void SetColor()
		{
			var area = GetNode<Area>("CSGPolygon/Area");
			var bodies = area.GetOverlappingBodies().Cast<Node>().Where(b => b.IsInGroup("Obstacles"));
			var canBuild = !bodies.Any();
			var csgMaterial = (SpatialMaterial)polygon.Material;
			csgMaterial.AlbedoColor = canBuild ? BpColor : NotAllowedColor;
		}

		public List<CurveSegment> GetSegments()
		{
			return ((RailCurve)Curve).GetSegments(Translation);
		}

		public List<CurveSegment> GetMidSegments()
		{
			return GetSegments()
				.Where(s => s.First != Start && s.First != End
						 && s.Second != Start && s.Second != End)
				.ToList();
		}

		public float GetPolygonWidth()
		{
			return polygon.Polygon[3].x;
		}

		public void JoinStartToEnd()
		{
			polygon.PathJoined = true;
			IsJoined = true;
		}

		public bool CanBeJoined(StartSnapper startSnapper, EndSnapper endSnapper)
		{
			return startSnapper.IsSnappedOnPathStartOrPathEnd
				&& endSnapper.IsSnappedOnPathStartOrPathEnd
				&& Start.IsEqualApprox(End);
		}

		public void EnlistCrossing(Vector3 point)
		{
			if (!Crossings.Contains(point))
			{
				Crossings.Add(point);
			}
			
			// GD.Print("!!!!!!!!!!!!!");
			// GD.Print(System.Environment.StackTrace);
			// GD.Print("!!!!!!!!!!!!!");
		}

		public void UpdateCrossing(Vector3 oldValue, Vector3 newValue)
		{
			Crossings.Remove(oldValue);
			EnlistCrossing(newValue);
		}
	}
}
