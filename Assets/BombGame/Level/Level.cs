using UnityEngine;
using System.Collections;

public class Level {

	public const int COLLIDER_RES = 4;
	public const int COLLIDER_THRESHOLD = 4;
	public const int WALL_HEIGHT = 8;

	public int width = 40;
	public int height = 20;

	private Texture2D floor;
	private Texture2D[][] walls;
	private GameObject root;
	private GameObject[,] colliderObjects;
	private BoxCollider2D[,] colliders;
	private bool[,] collisionMask;
	private bool[,] explosionMask;
	private bool[,] fireMask;

	public IEnumerator Generate ( ) {
		floor = new Texture2D(width * S.SIZE, height * S.SIZE, TextureFormat.RGBA32, false);
		floor.filterMode = FilterMode.Point;

		walls = new Texture2D[WALL_HEIGHT][];
		for (int i = 0; i < WALL_HEIGHT; i++) {
			walls[i] = new Texture2D[height];

			for (int j = 0; j < height; j++) {
				walls[i][j] = new Texture2D(width * S.SIZE, S.SIZE, TextureFormat.RGBA32, false);
				walls[i][j].filterMode = FilterMode.Point;
			}
		}

		var wallTex = G.I.sprites[0].texture;
		var floorTex = G.I.sprites[1].texture;

		root = new GameObject("Level Collision");
		colliderObjects = new GameObject[width, height];
		colliders = new BoxCollider2D[width * COLLIDER_RES, height * COLLIDER_RES];
		collisionMask = new bool[width * S.SIZE, height * S.SIZE];
		explosionMask = new bool[width * S.SIZE, height * S.SIZE];
		fireMask = new bool[width * S.SIZE, height * S.SIZE];

		var sprite = G.I.NewSprite(null, 0);
		sprite.renderer.sprite = Sprite.Create(floor, new Rect(0, 0, floor.width, floor.height), Vector2.zero, 1);
		sprite.depthOffset = -9999;

		for (int x = 0; x < floor.width; x++) {
			for (int y = 0; y < floor.height; y++) {
				floor.SetPixel(x, y, Color.clear);
				setWallPixel(x, y, Color.clear);
				collisionMask[x, y] = false;
				explosionMask[x, y] = false;
			}
		}

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				colliderObjects[x, y] = new GameObject(x + ", " + y);
				colliderObjects[x, y].transform.SetParent(root.transform);
				copyTexture(floorTex, false, (x) * S.SIZE, (y) * S.SIZE);
				if (Random.Range(0, 2) == 1) {
					copyTexture(wallTex, true, (x) * S.SIZE, (y) * S.SIZE);
					for (int _x = 0; _x < S.SIZE; _x++) {
						for (int _y = 0; _y < S.SIZE; _y++) {
							collisionMask[x * S.SIZE + _x, y * S.SIZE + _y] = true;
						}
					}
				} else if (Random.Range(0, 5) == 1) {
					var barrel = G.I.CreateEntity<Barrel>("Barrel");
					barrel.transform.position = new Vector3(x + 0.5f, y + 0.5f);
				}
			}
			//apply();
			//yield return new WaitForEndOfFrame();
		}

		UpdateColliders();
		apply();

		for (int i = 0; i < WALL_HEIGHT; i++) { // depth
			for (int j = 0; j < height; j++) { // y pos
				var tex = walls[i][j];
				var wallSprite = G.I.NewSprite(null, 0);
				wallSprite.renderer.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero, 1);
				wallSprite.transform.Translate(0, j * S.SIZE + i, 0);
				wallSprite.depthOffset = i * 2;
				wallSprite.transform.name = "Wall " + i + ", " + j;
				//yield return new WaitForEndOfFrame();
			}
		}

		yield return new WaitForEndOfFrame();
	}

	private void apply ( ) {
		floor.Apply();
		foreach (var layer in walls) {
			foreach (var tex in layer) {
				tex.Apply();
			}
		}
	}

	private void setWallPixel (int x, int y, Color color) {
		var index = y / S.SIZE;
		var _y = y - (index * S.SIZE);
		var count = 0f;
		foreach (var layer in walls) {
			var _color = color * (count / WALL_HEIGHT);
			_color.a = color.a;
			layer[index].SetPixel(x, _y, _color);
			count++;
		}
	}

	public void UpdateColliders ( ) {

		var res = S.SIZE / COLLIDER_RES;
		var colliderSize = 1f / COLLIDER_RES;
		var colliderOffset = 0.5f * colliderSize;

		for (int x = 0; x < width * COLLIDER_RES; x++) {
			for (int y = 0; y < height * COLLIDER_RES; y++) {

				int count = 0;
				for (int _x = 0; _x < res; _x++) {
					for (int _y = 0; _y < res; _y++) {
						count += collisionMask[x * res + _x, y * res + _y] ? 1 : 0;
					}
				}

				var collision = count > COLLIDER_THRESHOLD;
				if (colliders[x, y] == null) {
					if (collision) {
						var obj = colliderObjects[x / COLLIDER_RES, y / COLLIDER_RES];
						var newCollider = obj.AddComponent<BoxCollider2D>();
						newCollider.offset = new Vector2(x * colliderSize + colliderOffset, y * colliderSize + colliderOffset);
						newCollider.size = new Vector2(colliderSize, colliderSize);
						colliders[x, y] = newCollider;
					}
				} else {
					if (!collision) {
						Object.Destroy(colliders[x, y]);
						for (int _x = 0; _x < res; _x++) {
							for (int _y = 0; _y < res; _y++) {
								var __x = x * res + _x;
								var __y = y * res + _y;
								if (collisionMask[__x, __y]) {
									collisionMask[__x, __y] = false;
									floor.SetPixel(__x, __y, Color.gray);
									setWallPixel(__x, __y, Color.clear);
								}
							}
						}
					}
				}

			}
		}

		apply();

	}

	public void Explosion (Vector2 v, int radius) {
		Explosion(v.x, v.y, radius);
	}

	public void Explosion (float _x, float _y, int radius) {
		var __x = Mathf.RoundToInt(_x * S.SIZE);
		var __y = Mathf.RoundToInt(_y * S.SIZE);
		var center = new Vector2(__x, __y);
		for (int x = __x - radius; x < __x + radius; x++) {
			for (int y = __y - radius; y < __y + radius; y++) {
				if (x >= 0 && y >= 0 && x < floor.width && y < floor.height) {
					var dist = Vector2.Distance(new Vector2(x, y), center);
					if (dist < radius) {
						var gray = Mathf.FloorToInt((dist / radius / 6 + 0.05f) * 16) / 16f;
						var color = new Color(gray, gray, gray);
						if (explosionMask[x, y]) {
							var target = floor.GetPixel(x, y).r;
							if (target > gray) {
								floor.SetPixel(x, y, color);
								setWallPixel(x, y, Color.clear);
							}
						} else {
							floor.SetPixel(x, y, color);
							setWallPixel(x, y, Color.clear);
						}
						collisionMask[x, y] = false;
						explosionMask[x, y] = true;
					}
				}
			}
		}
		UpdateColliders();
		apply();
	}

	// Copy Texture With Alpha
	private void copyTexture (Texture2D src, bool walls, int x, int y) {
		var dest = floor;
		var xMax = x + src.width;
		if (xMax > dest.width) {
			xMax = dest.width;
		}
		var yMax = y + src.height;
		if (yMax > dest.height) {
			yMax = dest.height;
		}

		var srcStartX = 0;
		var srcStartY = 0;

		if (x < 0) {
			srcStartX = x * -1;
			x = 0;
		}

		if (y < 0) {
			srcStartY = y * -1;
			y = 0;
		}

		int __x = srcStartX;
		for (int _x = x; _x < xMax; _x++) {
			int __y = srcStartY;
			for (int _y = y; _y < yMax; _y++) {
				var pixel = src.GetPixel(__x, __y);
				if (pixel.a == 1) {
					dest.SetPixel(_x, _y, pixel);
					if (walls)
						setWallPixel(_x, _y, pixel);
				}
				__y++;
			}
			__x++;
		}
	}

}
