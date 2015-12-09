using UnityEngine;
using System.Collections;

public class WeaponSpawner : Entity {

	public enum TYPE : int {
		random,
		light,
		medium,
		heavy,
		melee,
		auto,
		explosives
	}

	static System.Type[][] weaponMap = new System.Type[][] {
		new System.Type[] {
			typeof(GolfClub),
			typeof(LMG),
			typeof(Magnum),
			typeof(Pistol),
			typeof(Rifle),
			typeof(LaserRifle),
		},
		new System.Type[] {
			typeof(GolfClub),
			typeof(Pistol),
		},
		new System.Type[] {
			typeof(Rifle),
		},
		new System.Type[] {
			typeof(LMG),
			typeof(Magnum),
			typeof(LaserRifle),
		},
		new System.Type[] {
			typeof(GolfClub),
		},
		new System.Type[] {
			typeof(LMG),
			typeof(Rifle),
		},
		new System.Type[] {
			typeof(GrenadeLauncher),
		},
	};

	S sprite;

	CircleCollider2D _trigger;

	public TYPE type;
	bool hasWeapon;

	void Awake ( ) {
		sprite = G.I.NewSprite(transform, 22);
		sprite.depthOffset = -1000;
		_trigger = gameObject.AddComponent<CircleCollider2D>();
		_trigger.radius = 0.5f;
		_trigger.isTrigger = true;
		gameObject.layer = 2;
	}

	void OnDisable ( ) {
		G.I.DeleteSprite(sprite);
	}

	//void OnTriggerEnter2D (Collider2D other) {
	//	if (IsEntity<Player>(other)) {
	//		var ply = other.GetComponent<Player>();
	//		if (ply.item == null) {
	//			SpawnWeapon();
	//		}
	//	}
	//}

	public void SpawnWeapon ( ) {
		var map = weaponMap[(int)type];
		var weapon = map[Random.Range(0, map.Length)];
		var ent = G.I.CreateEntity(weapon, weapon.ToString());
		//var ent = G.I.CreateEntity<LaserRifle>();
		ent.transform.position = transform.position;
		ent.GetComponent<Collider2D>().enabled = false;
	}

	public void FixedUpdate ( ) {
		if (!hasWeapon) {
			SpawnWeapon();
			hasWeapon = true;
		}
	}

}
