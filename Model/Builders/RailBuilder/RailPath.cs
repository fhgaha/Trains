using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Common;

namespace Trains.Model.Builders
{
	public class RailPath : Path
	{
		[Export] private Color bpColor;

		[Export] private Color notAllowedColor;

		public Vector3 Start { get => Translation + Curve.First(); }
		public Vector3 End { get => Translation + Curve.Last(); }

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

		private Curve3D originalBpCurve;

		public RailPath() { }

		public override void _Ready()
		{
			originalBpCurve = Curve;
		}

		public void Init(Path blueprint)
		{
			Transform = blueprint.Transform;
			Curve = (RailCurve)blueprint.Curve;
			GetNode<CSGPolygon>("CSGPolygon").UseCollision = true;
		}

		public void SetSimpleCurve(Vector3 first, Vector3 second)
		{
			var curve = new Curve3D();
			curve.AddPoint(first);
			curve.AddPoint(second);
			Curve = curve;
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
			var csgMaterial = (SpatialMaterial)GetNode<CSGPolygon>("CSGPolygon").Material;
			csgMaterial.AlbedoColor = canBuild ? bpColor : notAllowedColor;
		}

		public List<CurveSegment> GetSegments()
		{
			return ((RailCurve)Curve).GetSegments(Translation);
		}

		public float GetPolygonWidth()
		{
			return GetNode<CSGPolygon>("CSGPolygon").Polygon[3].x;
		}
	}
}
