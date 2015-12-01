
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

	public static Vector2 RandomVec ( ) {
		var vec = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		vec.Normalize();
        return vec;
	}

	public static Vector2 RandomVec (Vector2 min, Vector2 max) {
		var vec = new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
		return vec;
	}

	public static Vector3 RandomVec (Vector3 min, Vector3 max) {
		var vec = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
		return vec;
	}

	public static float RandomTo (float value) {
		return Random.Range(0, value);
	}

	public static float RandomRange (float value) {
		return Random.Range(-value, value);
	}

	public static Sprite[] SliceSprite (Sprite s, int frames) {
		var arr = new Sprite[frames];
		var frameWidth = s.texture.width / frames;
		var frameHeight = s.texture.height;
		for (int i = 0; i < frames; i++) {
			arr[i] = Sprite.Create(s.texture, new Rect(frameWidth * i, 0, frameWidth, frameHeight), new Vector2(0.5f, 0.5f), 1);
		}
		return arr;
	}

	public static float Step (float input, float steps) {
		return Mathf.FloorToInt(input * steps) / steps;
	}
}