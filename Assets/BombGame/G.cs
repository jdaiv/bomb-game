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

	public void Awake ( ) {
		I = this;

		InitSprites();
		InitEntities();

		level = new Level();
		StartCoroutine(level.Generate());

		bulletTrails = new BulletTrails();

		particles = new Particles();
		particles.RegisterSprite(sprites[7], 8);

		CreateEntity<Player>("Player One");
	}

	public void Update ( ) {
		var dt = Time.deltaTime;
		particles.Tick(dt);
	}

	public void FixedUpdate ( ) {
		bulletTrails.Decay();
	}

	#region Entities

	private List<Entity> _entities;

	public void InitEntities ( ) {
		_entities = new List<Entity>();
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
