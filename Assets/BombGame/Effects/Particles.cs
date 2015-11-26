using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Particles {

	class PData {
		public bool active;
		public Transform transform;
		public S sprite;
		public Vector2 velocity;
		public float frameTime;
		private float timer;
		private int frame;
		public Sprite[] effect;

		public PData () {
			transform = new GameObject("Particle").transform;
			transform.gameObject.hideFlags = HideFlags.HideInHierarchy;
			active = false;
			sprite = G.I.NewSprite(transform, 7);
			sprite.Hide();
		}

		public void Spawn (Sprite[] effect, float frameTime, Vector2 position) {
			this.effect = effect;
			this.frameTime = frameTime;
			frame = 0;
			timer = 0;

			transform.position = position;

			active = true;
			sprite.Show();
		}

		public void Tick (float dt) {
			transform.Translate(velocity * dt);
			timer += dt;
			if (timer > frameTime) {
				timer -= frameTime;
				sprite.renderer.sprite = effect[frame];
				frame++;
				if (frame >= effect.Length) {
					active = false;
					sprite.Hide();
				}
			}
		}
	}

	private List<PData> _particles;
	private List<Sprite[]> _sprites;

	public Particles () {
		_particles = new List<PData>();
		_sprites = new List<Sprite[]>();
	}

	public void RegisterSprite (Sprite s, int frames) {
		var arr = new Sprite[frames];
		var frameWidth = s.texture.width / frames;
		var frameHeight = s.texture.height;
        for (int i = 0; i < frames; i++) {
			arr[i] = Sprite.Create(s.texture, new Rect(frameWidth * i, 0, frameWidth, frameHeight), new Vector2(0.5f, 0.5f), 1);
		}
		_sprites.Add(arr);
	}

	public void Tick (float dt) {
		foreach (var p in _particles) {
			if (p.active) {
				p.Tick(dt);
			}
		}
	}

	public void Emit (Vector2 position, int count) {
		for (int i = 0; i < count; i++) {
			var p = getParticle();
			p.Spawn(_sprites[0], 0.05f, position);
		}
	}

	private PData getParticle () {
		foreach (var p in _particles) {
			if (!p.active) {
				return p;
			}
		}
		var newP = new PData();
		_particles.Add(newP);
        return newP;
	}

}
