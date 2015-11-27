using UnityEngine;

public class Item : Entity {

	S sprite;

	// animation
	float frameTime;
	float frameTimer;
	int frame;
	bool playing;
	Sprite[] frames;

	Rigidbody2D _rigidbody;
	CircleCollider2D _collider;
	CircleCollider2D _trigger;

	public Player attachedTo;
	public int direction;

	void Awake ( ) {
		sprite = G.I.NewSprite(transform, 8);
		frames = U.SliceSprite(G.I.sprites[8], 4);
		frameTime = 0.05f;
		frameTimer = 0f;
		playing = false;
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


	void Update ( ) {
		transform.rotation = Quaternion.Euler(0, 0, direction * 90);
		if (playing) {
			frameTimer += Time.deltaTime;
			if (frameTimer >= frameTime) {
				frameTimer -= frameTime;
				frame++;
				if (frame >= frames.Length) {
					frame = 0;
					playing = false;
				}
				sprite.renderer.sprite = frames[frame];
			}
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
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
			_trigger.enabled = true;
		}
	}

	public virtual void Use ( ) {
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
		var start = transform.position + transform.TransformDirection(7 / 16f, 2 / 16f, 0);
        var raycast = Physics2D.Raycast(start, dir);
		if (raycast.collider != null) {
			if (IsEntity(raycast.collider)) {
				KillEntity(raycast.collider);
			} else {
				G.I.level.Explosion(raycast.point, Random.Range(8, 10));
			}
			G.I.particles.Emit(raycast.point, 2);
			G.I.bulletTrails.AddTrail(start, raycast.point);
		} else {
			G.I.bulletTrails.AddTrail(start, (Vector2)start + (dir * 80));
		}

		playing = true;
		frameTimer = 0;
		frame = 0;
	}

}