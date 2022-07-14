using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Cells;
using Trains.Model.Common;
using static Trains.Model.Common.Enums;
using static Godot.Mathf;

namespace Trains.Model.Builders
{
	enum State { None, SelectStart, SelectEnd }

	public class RailBuilder : Spatial
	{
		private Color yellow = new Color("86e3db6b");
		private Color red = new Color("86e36b6b");
		private const float rayLength = 1000f;
		private List<Cell> cells;
		private Events events;
		private CurveCalculator calculator;
		private PackedScene railPathScene;
		private Camera camera;
		private RailPath blueprint;
		private Spatial railsHolder;   //Rails
		private State state = State.None;
		private Snapper snapper;
		private Stack<RailCurve> undoStack = new Stack<RailCurve>();
		private List<RailPath> pathList = new List<RailPath>();
		private Vector3 prevDir = Vector3.Zero;
		private RailPath currentPath;   //path from which railbuilding is being continued

		//!in editor for CSGPolygon property Path Local should be "On" to place polygon where the cursor is with no offset

		//build order:
		//1. press BS button, blueprint will show up as simple straight road with it's end following cursor. 
		//2. using mouse select a place to build first segment. it cannot be built on obstacle.
		//3. press lmb to place first segment. 
		//4. a new blueprint will show up with start in the end of previous segment with the end following mouse pos.
		//	 this time path will be curved.
		//5. press lmb again to place blueprint road.
		public void Init(List<Cell> cells, Camera camera, Spatial railsHolder, PackedScene railPathScene)
		{
			this.cells = cells;
			this.railsHolder = railsHolder;
			this.camera = camera;
			this.railPathScene = railPathScene;
			snapper = new Snapper();
			events = GetNode<Events>("/root/Events");
			events.Connect(nameof(Events.MainButtonPressed), this, nameof(onMainButtonPressed));
			events.Connect(nameof(Events.StartNewRoadPressed), this, nameof(onStartNewRoadPressed));
			events.Connect(nameof(Events.UndoRailPressed), this, nameof(onUndoRailPressed));
			events.Connect(nameof(Events.MainGUIPanelMouseEntered), this, nameof(onMainGUIPanelMouseEntered));
			events.Connect(nameof(Events.MainGUIPanelMouseExited), this, nameof(onMainGUIPanelMouseExited));
			calculator = GetNode<CurveCalculator>("Calculator");
		}

		public override void _Process(float delta)
		{
		}

		private void onMainButtonPressed(MainButtonType buttonType)
		{
			//other main button is pressed
			if (buttonType != MainButtonType.BuildRail)
			{
				ResetStateBlueprintPrevDir();
				currentPath = null;
				undoStack.Clear();
				return;
			}

			//"Build Rail" button was pressed and we press it again
			if (Global.MainButtonMode is MainButtonType.BuildRail)
			{
				Global.MainButtonMode = null;
				ResetStateBlueprintPrevDir();
				currentPath = null;
				undoStack.Clear();
				return;
			}

			Global.MainButtonMode = MainButtonType.BuildRail;
			InitStateAndBlueprint();
		}

		private void ResetStateBlueprintPrevDir()
		{
			state = State.None;
			blueprint?.QueueFree();
			blueprint = null;
			prevDir = Vector3.Zero;
		}

		private void InitStateAndBlueprint()
		{
			state = State.SelectStart;
			blueprint = railPathScene.Instance<RailPath>();
			AddChild(blueprint);
			blueprint.Name = "blueprint";
		}

		private void onStartNewRoadPressed()
		{
			ReinitStateAndBlueprint();
			undoStack.Clear();
		}

		private void ReinitStateAndBlueprint()
		{
			ResetStateBlueprintPrevDir();
			InitStateAndBlueprint();
		}

		private void onUndoRailPressed()
		{
			ReinitStateAndBlueprint();

			if (undoStack.Count == 0)
				return;

			var curveToDelete = undoStack.Pop();
			var curve = RailCurve.GetFrom(currentPath);
			curve.RemoveCurve(curveToDelete);

			if (undoStack.Count == 0)
				currentPath = null;
		}

		public override void _UnhandledInput(InputEvent @event)
		{
			if (@event is InputEventMouseButton evMouseButton1 && evMouseButton1.IsActionPressed("lmb"))
			{
				switch (state)
				{
					case State.None: return;
					case State.SelectStart:
						SelectStart();
						break;
					case State.SelectEnd:
						PlaceObject();
						break;
				}
			}

			if (@event is InputEventMouseButton evMouseButton2 && evMouseButton2.IsActionPressed("rmb"))
			{
				if (state == State.SelectEnd)
					ResetStateBlueprintPrevDir();
			}

			if (@event is InputEventMouseMotion)
			{
				switch (state)
				{
					case State.None: return;
					case State.SelectStart:
						UpdateBlueprint();
						break;
					case State.SelectEnd:
						DrawBlueprint();
						break;
				}
			}
		}

