//https://www.habrador.com/tutorials/unity-dubins-paths/3-dubins-paths-in-unity/
//Script 5 - A class that will hold one Dubins path
//This final script will hold everything we need when we have generated a Dubins path.

using Godot;
using System;
using System.Collections.Generic;

namespace Trains
{
	public class OneDubinsPath
	{
		public float totalLength;

		//Need the individual path lengths for debugging and to find the final path
		public float length1, length2, length3;

		//The 2 tangent points we need to connect the lines and curves
		public Vector3 tangent1, tangent2;
		public PathType pathType;

		//The coordinates of the final path
		public List<Vector3> pathCoordinates;

		//To simplify when we generate the final path coordinates
		//Are we turning or driving straight in segment 2?
		public bool segment2Turning;

		//Are we turning right in the particular segment?
		public bool segment1TurningRight, segment2TurningRight, segment3TurningRight;

		public OneDubinsPath(float length1, float length2, float length3, Vector3 tangent1, Vector3 tangent2, PathType pathType)
		{
			this.totalLength = length1 + length2 + length3;

			this.length1 = length1;
			this.length2 = length2;
			this.length3 = length3;

			this.tangent1 = tangent1;
			this.tangent2 = tangent2;

			this.pathType = pathType;
		}

		//Are we turning right in any of the segments?
		public void SetIfTurningRight(bool segment1TurningRight, bool segment2TurningRight, bool segment3TurningRight)
		{
			this.segment1TurningRight = segment1TurningRight;
			this.segment2TurningRight = segment2TurningRight;
			this.segment3TurningRight = segment3TurningRight;
		}
	}
}