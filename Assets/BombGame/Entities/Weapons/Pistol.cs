using UnityEngine;
using System.Collections;

public class Pistol : Weapon {

	protected override void Configure ( ) {
		animationId = 1;
		automatic = false;
		delay = 0.3f;
		ammo = 10;
		pellets = 1;
		spread = 0.05f;
		recoil = 1;
		power = 2;
		bounces = 0;
		piercing = false;
		muzzleOffset = new Vector2(7, 2);
		eject = true;
		ejectForce = 2;
	}

}
