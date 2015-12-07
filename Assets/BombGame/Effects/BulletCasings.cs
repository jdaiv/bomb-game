using UnityEngine;

public class BulletCasings {

	const int NUM_CASINGS = 1000;

	class Casing {
		public Vector3 position;
		public Vector3 velocity;
	}

	AS[] sprites;
	Casing[] casings;

	int casings_made;
	int index;
	int active;

	public BulletCasings ( ) {
		sprites = new AS[NUM_CASINGS];
		casings = new Casing[NUM_CASINGS];
		casings_made = 0;
		index = 0;
		active = 0;
	}

	public void Tick (float dt) {
		for (var i = 0; i < active; i++) {
			var c = casings[i];
			c.position += c.velocity * dt;
			if (c.position.z < 0) {
				c.position.z = 0;
				c.velocity.x *= 0.4f;
				c.velocity.y *= 0.4f;
				c.velocity.z *= -0.4f;
			}

			// gravity
			c.velocity.z -= 4 * dt;

			var s = sprites[i];
			s.transform.position = new Vector3(c.position.x,
				c.position.y + c.position.z) * S.SIZE;
			s.depthOffset = Mathf.FloorToInt(c.position.z * S.SIZE - S.SIZE / 2);

			if (c.velocity.magnitude <= 0.1f) {
				s.GoTo(0);
				s.Stop();
			} else {
				s.playing = true;
			}
		}
	}

	public void Clear ( ) {
		for (var i = 0; i < active; i++) {
			sprites[i].Hide();
		}
		index = 0;
		active = 0;
	}

	public void Add (Vector3 position, Vector3 velocity) {
		if (casings_made < NUM_CASINGS) {
			casings[index] = new Casing();
			sprites[index] = G.I.NewAnimatedSprite(null, 6);
			sprites[index].loop = true;
		}
		casings[index].position = position;
		casings[index].velocity = velocity;
		sprites[index].Show();
		sprites[index].Play();
		var shade = Random.Range(0.6f, 1f);
		sprites[index].renderer.color = new Color(shade, shade, shade);

		index++;
		if (index >= NUM_CASINGS) {
			index = 0;
		}
		if (active < NUM_CASINGS) {
			active++;
		}
		if (casings_made < NUM_CASINGS) {
			casings_made++;
		}
	}

}