using UnityEngine;
using System.Collections.Generic;
using System;

public class G : MonoBehaviour {

	public static G I;
	
	public Camera mainCamera;
	private Vector3 cameraPos;
	public float cameraShake;

	public Sprite[] sprites;
	public Sprite[][] animations;
	public AudioClip[] sounds;

	public Level level;
	public BulletTrails bulletTrails;
	public Particles particles;

	S hudBG;

	int currentSpawn;

	public void Awake ( ) {
		I = this;

		cameraPos = mainCamera.transform.position;

		animations = new Sprite[][]{
			U.SliceSprite(sprites[7], 9),
			U.SliceSprite(sprites[8], 4),
			U.SliceSprite(sprites[11], 4),
			U.SliceSprite(sprites[12], 4),
		};

		InitSprites();
		InitEntities();

		level = new Level();
		StartCoroutine(level.Generate());

		bulletTrails = new BulletTrails();
		particles = new Particles();
		particles.RegisterSprite(animations[0]);

		hudBG = NewSprite(null, 9);
		hudBG.transform.position = new Vector3(S.SIZE * 20, S.SIZE * 21f);

		currentSpawn = 0;
	}

	public void SpawnPlayer ( ) {
		var player = CreateEntity<Player>("Player One");
		player.transform.position = level.spawnLocations[currentSpawn % 4];
		currentSpawn++;
	}

	public void Update ( ) {
		if (Input.GetKey(KeyCode.S)) {
			SpawnPlayer();
		}

		foreach(var ent in _entitiesToRemove) {
			_entities.Remove(ent);
		}
		_entitiesToRemove.Clear();

		var dt = Time.deltaTime;
		particles.Tick(dt);
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
			particles.Emit(raycast.point, 1);
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
			} else {
				if (explosionRadius > 0) {
					level.Explosion(hit.point, explosionRadius, false);
				}
			}
			particles.Emit(hit.point, 1);
			bulletTrails.AddTrail(origin, hit.point);
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
		// camera logic!
	}

	#endregion

}
