using UnityEngine;
using System.Collections;

public class Weapon : Item {

	protected AS sprite;
	protected float killTimer;
	protected float fireTimer;

	#region weapon info
	
	protected int animationId;
	protected bool automatic;
	protected int pellets;
	protected float delay;
	protected int ammo;
	protected float recoil;
	protected float spread;
	protected int bounces;
	protected int power;
	protected bool piercing;

	protected Vector2 muzzleOffset; 

	#endregion

	public override void Awake ( ) {
		base.Awake();
		Configure();
		sprite = G.I.NewAnimatedSprite(this.transform, animationId);
		sprite.loop = false;
		fireTimer = 0;
	}

	public void OnDestroy ( ) {
		G.I.DeleteSprite(sprite);
	}

	public override void Update ( ) {
		base.Update();

		var dt = Time.deltaTime;

		if (fireTimer > 0) {
			fireTimer -= dt;
		}

		if (ammo <= 0 && attachedTo == null) {
			sprite.Toggle();
			killTimer -= dt;
			if (killTimer <= 0) {
				G.I.DeleteEntity(this);
			}
		}
	}

	protected virtual void Configure ( ) {
		animationId = 1;
		automatic = false;
		delay = 0.3f;
		ammo = 10;
		pellets = 1;
		spread = 0.5f;
		recoil = 1;
		power = 0;
		bounces = 5;
		piercing = false;
		muzzleOffset = new Vector2(7, 2);
	}

	protected override void CustomDetach ( ) {
		if (ammo > 0) {
			_trigger.enabled = true;
		} else {
			killTimer = 1;
			_trigger.enabled = false;
		}
	}

	public override void Use ( ) {
		if (fireTimer <= 0) {
			if (ammo > 0) {

				for (int i = 0; i < pellets; i++) {
					var dir = directionVector;
					dir += U.RandomVec() * spread;
					var start = transform.position + getOffset(muzzleOffset / S.SIZE);
					if (piercing) {
						G.I.FireHitscanNoCollision(start, dir, power);
					} else {
						G.I.FireHitscan(start, dir, power, bounces);
					}
				}

				ammo--;
				fireTimer = delay;

				if (ammo <= 0) {
					sprite.returnTo = 2;
				} else {
					sprite.returnTo = 0;
				}
				sprite.Play();

				Vector3 eject;

				switch (direction) {
					case 3:
						eject = U.RandomVec(new Vector3(1, -1, -1), new Vector3(2, 1, 1));
                        break;
					case 2:
						eject = U.RandomVec(new Vector3(-1, -1, -1), new Vector3(1, 1, -2));
						break;
					case 1:
						eject = U.RandomVec(new Vector3(-1, -1, -1), new Vector3(-2, 1, 1));
						break;
					case 0:
					default:
						eject = U.RandomVec(new Vector3(-1, -1, 1), new Vector3(1, 1, 2));
						break;
				}

				G.I.casings.Add(
					transform.position + new Vector3(0, -0.5f, 0.5f),
					eject
					);

			} else {

				sprite.returnTo = 2;
				sprite.Play(1, 2);

			}
		}
	}

	protected Vector3 getOffset (float x, float y, float z = 0) {
		switch (direction) {
			case 3:
				return new Vector3(y, -x, z);
			case 2:
				return new Vector3(-x, -y, z);
			case 1:
				return new Vector3(-y, x, z);
			case 0:
			default:
				return new Vector3(x, y, z);
		}
	}

	protected Vector3 getOffset (Vector3 v) {
		return getOffset(v.x, v.y, v.z);
	}

}
