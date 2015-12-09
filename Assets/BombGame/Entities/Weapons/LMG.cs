using UnityEngine;
using System.Collections;

public class LMG : Weapon {

	protected override void Configure ( ) {
		animationId = 8;
		soundId = 10;
		automatic = true;
		delay = 0.1f;
		ammo = 80;
		pellets = 1;
		spread = -0.6f;
		recoil = 0.4f;
		speed = 0.7f;
		power = 6;
		bounces = 0;
		piercing = false;
		muzzleOffset = new Vector2(11, 1);
		eject = true;
		ejectForce = 2;
	}

	bool fireCycle;

	public override void Use ( ) {
		bool willFire = ammo > 0 && fireTimer <= 0;
		base.Use();
		sprite.loop = false;
		if (willFire) {
			if (ammo > 2) {
				sprite.returnTo = 1;
				if (fireCycle) {
					sprite.Play(5, 6);
				} else {
					sprite.Play(7, 8);
				}
				fireCycle = !fireCycle;
			} else if (ammo > 1) {
				sprite.Play(7, 8);
				sprite.returnTo = 2;
			} else if (ammo > 0) {
				sprite.Play(9, 10);
				sprite.returnTo = 3;
			} else {
				sprite.Play(11, 12);
				sprite.returnTo = 4;
			}
		} else {
			if (fireTimer <= 0) {
				sprite.Stop();
				sprite.GoTo(4);
			}
		}
	}

}