		private void SelectStart()
		{
			var mousePos = this.GetIntersection(camera, rayLength);
			blueprint.Translation = mousePos;
			state = State.SelectEnd;

			//snap
			snapper.SnapIfNecessary(mousePos, pathList, blueprint);  //path should begin from snapped point with no offset
			if (snapper.SnappedDir != Vector3.Zero)
				prevDir = snapper.SnappedDir;
			if (!(snapper.SnappedPath is null))
				currentPath = snapper.SnappedPath;
		}

		private void UpdateBlueprint()
		{
			var mousePos = this.GetIntersection(camera, rayLength);
			blueprint.Translation = mousePos;
			snapper.SnapIfNecessary(mousePos, pathList, blueprint);

			//set base color
			var area = blueprint.GetNode<Area>("CSGPolygon/Area");
			var bodies = area.GetOverlappingBodies().Cast<Node>().Where(b => b.IsInGroup("Obstacles"));
			var canBuild = !bodies.Any();
			var csgMaterial = (SpatialMaterial)blueprint.GetNode<CSGPolygon>("CSGPolygon").Material;
			csgMaterial.AlbedoColor = canBuild ? yellow : red;
		}

		private void DrawBlueprint()
		{
			var mousePos = this.GetIntersection(camera, rayLength);
			var points = new List<Vector2>();

			var mousePosIsInBorders = mousePos != Vector3.Zero;
			if (mousePosIsInBorders)
			{
				var continuing = !(currentPath is null);
				points = calculator.CalculateCurvePoints
				(
					start: blueprint.Translation.ToVec2(),
					end: mousePos.ToVec2(),
					prevDir: prevDir.ToVec2(),
					firstSegmentIsPlaced: continuing
				);
			}
			blueprint.Curve = BuildBlueprintCurve(points);
		}

		private RailCurve BuildBlueprintCurve(List<Vector2> points)
		{
			var curve = new RailCurve();
			if (points.Count > 0)
			{
				points.ForEach(p => curve.AddPoint(p.ToVec3() - blueprint.Translation));
			}
			else
			{
				//add two points to prevent error "The faces count are 0, the mesh shape cannot be created"
				curve.AddPoint(Vector3.Zero);
				curve.AddPoint(prevDir == Vector3.Zero ? Vector3.Forward : prevDir);
			}
			return curve;
		}

		protected void PlaceObject()
		{
			if (currentPath is null)
				InitPath();
			else
				AddNewCurveToCurrentPath();

			GetNode<DebugHelper>("DebugHelper").DrawHelpers(currentPath);

			var curve = RailCurve.GetFrom(blueprint);
			undoStack.Push(curve);

			SaveVarsRedrawBlueprint(prevDir);
		}

		private void InitPath()
		{
			currentPath = railPathScene.Instance<RailPath>();
			AddChild(currentPath);
			pathList.Add(currentPath);
			currentPath.Init(blueprint);
			prevDir = currentPath.DirFromEnd;
		}

		private void AddNewCurveToCurrentPath()
		{
			var pathOriginToBpOrigin = blueprint.Translation - currentPath.Translation;
			var curveToAdd = (RailCurve)blueprint.Curve;
			var railCurve = (RailCurve)currentPath.Curve;

			if (curveToAdd.GetPointCount() == 0) return;

			if (blueprint.Start.IsEqualApprox(currentPath.Start))
			{
				railCurve.PrependCurve(pathOriginToBpOrigin, curveToAdd);
				prevDir = currentPath.DirFromStart;
			}

			if (blueprint.Start.IsEqualApprox(currentPath.End))
			{
				railCurve.AppendCurve(pathOriginToBpOrigin, curveToAdd);
				prevDir = currentPath.DirFromEnd;
			}
		}



		private void SaveVarsRedrawBlueprint(Vector3 direction)
		{
			blueprint.Translation = blueprint.End;
			prevDir = direction;

			//this is called so that there is no overlap of blueprint and path or 
			//generally wrong bp display until next frame starts
			DrawBlueprint();
		}

		private void onMainGUIPanelMouseEntered()
		{
			if (blueprint is null) return;
			blueprint.Visible = false;
		}

		private void onMainGUIPanelMouseExited()
		{
			if (blueprint is null) return;
			blueprint.Visible = true;
		}
	}
}
