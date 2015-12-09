using UnityEngine;
using System.Collections;

public class LaserRifle : Weapon {

	float delayTimer;
	bool fire;

	protected override void Configure ( ) {
		animationId = 10;
		automatic = true;
		delay = 0.75f;
		ammo = 1;
		pellets = 1;
		spread = 0;
		recoil = 0;
		power = 4;
		bounces = 0;
		piercing = false;
		muzzleOffset = new Vector2(8, -1);
		eject = true;
		ejectForce = 3;
	}

	public override void Update ( ) {
		base.Update();

		if (fire) {
			delayTimer -= Time.deltaTime;
			if (delayTimer <= 0) {
				Fire(transform.position + getOffset(muzzleOffset / S.SIZE), directionVector);
				fire = false;
			}
		}
	}

	public override void Fire (Vector2 origin, Vector2 dir) {
		G.I.FireHitscanLaser(origin, dir, 0.5f);
	}

	public override void Use ( ) {
		if (fireTimer <= 0) {
			active = true;
			delayTimer = 0.6f;
			fire = true;
			sprite.Play(1, 15);
			sprite.returnTo = 1;
			fireTimer = delay;
		}
	}

}
