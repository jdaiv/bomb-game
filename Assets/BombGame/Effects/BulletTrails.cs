using UnityEngine;

public class BulletTrails {

	private Texture2D[] textures;
	private S[] sprites;
	private bool[] dirty;
	private Color32[][] colors;
	private int width;
	private int _width;
	private int height;
	private int _height;
	private int length;
	private int _length;

	private bool[,] active;

	public BulletTrails ( ) {

		_width = G.I.level.width;
		_height = G.I.level.height;
		width = _width * S.SIZE;
		height = _height * S.SIZE;

		active = new bool[width, height];

		length = _width * _height;
		_length = S.SIZE * S.SIZE;
		textures = new Texture2D[length];
		sprites = new S[length];
		colors = new Color32[length][];
		dirty = new bool[length];

		for (int x = 0; x < _width; x++) {
			for (int y = 0; y < _height; y++) {
				var i = x + y * _width;
				var texture = new Texture2D(S.SIZE, S.SIZE, TextureFormat.ARGB32, false);
				texture.filterMode = FilterMode.Point;
				textures[i] = texture;
				var sprite = G.I.NewSprite(null, 0);
				sprite.renderer.sprite = Sprite.Create(texture, new Rect(0, 0, S.SIZE, S.SIZE), Vector2.zero, 1);
				sprite.depthOffset = 0;
				sprite.transform.name = "Bullet Trails " + i;
				sprite.transform.position = new Vector3(x * S.SIZE, y * S.SIZE);
				sprites[i] = sprite;
				colors[i] = new Color32[_length];
			}
		}

		for (int x = 0; x < _width; x++) {
			for (int y = 0; y < _height; y++) {
				var c = getArray(x, y);
				for (int i = 0; i < _length; i++) {
					c[i] = new Color32();
				}
				dirty[x + y * _width] = true;
			}
		}

		apply();

	}

	public void Clear ( ) {
		for (int x = 0; x < _width; x++) {
			for (int y = 0; y < _height; y++) {
				if (active[x, y]) {
					var colors = getArray(x, y);
					for (int i = 0; i < _length; i++) {
						colors[i] = C32.Clear;
					}
					dirty[x + y * _width] = true;
				}
			}
		}
		apply();
	}

	public void Decay ( ) {
		for (int x = 0; x < _width; x++) {
			for (int y = 0; y < _height; y++) {
				if (active[x, y]) {
					var colors = getArray(x, y);
					int count = 0;
					for (int i = 0; i < _length; i++) {
						if (colors[i].a > 0) {
							colors[i].a <<= 1;
							count++;
						}
					}
					if (count <= 0) {
						active[x, y] = false;
					}
					dirty[x + y * _width] = true;
				}
			}
		}
		apply();
	}

	public void AddTrail (Vector2 start, Vector2 end) {
		start *= S.SIZE;
		end *= S.SIZE;
		var dX = end.x - start.x;
		if (dX != 0) {
			var dY = end.y - start.y;
			var error = 0f;
			var dError = Mathf.Abs(dY / dX);
			var y = Mathf.RoundToInt(start.y);

			int x = Mathf.RoundToInt(start.x);
			int endX = Mathf.RoundToInt(end.x);
			if (start.x < end.x) {
				for (; x < endX; x++) {
					setPixel(x, y, C32.White);
					error += dError;
					while (error >= 0.5f) {
						setPixel(x, y, C32.White);
						y += Mathf.RoundToInt(Mathf.Sign(dY));
						error -= 1f;
					}
				}
			} else {
				for (; x > endX; x--) {
					setPixel(x, y, C32.White);
					error += dError;
					while (error >= 0.5f) {
						setPixel(x, y, C32.White);
						y += Mathf.RoundToInt(Mathf.Sign(dY));
						error -= 1f;
					}
				}
			}
		} else {
			int startY;
			int endY;
			if (start.y < end.y) {
				startY = Mathf.RoundToInt(start.y);
				endY = Mathf.RoundToInt(end.y);
			} else {
				startY = Mathf.RoundToInt(end.y);
				endY = Mathf.RoundToInt(start.y);
			}
			var x = Mathf.RoundToInt(start.x);
			for (int y = startY; y < endY; y++) {
				setPixel(x, y, C32.White);
			}
		}
	}

	private void setPixel (int x, int y, Color32 color) {
		if (x >= 0 && y >= 0 && x < width && y < height) {
			var _x = x / S.SIZE;
			var _y = y / S.SIZE;
			var i = _x + _y * _width;
			var __x = x - _x * S.SIZE;
			var __y = y - _y * S.SIZE;
			var _i = __x + __y * S.SIZE;
			colors[i][_i] = color;
			dirty[i] = true;
			active[_x, _y] = true;
		}
	}

	private Color32[] getArray (int x, int y) {
		if (x >= 0 && y >= 0 && x < _width && y < _height) {
			return colors[x + y * _width];
		} else {
			return null;
		}
	}

	private void apply ( ) {
		int dirtyCount = 0;
		for (int i = 0; i < length; i++) {
			if (dirty[i]) {
				textures[i].SetPixels32(colors[i]);
				textures[i].Apply();
				dirty[i] = false;
				dirtyCount++;
			}
		}
		Debug.Log(dirtyCount);
	}

}
