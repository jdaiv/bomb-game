using UnityEngine;
using System.Collections;

public class Barrel : Entity {

	S sprite;

	Rigidbody2D _rigidbody;
	CircleCollider2D _collider;

	void Awake ( ) {
		sprite = G.I.NewSprite(transform, 3);
		_rigidbody = gameObject.AddComponent<Rigidbody2D>();
		_rigidbody.gravityScale = 0;
		_rigidbody.drag = 10;
		_rigidbody.freezeRotation = true;
		_rigidbody.mass = 0.1f;
		_collider = gameObject.AddComponent<CircleCollider2D>();
		_collider.radius = 0.45f;
	}

	public override void Kill ( ) {
		base.Kill();
		G.I.DeleteSprite(sprite);
		G.I.level.Explosion(transform.position, Random.Range(24, 32));
		G.I.particles.Emit(transform.position, 2);
		G.I.DeleteEntity(this);
	}

}
