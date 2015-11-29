using UnityEngine;
using System.Collections;

public class SpaghettiCannon : Weapon {

	protected override void Configure ( ) {
		animationId = 3;
		automatic = false;
		delay = 0.3f;
		ammo = 10;
		pellets = 20;
		spread = 10f;
		recoil = 1;
		power = 8;
		bounces = 10;
		piercing = false;
		muzzleOffset = new Vector2(7, 2);
	}

}

