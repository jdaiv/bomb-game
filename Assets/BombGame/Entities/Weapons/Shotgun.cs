using UnityEngine;
using System.Collections;

public class Shotgun : Weapon {

	protected override void Configure ( ) {
		animationId = 11;
		soundId = 16;
		delay = new FrameTimer(50);
		ammo = 8;
		pellets = 9;
		spread = 0.2f;
		recoil = 1;
		power = 12;
		bounces = 0;
		piercing = false;
		muzzleOffset = new Vector2(11, 00);
	}

}

