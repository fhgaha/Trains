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
		private PackedScene railRemoverScene = GD.Load<PackedScene>("res://Scenes/Removers/RailRemover.tscn");
		private List<Cell> cells;
		private Events events;
		private CurveCalculator calculator;
		private StartSnapper bpStartSnapper;
		private EndSnapper bpEndSnapper;
		private PackedScene railPathScene;
		private Camera camera;
		private RailContainer railContainer;   //Rails
		private RailRemover railRemover;
		private Stack<RailCurve> undoStack = new Stack<RailCurve>();
		private ActualRailBuilder actualRailBuilder;

		//Vars
		private RailPath blueprint;
		private State state = State.None;
		private Vector3 prevDir = Vector3.Zero;
		private RailPath currentPath;
		private bool AreWeContinuingPath { get => currentPath != null; }

		//!in editor for CSGPolygon property Path Local should be "On" to place polygon where the cursor is with no offset

		//build order:
		//1. press BS button, blueprint will show up as simple straight road with it's end following cursor. 
		//2. using mouse select a place to build first segment. it cannot be built on obstacle.
		//3. press lmb to place first segment. 
		//4. a new blueprint will show up with start in the end of previous segment with the end following mouse pos.
		//	 this time path will be curved.
		//5. press lmb again to place blueprint road.
		public void Init(List<Cell> cells, Camera camera, RailContainer railContainer, PackedScene railPathScene)
		{
			this.cells = cells;
			this.railContainer = railContainer;
			this.camera = camera;
			this.railPathScene = railPathScene;
			bpStartSnapper = new StartSnapper();
			bpEndSnapper = new EndSnapper();
			calculator = GetNode<CurveCalculator>("Calculator");
			actualRailBuilder = new ActualRailBuilder();

			events = GetNode<Events>("/root/Events");
			events.Connect(nameof(Events.MainButtonModeChanged), this, nameof(onMainButtonModeChanged));
			events.Connect(nameof(Events.StartNewRoadPressed), this, nameof(onStartNewRoadPressed));
			events.Connect(nameof(Events.UndoRailPressed), this, nameof(onUndoRailPressed));
			events.Connect(nameof(Events.RemoveRailPressed), this, nameof(onRemoveRailPressed));
			events.Connect(nameof(Events.MainGUIPanelMouseEntered), this, nameof(onMainGUIPanelMouseEntered));
			events.Connect(nameof(Events.MainGUIPanelMouseExited), this, nameof(onMainGUIPanelMouseExited));
		}

		private void onMainButtonModeChanged(MainButtonType mode)
		{
			RemoveRailRemoverIfExists();

			if (mode == MainButtonType.BuildRail)
			{
				InitStateAndBlueprint();
			}
			else
			{
				ResetStateBlueprintPrevDir();
			}
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
			RemoveRailRemoverIfExists();
			undoStack.Clear();
			currentPath = null;
		}

		private void ReinitStateAndBlueprint()
		{
			ResetStateBlueprintPrevDir();
			InitStateAndBlueprint();
		}

		private void onUndoRailPressed()
		{
			ReinitStateAndBlueprint();
			RemoveRailRemoverIfExists();

			if (undoStack.Count == 0)
				return;

			var curveToDelete = undoStack.Pop();
			var curve = RailCurve.GetFrom(currentPath);
			curve.RemoveEdgeCurve(curveToDelete);

			if (undoStack.Count == 0)
				currentPath = null;
		}

		private void onRemoveRailPressed()
		{
			if (railRemover is null)
			{
				// GD.Print("onRemoveRailPressed");
				ResetStateBlueprintPrevDir();
				railRemover = railRemoverScene.Instance<RailRemover>();
				AddChild(railRemover);
				railRemover.Init(camera);
			}
		}

		private void RemoveRailRemoverIfExists()
		{
			railRemover?.QueueFree();
			railRemover = null;
		}

		public override void _UnhandledInput(InputEvent @event)
		{
			if (@event.IsActionPressed("lmb"))
			{
				switch (state)
				{
					case State.None:
						break;
					case State.SelectStart:
						ProcessSelectedStart();
						break;
					case State.SelectEnd:
						PlaceObject();
						break;
				}
			}

			if (@event.IsActionPressed("rmb"))
			{
				if (state == State.SelectEnd)
					ResetStateBlueprintPrevDir();
			}

			if (@event is InputEventMouseMotion)
			{
				switch (state)
				{
					case State.None:
						break;
					case State.SelectStart:
						DrawEmptyBlueprint();
						break;
					case State.SelectEnd:
						DrawFilledBlueprint();
						break;
				}
			}
		}

		private void ProcessSelectedStart()
		{
			var mousePos = this.GetIntersection(camera);
			blueprint.Translation = mousePos;

			bpStartSnapper.TrySnap(mousePos, railContainer.PathList, blueprint);
			if (bpStartSnapper.IsSnappedOnPathStartOrPathEnd)
				prevDir = bpStartSnapper.SnappedDir;
			if (bpStartSnapper.IsSnapped)
				currentPath = bpStartSnapper.SnappedPath;

			state = State.SelectEnd;
		}

		protected void PlaceObject()
		{
			if (bpStartSnapper.IsSnappedOnSegment || !AreWeContinuingPath)
				InitPath();
			else
				AddNewCurveToCurrentPath();

			GetNode<DebugHelper>("DebugHelper").SetPath(currentPath);
			undoStack.Push(RailCurve.GetFrom(blueprint));

			if (currentPath.CanBeJoined(bpStartSnapper, bpEndSnapper))
				JoinCurrentPath();
			else
				TranslateAndRedrawBp();
		}

		private void InitPath()
		{
			if (blueprint.Curve.GetPointCount() < 2) return;

			//blueprint.Duplicate() does not work for some reason. I use blueprint.Instance() instead.
			currentPath = railPathScene.Instance<RailPath>();
			railContainer.AddRailPath(currentPath);
			currentPath.InitOnPlacement(blueprint);
			prevDir = currentPath.DirFromEnd;

			currentPath.GetNode<CSGPolygon>("CSGPolygon").UseCollision = true;

			if (bpEndSnapper.IsSnappedOnSegment)
			{
				bpEndSnapper.SnappedPath.EnlistCrossing(bpEndSnapper.SnappedPoint);
			}

			actualRailBuilder.UpdateActualRails();

			//i dont know why but without this mid snapping logic of bpStartSnpper breaks
			bpStartSnapper.Reset();
			bpEndSnapper.Reset();
		}

		private void AddNewCurveToCurrentPath()
		{
			var pathOriginToBpOrigin = blueprint.Translation - currentPath.Translation;
			var curveToAdd = (RailCurve)blueprint.Curve;
			var railCurve = (RailCurve)currentPath.Curve;

			if (curveToAdd.GetPointCount() == 0) return;

			if (blueprint.Start.IsEqualApprox(currentPath.Start))
			{
				var oldValue = currentPath.Start;
				railCurve.PrependCurve(pathOriginToBpOrigin, curveToAdd);
				prevDir = currentPath.DirFromStart;

				currentPath.UpdateCrossing(oldValue, currentPath.Start);
			}
			else if (blueprint.Start.IsEqualApprox(currentPath.End))
			{
				var oldValue = currentPath.End;
				railCurve.AppendCurve(pathOriginToBpOrigin, curveToAdd);
				prevDir = currentPath.DirFromEnd;

				currentPath.UpdateCrossing(oldValue, currentPath.End);
			}

			if (bpEndSnapper.IsSnappedOnSegment)
			{
				bpEndSnapper.SnappedPath.EnlistCrossing(bpEndSnapper.SnappedPoint);
			}

			actualRailBuilder.UpdateActualRails();
		}

		private void JoinCurrentPath()
		{
			currentPath.JoinStartToEnd();
			ResetStateBlueprintPrevDir();
		}

		private void TranslateAndRedrawBp()
		{
			blueprint.Translation = blueprint.End;
			bpStartSnapper.TrySnap(blueprint.Translation, railContainer.PathList, blueprint);
			//redraw before next frame
			DrawFilledBlueprint();
		}

		private void DrawEmptyBlueprint()
		{
			var mousePos = this.GetIntersection(camera);
			blueprint.Translation = mousePos;
			bpStartSnapper.TrySnap(mousePos, railContainer.PathList, blueprint);
			blueprint.SetColor();
		}

		private void DrawFilledBlueprint()
		{
			var mousePos = this.GetIntersection(camera);
			var mousePosIsInMapBorders = mousePos != Vector3.Zero;
			if (!mousePosIsInMapBorders) return;

			var points = new List<Vector2>();
			bpEndSnapper.TrySnap(mousePos, railContainer.PathList, blueprint);

			if (bpStartSnapper.IsSnappedOnSegment)
				prevDir = bpStartSnapper.GetBpStartSnappedSegmentToCursorDirection(mousePos);

			if (!bpEndSnapper.IsSnapped)
			{
				points = calculator.CalculateCurvePoints
				(
					start: blueprint.Translation.ToVec2(),
					end: mousePos.ToVec2(),
					prevDir: prevDir.ToVec2()
				);
			}
			else
			{
				points = calculator.CalculateCurvePointsWithSnappedEnd
				(
					start: blueprint.Translation.ToVec2(),
					end: bpEndSnapper.SnappedPoint.ToVec2(),
					startDir: prevDir.ToVec2(),
					finishDir: bpEndSnapper.SnappedDir.ToVec2().Rotated(Pi)
				);
			}

			BuildBlueprintCurve(points);
		}

		private void BuildBlueprintCurve(List<Vector2> points)
		{
			var curve = new RailCurve();
			if (points.Count > 0)
			{
				//first point (4.033; 5.023) - bpTranslat (4.033112, -1.92047E-08, 3.023653) = (0;0;2)
				//who messes with bpTranslation?
				points.ForEach(p => curve.AddPoint(p.ToVec3() - blueprint.Translation));
			}
			else
			{
				var from = Vector3.Zero;
				var to = prevDir == Vector3.Zero ? Vector3.Forward : prevDir;
				curve = RailCurve.GetSimpleCurve(from, to);
			}

			blueprint.Curve = curve;
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
