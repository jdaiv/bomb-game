using System;
using UnityEngine;

public class AS : S {

	public const float FRAME_TIME = 0.05f;

	public Sprite[] frames;

	float timer;
	public int frame;
	public bool playing;
	public bool loop;
	public int returnTo;
	public int end;

	public AS (Sprite[] frames) {
		this.frames = frames;
		returnTo = frames.Length - 1;
	}

	public override void Update ( ) {

		if (playing) {
			timer += Time.deltaTime;
			if (timer >= FRAME_TIME) {
				timer -= FRAME_TIME;
				frame++;
				if (frame >= end) {
					if (loop) {
						frame = 0;
					} else {
						frame = returnTo;
						playing = false;
					}
				}
				UpdateFrame();
            }
		}

		base.Update();

	}

	public void UpdateFrame () {
		renderer.sprite = frames[frame];
	}

	public void Play (int from = 0, int to = -1) {
		playing = true;
		frame = from;
		timer = 0;
		if (to >= 0) {
			end = to + 1;
		} else {
			end = frames.Length;
		}
		UpdateFrame();
	}

	public void GoTo (int frame) {
		this.frame = frame;
		timer = 0;
		UpdateFrame();
	}

	public void Stop ( ) {
		playing = false;
		frame = 0;
		timer = 0;
		UpdateFrame();
	}

	public void Pause ( ) {
		playing = false;
	}

	public void Resume ( ) {
		playing = true;
	}

}