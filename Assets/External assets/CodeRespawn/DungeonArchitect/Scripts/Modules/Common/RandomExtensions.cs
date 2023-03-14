//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

/// <summary>
/// Extends System.Random with gamedev based utility functions
/// </summary>
public static class RandomExtensions {
	
	public static float NextFloat(this System.Random random)
	{
		return (float)random.NextDouble();
	}

	public static Vector3 OnUnitSphere(this System.Random random) {
		var z = (float)random.NextDouble() * 2 - 1;
		var rxy = Mathf.Sqrt(1 - z*z);
		var phi = (float)random.NextDouble() * 2 * Mathf.PI;
		var x = rxy * Mathf.Cos(phi);
		var y = rxy * Mathf.Sin(phi);
		return new Vector3(x, y, z);
	}
	
	public static float Range(this System.Random random, float a, float b) {
		return a + NextFloat(random) * (b - a);
	}

    /// <summary>
    /// Random number between &gt= a and &lt= b
    /// </summary>
    /// <param name="random"></param>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
	public static int Range(this System.Random random, int a, int b) {
		return Mathf.RoundToInt(a + NextFloat(random) * (b - a));
	}

	public static float value(this System.Random random) {
		return NextFloat(random);
	}
}
