using UnityEngine;
using System.Collections;

public class Magnum : Weapon {

	protected override void Configure ( ) {
		animationId = 3;
		automatic = false;
		delay = 0.3f;
		ammo = 6;
		pellets = 1;
		spread = 0;
		recoil = 1;
		power = 8;
		bounces = 10;
		piercing = true;
		muzzleOffset = new Vector2(7, 2);
	}

}