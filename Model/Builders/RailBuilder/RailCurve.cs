using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Common;

namespace Trains.Model.Builders
{
	public class RailCurve : Curve3D
	{
		public List<CurveSegment> Segments { get; set; } = new List<CurveSegment>();
		private List<Vector3> points = new List<Vector3>();
		public Vector3 Origin { get; set; }

		public RailCurve() { }

		public RailCurve(Curve3D curve)
		{
			AppendCurve(curve.First(), curve);
		}

		public RailCurve(RailPath path) : this(path.Curve) { }

		public void PrependCurve(Vector3 origin, Curve3D curve)
		{
			var indexWhereInsertTo = 0;
			PlaceCurveOnMap(origin, curve, _atPosition: 0);
			AddCurveToSegments(curve, indexWhereInsertTo);
		}

		public void AppendCurve(Vector3 origin, Curve3D curve)
		{
			var indexWhereInsertTo = Segments.Count == 0 ? 0 : Segments.IndexOf(Segments.Last());
			PlaceCurveOnMap(origin, curve);
			AddCurveToSegments(curve, indexWhereInsertTo);
		}

		public void PlaceCurveOnMap(Vector3 origin, Curve3D curve, int _atPosition = -1)
		{
			var points = curve.Tessellate();

			for (int i = 0; i < points.Length; i++)
			{
				AddPoint(origin + points[i], atPosition: _atPosition);
			}
		}

		public void AddCurveToSegments(Curve3D curve, int index)
		{
			var segments = curve.ToSegments();
			Segments.InsertRange(index, segments);
		}

		public void RemoveCurve(RailCurve ctd)
		{
			// var accuracy = 1f;

			// var ctdFirst = ctd.First();
			// var ctdLast = ctd.Last();
			// var thisFirst = this.First();
			// var thisLast = this.Last();

			// Vector3 ctdPoint, thisPoint;

			// GD.Print("ctdFirst: " + ctdFirst);
			// GD.Print("ctdLast: " + ctdLast);
			// GD.Print("thisFirst: " + thisFirst);
			// GD.Print("thisLast: " + thisLast);
			// GD.Print();

			// var ctdOr = ctd.Origin;
			// var or = Origin;
			// ctdPoint = ctd.First() + ctd.Origin;
			// thisPoint = this.First() + Origin;
			// GD.Print("curveToDelete.First().IsEqualApprox(this.First()): " + ctdPoint.IsEqualApprox(thisPoint, accuracy));

			// ctdPoint = ctd.First() + ctd.Origin;
			// thisPoint = this.Last() + Origin;
			// GD.Print("curveToDelete.First().IsEqualApprox(this.Last()): " + ctdPoint.IsEqualApprox(thisPoint, accuracy));

			// ctdPoint = ctd.Last() + ctd.Origin;
			// thisPoint = this.First() + Origin;
			// GD.Print("curveToDelete.Last().IsEqualApprox(this.First()): " + ctdPoint.IsEqualApprox(thisPoint, accuracy));

			// ctdPoint = ctd.Last() + ctd.Origin;
			// thisPoint = this.Last() + Origin;
			// GD.Print("curveToDelete.Last().IsEqualApprox(this.Last()): " + ctdPoint.IsEqualApprox(thisPoint, accuracy));
			// GD.Print();


			var pointsToDeleteAmount = ctd.GetPointCount();
			for (int i = 0; i < pointsToDeleteAmount; i++)
			{
				var lastPointIndex = GetPointCount() - 1;
				RemovePoint(lastPointIndex);
			}


		}
	}
}
