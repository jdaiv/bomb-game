﻿using UnityEngine;

public class GolfClub : Weapon {

	float swingTimer;
	float swing;

	protected override void Configure ( ) {
		animationId = 7;
		automatic = false;
		delay = 0.5f;
		ammo = 10;
		pellets = 0;
		spread = 0.05f;
		recoil = 1;
		power = 2;
		bounces = 0;
		piercing = false;
		muzzleOffset = new Vector2(0, 0);

		speed = 1.3f;
	}

	public override void _Update (float dt) {
		base._Update(dt);
		if (swingTimer <= 0) {
			if (swing > 0) {
				var circleCast = Physics2D.CircleCastAll(transform.position, 0.7f, directionVector, 0.1f);
				foreach (var hit in circleCast) {
					if (IsEntity(hit.collider)) {
						var ent = hit.collider.GetComponent<Entity>();
						if (ent != attachedTo) {
							KillEntity(hit.collider, attachedTo);
							if (ent.GetComponent<Rigidbody2D>() != null) {
								ent.GetComponent<Rigidbody2D>().AddForce(hit.normal * -20, ForceMode2D.Impulse);
							}
						}
					}
				}
				swing -= dt;
			}
		} else {
			swingTimer -= dt;
		}
	}

	public override void Use ( ) {
		if (fireTimer <= 0) {
			fireTimer = delay;
			sprite.returnTo = 1;
			sprite.Play(1);
			swingTimer = 0.18f;
			swing = 0.05f;
			active = true;
		}
	}

}
