using Godot;
using System;

namespace Trains
{
	public class DrawingArcs2D : Node2D
	{

		private Color white = new Color(1, 1, 1, 1);
		private Color green = new Color(0.2f, 1f, 0.2f, 1);

		public override void _Process(float delta)
		{
			Update();
		}

		public override void _Draw()
		{
			Arc2();
		}

		private void Arc()
		{
			var s = new Vector2(512, 300);
			//var s = new Vector2(960, 480);
			var d = Vector2.Up;
			var e = GetGlobalMousePosition();
			var aplha = e.Dot(d);
			var se = e - s;
			var M = se / 2;
			var perp = new Vector2(s.y - e.y, e.x - s.x);
			//perp *= aplha / Mathf.Pi;
			//var perp = -d * 1;
			//var perp = M.Rotated(Mathf.Pi/180 * -90);
			var coeff = 0.1f;
			var t = ((s.x - M.x) * d.x + (s.y - M.y) * d.y) / (perp.x * d.x + perp.y * d.y) * coeff;
			//t *= Mathf.Pi/2 - aplha;
			//var t = (s - M).Dot(d)/perp.Dot(d);

			var C = s + M + perp * t;
			var R = (s - C).Length();
			

			DrawCircle(s, 10, white);
			//white line
			DrawLine(s, e, white);
			DrawLine(s + M, C, white);
			DrawArc(C, R, 0, Mathf.Pi / 180 * 360, 100, white);

			ShowValues(s, d, e, se, M, perp, t, C);
		}

		private void Arc2()
		{
			var s = new Vector2(512, 300);
			var d = Vector2.Up;
			var e = GetGlobalMousePosition();
			var sex = e.x - s.x;
			var sey = e.y - s.y;
			var perpx = -sey;
			var perpy = sex;
			var mx = (e.x + s.x) / 2;
			var my = (e.y + s.y) / 2;
			var p = perpx * d.x + perpy * d.y;
			float t = 0f;
			if (p > 0)	 t = -0.5f*(sex*d.x + sey*d.x) / p;
			else return;

			var cx = mx + perpx * t;
			var cy = my + perpy * t;
			var radius = new Vector2(cx - s.x, cy - s.y).Length();
			var C = new Vector2(cx, cy);
			var M = new Vector2(mx, my);
			var se = new Vector2(sex, sey);
			var perp = new Vector2(perpx, perpy);

			DrawCircle(s, 10, white);
			//white line
			DrawLine(s, e, white);
			DrawLine(s + M, C, white);
			DrawArc(C, radius, 0, Mathf.Pi / 180 * 360, 100, white);

			ShowValues(s, d, e, se, M, perp, t, C);
		}

		private void ShowValues(Vector2 s, Vector2 d, Vector2 e, Vector2 se, Vector2 M, Vector2 perp, float t, Vector2 C)
		{
			GetNode<Label>("VBoxContainer/s").Text = GetNode<Label>("VBoxContainer/s").Name + ": " + s;
			GetNode<Label>("VBoxContainer/d").Text = GetNode<Label>("VBoxContainer/d").Name + ": " + d;
			GetNode<Label>("VBoxContainer/e").Text = GetNode<Label>("VBoxContainer/e").Name + ": " + e;
			GetNode<Label>("VBoxContainer/se").Text = GetNode<Label>("VBoxContainer/se").Name + ": " + se;
			GetNode<Label>("VBoxContainer/M").Text = GetNode<Label>("VBoxContainer/M").Name + ": " + M;
			GetNode<Label>("VBoxContainer/perp").Text = GetNode<Label>("VBoxContainer/perp").Name + ": " + perp;
			GetNode<Label>("VBoxContainer/t").Text = GetNode<Label>("VBoxContainer/t").Name + ": " + t;
			GetNode<Label>("VBoxContainer/C").Text = GetNode<Label>("VBoxContainer/C").Name + ": " + C;
		}

		private void Polyline()
		{
			var points = new Vector2[5];
			points[0] = new Vector2(550, 310);
			points[1] = new Vector2(600, 295);
			points[2] = new Vector2(650, 305);
			points[3] = new Vector2(700, 400);
			points[4] = new Vector2(750, 250);
			DrawPolyline(points, white, 1, true);
		}
	}
}