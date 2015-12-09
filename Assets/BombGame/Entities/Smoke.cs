using UnityEngine;
using System.Collections;

public class Smoke : Entity {
	
	bool emit;

	override public void _FixedUpdate ( ) {
		if (alive) {
			if (emit) {
				G.I.particles.Emit(3, transform.position, 1, new Vector2(-1, 0), new Vector2(1, 4));
				emit = false;
			} else {
				emit = true;
			}
		}
	}

}
