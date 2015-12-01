using UnityEngine;
using System.Collections;

public class PlayerSpawn : Entity {

	S sprite;

	void Awake ( ) {
		sprite = G.I.NewSprite(transform, 10);
		sprite.depthOffset = -1000;
	}

	void OnDisable ( ) {
		G.I.DeleteSprite(sprite);
	}

}
