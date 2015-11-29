using UnityEngine;

public class Item : Entity {

	AS sprite;

	// animation
	float killTimer;

	Rigidbody2D _rigidbody;
	CircleCollider2D _collider;
	CircleCollider2D _trigger;

	public Player attachedTo;
	public int direction;

	#region weapon info

	int ammo = 1;

	#endregion

	void Awake ( ) {
		sprite = G.I.NewAnimatedSprite(transform, 2);
		sprite.loop = false;
		_rigidbody = gameObject.AddComponent<Rigidbody2D>();
		_rigidbody.gravityScale = 0;
		_rigidbody.drag = 10;
		_rigidbody.freezeRotation = true;
		_rigidbody.mass = 0.1f;
		_collider = gameObject.AddComponent<CircleCollider2D>();
		_collider.radius = 0.2f;
		_trigger = gameObject.AddComponent<CircleCollider2D>();
		_trigger.isTrigger = true;
		_trigger.radius = 0.5f;

		// Ignore hitscan weapons?
		gameObject.layer = 2;

		ammo = 10;
	}

	void OnDestroy ( ) {
		G.I.DeleteSprite(sprite);
	}

	void Update ( ) {
		transform.rotation = Quaternion.Euler(0, 0, direction * 90);
		if (ammo <= 0 && attachedTo == null) {
			sprite.Toggle();
			killTimer -= Time.deltaTime;
			if (killTimer <= 0) {
				G.I.DeleteEntity(this);
			}
		}
	}

	void OnTriggerStay2D (Collider2D other) {
		if (IsEntity<Player>(other)) {
			AttachTo(other.GetComponent<Player>());
		}
	}

	public void AttachTo (Player ply) {
		if (ply.item == null) {
			ply.item = this;
			_collider.enabled = false;
			_trigger.enabled = false;
			attachedTo = ply;
		}
	}

	public void Detach ( ) {
		if (attachedTo != null) {
			attachedTo.item = null;
			attachedTo = null;
			_collider.enabled = true;
			if (ammo > 0) {
				_trigger.enabled = true;
			} else {
				killTimer = 2;
			}
		}
	}

	public void Throw (float force) {
		Detach();
		Vector2 dir;
		switch (direction) {
			case 0:
				dir = Vector2.right;
				break;
			case 3:
				dir = Vector2.down;
				break;
			case 2:
				dir = Vector2.left;
				break;
			case 1:
				dir = Vector2.up;
				break;
			default:
				dir = Vector2.right;
				break;
		}
		_rigidbody.AddForce(dir * force, ForceMode2D.Impulse);
	}

	public virtual void Use ( ) {
		if (ammo > 0) {
			Vector2 dir;
			switch (direction) {
				case 0:
					dir = Vector2.right;
					break;
				case 3:
					dir = Vector2.down;
					break;
				case 2:
					dir = Vector2.left;
					break;
				case 1:
					dir = Vector2.up;
					break;
				default:
					dir = Vector2.right;
					break;
			}
			dir += U.RandomVec() * 0.05f;
			//var start = transform.position + transform.TransformDirection(7f / S.SIZE, 2f / S.SIZE, 0);
			var start = transform.position + transform.TransformDirection(11f / S.SIZE, 0, 0);
			G.I.FireHitscan(start, dir, 8);
			sprite.returnTo = 0;
			sprite.Play();
			ammo--;
		} else {
			sprite.returnTo = 2;
			sprite.Play(1, 2);
		}
	}

}