using UnityEngine;
using System.Collections;

public class Level {

	public const int COLLIDER_RES = 2;
	public const int COLLIDER_THRESHOLD = 2;
	public const int WALL_HEIGHT = 8;

	public int width;
	public int height;

	private Texture2D floor;
	private Texture2D[] walls;
	private GameObject root;
	private GameObject[,] colliderObjects;
	private BoxCollider2D[,] colliders;
	private bool[] colliderExists;
	private bool[] colliderExistsLast;
	private bool[,] collisionMask;
	private bool[,] explosionMask;
	private bool[,] wallMask;

	private S[] sprites;

	#region level data

	public Entity[] entities;
	public Vector2[] spawnLocations;

	#endregion

	public void Generate ( ) {
		OgmoLoader.Load();
		var levelData = OgmoLoader.levels[Random.Range(0, OgmoLoader.levels.Count)];
		width = levelData.width;
		height = levelData.height;

		floor = new Texture2D(width * S.SIZE, height * S.SIZE, TextureFormat.RGBA32, false);
		floor.filterMode = FilterMode.Point;

		walls = new Texture2D[height];
		for (int i = 0; i < height; i++) {
			walls[i] = new Texture2D(width * S.SIZE, S.SIZE, TextureFormat.RGBA32, false);
			walls[i].filterMode = FilterMode.Point;
		}

		var wallTex = G.I.sprites[0].texture;
		var floorTex = G.I.sprites[1].texture;

		root = new GameObject("Level Collision");

		if (colliderObjects != null) {
			foreach (var c in colliderObjects) {
				Object.Destroy(c);
			}
		}

		colliderObjects = new GameObject[width, height];
		colliders = new BoxCollider2D[width * COLLIDER_RES, height * COLLIDER_RES];
		colliderExists = new bool[width * COLLIDER_RES * height * COLLIDER_RES];
		colliderExistsLast = new bool[width * COLLIDER_RES * height * COLLIDER_RES];
		collisionMask = new bool[width * S.SIZE, height * S.SIZE];
		explosionMask = new bool[width * S.SIZE, height * S.SIZE];
		wallMask = new bool[width * S.SIZE, height * S.SIZE];

		if (sprites != null) {
			foreach (var s in sprites) {
				G.I.DeleteSprite(s);
			}
		}
		sprites = new S[1 + height * WALL_HEIGHT];

		var sprite = G.I.NewSprite(null, 0);
		sprite.renderer.sprite = Sprite.Create(floor, new Rect(0, 0, floor.width, floor.height), Vector2.zero, 1);
		sprite.depthOffset = -9999;
		sprites[0] = sprite;

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
				var tile = levelData.tiles[y * levelData.width + x];

				if (tile > 0) {
					colliderObjects[x, y] = new GameObject(x + ", " + y);
					colliderObjects[x, y].transform.SetParent(root.transform);
					copyTexture(floorTex, false, (x) * S.SIZE, (y) * S.SIZE);
					if (tile == 2) {
						copyTexture(wallTex, true, (x) * S.SIZE, (y) * S.SIZE);
						for (int _x = 0; _x < S.SIZE; _x++) {
							for (int _y = 0; _y < S.SIZE; _y++) {
								collisionMask[x * S.SIZE + _x, y * S.SIZE + _y] = true;
							}
						}
					}
				}
			}
			//apply();
			//yield return new WaitForEndOfFrame();
		}

		entities = new Entity[levelData.entities.Length];
		spawnLocations = new Vector2[4];

		var spawnCount = 0;

		for (int i = 0; i < entities.Length; i++) {
			var ent = levelData.entities[i];
			Debug.Log("[Level] Spawning Entity Type: " + ent.type);
			Entity newEntity;
			switch (ent.type) {
				case "WeaponSpawner":
					switch (Random.Range(0, 3)) {
						case 2:
							newEntity = G.I.CreateEntity<GolfClub>("Item");
							break;
						case 1:
							newEntity = G.I.CreateEntity<Rifle>("Item");
							break;
						default:
						case 0:
							newEntity = G.I.CreateEntity<Pistol>("Item");
							break;
					}
					break;
				case "Teleporter":
					newEntity = G.I.CreateEntity<Teleporter>("Teleporter");
					(newEntity as Teleporter).target = int.Parse(ent.data["Target"]);
					break;
				case "PlayerSpawn":
					newEntity = G.I.CreateEntity<PlayerSpawn>("Player Spawn " + spawnCount);
					spawnLocations[spawnCount] = ent.position;
					spawnCount++;
					break;
				case "barrel":
				default:
					newEntity = G.I.CreateEntity<Barrel>("Barrel");
					break;
			}
			newEntity.transform.position = ent.position;
			entities[i] = newEntity;
		}

		Update();

		for (int i = 0; i < height; i++) { // depth
			var tex = walls[i];
			for (int j = 0; j < WALL_HEIGHT; j++) { // y pos
				var wallSprite = G.I.NewSprite(null, 0);
				wallSprite.renderer.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero, 1);
				var brightness = (float)j / WALL_HEIGHT;
				wallSprite.renderer.color = new Color(brightness, brightness, brightness, 1);
				wallSprite.transform.Translate(0, i * S.SIZE + j, 0);
				wallSprite.depthOffset = j * 2;
				wallSprite.transform.name = "Wall " + i + ", " + j;
				sprites[i * WALL_HEIGHT + j + 1] = wallSprite;
				//yield return new WaitForEndOfFrame();
			}
		}

		//yield return new WaitForEndOfFrame();
	}

	public void Update ( ) {
		UpdateColliders();
		apply();
	}

	private void apply ( ) {
		floor.Apply();
		foreach (var tex in walls) {
			tex.Apply();
		}
	}

	private void setWallPixel (int x, int y, Color color) {
		var index = y / S.SIZE;
		var _y = y - (index * S.SIZE);
		walls[index].SetPixel(x, _y, color);
	}

	private Color getWallPixel (int x, int y) {
		var index = y / S.SIZE;
		var _y = y - (index * S.SIZE);
		return walls[index].GetPixel(x, _y);
	}

	public void UpdateColliders ( ) {

		colliderExists.CopyTo(colliderExistsLast, 0);

		var res = S.SIZE / COLLIDER_RES;
		var colliderSize = 1f / COLLIDER_RES;
		var colliderOffset = 0.5f * colliderSize;

		var _width = width * COLLIDER_RES;
		var _height = height * COLLIDER_RES;

		for (int x = 0; x < _width; x++) {
			for (int y = 0; y < _height; y++) {

				int count = 0;
				for (int _x = 0; _x < res; _x++) {
					for (int _y = 0; _y < res; _y++) {
						count += collisionMask[x * res + _x, y * res + _y] ? 1 : 0;
					}
				}

				var collision = count > COLLIDER_THRESHOLD;
				var cX = x % _width;
				var cY = y * _width;
				colliderExists[cX + cY] = collision;
			}
		}

		for (int x = 0; x < width * COLLIDER_RES; x++) {
			for (int y = 0; y < height * COLLIDER_RES; y++) {

				var cX = x % _width;
				var cY = y * _width;
				var i = cX + cY;

				if (colliderExists[i] != colliderExistsLast[i]) {
					if (colliders[x, y] == null) {
						if (colliderExists[i]) {
							var obj = colliderObjects[x / COLLIDER_RES, y / COLLIDER_RES];
							var newCollider = obj.AddComponent<BoxCollider2D>();
							newCollider.offset = new Vector2(x * colliderSize + colliderOffset, y * colliderSize + colliderOffset);
							newCollider.size = new Vector2(colliderSize, colliderSize);
							colliders[x, y] = newCollider;
						}
					} else {
						if (!colliderExists[i]) {
							Object.Destroy(colliders[x, y]);
							for (int _x = 0; _x < res; _x++) {
								for (int _y = 0; _y < res; _y++) {
									var __x = x * res + _x;
									var __y = y * res + _y;
									if (collisionMask[__x, __y]) {
										collisionMask[__x, __y] = false;
										setWallPixel(__x, __y, Color.clear);
									} // aahh
								} // aaahhhh
							} // aaaahhhhh...
						}
					}
				}

			}
		} // down we go

	}

	public void Explosion (Vector2 v, int radius, bool burn = true) {
		Explosion(v.x, v.y, radius, burn);
	}

	public void Explosion (float _x, float _y, int radius, bool burn = true) {
		var __x = Mathf.RoundToInt(_x * S.SIZE);
		var __y = Mathf.RoundToInt(_y * S.SIZE);
		var center = new Vector2(__x, __y);

		var fullRadius = Mathf.FloorToInt(radius * 1.5f);

		for (int x = __x - fullRadius; x < __x + fullRadius; x++) {
			for (int y = __y - fullRadius; y < __y + fullRadius; y++) {
				if (x >= 4 && y >= 4 && x < floor.width - 4 && y < floor.height - 4) {
					var dist = Vector2.Distance(new Vector2(x, y), center);
					if (dist < fullRadius) {
						var intensity = Mathf.Pow(1 - ((fullRadius - dist) / fullRadius), 2);

						if (burn) {
							var color = floor.GetPixel(x, y) * U.Step(intensity, 5);
							color.a = 1;
							floor.SetPixel(x, y, color);
						}

						if (dist < radius) {
							if (wallMask[x, y]) {
								setWallPixel(x, y, Color.clear);
								wallMask[x, y] = false;
							}
							collisionMask[x, y] = false;
						} else {
							if (wallMask[x, y]) {
								var color = getWallPixel(x, y) * U.Step(intensity, 10);
								color.a = 1;
								setWallPixel(x, y, color);
							}
						}
					}
				}
			}
		}
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
					if (walls) {
						setWallPixel(_x, _y, pixel);
						wallMask[_x, _y] = true;
					} else {
						dest.SetPixel(_x, _y, pixel);
					}
				}
				__y++;
			}
			__x++;
		}
	}

}
