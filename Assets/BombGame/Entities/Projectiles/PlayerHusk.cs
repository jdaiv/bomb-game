using UnityEngine;
using System.Collections;

public class PlayerHusk : Entity {

	S sprite;

	public Rigidbody2D _rigidbody;
	CircleCollider2D _collider;

	FrameTimer explode;
	FrameTimer particles;

	bool flash;

	Entity owner;

	void Awake ( ) {
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

		explode = new FrameTimer(40);
		particles = new FrameTimer(4, true);
	}

	public void SetSprite (int id) {
		sprite = G.I.NewSprite(transform, G.I.playerSprites[id]);
	}

	void OnDisable ( ) {
		G.I.DeleteSprite(sprite);
	}

	override public void Tick ( ) {
		explode.Tick();
		particles.Tick();
		if (explode) {
			Explode();
		}
		if (particles) {
			G.I.particles.Emit(2, transform.position, 1, new Vector2(-1, 0), new Vector2(1, 4));
			flash = !flash;
			sprite.renderer.color = flash ? Color.white : Color.black;
		}
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
		explode.Reset();
		explode.Start();
	}

}

