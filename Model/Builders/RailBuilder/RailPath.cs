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
		public Vector3 Start { get => Translation + Curve.First(); }
		public Vector3 End { get => Translation + Curve.Last(); }
		public bool IsJoined { get; private set; }

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

		private CSGPolygon polygon;

		private Curve3D originalBpCurve;

		public RailPath() { }

		public override void _Ready()
		{
			originalBpCurve = Curve;
			polygon = GetNode<CSGPolygon>("CSGPolygon");
		}

		public RailPath Clone()
		{
			//An instance stays linked to the original so when the original changes, the instance changes too.
			var path = (RailPath)this.Duplicate();
			var curve = RailCurve.GetFrom(Curve);
			curve.Origin = GlobalTransform.origin;
			path.IsJoined = this.IsJoined;
			path.polygon = this.polygon;
			path.originalBpCurve = this.originalBpCurve;
			// path.GlobalTransform = this.GlobalTransform;
			path.Curve = curve;
			return path;
		}

		public void InitOnPlacement(Path blueprint)
		{
			Transform = blueprint.Transform;
			Curve = (RailCurve)blueprint.Curve;
			polygon.UseCollision = true;
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
	}
}
