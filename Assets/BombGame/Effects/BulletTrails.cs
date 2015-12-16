using System;
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

	private bool[] active;

	public BulletTrails ( ) {

		_width = G.I.level.width;
		_height = G.I.level.height;
		width = _width * S.SIZE;
		height = _height * S.SIZE;


		length = _height;
		_length = S.SIZE * width;
		textures = new Texture2D[length];
		sprites = new S[length];
		colors = new Color32[length][];
		active = new bool[length];
		dirty = new bool[length];

		for (int y = 0; y < _height; y++) {
			var i = y;
			var texture = new Texture2D(width, S.SIZE, TextureFormat.ARGB32, false);
			texture.filterMode = FilterMode.Point;
			textures[i] = texture;
			var sprite = G.I.NewSprite(null, 0);
			sprite.renderer.sprite = Sprite.Create(texture, new Rect(0, 0, width, S.SIZE), Vector2.zero, 1);
			sprite.depthOffset = 1000;
			sprite.transform.name = "Bullet Trails " + i;
			sprite.transform.position = new Vector3(0, y * S.SIZE);
			sprites[i] = sprite;
			colors[i] = new Color32[_length];
		}

		for (int y = 0; y < _height; y++) {
			var c = colors[y];
			for (int i = 0; i < _length; i++) {
				c[i] = C32.Clear;
			}
			dirty[y] = true;
		}

		apply();

	}

	public void Clear ( ) {
		for (int y = 0; y < _height; y++) {
			if (active[y]) {
				var c = colors[y];
				for (int i = 0; i < _length; i++) {
					c[i] = C32.Clear;
				}
				dirty[y] = true;
			}
		}
		apply();
	}

	public void Decay ( ) {
		for (int y = 0; y < _height; y++) {
			if (active[y]) {
				var c = colors[y];
				int count = 0;
				for (int i = 0; i < _length; i++) {
					if (c[i].a > 0) {
						if (c[i].a > 10) {
							c[i].a -= 10;
							count++;
						} else {
							c[i].a = 0;
						}
					}
				}
				if (count <= 0) {
					active[y] = false;
				}
				dirty[y] = true;
			}
		}
		apply();
	}

	private void swap (ref int a, ref int b) {
		var temp = a;
		a = b;
		b = temp;
	}

	public void AddTrail (int x0, int y0, int x1, int y1) {

		if (x0 == x1) {

			if (y1 < y0) {
				swap(ref y0, ref y1);
			}

			for (var y = y0; y <= y1; y++) {
				setPixel(x0, y, C32.White);
			}

		} else if (y0 == y1) {

			if (x1 < x0) {
				swap(ref x0, ref x1);
			}

			for (var x = x0; x <= x1; x++) {
				setPixel(x, y0, C32.White);
			}

		} else {

			var dx = Math.Abs(x1 - x0);
			var dy = Math.Abs(y1 - y0);
			var steep = dy > dx;

			if (steep) {
				swap(ref x0, ref y0);
				swap(ref x1, ref y1);
			}

			if (x0 > x1) {
				swap(ref x0, ref x1);
				swap(ref y0, ref y1);
			}

			dx = x1 - x0;
			dy = Math.Abs(y1 - y0);

			var err = dx / 2;
			var step = (y0 < y1 ? 1 : -1);

			var y = y0;

			for (var x = x0; x <= x1; x++) {
				if (!steep) {
					setPixel(x, y, C32.White);
				} else {
					setPixel(y, x, C32.White);
				}
				err -= dy;
				if (err < 0) {
					y += step;
					err += dx;
				}
			}

		}

	}

	public void AddTrail (Vector2 start, Vector2 end) {
		start *= S.SIZE;
		end *= S.SIZE;
		AddTrail(Mathf.FloorToInt(start.x), Mathf.FloorToInt(start.y),
			Mathf.FloorToInt(end.x), Mathf.FloorToInt(end.y));
	}

	private void setPixel (int x, int y, Color32 color) {
		if (x >= 0 && y >= 0 && x < width && y < height) {
			var _y = y / S.SIZE;
			var _i = x + ((y - (_y * S.SIZE)) * width);
			colors[_y][_i] = color;
			dirty[_y] = true;
			active[_y] = true;
		}
	}

	private void apply ( ) {
		for (int i = 0; i < length; i++) {
			if (dirty[i]) {
				textures[i].SetPixels32(colors[i]);
				textures[i].Apply();
				dirty[i] = false;
			}
		}
	}

}
