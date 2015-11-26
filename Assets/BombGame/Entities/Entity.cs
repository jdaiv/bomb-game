using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {
	
	public static bool IsEntity (Collider2D collider) {
		return (collider.GetComponent<Entity>() != null);
	}

	public static void KillEntity (Collider2D collider) {
		collider.GetComponent<Entity>().Kill();
	}

	public bool Is<T>() where T : Entity {
		return (GetType() == typeof(T));
	}

	public virtual void Kill () {

	}

}
