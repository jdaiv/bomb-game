using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Particles {

	class PData {
		public bool active;
		public Vector2 position;
		public AS sprite;
		public Vector2 velocity;
		private FrameTimer timer;

		public PData () {
			active = false;
			sprite = G.I.NewAnimatedSprite(null, 0);
			sprite.depthOffset = 0;
			sprite.Hide();
			sprite.loop = false;
		}

		public void Spawn (Sprite[] effect, Vector2 position, Vector2 velocity, int depthOffset = 0) {
			sprite.frames = effect;
			// sprite expects a valid frame number, length is the last frame + 1.
			sprite.returnTo = effect.Length - 1;
			sprite.Play();
			sprite.depthOffset = depthOffset;

			this.position = position;
			sprite.transform.position = new Vector3(
				Mathf.Round(this.position.x * S.SIZE),
				Mathf.Round(this.position.y * S.SIZE)
			);
			this.velocity = velocity;

			timer = new FrameTimer(effect.Length * AS.FRAME_DURATION);
			timer.Start();

			active = true;
			sprite.Show();
		}

		public void Tick (float dt) {
			position += (velocity * dt);
			sprite.transform.position = new Vector3(
				Mathf.Round(position.x * S.SIZE),
				Mathf.Round(position.y * S.SIZE)
			);
			if (timer) {
				sprite.Hide();
				active = false;
			}
		}
	}

	private List<PData> _particles;
	private List<Sprite[]> _sprites;

	public Particles () {
		_particles = new List<PData>(400);
		_sprites = new List<Sprite[]>(20);
	}

	public void RegisterSprite (Sprite[] s) {
		_sprites.Add(s);
	}

	public void Tick () {
		var dt = (float)G.STEP;
		foreach (var p in _particles) {
			if (p.active) {
				p.Tick(dt);
			}
		}
	}

	public void Emit (int effect, Vector2 position, int count, Vector2 velocityMin = new Vector2(), Vector2 velocityMax = new Vector2()) {
		for (int i = 0; i < count; i++) {
			var p = getParticle();
			p.Spawn(_sprites[effect], position, U.RandomVec(velocityMin, velocityMax), effect == 0 ? 10000 : 0);
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
