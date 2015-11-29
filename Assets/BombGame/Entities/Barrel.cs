using UnityEngine;
using System.Collections;

public class Barrel : Entity {

	S sprite;

	Rigidbody2D _rigidbody;
	CircleCollider2D _collider;

	float explodeTimer;
	bool explode;

	void Awake ( ) {
		sprite = G.I.NewSprite(transform, 3);
		_rigidbody = gameObject.AddComponent<Rigidbody2D>();
		_rigidbody.gravityScale = 0;
		_rigidbody.drag = 10;
		_rigidbody.freezeRotation = true;
		_rigidbody.mass = 0.1f;
		_collider = gameObject.AddComponent<CircleCollider2D>();
		_collider.radius = 0.45f;
		explode = false;
	}

	void Update ( ) {
		if (explode) {
			explodeTimer -= Time.deltaTime;
			if (explodeTimer < 0) {
				Explode();
			}
		}
	}

	void FixedUpdate ( ) {
		if (explode) {
			G.I.particles.Emit(transform.position, 1);
		}
	}

	void Explode ( ) {
		G.I.DeleteSprite(sprite);
		G.I.RadialDamage(transform.position, 2f);
		G.I.level.Explosion(transform.position, Random.Range(24, 32));
		G.I.particles.Emit(transform.position, 1);
		G.I.Shake(16);
		G.I.DeleteEntity(this);
	}

	public override void Kill ( ) {
		if (!explode) {
			explode = true;
			explodeTimer = 0.5f;
		}
	}

}
