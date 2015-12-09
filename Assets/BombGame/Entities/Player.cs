using UnityEngine;
using System.Collections;
using InControl;

public class Player : Entity {

	S bodySprite;
	Texture2D texture;

	Rigidbody2D _rigidbody;
	CircleCollider2D _collider;
	Actions _actions;

	Vector2 input;
	Vector2 lastInput;
	Vector2 itemPos;

	public Item item;

	void Awake ( ) {
	}

	public void Init (int sprite, InputDevice device) {
		//texture = new Texture2D(S.SIZE, S.SIZE, TextureFormat.RGBA32, false);
		//texture.filterMode = FilterMode.Point;

		//for (int x = 0; x < 16; x++) {
		//	for (int y = 0; y < 16; y++) {
		//		texture.SetPixel(x, y, Color.black);
		//	}
		//}
		//texture.Apply();

		bodySprite = G.I.NewSprite(transform, sprite);
		//bodySprite.renderer.sprite = Sprite.Create(texture, new Rect(0, 0, 16, 16), Vector2.one * 0.5f, 1);
		
		//for (int x = 0; x < 16; x++) {
		//	for (int y = 0; y < 16; y++) {
		//		texture.SetPixel(x, y, Color.clear);
		//	}
		//}
		//texture.Apply();

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
		if (device != null) {
			_actions = Actions.CreateWithDefaultBindings(false);
			_actions.Device = device;
		} else {
			_actions = Actions.CreateWithDefaultBindings(true);
		}
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
			if (_actions.Fire != Vector2.zero) {

				if (_actions.FireDown) {
					item.direction = 3;
				} else if (_actions.FireUp) {
					item.direction = 1;
				} else if (_actions.FireRight) {
					item.direction = 0;
				} else if (_actions.FireLeft) {
					item.direction = 2;
				}
				item.UpdateDir();
				var dir = item.directionVector;
				itemPos = dir * 0.7f;
				item.transform.position = (Vector2)transform.position + itemPos;
				item.Use();
				lastInput = _actions.Fire;

			} else {

				if (!item.active) {
					float greatest = 0;
					if (lastInput.y > greatest) {
						item.direction = 1;
						greatest = lastInput.y;
					}
					if (Mathf.Abs(lastInput.y) > greatest) {
						item.direction = 3;
						greatest = Mathf.Abs(lastInput.y);
					}
					if (lastInput.x > greatest) {
						item.direction = 0;
						greatest = lastInput.x;
					}
					if (Mathf.Abs(lastInput.x) > greatest) {
						item.direction = 2;
						greatest = Mathf.Abs(lastInput.x);
					}
					item.UpdateDir();
					var dir = item.directionVector;
					Vector2 targetPos;
					targetPos = lastInput * 0.7f;
					itemPos = Vector2.Lerp(itemPos, targetPos, dt * 8);
				}

			}

			item.transform.position = (Vector2)transform.position + itemPos;

			if (_actions.Throw.WasPressed) {
				item.Throw(2);
			}
		}

		if (_actions.Start.WasPressed) {
			Application.LoadLevel(0);
		}

		//var center = new Vector2(7.5f, 7.5f);
		//for (int x = 0; x < 16; x++) {
		//	for (int y = 0; y < 16; y++) {
		//		if (Vector2.Distance(new Vector2(x, y), center) <= 8) {
		//			var color = 0f;
		//			var pixel = (Vector2)transform.position + new Vector2((float)x / S.SIZE - 0.5f, (float) y / S.SIZE - 0.5f);
		//			foreach (var l in G.I.lighting.lights) {
		//				if (Vector2.Distance(l.position, pixel) < l.range) {
		//					color += 0.5f;
		//				}
		//			}
		//			texture.SetPixel(x, y, new Color(color, color, color));
		//		}
		//	}
		//}
		//texture.Apply();
	}

	void FixedUpdate ( ) {
		var speed = 1f;
		if (item is Weapon) {
			speed = (item as Weapon).speed;
		}
		_rigidbody.AddForce(input * 16 * speed, ForceMode2D.Force);
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
