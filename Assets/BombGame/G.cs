using UnityEngine;
using System.Collections.Generic;
using System;
using InControl;

public class G : MonoBehaviour {

	public static G I;

	public P players;

	public Camera mainCamera;
	private Vector3 cameraPos;
	public float cameraShake;

	public Sprite[] sprites;
	public Sprite[][] animations;
	public AudioClip[] sounds;

	public Level level;
	public BulletTrails bulletTrails;
	public Particles particles;
	public BulletCasings casings;

	S hudBG;

	int currentSpawn;
	int[] playerSprites = { 4, 15, 16, 17 };

	public void Awake ( ) {
		I = this;

		players = new P();

		cameraPos = mainCamera.transform.position;

		animations = new Sprite[][]{
			U.SliceSprite(sprites[7], 9), // 0
			U.SliceSprite(sprites[8], 4),
			U.SliceSprite(sprites[11], 4),
			U.SliceSprite(sprites[12], 4),
			U.SliceSprite(sprites[13], 7),
			U.SliceSprite(sprites[14], 7), // 5
			U.SliceSprite(sprites[18], 4),
			U.SliceSprite(sprites[19], 9),
		};

		InitSprites();
		InitEntities();

		level = new Level();
		level.Generate();

		bulletTrails = new BulletTrails();
		particles = new Particles();
		particles.RegisterSprite(animations[0]);
		particles.RegisterSprite(animations[4]);
		particles.RegisterSprite(animations[5]);
		casings = new BulletCasings();

		hudBG = NewSprite(null, 9);
		hudBG.transform.position = new Vector3(S.SIZE * 20, S.SIZE * 21f);

		currentSpawn = 0;
	}

	public void NewRound ( ) {
		casings.Clear();
		for (int i = 0; i < _entities.Count; i++) {
			var e = _entities[i];
			Destroy(e.gameObject);
		}
		_entities.Clear();
		_entitiesToRemove.Clear();
		level.Generate();

		SpawnPlayers();
	}

	public void SpawnPlayers ( ) {
		currentSpawn++;
		foreach (var ply in players.players) {
			if (ply.active) {
				SpawnPlayer(ply.device);
			}
		}
	}

	public void SpawnPlayer (InputDevice device) {
		var player = CreateEntity<Player>("Player");
		(player as Player).Init(playerSprites[currentSpawn % 4], device);
		player.transform.position = level.spawnLocations[currentSpawn % 4];
		currentSpawn++;
	}

	public void Update ( ) {
		players.CheckPlayers();

		if (Input.GetKeyDown(KeyCode.R)) {
			NewRound();
		}

		foreach (var ent in _entitiesToRemove) {
			_entities.Remove(ent);
		}
		_entitiesToRemove.Clear();

		var dt = Time.deltaTime;
		particles.Tick(dt);
		casings.Tick(dt);
	}

	public void FixedUpdate ( ) {
		cameraShake *= 0.9f;
		mainCamera.transform.position = new Vector3(
				Mathf.Round(U.RandomRange(cameraShake) + cameraPos.x),
				Mathf.Round(U.RandomRange(cameraShake) + cameraPos.y),
				cameraPos.z
			);
		bulletTrails.Decay();
	}

	public void Shake (float amount) {
		cameraShake += amount;
	}

	public void FireHitscan (Vector2 origin, Vector2 direction, int explosionRadius = 0, int bounces = 0, int teleports = 0) {
		if (teleports > 4) {
			return;
		}
		direction.Normalize();
		var raycast = Physics2D.Raycast(origin, direction);
		if (raycast.collider != null) {
			if (Entity.IsEntity(raycast.collider)) {
				if (Entity.IsEntity<Teleporter>(raycast.collider)) {
					var teleporter = raycast.collider.GetComponent<Teleporter>();
					FireHitscan((Vector2)level.entities[teleporter.target].transform.position + (direction * (Teleporter.RADIUS + 0.01f)), direction, explosionRadius, bounces, teleports + 1);
				} else {
					Entity.KillEntity(raycast.collider);
				}
			} else {
				if (explosionRadius > 0) {
					level.Explosion(raycast.point, explosionRadius, false);
				}
				if (bounces > 0) {
					FireHitscan(raycast.point, Vector2.Reflect(direction, raycast.normal), explosionRadius, bounces - 1, teleports);
				}
			}
			particles.Emit(1, raycast.point, 1, new Vector2(-1, -1), new Vector2(1, 1));
			bulletTrails.AddTrail(origin, raycast.point);
		} else {
			bulletTrails.AddTrail(origin, origin + (direction * 80));
		}
	}

	public void FireHitscanNoCollision (Vector2 origin, Vector2 direction, int explosionRadius = 0) {
		direction.Normalize();
		var raycast = Physics2D.RaycastAll(origin, direction);
		foreach (var hit in raycast) {
			if (Entity.IsEntity(hit.collider)) {
				Entity.KillEntity(hit.collider);
			}
			particles.Emit(1, hit.point, 1);
		}
		bulletTrails.AddTrail(origin, origin + (direction * 80));

	}

	#region Entities

	private List<Entity> _entities;
	private List<Entity> _entitiesToRemove;

	public void InitEntities ( ) {
		_entities = new List<Entity>();
		_entitiesToRemove = new List<Entity>();
	}

	public Entity CreateEntity<T>(string name = "Entity") where T : Entity {
		var go = new GameObject();
		go.name = name;
		var e = go.AddComponent<T>();
		_entities.Add(e);
		return e;
	}

	public void DeleteEntity (Entity e) {
		Destroy(e.gameObject);
		_entitiesToRemove.Add(e);
	}

	public void RadialDamage (Vector2 pos, float radius) {
		foreach (var ent in _entities) {
			if (ent.alive) {
				if (Vector3.Distance(ent.transform.position, pos) <= radius) {
					ent.Kill();
					if (ent.GetComponent<Rigidbody2D>() != null) {
						var force = pos - (Vector2)ent.transform.position;
						force.Normalize();
						force.x = (-Mathf.Sign(force.x)) + force.x;
						force.y = (-Mathf.Sign(force.y)) + force.y;
						force *= radius;
						ent.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
					}
				}
			}
		}
	}

	#endregion

	#region Sprites

	private List<S> _sprites;

	public void InitSprites ( ) {
		_sprites = new List<S>();
	}

	public S NewSprite (Transform link, int s) {
		var sprite = new S();
		var go = new GameObject();
		go.hideFlags = HideFlags.HideInHierarchy;
		sprite.transform = go.transform;
		sprite.renderer = go.AddComponent<SpriteRenderer>();
		sprite.renderer.sprite = sprites[s];
		sprite.linkedObject = link;
		_sprites.Add(sprite);
		return sprite;
	}

	public AS NewAnimatedSprite (Transform link, int s) {
		var sprite = new AS(animations[s]);
		var go = new GameObject();
		go.hideFlags = HideFlags.HideInHierarchy;
		sprite.transform = go.transform;
		sprite.renderer = go.AddComponent<SpriteRenderer>();
		sprite.linkedObject = link;
		sprite.UpdateFrame();
		_sprites.Add(sprite);
		return sprite;
	}

	public void DeleteSprite (S sprite) {
		_sprites.Remove(sprite);
		if (sprite.transform != null) {
			Destroy(sprite.transform.gameObject);
		}
	}

	public void LateUpdate ( ) {
		level.Update();
		foreach (var s in _sprites) {
			s.Update();
		}
	}

	#endregion

}
