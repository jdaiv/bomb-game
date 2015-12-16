
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

public struct V2 {
	public int x;
	public int y;

	public V2 (int x, int y) {
		this.x = x;
		this.y = y;
	}
}

public class FrameTimer {
	public int frames;
	public int count;
	public bool running;
	public bool done;
	public bool repeat;

	public FrameTimer (int frames, bool repeat = false) {
		this.frames = frames;
		count = 0;
		running = false;
		done = false;
		this.repeat = repeat;
	}

	public void Start () {
		running = true;
	}

	public void Pause () {
		running = false;
	}

	public void Stop () {
		running = false;
		Reset();
	}

	public void Reset () {
		count = 0;
		done = false;
	}

	public void Tick () {
		if (done) {
			Reset();
		}
		if (running) {
			count++;
			if (count >= frames) {
				done = true;
				if (!repeat) {
					running = false;
				}
			}
		}
	}

	public static implicit operator bool (FrameTimer t) {
		return t.done;
	}
}

public static class C32 {

	public static Color32 Clear = new Color32(0, 0, 0, 0);
	public static Color32 Red = new Color32(255, 0, 0, 255);
	public static Color32 Green = new Color32(0, 255, 0, 255);
	public static Color32 Blue = new Color32(0, 0, 255, 255);
	public static Color32 Black = new Color32(0, 0, 0, 255);
	public static Color32 White = new Color32(255, 255, 255, 255);

	public static bool Eq (Color32 a, Color32 b) {
		return 
			a.r == b.r &&
			a.g == b.g &&
			a.b == b.b &&
			a.a == b.a;
	}

}