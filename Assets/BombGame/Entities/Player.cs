using UnityEngine;
using System.Collections;

public class Player : Entity {

	S bodySprite;

	Rigidbody2D _rigidbody;
	CircleCollider2D _collider;

	Vector2 velocity;
	Vector2 lastInput;
	Vector2 itemPos;

	public Item item;

	void Awake ( ) {
		bodySprite = G.I.NewSprite(transform, 4);
		_rigidbody = gameObject.AddComponent<Rigidbody2D>();
		_rigidbody.gravityScale = 0;
		_rigidbody.drag = 1;
		_rigidbody.freezeRotation = true;
		_rigidbody.mass = 1f;
		_collider = gameObject.AddComponent<CircleCollider2D>();
		_collider.radius = 0.35f;
		transform.position = new Vector3(20, 10);
	}

	void OnDisable ( ) {
		G.I.DeleteSprite(bodySprite);
	}

	void Update ( ) {
		var dt = Time.deltaTime;
		velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		velocity.Normalize();

		if (velocity != Vector2.zero) {
			lastInput = velocity;
        }
		
		if (item != null) {
			if (lastInput.y > 0) {
				item.direction = 1;
			} else if (lastInput.y < 0) {
				item.direction = 3;
			} else if (lastInput.x > 0) {
				item.direction = 0;
			} else if (lastInput.x < 0) {
				item.direction = 2;
			} else {
				item.direction = 0;
			}
			Physics2D.queriesStartInColliders = false;
			var raycast = Physics2D.Raycast(transform.position, lastInput, 1);
			Vector2 targetPos;
			if (raycast.collider != null) {
				targetPos = raycast.point - (Vector2)transform.position;
			} else {
				targetPos = lastInput;
			}
			targetPos *= 0.7f;
			itemPos = Vector2.Lerp(itemPos, targetPos, dt * 8);
			item.transform.position = (Vector2)transform.position + itemPos;
			if (Input.GetButtonDown("Fire1")) {
				item.Use();
			}
			if (Input.GetButtonDown("Fire2")) {
				item.Throw(2);
			}
		}

		if (Input.GetKeyDown(KeyCode.R)) {
			Application.LoadLevel(0);
		}
	}

	void FixedUpdate ( ) {
		_rigidbody.AddForce(velocity * 200 * Time.fixedDeltaTime, ForceMode2D.Force);
	}

}
