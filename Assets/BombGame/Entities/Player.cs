using UnityEngine;
using System.Collections;

public class Player : Entity {

	S bodySprite;

	Rigidbody2D _rigidbody;
	CircleCollider2D _collider;

	Vector2 velocity;

	void Awake ( ) {
		bodySprite = G.I.NewSprite(transform, 4);
		_rigidbody = gameObject.AddComponent<Rigidbody2D>();
		_rigidbody.gravityScale = 0;
		_rigidbody.drag = 10;
		_rigidbody.freezeRotation = true;
		_rigidbody.mass = 0.1f;
		_collider = gameObject.AddComponent<CircleCollider2D>();
		_collider.radius = 0.35f;
		transform.position = new Vector3(20, 10);
	}

	void OnDisable ( ) {
		G.I.DeleteSprite(bodySprite);
	}

	void Update ( ) {
		var dt = Time.deltaTime;
		velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * 100;
		
		if (Input.GetKeyDown(KeyCode.R)) {
			Application.LoadLevel(0);
		}
	}

	void FixedUpdate ( ) {
		_rigidbody.AddForce(velocity * Time.fixedDeltaTime, ForceMode2D.Force);

		if (Input.GetButton("Jump") && velocity != Vector2.zero) {
			var raycast = Physics2D.Raycast((Vector2)transform.position + velocity.normalized, velocity.normalized);
			if (raycast.collider != null) {
				if (IsEntity(raycast.collider)) {
					KillEntity(raycast.collider);
				} else {
					G.I.level.Explosion(raycast.point, Random.Range(4, 6));
				}
				G.I.bulletTrails.AddTrail(transform.position, raycast.point);
			}
		}
	}

}
