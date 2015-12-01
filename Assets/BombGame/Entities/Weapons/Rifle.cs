using UnityEngine;
using System.Collections;

public class Rifle : Weapon {

	protected override void Configure ( ) {
		animationId = 2;
		automatic = true;
		delay = 0.1f;
		ammo = 30;
		pellets = 1;
		spread = 0.2f;
		recoil = 1;
		power = 4;
		bounces = 0;
		piercing = false;
		muzzleOffset = new Vector2(11, 0);
	}

}
