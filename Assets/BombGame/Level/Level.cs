using UnityEngine;
using System.Collections;

public class Level {
	
	public const int WALL_HEIGHT = 8;

	public int width;
	public int height;

	private Texture2D floor;
	private Texture2D[] walls;
	private bool[] wallDirty;
	private GameObject root;
	private LevelCollider[,] colliders;
	private bool[,] colliderActive;
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
		wallDirty = new bool[height];
		for (int i = 0; i < height; i++) {
			walls[i] = new Texture2D(width * S.SIZE, S.SIZE, TextureFormat.RGBA32, false);
			walls[i].filterMode = FilterMode.Point;
		}

		var wallTex = G.I.sprites[0].texture;
		var floorTex = G.I.sprites[1].texture;

		root = new GameObject("Level Collision");

		if (colliders != null) {
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					if (colliderActive[x, y]) {
						colliders[x, y].Destroy();
					}
				}
			}
		}

		colliders = new LevelCollider[width, height];
		colliderActive = new bool[width, height];
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
				explosionMask[x, y] = false;
			}
		}

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				var tile = levelData.tiles[y * levelData.width + x];

				if (tile > 0) {
					copyTexture(floorTex, false, (x) * S.SIZE, (y) * S.SIZE);
					if (tile == 2) {
						copyTexture(wallTex, true, (x) * S.SIZE, (y) * S.SIZE);
						colliderActive[x, y] = true;
						colliders[x, y] = new LevelCollider(x, y, root.transform);
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
					newEntity = G.I.CreateEntity<WeaponSpawner>("Weapon Spawner");
					(newEntity as WeaponSpawner).type = (WeaponSpawner.TYPE)System.Enum.Parse(typeof(WeaponSpawner.TYPE), ent.data["Class"].ToString());
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
		for (int i = 0; i < height; i++) {
			if (wallDirty[i]) {
				walls[i].Apply();
				wallDirty[i] = false;
			}
		}
	}

	private void setWallPixel (int x, int y, Color color) {
		var index = y / S.SIZE;
		var _y = y - (index * S.SIZE);
		walls[index].SetPixel(x, _y, color);
		wallDirty[index] = true;
	}

	private Color getWallPixel (int x, int y) {
		var index = y / S.SIZE;
		var _y = y - (index * S.SIZE);
		return walls[index].GetPixel(x, _y);
	}

	private void clearCollision (int x, int y) {
		var ix = x / S.SIZE;
		var iy = y / S.SIZE;
		if (colliderActive[ix, iy]) {
			var dx = x % S.SIZE;
			var dy = y % S.SIZE;
			colliders[ix, iy].ClearBit(dx, dy);
		}
	}

	public void UpdateColliders ( ) {
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (colliderActive[x, y]) {
					colliders[x, y].Update();
				}
			}
		}
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
							clearCollision(x, y);
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
