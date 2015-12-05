using UnityEngine;
using System.Collections;

public class LMG : Weapon {

	protected override void Configure ( ) {
		animationId = 8;
		automatic = true;
		delay = 0.005f;
		ammo = 10000;
		pellets = 1;
		spread = 0.2f;
		recoil = 1;
		power = 4;
		bounces = 0;
		piercing = false;
		muzzleOffset = new Vector2(11, 1);
		eject = true;
		ejectForce = 2;
	}

}
