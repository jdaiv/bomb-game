using UnityEngine;
using System.Collections;

public class LMG : Weapon {

	protected override void Configure ( ) {
		animationId = 8;
		automatic = true;
		delay = 0.025f;
		ammo = 100;
		pellets = 1;
		spread = 0.2f;
		recoil = 1;
		power = 4;
		bounces = 0;
		piercing = false;
		muzzleOffset = new Vector2(11, 1);
		eject = true;
		ejectForce = 10;
	}

}
