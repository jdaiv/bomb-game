
using UnityEngine;

public static class U {
	public static Vector2 RandomCoords (float x0, float x1, float y0, float y1) {
		return new Vector2(
			Random.Range(x0, x1),
			Random.Range(y0, y1)
            );
	}

	public static bool RandomBool () {
		return Random.Range(0, 2) == 1;
	}

	public static float RandomTo (float value) {
		return Random.Range(0, value);
	}
}