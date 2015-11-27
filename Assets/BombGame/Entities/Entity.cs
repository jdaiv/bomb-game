using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {

	public bool alive = true;

	public static bool IsEntity (Component c) {
		return (c.GetComponent<Entity>() != null);
	}

	public static bool IsEntity<T> (Component c) where T : Entity {
		return (c.GetComponent<T>() != null);
	}

	public static void KillEntity (Collider2D collider) {
		collider.GetComponent<Entity>().Kill();
	}

	public bool Is<T>() where T : Entity {
		return (GetType() == typeof(T));
	}

	public virtual void Kill () {
		alive = false;
	}

}
