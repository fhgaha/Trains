using Godot;
using System.Collections.Generic;

namespace Trains.Model.Common
{
	public class Vector3IgnoreYComparer : IEqualityComparer<Vector3>
	{
		public bool Equals(Vector3 v1, Vector3 v2)
		{
			//ignore y
			return Mathf.IsEqualApprox(v1.x, v2.x) && Mathf.IsEqualApprox(v1.z, v2.z);
		}

		public int GetHashCode(Vector3 v)
		{
			return 0;
		}
	}
}
