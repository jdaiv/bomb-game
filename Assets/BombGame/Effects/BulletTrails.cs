using UnityEngine;

public class BulletTrails {

	private Texture2D[] textures;
	private S[] sprites;
	private int width;
	private int height;

	private bool[,] activePixels;

	public BulletTrails ( ) {

		width = G.I.level.width * S.SIZE;
		height = G.I.level.height * S.SIZE;

		activePixels = new bool[width, height];

		textures = new Texture2D[G.I.level.height];
		sprites = new S[G.I.level.height];

		for (int i = 0; i < G.I.level.height; i++) {
			var texture = new Texture2D(width, S.SIZE, TextureFormat.ARGB32, false);
			texture.filterMode = FilterMode.Point;
			textures[i] = texture;
			var sprite = G.I.NewSprite(null, 0);
			sprite.renderer.sprite = Sprite.Create(texture, new Rect(0, 0, width, S.SIZE), Vector2.zero, 1);
			sprite.depthOffset = 0;
			sprite.transform.name = "Bullet Trails " + i;
			sprite.transform.position = new Vector3(0, i * S.SIZE);
			sprites[i] = sprite;
		}

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				clearPixel(x, y);
			}
		}

		apply();

	}

	public void Decay ( ) {
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (activePixels[x, y]) {
					if (Random.Range(0, 4) == 1) {
						clearPixel(x, y);
						activePixels[x, y] = false;
					}
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
					setPixel(x, y, Color.white);
					error += dError;
					while (error >= 0.5f) {
						setPixel(x, y, Color.white);
						y += Mathf.RoundToInt(Mathf.Sign(dY));
						error -= 1f;
					}
				}
			} else {
				for (; x > endX; x--) {
					setPixel(x, y, Color.white);
					error += dError;
					while (error >= 0.5f) {
						setPixel(x, y, Color.white);
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
				setPixel(x, y, Color.white);
			}
		}
	}

	private void clearPixel (int x, int y) {
		var index = y / S.SIZE;
		textures[index].SetPixel(x, y, Color.clear);
		activePixels[x, y] = false;
	}

	private void setPixel (int x, int y, Color color) {
		if (x >= 0 && y >= 0 && x < width && y < height) {
			var index = y / S.SIZE;
			textures[index].SetPixel(x, y, color);
			activePixels[x, y] = true;
		}
	}

	private void apply () {
		foreach (var t in textures) {
			t.Apply();
		}
	}

}
