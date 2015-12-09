using UnityEngine;
using System.Collections;

public class RocketLauncher : Weapon {

	protected override void Configure ( ) {
		animationId = 12;
		soundId = 19;
		automatic = true;
		delay = 0.40f;
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

	protected override void CustomAttach ( ) {
		if (ammo > 0) {
			sprite.GoTo(1);
		} else {
			sprite.GoTo(2);
		}
	}

	public override void Use ( ) {
		bool willFire = ammo > 0 && fireTimer <= 0;
		sprite.loop = false;
		if (willFire) {
			var ent = G.I.CreateEntity<RPG>();
			ent.transform.position = transform.position + getOffset(muzzleOffset / S.SIZE);
			ent.transform.rotation = transform.rotation;
			ent.GetComponent<Rigidbody2D>().AddForce(directionVector * 20, ForceMode2D.Impulse);
			ent.Kill(attachedTo);
			ammo--;
			sprite.GoTo(2);
			fireTimer = delay;
			G.I.PlaySound(Random.Range(soundId, soundId + 3));
		} else {
			G.I.PlaySound(2);
			Detach();
		}
	}

}
