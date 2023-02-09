//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;
using SharpNav.Geometry;

namespace DungeonArchitect.Navigation {
	public class NavigationTriangleProvider : MonoBehaviour {

		public virtual void AddNavTriangles(List<Triangle3> triangles) {
			// Implementations should override and implement this function
		}

	}
}
