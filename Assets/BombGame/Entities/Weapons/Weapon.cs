using UnityEngine;
using System.Collections;

public class Weapon : Item {

	protected AS sprite;
	protected FrameTimer killTimer;
	protected float spreadInc;

	#region weapon info
	
	protected int animationId;
	protected int soundId = 4;
	protected int pellets;
	protected FrameTimer delay;
	protected int ammo;
	protected float recoil;
	protected float spread;
	protected int bounces;
	protected int power;
	protected bool piercing;
	protected bool eject = false;
	protected float ejectForce = 2;

	public float speed = 1;
	public Vector2 muzzleOffset; 

	#endregion

	public override void Awake ( ) {
		base.Awake();
		Configure();
		sprite = G.I.NewAnimatedSprite(transform, animationId);
		sprite.loop = false;
		killTimer = new FrameTimer(40);
	}

	public void OnDestroy ( ) {
		G.I.DeleteSprite(sprite);
	}

	override public void Tick ( ) {
		delay.Tick();
		active = delay.running;

		if (killTimer.running) {
			killTimer.Tick();
			sprite.Toggle();
			if (killTimer) {
				G.I.DeleteEntity(this);
			}
		}
		spreadInc *= 0.9f;
	}

	protected virtual void Configure ( ) {
		animationId = 1;
		delay = new FrameTimer(20);
		ammo = 10;
		pellets = 1;
		spread = 0.5f;
		recoil = 1;
		power = 0;
		bounces = 5;
		piercing = false;
		muzzleOffset = new Vector2(7, 2);
	}

	protected override void CustomAttach ( ) {
		sprite.Stop();
		sprite.GoTo(1);
		G.I.PlaySound(1);
	}

	protected override void CustomDetach ( ) {
		if (ammo > 0) {
			sprite.Stop();
			sprite.GoTo(0);
			_trigger.enabled = true;
		} else {
			killTimer.Start();
			_trigger.enabled = false;
		}
		G.I.PlaySound(1);
	}

	public virtual void Fire (Vector2 origin, Vector2 dir) {
		if (piercing) {
			G.I.FireHitscanNoCollision(attachedTo, origin, dir, power);
		} else {
			G.I.FireHitscan(attachedTo, origin, dir, power, bounces);
		}
	}

	protected override void use ( ) {
		if (!delay.running) {
			if (ammo > 0) {

				for (int i = 0; i < pellets; i++) {
					var dir = directionVector;
					dir += U.RandomVec() * (spread + spreadInc);
					var start = transform.position + getOffset(muzzleOffset / S.SIZE);
					Fire(start, dir);
				}

				ammo--;
				delay.Start();

				if (ammo <= 0) {
					sprite.returnTo = 3;
				} else {
					sprite.returnTo = 1;
				}
				sprite.Play(2);

				if (this.eject) {
					Vector3 eject;

					switch (direction) {
						case 3:
							eject = U.RandomVec(new Vector3(1, -1, -1), new Vector3(ejectForce, 1, ejectForce / 3));
							break;
						case 2:
							eject = U.RandomVec(new Vector3(-1, -1, -1), new Vector3(ejectForce / 3, 1, -ejectForce));
							break;
						case 1:
							eject = U.RandomVec(new Vector3(-1, -1, ejectForce / -3), new Vector3(-ejectForce, 1, 1));
							break;
						case 0:
						default:
							eject = U.RandomVec(new Vector3(ejectForce / -3, -1, 1), new Vector3(1, 1, ejectForce));
							break;
					}

					G.I.casings.Add(
						transform.position + new Vector3(0, -0.5f, 0.5f),
						eject
						);
				}

				G.I.PlaySound(Random.Range(soundId, soundId + 3));

				// Did the player suicide?
				if (attachedTo != null)
					attachedTo.GetComponent<Rigidbody2D>().AddForce(directionVector * -recoil, ForceMode2D.Impulse);
				spreadInc += recoil * 0.5f;

				active = true;

			} else {

				sprite.returnTo = 3;
				sprite.Play(2, 3);
				G.I.PlaySound(2);
				delay.Start();

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
