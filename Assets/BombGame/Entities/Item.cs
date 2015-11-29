using UnityEngine;

public class Item : Entity {

	protected Rigidbody2D _rigidbody;
	protected CircleCollider2D _collider;
	protected CircleCollider2D _trigger;

	public Player attachedTo;
	public int direction;
	protected Vector2 directionVector;

	public virtual void Awake ( ) {
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
		gameObject.layer = 2;
	}

	public virtual void Update ( ) {
		transform.rotation = Quaternion.Euler(0, 0, direction * 90);
		
		switch (direction) {
			case 0:
				directionVector = Vector2.right;
				break;
			case 3:
				directionVector = Vector2.down;
				break;
			case 2:
				directionVector = Vector2.left;
				break;
			case 1:
				directionVector = Vector2.up;
				break;
			default:
				directionVector = Vector2.right;
				break;
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
			_trigger.enabled = true;
			CustomDetach();
		}
	}

	protected virtual void CustomDetach ( ) {

	}

	public void Throw (float force) {
		Detach();
		_rigidbody.AddForce(directionVector * force, ForceMode2D.Impulse);
	}

	public virtual void Use ( ) {
		
	}

}