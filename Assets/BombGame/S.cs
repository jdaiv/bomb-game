using System;
using UnityEngine;

public class S {

	public const int SIZE = 16;

	public SpriteRenderer renderer;
	public Transform transform;
	public Transform linkedObject;
	public int depthOffset;

	public virtual void Update ( ) {
		if (linkedObject != null) {
			transform.rotation = linkedObject.rotation;
			var p = linkedObject.position;
			transform.position = new Vector3(
				Mathf.Round(p.x * SIZE),
				Mathf.Round(p.y * SIZE)
			);
		}
		renderer.sortingOrder = Mathf.RoundToInt(1000 - transform.position.y) + depthOffset;
	}

	public void Show ( ) {
		renderer.enabled = true;
	}

	public void Hide () {
		renderer.enabled = false;
	}

	public void Toggle () {
		renderer.enabled = !renderer.enabled;
	}

	public bool Equals (S other) {
		if (linkedObject == null || other.linkedObject == null) return false;
		else return linkedObject == other.linkedObject;
	}

}