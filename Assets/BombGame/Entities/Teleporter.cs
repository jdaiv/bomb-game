using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Teleporter : Entity {

	public static float RADIUS = 0.3f;

	S sprite;
	
	CircleCollider2D _trigger;

	public int target;

	List<Entity> skipList;

	void Awake ( ) {
		sprite = G.I.NewSprite(transform, 6);
		sprite.depthOffset = -1000;
		_trigger = gameObject.AddComponent<CircleCollider2D>();
		_trigger.radius = RADIUS;
		_trigger.isTrigger = true;
		skipList = new List<Entity>();
	}

	void OnDisable ( ) {
		G.I.DeleteSprite(sprite);
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (IsEntity(other)) {
			var ent = other.GetComponent<Entity>();
			if (!skipList.Contains(ent)) {
				var endPoint = (Teleporter)G.I.level.entities[target];
				endPoint.skipList.Add(ent);
				ent.transform.position = endPoint.transform.position;
				G.I.particles.Emit(2, transform.position, 2);
				G.I.particles.Emit(2, endPoint.transform.position, 2);
			}
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (IsEntity(other)) {
			var ent = other.GetComponent<Entity>();
			if (skipList.Contains(ent)) {
				skipList.Remove(ent);
			}
        }
	}

}
