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
		//Do not use setter! It is only for editor.
		[Export] public Vector3 Start { get => Translation + Curve.First(); set => Start = value; }
		//Do not use setter! It is only for editor.
		[Export] public Vector3 End { get => Translation + Curve.Last(); set => End = value; }
		[Export] public Vector3[] Points { get => Curve.ToArray(); set => Points = value; }
		[Export] public List<Vector3> Crossings { get; private set; } = new List<Vector3>();

		private bool showBakedPoints;
		[Export]
		private bool ShowBakedPoints
		{
			get => showBakedPoints;
			set
			{
				showBakedPoints = value;
				if (helperScene is null) return;

				if (value)
				{
					Curve.GetBakedPoints().ToList().ForEach(p => AddHelper(p));
				}
				else
				{
					GetChildren().Cast<Node>()
						.Where(n => n.Name.Contains("curvePoint")).ToList()
						.ForEach(n => n.QueueFree());
				}
			}
		}

		private bool showDefaultPoints;
		[Export]
		private bool ShowDefaultPoints
		{
			get => showDefaultPoints;
			set
			{
				showDefaultPoints = value;
				if (helperScene is null) return;

				if (value)
				{
					for (int i = 0; i < Curve.GetPointCount(); i++)
						AddHelper(Curve.GetPointPosition(i));
				}
				else
				{
					GetChildren().Cast<Node>()
						.Where(n => n.Name.Contains("curvePoint")).ToList()
						.ForEach(n => n.QueueFree());
				}
			}
		}

		[Export] private PackedScene helperScene;
		[Export] private PackedScene railCsg;

		public bool IsJoined { get; private set; }

		private CSGPolygon polygon;
		private Curve3D originalBpCurve;

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
			var curve = RailCurve.GetFrom(blueprint.Curve);
			curve.Origin = blueprint.Translation;
			Curve = curve;

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
						 && s.Second != Start && s.Second != End).ToList();
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
		}

		public void UpdateCrossing(Vector3 oldValue, Vector3 newValue)
		{
			Crossings.Remove(oldValue);
			EnlistCrossing(newValue);
		}

		private void AddHelper(Vector3 position)
		{
			var helper = helperScene.Instance<MeshInstance>();
			helper.MaterialOverride = new SpatialMaterial() { AlbedoColor = new Color("8730abf3") };
			helper.Translation = position;
			AddChild(helper);
		}

		public static RailPath BuildNoMeshRail(PackedScene scene, IEnumerable<Vector3> points, Vector3 translation)
		{
			var newPath = scene.Instance<RailPath>();
			var curve = new Curve3D();

			foreach (var v in points)
			{
				curve.AddPoint(v);
			}

			var railCurve = RailCurve.GetFrom(curve);
			railCurve.Origin = translation;
			newPath.Curve = railCurve;
			newPath.Translation = translation;
			return newPath;
		}

		public void ConvertCsgToMeshInstance()
		{
			var csg = GetNode<CSGPolygon>("CSGPolygon");

			var meshInstance = new MeshInstance();
			csg.Call("_update_shape");
			//GD.Print(csg.GetMeshes());
			var csgMesh = (Mesh)csg.GetMeshes()[1]; //Only works when this node is the root shape.
			var csgTransform = csg.GlobalTransform;
			// var csgName = csg.Name;
			meshInstance.Mesh = csgMesh;

			csg.GetParent().AddChild(meshInstance);
			var csgChildren = csg.GetChildren();
			csg.RemoveAllChildren();
			foreach (Node c in csgChildren) c.QueueFree();
			csg.GetParent().RemoveChild(csg);
			csg.QueueFree();

			meshInstance.Owner = this;
			meshInstance.GlobalTransform = csgTransform;
			// meshInstance.Name = csgName;
			meshInstance.Name = "MeshInstanceConvertedFromCsg";
			meshInstance.CreateConvexCollision(simplify: true); //if collisions get weird this should be changed probably
		}

		public void UpdateMeshInstance()
		{	
			//crashes on 1kk objets, 400k orphan nodes

			//remove meshInstace
			//create new csg
			//add curve points of bp to path curve
			//apply csg to path
			//convert csg to mesh instance

			var oldMeshInstanceNode = GetNode<MeshInstance>("MeshInstanceConvertedFromCsg");
			RemoveChild(oldMeshInstanceNode);
			oldMeshInstanceNode.QueueFree();
			var csg = railCsg.Instance<CSGPolygon>();
			AddChild(csg);
			ConvertCsgToMeshInstance();
		}
	}
}
