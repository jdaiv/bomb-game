using UnityEngine;

public class Item : Entity {

	S Sprite;

	Rigidbody2D _rigidbody;
	CircleCollider2D _collider;
	CircleCollider2D _trigger;

	void Awake ( ) {
		Sprite = G.I.NewSprite(transform, 8);
		_rigidbody = gameObject.AddComponent<Rigidbody2D>();
		_rigidbody.gravityScale = 0;
		_rigidbody.drag = 10;
		_rigidbody.freezeRotation = true;
		_rigidbody.mass = 0.1f;
		_collider = gameObject.AddComponent<CircleCollider2D>();
		_collider.radius = 0.1f;
		_trigger = gameObject.AddComponent<CircleCollider2D>();
		_trigger.isTrigger = true;
		_trigger.radius = 0.2f;
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (IsEntity<Player>(other)) {
			
		}
	}

}