using UnityEngine;
using System.Collections;

public class Player : Entity {

	S bodySprite;

	Rigidbody2D _rigidbody;
	CircleCollider2D _collider;
	Actions _actions;

	Vector2 input;
	Vector2 lastInput;
	Vector2 itemPos;

	public Item item;

	void Awake ( ) {
	}

	public void Init (int sprite) {
		bodySprite = G.I.NewSprite(transform, sprite);
		_rigidbody = gameObject.AddComponent<Rigidbody2D>();
		_rigidbody.gravityScale = 0;
		_rigidbody.drag = 4;
		_rigidbody.freezeRotation = true;
		_rigidbody.mass = 1f;
		var physMat = new PhysicsMaterial2D();
		physMat.friction = 0;
		physMat.bounciness = 0;
		_collider = gameObject.AddComponent<CircleCollider2D>();
		_collider.radius = 0.4f;
		_collider.sharedMaterial = physMat;
		transform.position = new Vector3(20, 10);
		_actions = Actions.CreateWithDefaultBindings(true);
	}

	void OnDisable ( ) {
		G.I.DeleteSprite(bodySprite);
		_actions.Destroy();
	}

	void Update ( ) {
		var dt = Time.deltaTime;
		input = _actions.Move.Vector;

		if (input != Vector2.zero) {
			lastInput = input;
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
			if (_actions.Fire) {
				item.Use();
			}
			if (_actions.Throw.WasPressed) {
				item.Throw(2);
			}
		}

		if (_actions.Start.WasPressed) {
			Application.LoadLevel(0);
		}
	}

	void FixedUpdate ( ) {
		_rigidbody.AddForce(input * 16, ForceMode2D.Force);
	}

	public override void Kill ( ) {
		alive = false;
		if (item != null) {
			item.Detach();
		}
		G.I.DeleteSprite(bodySprite);
		G.I.RadialDamage(transform.position, 2f);
		G.I.level.Explosion(transform.position, Random.Range(24, 32));
		G.I.particles.Emit(0, transform.position, 1);
		G.I.Shake(32);
		G.I.DeleteEntity(this);
	}

}
