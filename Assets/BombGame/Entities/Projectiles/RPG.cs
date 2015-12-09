using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RPG : Entity {

	public static float RADIUS = 0.3f;

	S sprite;

	Entity owner;

	CircleCollider2D _trigger;
	Rigidbody2D _rigidbody;

	void Awake ( ) {
		sprite = G.I.NewSprite(transform, 27);
		_trigger = gameObject.AddComponent<CircleCollider2D>();
		_trigger.radius = RADIUS;
		_trigger.isTrigger = true;
		_rigidbody = gameObject.AddComponent<Rigidbody2D>();
		_rigidbody.gravityScale = 0;
		_rigidbody.drag = 0;
		_rigidbody.freezeRotation = true;
		_rigidbody.mass = 1;

	}

	void OnDisable ( ) {
		G.I.DeleteSprite(sprite);
	}

	bool emit;

	override public void _FixedUpdate ( ) {
		if (alive) {
			if (emit) {
				G.I.particles.Emit(1, transform.position, 1, new Vector2(-1, 0), new Vector2(1, 4));
				emit = false;
			} else {
				emit = true;
			}
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		Explode();
	}

	void Explode ( ) {
		alive = false;
		G.I.RadialDamage(owner, transform.position, 2f);
		G.I.level.Explosion(transform.position, Random.Range(24, 32));
		G.I.particles.Emit(0, transform.position, 1);
		G.I.Shake(16);
		G.I.PlaySound(0);
		G.I.DeleteEntity(this);
	}

	public override void Kill (Entity attacker) {
		owner = attacker;
	}

}
