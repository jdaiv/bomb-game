using UnityEngine;
using System.Collections;

public abstract class Entity : MonoBehaviour {

	public bool alive = true;

	public void OnEnable ( ) {
		gameObject.tag = "Entity";
	}

	public static bool IsEntity (Component c) {
		return (c.tag == "Entity");
	}

	public static bool IsEntity<T> (Component c) where T : Entity {
		return (c.tag == "Entity" ? c.GetComponent<T>() != null : false);
	}

	public static void KillEntity (Collider2D collider, Entity attacker = null) {
		collider.GetComponent<Entity>().Kill(attacker);
	}

	public virtual void Tick () {

	}

	public virtual void PhysicsTick () {

	}

	public bool Is<T>() where T : Entity {
		return (GetType() == typeof(T));
	}

	public virtual void Kill (Entity attacker) {
		alive = false;
	}

}
