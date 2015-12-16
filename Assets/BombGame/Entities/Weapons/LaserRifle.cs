using UnityEngine;
using System.Collections;

public class LaserRifle : Weapon {

	FrameTimer fire;

	protected override void Configure ( ) {
		animationId = 10;
		delay = new FrameTimer(45);
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

		fire = new FrameTimer(40);
	}

	public override void Tick ( ) {
		base.Tick();

		fire.Tick();

		if (fire) {
			Fire(transform.position + getOffset(muzzleOffset / S.SIZE), directionVector);
		}

		if (fire.running) { 
			speed = 0.4f;
		} else {
			speed = 1f;
		}
	}

	public override void Fire (Vector2 origin, Vector2 dir) {
		G.I.FireHitscanLaser(attachedTo, origin, dir, 0.5f);
		G.I.PlaySound(22);
	}

	protected override void use ( ) {
		if (!delay.running) {
			fire.Start();
			sprite.Play(1, 15);
			sprite.returnTo = 1;
			G.I.PlaySound(23);
		}
	}

}
