using UnityEngine;

public class GolfClub : Weapon {

	const int ACTIVE_FRAMES = 6;

	FrameTimer swing;
	int activeFrames;

	protected override void Configure ( ) {
		animationId = 7;
		soundId = 27;
		delay = new FrameTimer(40);
		ammo = 10;
		pellets = 0;
		spread = 0.05f;
		recoil = 1;
		power = 2;
		bounces = 0;
		piercing = false;
		muzzleOffset = new Vector2(0, 0);

		speed = 1.3f;

		swing = new FrameTimer(12);
	}

	public override void Tick ( ) {
		base.Tick();

		swing.Tick();
		if (swing) {
			activeFrames = ACTIVE_FRAMES;
			G.I.PlaySound(Random.Range(soundId, soundId + 3));
		}
		if (activeFrames > 0) {
			var circleCast = Physics2D.CircleCastAll(transform.position, 0.7f, directionVector, 0.1f);
			foreach (var hit in circleCast) {
				if (IsEntity(hit.collider)) {
					var ent = hit.collider.GetComponent<Entity>();
					if (ent != attachedTo) {
						if (ent.Is<Player>()) {
							(ent as Player).KillSilent(attachedTo);
							var husk = (PlayerHusk)G.I.CreateEntity<PlayerHusk>();
							husk.transform.position = ent.transform.position;
							husk.SetSprite((ent as Player).id);
							husk._rigidbody.AddForce(hit.normal * -10, ForceMode2D.Impulse);
						} else {
							KillEntity(hit.collider, attachedTo);
							if (ent.GetComponent<Rigidbody2D>() != null) {
								ent.GetComponent<Rigidbody2D>().AddForce(hit.normal * -10, ForceMode2D.Impulse);
							}
						}
						G.I.PlaySound(Random.Range(30, 33));
					}
				}
			}
			activeFrames--;
		}
	}

	protected override void use ( ) {
		if (!delay.running) {
			delay.Start();
			sprite.returnTo = 1;
			sprite.Play(1);
			swing.Start();
			active = true;
		}
	}

}
