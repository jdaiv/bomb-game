using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class G : MonoBehaviour {

	public static G I;

	public Sprite[] sprites;
	public AudioClip[] sounds;

	public Level level;
	public BulletTrails bulletTrails;
	public Particles particles;

	S hudBG;

	public void Awake ( ) {
		I = this;

		InitSprites();
		InitEntities();

		level = new Level();
		StartCoroutine(level.Generate());

		bulletTrails = new BulletTrails();

		particles = new Particles();
		particles.RegisterSprite(sprites[7], 8);

		hudBG = NewSprite(null, 9);
		hudBG.transform.position = new Vector3(S.SIZE * 20, S.SIZE * 21f);

		var player = CreateEntity<Player>("Player One");
		player.transform.position = level.spawnLocations[0];
	}

	public void Update ( ) {
		foreach(var ent in _entitiesToRemove) {
			_entities.Remove(ent);
		}
		_entitiesToRemove.Clear();

		var dt = Time.deltaTime;
		particles.Tick(dt);
	}

	public void FixedUpdate ( ) {
		bulletTrails.Decay();
	}

	public void FireHitscan (Vector2 origin, Vector2 direction, int explosionRadius) {
		direction.Normalize();
		var raycast = Physics2D.Raycast(origin, direction);
		if (raycast.collider != null) {
			if (Entity.IsEntity(raycast.collider)) {
				if (Entity.IsEntity<Teleporter>(raycast.collider)) {
					var teleporter = raycast.collider.GetComponent<Teleporter>();
					FireHitscan((Vector2)level.entities[teleporter.target].transform.position + (direction * (Teleporter.RADIUS + 0.01f)), direction, explosionRadius);
				} else {
					Entity.KillEntity(raycast.collider);
				}
			} else {
				G.I.level.Explosion(raycast.point, explosionRadius);
			}
			G.I.particles.Emit(raycast.point, 2);
			G.I.bulletTrails.AddTrail(origin, raycast.point);
		} else {
			G.I.bulletTrails.AddTrail(origin, origin + (direction * 80));
		}
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
				}
			}
		}
	}

	#endregion

	#region Rendering

	public Camera mainCamera;
	private List<S> _sprites;

	public void InitSprites ( ) {
		_sprites = new List<S>();
	}

	public S NewSprite (Transform link, int s) {
		var sprite = new S();
		var go = new GameObject();
		go.name = link == null ? "(!s) null" : "(!s) " + link.name;
		//go.hideFlags = HideFlags.HideInHierarchy;
		sprite.transform = go.transform;
		sprite.renderer = go.AddComponent<SpriteRenderer>();
		sprite.renderer.sprite = sprites[s];
		sprite.linkedObject = link;
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
		foreach (var s in _sprites) {
			s.Update();
		}
		// camera logic!
	}

	#endregion

}
