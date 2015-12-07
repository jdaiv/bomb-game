using UnityEngine;
using System.Collections;

public class Rifle : Weapon {

	protected override void Configure ( ) {
		animationId = 2;
		automatic = true;
		delay = 0.1f;
		ammo = 30;
		pellets = 1;
		spread = 0.01f;
		recoil = 0.2f;
		power = 4;
		bounces = 0;
		piercing = false;
		muzzleOffset = new Vector2(11, 0);
		eject = true;
		ejectForce = 3;
	}

}
