using UnityEngine;

public class BulletTrails {

	private Texture2D texture;
	private S sprite;
	private int width;
	private int height;

	private bool[,] activePixels;

	public BulletTrails ( ) {

		width = G.I.level.width * S.SIZE;
		height = G.I.level.height * S.SIZE;

		texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
		texture.filterMode = FilterMode.Point;

		activePixels = new bool[width, height];

		sprite = G.I.NewSprite(null, 0);
		sprite.renderer.sprite = Sprite.Create(texture, new Rect(0, 0, width, height), Vector2.zero, 1);
		sprite.depthOffset = 1000;
		sprite.transform.name = "Bullet Trails";

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				texture.SetPixel(x, y, Color.clear);
			}
		}

		texture.Apply();

	}

	public void Decay ( ) {
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (activePixels[x, y]) {
					if (Random.Range(0, 4) == 1) {
						texture.SetPixel(x, y, Color.clear);
						activePixels[x, y] = false;
					}
				}
			}
		}
		texture.Apply();
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
					setPixel(x, y, Color.yellow);
					error += dError;
					while (error >= 0.5f) {
						setPixel(x, y, Color.yellow);
						y += Mathf.RoundToInt(Mathf.Sign(dY));
						error -= 1f;
					}
				}
			} else {
				for (; x > endX; x--) {
					setPixel(x, y, Color.yellow);
					error += dError;
					while (error >= 0.5f) {
						setPixel(x, y, Color.yellow);
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
				setPixel(x, y, Color.yellow);
			}
		}
		texture.Apply();
	}

	private void setPixel (int x, int y, Color color) {
		if (x >= 0 && y >= 0 && x < width && y < height) {
			texture.SetPixel(x, y, color);
			activePixels[x, y] = true;
		}
	}

}
