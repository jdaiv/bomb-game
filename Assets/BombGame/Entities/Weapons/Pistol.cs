using UnityEngine;
using System.Collections;

public class Pistol : Weapon {

	protected override void Configure ( ) {
		animationId = 1;
		soundId = 4;
		delay = new FrameTimer(24);
		ammo = 10;
		pellets = 1;
		spread = 0f;
		recoil = 0.03f;
		power = 4;
		bounces = 0;
		piercing = false;
		muzzleOffset = new Vector2(6, 1);
		eject = true;
		ejectForce = 2;
	}

}
