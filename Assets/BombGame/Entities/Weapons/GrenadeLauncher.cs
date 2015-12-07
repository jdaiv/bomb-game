﻿using UnityEngine;
using System.Collections;

public class GrenadeLauncher : Weapon {

	protected override void Configure ( ) {
		animationId = 9;
		automatic = true;
		delay = 0.040f;
		ammo = 400;
		pellets = 1;
		spread = 0f;
		recoil = 1;
		power = 4;
		bounces = 0;
		piercing = false;
		muzzleOffset = new Vector2(11, 1);
		eject = false;
		ejectForce = 0;
	}

	public override void Use ( ) {
		bool willFire = ammo > 0 && fireTimer <= 0;
		sprite.loop = false;
		if (willFire) {
			var ent = G.I.CreateEntity<Barrel>();
			ent.transform.position = transform.position + getOffset(muzzleOffset / S.SIZE);
			ent.GetComponent<Rigidbody2D>().AddForce(directionVector * 20, ForceMode2D.Impulse);
			ent.Kill();
			ammo--;
			if (ammo > 2) {
				sprite.Play(2, 8);
				sprite.returnTo = 9;
			} else if (ammo > 1) {
				sprite.Play(10, 16);
				sprite.returnTo = 17;
			} else if (ammo > 0) {
				sprite.Play(18, 24);
				sprite.returnTo = 25;
			} else {
				sprite.Play(26, 32);
				sprite.returnTo = 32;
			}
			fireTimer = delay;
		} else {
			if (fireTimer <= 0) {
				sprite.Play(29, 32);
				sprite.returnTo = 32;
				fireTimer = delay;
			}
		}
	}

}