using UnityEngine;
using System.Collections;

public class Shotgun : Weapon {

	protected override void Configure ( ) {
		animationId = 11;
		soundId = 16;
		automatic = false;
		delay = 0.9f;
		ammo = 8;
		pellets = 9;
		spread = 0.2f;
		recoil = 1;
		power = 8;
		bounces = 0;
		piercing = false;
		muzzleOffset = new Vector2(11, 00);
	}

}

