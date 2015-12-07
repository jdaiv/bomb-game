using UnityEngine;
using System.Collections;

public class LMG : Weapon {

	protected override void Configure ( ) {
		animationId = 8;
		automatic = true;
		delay = 0.10f;
		ammo = 40;
		pellets = 1;
		spread = 0.0f;
		recoil = 0.5f;
		speed = 0.7f;
		power = 4;
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
				sprite.returnTo = 0;
				if (fireCycle) {
					sprite.Play(4, 5);
				} else {
					sprite.Play(6, 7);
				}
				fireCycle = !fireCycle;
			} else if (ammo > 1) {
				sprite.Play(6, 7);
				sprite.returnTo = 1;
			} else if (ammo > 0) {
				sprite.Play(8, 9);
				sprite.returnTo = 2;
			} else {
				sprite.Play(10, 11);
				sprite.returnTo = 3;
			}
		} else {
			if (fireTimer <= 0) {
				sprite.Stop();
				sprite.GoTo(3);
			}
		}
	}

}
