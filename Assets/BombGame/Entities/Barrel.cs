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
		_rigidbody.drag = 1;
		_rigidbody.freezeRotation = true;
		_rigidbody.mass = 1;
		_collider = gameObject.AddComponent<CircleCollider2D>();
		_collider.radius = 0.45f;
		var physMat = new PhysicsMaterial2D();
		physMat.friction = 0;
		physMat.bounciness = 0.4f;
		_collider.sharedMaterial = physMat;
		explode = false;
	}

	void OnDisable ( ) {
		G.I.DeleteSprite(sprite);
	}


	void Update ( ) {
		if (explode) {
			explodeTimer -= Time.deltaTime;
			if (explodeTimer < 0) {
				Explode();
			}
		}
	}

	bool emit;

	void FixedUpdate ( ) {
		if (explode) {
			if (emit) {
				G.I.particles.Emit(2, transform.position + new Vector3(0, 0.35f), 1, new Vector2(-1, 0), new Vector2(1, 4));
				emit = false;
			} else {
				emit = true;
			}
		}
	}

	void Explode ( ) {
		G.I.RadialDamage(transform.position, 2f);
		G.I.level.Explosion(transform.position, Random.Range(24, 32));
		G.I.particles.Emit(0, transform.position, 1);
		G.I.Shake(16);
		G.I.PlaySound(0);
		G.I.DeleteEntity(this);
	}

	public override void Kill ( ) {
		if (!explode) {
			explode = true;
			explodeTimer = 0.75f;
		}
	}

}
