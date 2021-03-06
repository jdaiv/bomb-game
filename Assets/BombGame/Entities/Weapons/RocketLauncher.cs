﻿using UnityEngine;
using System.Collections;

public class RocketLauncher : Weapon {

	protected override void Configure ( ) {
		animationId = 12;
		soundId = 19;
		delay = new FrameTimer(1);
		ammo = 1;
		pellets = 1;
		spread = 0f;
		recoil = 2;
		power = 4;
		bounces = 0;
		piercing = false;
		muzzleOffset = new Vector2(6, 0);
		eject = false;
		ejectForce = 0;
		speed = 0.6f;
	}

	protected override void use ( ) {
		bool willFire = ammo > 0;
		sprite.loop = false;
		if (willFire) {
			var ent = G.I.CreateEntity<RPG>();
			ent.transform.position = transform.position + getOffset(muzzleOffset / S.SIZE);
			ent.transform.rotation = transform.rotation;
			ent.GetComponent<Rigidbody2D>().AddForce(directionVector * 20, ForceMode2D.Impulse);
			ent.Kill(attachedTo);
			ammo--;
			sprite.GoTo(2);
			G.I.PlaySound(Random.Range(soundId, soundId + 3));
		} else {
			G.I.PlaySound(2);
			Detach();
		}
	}

}
